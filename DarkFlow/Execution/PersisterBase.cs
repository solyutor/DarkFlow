﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Matchers;
using Codestellation.DarkFlow.Misc;
using NLog;
using Newtonsoft.Json;

namespace Codestellation.DarkFlow.Execution
{
    public abstract class PersisterBase : IPersister
    {
        private readonly IDatabase _database;
        private readonly IMatcher _matcher;
        protected readonly JsonSerializerSettings Settings;
        protected readonly Logger Logger;

        public PersisterBase(IDatabase database, IMatcher matcher)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            if (matcher == null)
            {
                throw new ArgumentNullException("matcher");
            }

            Logger = LogManager.GetLogger(GetType().FullName);

            _database = database;
            _matcher = matcher;

            Settings = new JsonSerializerSettings
                            {
                                Formatting = Formatting.Indented,
                                TypeNameHandling = TypeNameHandling.All, 
                                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                            };
        }

        protected abstract string Serialize(ITask task);

        protected abstract ITask Deserialize(string serialized);

        public void Delete(Identifier identifier)
        {
            Contract.Require(identifier.IsValid, "identifier.IsValid");

            _database.Remove(identifier);
        }

        public ITask Get(Identifier identifier)
        {
            var serialized = _database.Get(identifier);
            var result = Deserialize(serialized);
            return result;
        }

        public bool IsPersistent(ITask task)
        {
            Contract.Require(task != null, "task != null");

            return _matcher.TryMatch(task);
        }

        public void Persist(Identifier identifier, ITask task)
        {
            Contract.Require(task != null, "task != null");
            Contract.Require(identifier.IsValid, "identifier.IsValid");

            var serialized = Serialize(task);

            _database.Persist(identifier, serialized);

            Logger.Debug("Serialized task:{0}{1}", Environment.NewLine, serialized);

        }

        public IEnumerable<KeyValuePair<Identifier,ITask>> LoadAll(Region region)
        {
            Contract.Require(region.IsValid, "region.IsValid");

            var serialized = _database.GetAll(region);

            var results = serialized.Select(Deserialize);

            return results;
        }

        private KeyValuePair<Identifier, ITask> Deserialize(KeyValuePair<Identifier, string> input)
        {
            var deserialized = Deserialize(input.Value);
            return new KeyValuePair<Identifier, ITask>(input.Key, deserialized);
        }
    }
}