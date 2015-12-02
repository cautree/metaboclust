﻿using System.Linq;
using MetaboliteLevels.Utilities;
using System.Collections.Generic;
using MetaboliteLevels.Data.Visualisables;
using MetaboliteLevels.Algorithms.Statistics.Arguments;
using MetaboliteLevels.Algorithms.Statistics.Configurations;
using MetaboliteLevels.Settings;
using MetaboliteLevels.Data.Session;

namespace MetaboliteLevels.Algorithms.Statistics.Clusterers
{
    /// <summary>
    /// Represents script-based clustering algorithms.
    /// </summary>
    class ClustererScript : ClustererBase
    {
        public const string INPUTS = @"value.matrix=x,distance.matrix=-";
        public readonly RScript _script;

        public ClustererScript(string script, string id, string name)
            : base(id, name)
        {
            this._script = new RScript(script, INPUTS, null, "-1", AlgoParameters.ESpecial.None);

            UiControls.Assert(_script.IsInputPresent(0) || _script.IsInputPresent(1),
                              "ClustererScript must take at least one of value matrix or distance matrix");
        }

        protected override IEnumerable<Cluster> Cluster(ValueMatrix vmatrix, DistanceMatrix dmatrix, ArgsClusterer args, ConfigurationClusterer tag, IProgressReporter prog)
        {
            object[] inputs =
            {
                _script.IsInputPresent(0) ? vmatrix.Flatten() : null,
                _script.IsInputPresent(1) ? dmatrix.Values : null
            };

            prog.ReportProgress("Running script");
            prog.ReportProgress(-1);
            int[] clusters = Arr.Instance.RunScriptIntV(_script, inputs, args.Parameters).ToArray();
            prog.ReportProgress("Creating clusters");
            return CreateClustersFromIntegers(vmatrix, clusters, tag);
        }

        public override AlgoParameters GetParams()
        {
            return _script.RequiredParameters;
        }
    }
}
