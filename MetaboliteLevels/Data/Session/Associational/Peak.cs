﻿using System;
using System.Collections.Generic;
using System.Linq;
using MetaboliteLevels.Algorithms.Statistics.Configurations;
using MetaboliteLevels.Data.General;
using MetaboliteLevels.Data.Session;
using MetaboliteLevels.Properties;
using MetaboliteLevels.Settings;
using MetaboliteLevels.Utilities;
using MetaboliteLevels.Viewers.Lists;
using System.Drawing;
using MetaboliteLevels.Data.DataInfo;
using MGui.Datatypes;
using MSerialisers;
using MGui.Helpers;
using MetaboliteLevels.Data.Session.Associational;

namespace MetaboliteLevels.Data.Visualisables
{
    /// <summary>
    /// Variables
    /// (aka. Peaks, Columns, Dependent variables, Uncontrolled variables, Y, etc.)
    /// 
    /// The independed variables are in the classes: ConditionInfo, GroupInfo, ObservationInfo
    /// </summary>
    [Serializable]
    [DeferSerialisation]
    class Peak : Associational
    {                                                  
        public const string ID_COLUMN_CLUSTERCOMBINATION = "Clusters\\Combination (for colours)";                           

        /// <summary>
        /// The ID (name) of the peak.
        /// </summary>
        public readonly string Id;        

        /// <summary>
        /// Comment flags.
        /// </summary>
        public List<PeakFlag> CommentFlags = new List<PeakFlag>();        

        /// <summary>
        /// M/Z
        /// </summary>
        public readonly decimal Mz;

        /// <summary>
        /// Retention time
        /// </summary>
        public decimal Rt;

        /// <summary>
        /// Other information the user loaded and may want to view in but the program doesn't actually need.
        /// </summary>
        public readonly MetaInfoCollection MetaInfo = new MetaInfoCollection();

        /// <summary>
        /// Potential compounds.
        /// </summary>
        public readonly List<Annotation> Annotations = new List<Annotation>();

        /// <summary>
        /// Similar peaks.
        /// </summary>
        public readonly List<Peak> SimilarPeaks = new List<Peak>();

        /// <summary>
        /// LC-MS Mode
        /// </summary>
        public readonly ELcmsMode LcmsMode;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Peak(string id, ELcmsMode lcmsmode, decimal mz, decimal rt)
        {                        
            this.Id = id;                               
            this.Mz = mz;
            this.Rt = rt;
            this.LcmsMode = lcmsmode;
        }

        public override EPrevent SupportsHide => EPrevent.Hide;

        /// <summary>     
        /// Default display name.
        /// </summary>
        public override string DefaultDisplayName=> Id;       

