﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MetaboliteLevels.Data.General;
using MetaboliteLevels.Data.Session;
using MetaboliteLevels.Utilities;
using MetaboliteLevels.Viewers;
using MetaboliteLevels.Viewers.Charts;
using MetaboliteLevels.Viewers.Lists;
using MetaboliteLevels.Algorithms;
using MetaboliteLevels.Settings;
using MSerialisers;
using System.Drawing.Drawing2D;

namespace MetaboliteLevels.Data.Visualisables
{
    /// <summary>
    /// Pathways (essentially sets of compounds)
    /// </summary>
    [Serializable]
    [DeferSerialisation]
    class Pathway : IVisualisable
    {
        /// <summary>
        /// Pathway name
        /// </summary>
        private readonly string _defaultName;

        /// <summary>
        /// Unique ID
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>
        public string OverrideDisplayName { get; set; }

        /// <summary>
        /// Source libraries.
        /// </summary>
        public readonly List<CompoundLibrary> Libraries = new List<CompoundLibrary>();

        /// <summary>
        /// Columns in the original data we didn't use for anything
        /// </summary>
        public readonly MetaInfoCollection MetaInfo = new MetaInfoCollection();

        /// <summary>
        /// Related pathways (because the source database says so)
        /// </summary>
        public readonly List<Pathway> RelatedPathways = new List<Pathway>();

        /// <summary>
        /// Compounds in this pathway
        /// </summary>
        public readonly List<Compound> Compounds = new List<Compound>();

        /// <summary>
        /// CONSTRUCTOR
        /// </summary> 
        public Pathway(CompoundLibrary tag, string name, string id)
        {
            if (tag != null)
            {
                this.Libraries.Add(tag);
            }

            this.Id = id;
            this._defaultName = !string.IsNullOrWhiteSpace(name) ? name : "Compounds not assigned to any pathway";
        }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// Unused (can't be disabled)
        /// </summary>
        bool ITitlable.Enabled { get { return true; } set { } }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>
        public string DefaultDisplayName
        {
            get
            {
                return _defaultName;
            }
        }

        /// <summary>
        /// OVERRIDES Object
        /// </summary>
        public override string ToString()
        {
            return DefaultDisplayName;
        }

