﻿using System;
using MetaboliteLevels.Algorithms.Statistics.Arguments;
using MetaboliteLevels.Data.Session;
using MetaboliteLevels.Data.Visualisables;
using System.Collections.Generic;
using System.Linq;
using MetaboliteLevels.Algorithms.Statistics.Configurations;
using MetaboliteLevels.Algorithms.Statistics.Results;
using MetaboliteLevels.Utilities;

namespace MetaboliteLevels.Algorithms.Statistics.Clusterers
{
    /// <summary>
    /// Clustering algorithm base class.
    /// </summary>
    abstract class ClustererBase : AlgoBase
    {
        protected ClustererBase(string id, string name)
            : base(id, name)
        {
            // NA
        }

        public ResultClusterer Calculate(Core core, int isPreview, ArgsClusterer args, ConfigurationClusterer tag, IProgressReporter prog, out ValueMatrix vmatrixOut, out DistanceMatrix dmatrixOut)
        {
            IReadOnlyList<Peak> peaks;

            if (isPreview > 0 && isPreview < core.Peaks.Count)
            {
                var p = core.Peaks.ToList();
                p.Shuffle();

                p = p.GetRange(0, Math.Min(isPreview, p.Count)).ToList();

                // Make sure any seed peaks are in the list
                foreach (object par in tag.Args.Parameters)
                {
                    if (par is WeakReference<Peak>)
                    {
                        Peak peak = ((WeakReference<Peak>)par).GetTargetOrThrow();
                        p.Insert(0, peak);
                        p.RemoveAt(p.Count - 1);
                    }
                }

                peaks = p;
            }
            else
            {
                peaks = core.Peaks;
            }

            // FILTER PEAKS
            var pfilter = args.PeakFilter ?? Settings.PeakFilter.Empty;

            var filter = pfilter.Test(peaks);
            List<Cluster> insigs = new List<Cluster>();

            if (filter.Failed.Count != 0)
            {
                Cluster insig = new Cluster("Insig", tag);
                insig.States |= Data.Visualisables.Cluster.EStates.Insignificants;

                // We still need the vmatrix for plotting later
                ValueMatrix insigvMatrix = ValueMatrix.Create(filter.Failed, args.SourceMode == EAlgoSourceMode.Trend, core, args.ObsFilter, false, prog);

                for (int index = 0; index < insigvMatrix.NumVectors; index++)
                {
                    Vector p = insigvMatrix.Vectors[index];
                    insig.Assignments.Add(new Assignment(p, insig, double.NaN));
                }

                insigs.Add(insig);
            }

            // CREATE VMATRIX AND FILTER OBSERVATIONS
            bool useTrend = args.SourceMode != EAlgoSourceMode.Full;

            prog.ReportProgress("Creating value matrix");
            ValueMatrix vmatrix = ValueMatrix.Create(filter.Passed, useTrend, core, args.ObsFilter, args.SplitGroups, prog);
            prog.ReportProgress("Creating distance matrix");
            DistanceMatrix dmatrix = GetParams().Special.HasFlag(AlgoParameters.ESpecial.ClustererIgnoresDistanceMatrix) ? null : DistanceMatrix.Create(core, vmatrix, args.Distance, prog);
            IEnumerable<Cluster> clusters;

            // CLUSTER USING VMATRIX OR DMATRIX
            prog.ReportProgress("Clustering");
            clusters = Cluster(vmatrix, dmatrix, args, tag, prog);

            vmatrixOut = vmatrix;
            dmatrixOut = dmatrix;
            return new ResultClusterer(insigs.Concat(clusters));
        }

        /// <summary>
        /// Clustering
        /// 
        /// If the cluster does't make use of the distance matrix OR the distance metric it should flag itself with DoesNotSupportDistanceMetrics.
        /// </summary>
        protected abstract IEnumerable<Cluster> Cluster(ValueMatrix vmatrix, DistanceMatrix dmatrix, ArgsClusterer args, ConfigurationClusterer tag, IProgressReporter prog);

        protected static IEnumerable<Cluster> CreateClustersFromIntegers(ValueMatrix vmatrix, IList<int> clusters, ConfigurationClusterer tag)
        {
            Dictionary<int, Cluster> pats = new Dictionary<int, Cluster>();
            List<Cluster> r = new List<Cluster>();

            for (int n = 0; n < vmatrix.NumVectors; n++)
            {
                Vector p = vmatrix.Vectors[n];

                int c = clusters != null ? clusters[n] : 0;
                Cluster pat;

                if (!pats.TryGetValue(c, out pat))
                {
                    pat = new Cluster((pats.Count + 1).ToString(), tag);
                    pats.Add(c, pat);
                    r.Add(pat);
                }

                pat.Assignments.Add(new Assignment(p, pat, double.NaN));
            }

            return r;
        }

        public abstract override AlgoParameters GetParams();
    }
}
