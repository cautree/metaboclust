﻿using System.Collections.Generic;
using System.Linq;
using MetaboliteLevels.Algorithms.Statistics.Arguments;
using MetaboliteLevels.Algorithms.Statistics.Configurations;
using MetaboliteLevels.Algorithms.Statistics.Inputs;
using System;
using MetaboliteLevels.Utilities;
using MGui.Helpers;

namespace MetaboliteLevels.Algorithms.Statistics.Statistics
{
    /// <summary>
    /// Special case of Statistic that take other statistics as inputs.
    /// </summary>
    sealed class StatisticConsumer : StatisticBase
    {
        private readonly AlgoDelegate_Input1 _delegate;

        public StatisticConsumer(AlgoDelegate_Input1 method, string id, string name)
            : base(id, name)
        {
            this._delegate = method;
        }

        public override double Calculate(InputStatistic input)
        {
            var src = (IEnumerable<WeakReference<ConfigurationStatistic>>)input.Args.Parameters[0];

            double[] vals = src.Select(delegate (WeakReference<ConfigurationStatistic> z)
                                       {
                                           ConfigurationStatistic a = z.GetTarget();

                                           if (a != null)
                                           {
                                               return input.PeakA.Statistics[a];
                                           }
                                           else
                                           {
                                               throw new InvalidOperationException("Statistic in " + this.ToString() + " no longer exists.");
                                           }
                                       }).ToArray();

            return _delegate(vals);
        }

        protected override AlgoParameterCollection CreateParamaterDesription()
        {
            return new AlgoParameterCollection(new AlgoParameter("statistics", EAlgoParameterType.WeakRefStatisticArray));
        }

        public override bool SupportsInputFilters { get { return false; } }
    }
}