        /// <summary>
        /// Toggles a comment flag on and off.
        /// </summary>
        /// <param name="f">The flag to toggle</param>
        /// <returns>The new status of the flag</returns>
        public bool ToggleCommentFlag(PeakFlag f)
        {
            if (!CommentFlags.Contains(f))
            {
                CommentFlags.Add(f);
                return true;
            }
            else
            {
                CommentFlags.Remove(f);
                return false;
            }
        }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>
        public override void FindAssociations(ContentsRequest request)
        {
            switch (request.Type)
            {
                case EVisualClass.Peak:
                    request.Text = "Peaks with similar m/z to {0} (" + Maths.SignificantDigits((double)this.Mz, 5) + ")";
                    request.AddRange(this.SimilarPeaks);
                    break;

                case EVisualClass.Cluster:
                    request.Text = "Assigned clusters for {0}";
                    Assignment.AddHeaders(request);

                    foreach (var ass in FindAssignments(request.Core ))
                    {
                        request.Add(ass.Cluster, ass.GetHeaders());
                    }

                    break;

                case EVisualClass.Assignment:
                    request.Text = "Assignments for {0}";
                    request.AddRange( FindAssignments( request.Core ) );
                    break;


                case EVisualClass.Compound:
                    request.Text = "Potential compounds of {0}";

                    foreach (var annotation in Annotations)
                    {
                        request.Add(annotation.Compound);
                    }

                    break;

                case EVisualClass.Annotation:
                    request.Text = "Annotations for {0}";

                    foreach (var annotation in Annotations)
                    {
                        request.Add(annotation);
                    }

                    break;

                case EVisualClass.Adduct:
                    {
                        request.Text = "Adducts of potential compounds for {0}";

                        HashSet<Adduct> counter = new HashSet<Adduct>();

                        foreach (var c in Annotations)
                        {
                            counter.Add(c.Adduct);
                        }

                        foreach (var kvp in counter)
                        {
                            request.Add(kvp);
                        }
                    }
                    break;

                case EVisualClass.Pathway:
                    {
                        request.Text = "Pathways of potential compounds for {0}";
                        request.AddExtraColumn("Compounds", "Compounds in this pathway potentially represented by {0}.");

                        Dictionary<Pathway, List<Compound>> counter = new Dictionary<Pathway, List<Compound>>();

                        foreach (var pc in Annotations)
                        {
                            foreach (var p in pc.Compound.Pathways)
                            {
                                counter.GetOrNew(p).Add(pc.Compound);
                            }
                        }

                        foreach (var kvp in counter)
                        {
                            request.Add(kvp.Key, kvp.Value);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>
        public override EVisualClass AssociationalClass
        {
            get { return EVisualClass.Peak; }
        }

        /// <summary>
        /// Gets the statistic by the name of "x", otherwise NaN.
        /// </summary>
        internal double GetStatistic(ConfigurationStatistic x)
        {
            double v;

            if (x.Results.Results.TryGetValue(this, out v))
            {
                return v;
            }

            return double.NaN;
        }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>              
        public override IEnumerable<Column> GetColumns(Core core)
        {
            var columns = new List<Column<Peak>>();

            columns.Add("Name", EColumn.Visible, λ => λ.DisplayName);
            columns.Add("Comment", EColumn.None, λ => λ.Comment);
            columns.Add("№ corrections", EColumn.Advanced, λ => core.AllCorrections.WhereEnabled());
            columns.Add("ID", EColumn.None, λ => λ.Id);
            columns.Add("LC-MS mode", EColumn.None, λ => λ.LcmsMode);
            columns.Add("m/z", EColumn.None, λ => λ.Mz);
            columns.Add( "rt", EColumn.None, λ => λ.Rt );

            columns.Add("Clusters\\All", EColumn.None, λ => λ.FindAssignments( core ).Select(z=> z.Cluster ));
            columns.Add(ID_COLUMN_CLUSTERCOMBINATION, EColumn.Advanced, z => StringHelper.ArrayToString(z.FindAssignments( core ).Select(zz=> zz.Cluster )));
            columns.Add("Clusters\\All (scores)", EColumn.Advanced, λ => λ.FindAssignments( core ).Select(z=> z.Score ));

            columns.Add("Clusters\\Unique", EColumn.Advanced, λ => new HashSet<Cluster>(λ.FindAssignments( core ).Select(z=> z.Cluster )).ToArray());
            columns.Add("Clusters\\Grouped", EColumn.Advanced, λ => StringHelper.ArrayToString(λ.FindAssignments( core ).OrderBy(z => z.Vector.Group?.DisplayPriority).Select(z => (z.Vector.Group != null ? (z.Vector.Group.DisplayShortName + "=") : "") + z.Cluster.ShortName)));

            foreach (GroupInfo group in core.Groups)
            {
                var closure = group;
                columns.Add("Clusters\\" + group.DisplayName, EColumn.Advanced, λ => λ.FindAssignments( core ).Where(z => z.Vector.Group == closure).Select(z => z.Cluster).ToArray(), z => closure.Colour);
                columns.Add("Clusters\\" + group.DisplayName + " (scores)", EColumn.Advanced, λ => λ.FindAssignments( core ).Where(z => z.Vector.Group == closure).Select(z => z.Score).ToArray(), z => closure.Colour);
            }

            foreach (PeakFlag flag in core.Options.PeakFlags)
            {
                var closure = flag;
                columns.Add("Flags\\" + flag, EColumn.Advanced, λ => λ.CommentFlags.Contains(closure) ? closure.DisplayName : string.Empty, z => closure.Colour);
            }

            columns.Add("Clusters\\Groupless", EColumn.Advanced, λ => λ.FindAssignments( core ).Where(z => z.Vector.Group == null).Select(z => z.Cluster).ToList());
            columns.Add("Clusters\\Groupless (scores)", EColumn.Advanced, λ => λ.FindAssignments( core ).Where(z => z.Vector.Group == null).Select(z => z.Score).ToList());

            columns.Add("Flags\\All", EColumn.None, λ => StringHelper.ArrayToString(λ.CommentFlags), z=> z.CommentFlags.Count == 1 ? z.CommentFlags[0].Colour : Color.Black );


            columns.Add("Comment", EColumn.None, λ => λ.Comment);

            foreach (ConfigurationStatistic stat in core.AllStatistics.WhereEnabled())
            {
                var closure = stat;
                columns.Add("Statistic\\" + stat.ToString(), EColumn.Statistic, λ => λ.GetStatistic(closure));
                columns[columns.Count - 1].Colour = z => UiControls.StatisticColour( z.GetStatistic( closure ), stat.Results.Min, stat.Results.Max);
            }

            columns.Add( "Annotations", EColumn.None, λ => λ.Annotations );
            columns.Add( "Annotation status", EColumn.None, λ => λ.GetAnnotationStatus() );
            columns.Add( "Annotations\\Compounds", EColumn.Advanced, λ => λ.Annotations.Select(λλ => λλ.Compound));
            columns.Add( "Annotations\\Adducts", EColumn.Advanced, λ => λ.Annotations.Select(λλ => λλ.Adduct));
            columns.Add( "Annotations\\Statuses", EColumn.Advanced, λ => λ.Annotations.Select( λλ => λλ.Status ) );
            columns.Add("Similar peaks", EColumn.None, λ => λ.SimilarPeaks);

            core._peakMeta.ReadAllColumns(z => z.MetaInfo, columns);

            foreach (PeakFilter fi in core.AllPeakFilters)
            {
                var closure = fi;
                columns.Add("Filter\\" + fi.ToString(), EColumn.Advanced, z => fi.Test(z) ? "✔" : "✘");
            }

            return columns;
        }

        public EAnnotation GetAnnotationStatus()
        {
            // IMAGE
            if (this.Annotations.Count == 0)
            {
                return (EAnnotation)(-1);
            }
            else
            {
                return this.Annotations.Max( z => z.Status );
            }
        }

        /// <summary>
        /// IMPLEMENTS IVisualisable
        /// </summary>              
        public override UiControls.ImageListOrder Icon
        {
            get
            {
                // IMAGE
                if (this.Annotations.Count == 0)
                {
                    return UiControls.ImageListOrder.Variable0;
                }
                else
                {
                    switch (this.Annotations.Max( z => z.Status ))
                    {
                        case EAnnotation.Tentative:
                            return UiControls.ImageListOrder.VariableT;

                        case EAnnotation.Affirmative:
                            return UiControls.ImageListOrder.VariableA;

                        case EAnnotation.Confirmed:
                            return UiControls.ImageListOrder.VariableC;

                        default:
                            throw new SwitchException();
                    }
                }
            }
        }

        public IEnumerable<Assignment> FindAssignments( Core core )
        {
            foreach (Cluster clu in core.Clusters)
            {
                foreach (Assignment ass in clu.Assignments.List.Where( z => z.Peak == this ))
                {
                    yield return ass;
                }
            }
        }
    }
}
