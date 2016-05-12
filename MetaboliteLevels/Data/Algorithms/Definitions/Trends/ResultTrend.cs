﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaboliteLevels.Algorithms.Statistics.Results
{
    [Serializable]
    class ResultTrend : ResultBase
    {
        private int NumPeaks;

        public ResultTrend(int numPeaks)
        {
            this.NumPeaks = numPeaks;
        }
    }
}