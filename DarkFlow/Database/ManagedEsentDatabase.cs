﻿using System;
using System.Collections.Generic;
using Codestellation.DarkFlow.Misc;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Codestellation.DarkFlow.Database
{
    public class ManagedEsentDatabase : Disposable, IDatabase
    {
        private readonly string _persistFolder;
        private readonly PersistentDictionary<string, string> _database;
        
        public const string DefaultTaskFolder = "PersistedTasks";

        public ManagedEsentDatabase() :this(DefaultTaskFolder)
        {
            
        }

        public ManagedEsentDatabase(string persistFolder)
        {
            _persistFolder = persistFolder;
            _database = new PersistentDictionary<string, string>(persistFolder);
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Created managed esent task database at folder '{0}'", persistFolder);
            }
        }

        public string PersistFolder
        {
            get { return _persistFolder; }
        }

        public Identifier Persist(Region region, string serializedTask)
        {
            var id = region.NewIdentifier();
            Persist(id, serializedTask);
            return id;
        }

        public void Persist(Identifier id, string serializedTask)
        {
            _database[id.ToString()] = serializedTask;

            if (Logger.IsDebugEnabled)
        {
            Logger.Debug("Persisted: id='{0}'; task='{1}'", id, serializedTask);
        }
        }

        public string Get(Identifier id)
        {
            string result;
            if (_database.TryGetValue(id.ToString(), out result))
            {
                return result;
            }
            //TODO: Use own exception class
            throw new InvalidOperationException(string.Format("String with id='{0}' was not found. Possible concurrency issue.", id));
        }

        public void Remove(Identifier id)
        {
            _database.Remove(id.ToString());
            
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Removed: id='{0}';", id);
            }
        }

        public IEnumerable<KeyValuePair<Identifier, string>> GetAll(Region region)
        {
            foreach (var kvp in _database)
            {
                var id = Identifier.Parse(kvp.Key);
                
                if(!id.Region.Equals(region)) continue;

                yield return new KeyValuePair<Identifier, string>(id, kvp.Value);
            }
        }

        protected override void DisposeManaged()
        {
            _database.Flush();
            //TODO: this is commented to prevent deadlock between topshelf and managedesent. Just let finalizer do it work :(
            // PersistentDictionary implementes correct disposable pattern, resources would be freed.
            _database.Dispose();
        }
    }
}