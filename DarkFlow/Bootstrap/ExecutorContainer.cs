﻿using System;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class ExecutorContainer : Disposable, IExecutor
    {
        private readonly IExecutor _executor;
        private readonly IDisposable[] _disposables;

        public ExecutorContainer(IExecutor executor, IDisposable[] disposables)
        {
            _executor = executor;
            _disposables = disposables;
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }

            if (disposables == null)
            {
                throw new ArgumentNullException("disposables");
            }
        }

        protected override void DisposeManaged()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        public void Execute(ITask task)
        {
            _executor.Execute(task);
        }

        public void Execute(IPersistentTask task)
        {
            _executor.Execute((ITask) task);
        }
    }
}