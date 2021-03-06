﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaboliteLevels.Data.Algorithms.Definitions.Base;
using MetaboliteLevels.Data.Session.Associational;
using MetaboliteLevels.Data.Session.Main;

namespace MetaboliteLevels.Data.Algorithms.Definitions.Corrections
{
    [Serializable]
    class ResultCorrection : ResultBase
    {
        /// <summary>
        /// Resultant intensity matrix
        /// </summary>
        public readonly IntensityMatrix Matrix;

        public ResultCorrection( IntensityMatrix result )
        {
            Matrix = result;
        }

        public override string ToString()
        {
            return Matrix.ToString();
        }
    }
}
