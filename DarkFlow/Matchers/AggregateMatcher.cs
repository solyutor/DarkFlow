﻿using NLog;

namespace Codestellation.DarkFlow.Matchers
{
    public class AggregateMatcher : IMatcher
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMatcher[] _matchers;

        public AggregateMatcher(params IMatcher[] matchers)
        {
            _matchers = matchers;
        }

        public MatchResult TryMatch(ITask task)
        {
            for (int i = 0; i < _matchers.Length; i++)
            {
                var matcher = _matchers[i];
            
                var result = matcher.TryMatch(task);

                Logger.Debug("Task {0} matched.", task);

                if (result) return result;
            }

            Logger.Debug("Task {0} matched.", task);

            return MatchResult.NonMatched;
        }
    }
}