        /// <summary>
        /// Creates a StylisedCluster for plotting from this pathway.
        /// </summary>
        /// <param name="core">Core</param>
        /// <param name="highlightContents">What to highlight in the plot</param>
        /// <returns>A StylisedCluster</returns>
        internal StylisedCluster CreateStylisedCluster(Core core, IVisualisable highlightContents)
        {
            var colours = new Dictionary<Peak, LineInfo>();

            // Adduct: NA
            // Peak: Pathways for peak -> Peaks (THIS PEAK)
            // Cluster: Pathways for cluster -> Peaks (PEAKS IN CLUSTER)
            // Compound: Pathway for compound -> Peaks (PEAKS IN COMPOUND)
            // Pathway: NA

            StylisedCluster.HighlightElement[] toHighlight = null;
            const string caption1 = "Plot of peaks potentially representing compounds implicated in {0}.";
            string caption2 = " Sets of peaks that may represent the same compound or set of compounds are shown in the same colour. Other peaks are shown in black.";
            string caption3 = "";

            if (highlightContents != null)
            {
                switch (highlightContents.VisualClass)
                {
                    case VisualClass.Compound:
                        Compound highlightCompound = (Compound)highlightContents;
                        toHighlight = highlightCompound.Annotations.Select(z => new StylisedCluster.HighlightElement(z, null)).ToArray();
                        caption3 = " Peaks potentially representing {1} are shown in red.";
                        break;

                    case VisualClass.Cluster:
                        Cluster highlightCluster = (Cluster)highlightContents;
                        toHighlight = highlightCluster.Assignments.Vectors.Select(StylisedCluster.HighlightElement.FromVector).ToArray();
                        caption3 = " Peaks potentially representing compounds in {1} are shown in red.";

                        break;

                    case VisualClass.Peak:
                        toHighlight = new StylisedCluster.HighlightElement[] { new StylisedCluster.HighlightElement((Peak)highlightContents, null) };
                        caption3 = " {1} is shown in red.";
                        break;
                }
            }

            // Make a list of the variables and the compounds in this cluster they might represent
            Dictionary<Peak, List<Compound>> peaks = new Dictionary<Peak, List<Compound>>();

            foreach (Compound compound in this.Compounds)
            {
                foreach (Annotation annotation in compound.Annotations)
                {
                    Peak peak = annotation.Peak;

                    if (peaks.ContainsKey(peak))
                    {
                        peaks[peak].Add(compound);
                    }
                    else
                    {
                        peaks[peak] = new List<Compound>();
                        peaks[peak].Add(compound);
                    }
                }
            }

            // Assign each combination of compounds a unique colour
            Cluster fakeCluster = new Cluster(this.DefaultDisplayName, null);
            List<List<Compound>> uniqueCombinations = new List<List<Compound>>();
            Color[] colors = { Color.Blue, Color.Green, Color.Olive, Color.DarkRed, Color.DarkMagenta, Color.DarkCyan, Color.LightGreen, Color.LightBlue, Color.Magenta, Color.Cyan, Color.YellowGreen };
            int cindex = -1;

            ValueMatrix vm = ValueMatrix.Create(peaks.Keys.ToArray(), true, core, ObsFilter.Empty, false, ProgressReporter.GetEmpty());

            for (int vIndex = 0; vIndex < vm.NumVectors; vIndex++)
            {
                var vec = vm.Vectors[vIndex];
                Peak peak = vec.Peak;
                List<Compound> compounds = peaks[peak];

                // Find or create peak in list
                int uniqueIndex = Maths.FindMatch(uniqueCombinations, compounds);

                if (uniqueIndex == -1)
                {
                    uniqueIndex = uniqueCombinations.Count;
                    uniqueCombinations.Add(compounds); // add list of peaks for this peak
                }

                StringBuilder legend = new StringBuilder();
                int xCount = 0;

                foreach (Annotation potential in peak.Annotations)
                {
                    if (this.Compounds.Contains(potential.Compound))
                    {
                        if (legend.Length != 0)
                        {
                            legend.Append(" OR ");
                        }

                        legend.Append(potential.Compound.DefaultDisplayName);
                    }
                    else
                    {
                        xCount++;
                    }
                }

                if (xCount != 0)
                {
                    legend.Append(" OR " + xCount + " others not in this pathway");
                }

                fakeCluster.Assignments.Add(new Assignment(vec, fakeCluster, double.NaN));

                Color col;

                if (uniqueCombinations[uniqueIndex].Count == 1)
                {
                    col = Color.Black;
                }
                else if (cindex == colors.Length)
                {
                    col = Color.Black;
                }
                else
                {
                    cindex++;

                    if (cindex == colors.Length)
                    {
                        // If there are too many don't bother
                        col = Color.Black;
                        caption2 = "";

                        foreach (var lii in colours)
                        {
                            lii.Value.Colour = Color.Black;
                        }
                    }
                    else
                    {
                        col = colors[cindex % colors.Length];
                    }
                }

                string seriesName = peak.DisplayName + (!peak.Assignments.List.IsEmpty() ? (" (" + StringHelper.ArrayToString(peak.Assignments.Clusters) + ")") : "") + ": " + legend.ToString();
                var li = new LineInfo(seriesName, col, DashStyle.Solid);
                colours.Add(peak, li);
            }

            var r = new StylisedCluster(fakeCluster, this, colours);
            r.IsFake = true;
            r.Highlight = toHighlight;
            r.CaptionFormat = (caption1 + caption2 + caption3);
            r.Source = highlightContents;
            return r;
        }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>
        public string DisplayName
        {
            get { return IVisualisableExtensions.FormatDisplayName(OverrideDisplayName, DefaultDisplayName); }
        }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>
        VisualClass IVisualisable.VisualClass
        {
            get { return VisualClass.Pathway; }
        }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>
        void IVisualisable.RequestContents(ContentsRequest request)
        {
            switch (request.Type)
            {
                case VisualClass.Peak:
                    {
                        request.Text = "Potential peaks of compounds in {0}";
                        request.AddExtraColumn("Compounds", "Compounds potentially representing this peak in {0}.");

                        Dictionary<Peak, List<Compound>> dict = new Dictionary<Peak, List<Compound>>();

                        foreach (Compound c in this.Compounds)
                        {
                            foreach (Annotation p in c.Annotations)
                            {
                                dict.GetOrNew(p.Peak).Add(c);
                            }
                        }

                        foreach (var kvp in dict)
                        {
                            request.Add(kvp.Key, kvp.Value);
                        }
                    }
                    break;

                case VisualClass.Annotation:
                    request.Text = "Annotations with compounds in {0}";

                    foreach (Compound c in this.Compounds)
                    {
                        request.AddRange(c.Annotations);
                    }
                    break;

                case VisualClass.Cluster:
                    {
                        request.Text = "Clusters representing potential peaks of compounds in {0}";
                        request.AddExtraColumn("Peaks", "Number of peaks in this cluster in with compounds in {0}");
                        request.AddExtraColumn("Compounds", "Number of compounds with peaks in this cluster with peaks also in {0}");

                        foreach (Cluster cluster in request.Core.Clusters)
                        {
                            var peaks = new HashSet<Peak>();
                            var comps = new HashSet<Compound>();

                            foreach (Peak peak in cluster.Assignments.Peaks)
                            {
                                foreach (Annotation comp in peak.Annotations)
                                {
                                    comps.Add(comp.Compound);

                                    if (comp.Compound.Pathways.Contains(this))
                                    {
                                        peaks.Add(peak);
                                    }
                                }
                            }

                            if (peaks.Count != 0)
                            {
                                request.Add(cluster, peaks.ToArray(), comps.ToArray());
                            }
                        }
                    }
                    break;

                case VisualClass.Compound:
                    {
                        request.Text = "Compounds in {0}";
                        request.AddExtraColumn("Clusters", "Clusters");  // TODO: This is actually generic to all compounds and not related to this Pathway class

                        foreach (var c in this.Compounds)
                        {
                            HashSet<Cluster> pats = new HashSet<Cluster>();

                            foreach (Annotation p in c.Annotations)
                            {
                                foreach (Cluster pat in p.Peak.Assignments.Clusters)
                                {
                                    pats.Add(pat);
                                }
                            }

                            request.Add(c, (object)pats.ToArray());
                        }
                    }
                    break;

                case VisualClass.Adduct:
                    break;

                case VisualClass.Pathway:
                    {
                        request.Text = "Pathways related to {0}";

                        request.AddRange(RelatedPathways);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// URL used for "view online" option
        /// </summary>
        public string Url
        {
            get
            {
                return "http://mediccyc.noble.org/MEDIC/new-image?type=PATHWAY&object=" + Id; // TODO: No!
            }
        }    

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>              
        IEnumerable<Column> IVisualisable.GetColumns(Core core)
        {            
            var result = new List<Column<Pathway>>();

            result.Add("ID", EColumn.None, λ => λ.Id);
            result.Add("Name", EColumn.Visible, λ => λ.DefaultDisplayName);
            result.Add("Library", EColumn.None, λ => λ.Libraries);
            result.Add("Compounds", EColumn.None, λ => λ.Compounds);
            result.Add("Comment", EColumn.None, λ => λ.Comment);
            result.Add("Libraries", EColumn.None, λ => λ.Libraries);
            result.Add("Related pathways", EColumn.None, λ => λ.RelatedPathways);
            result.Add("URL", EColumn.None, λ => λ.Url);

            core._pathwaysMeta.ReadAllColumns(z => z.MetaInfo, result);

            return result;
        }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>              
        UiControls.ImageListOrder IVisualisable.GetIcon()
        {
            return UiControls.ImageListOrder.Pathway;
        }
    }
}
