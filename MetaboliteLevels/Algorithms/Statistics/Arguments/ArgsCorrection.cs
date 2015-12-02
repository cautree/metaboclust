﻿using System;
using System.Text;

namespace MetaboliteLevels.Algorithms.Statistics.Arguments
{
    /// <summary>
    /// Arguments for CorrectionBase derivatives.
    /// </summary>
    [Serializable]
    class ArgsCorrection : ArgsBase
    {
        public ArgsCorrection(object[] parameters)
            : base(parameters)
        {
        }

        public override string ToString(AlgoBase algorithm)
        {
            StringBuilder sb = new StringBuilder();

            if (Parameters != null)
            {
                sb.Append(AlgoParameters.ParamsToHumanReadableString(Parameters,algorithm ));
            }

            return sb.ToString();
        }
    }
}
