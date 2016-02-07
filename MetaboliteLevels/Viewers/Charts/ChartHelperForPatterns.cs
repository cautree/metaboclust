﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetaboliteLevels.Data;
using MetaboliteLevels.Data.DataInfo;
using MetaboliteLevels.Data.Session;
using MetaboliteLevels.Data.Visualisables;
using MetaboliteLevels.Utilities;
using System;
using MetaboliteLevels.Algorithms;
using MetaboliteLevels.Forms;
using MCharting;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace MetaboliteLevels.Viewers.Charts
{
    class LineInfo
    {
        public string SeriesName;
        public Color Colour;
        public DashStyle DashStyle;

        public LineInfo(string seriesName, Color colour, DashStyle dashStyle)
        {
            this.SeriesName = seriesName;
            this.Colour = colour;
            this.DashStyle = dashStyle;
        }
    }

    class ChartHelperForClusters : ChartHelper
    {
        public StylisedCluster SelectedCluster { get; private set; }

        public override IVisualisable CurrentPlot
        {
            get
            {
                return SelectedCluster?.ActualElement;
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ChartHelperForClusters(ISelectionHolder selector, Core core, Control targetSite)
            : base(selector, core, targetSite, true)
        {
            _chart.SelectionChanging += _chart_SelectionChanging;
        }

        /// <summary>
        /// Selects the specified series on the chart
        /// </summary>
        public void SelectSeries(Peak peak)
        {
            if (_chart.SelectedItem.SelectedSeries != null && _chart.SelectedItem.SelectedSeries.Tag == peak)
            {
                // Already selected
                return;
            }

            _chart.SelectedItem = new MChart.Selection(GetSeries(peak).ToArray());
        }

        private List<MChart.Series> GetSeries(Peak peak)
        {
            List<MChart.Series> theSeries = new List<MChart.Series>();

            foreach (MChart.Series series in _chart.CurrentPlot.Series)
            {
                if (series.Tag == peak)
                {
                    theSeries.Add(series);
                }
            }

            return theSeries;
        }

        /// <summary>
        /// Creates a bitmap of the specified cluster
        /// </summary>
        public Bitmap CreateBitmap(int width, int height, StylisedCluster cluster)
        {
            if (cluster.Cluster.Assignments.Count == 0)
            {
                return null;
            }

            Plot(cluster);
            return CreateBitmap(width, height);
        }

        private void _chart_SelectionChanging(object sender, SelectingChangingEventArgs e)
        {
            if (e.Selection.Series.Length == 1)
            {
                Peak peak = (Peak)e.Selection.Series[0].Tag;
                e.Selection = new MChart.Selection(GetSeries(peak).ToArray(), e.Selection.DataPoint, e.Selection.DataIndex);
            }
        }

        /// <summary>
        /// Actual plotting.
        /// </summary>
        /// <param name="stylisedCluster"></param>
        public void Plot(StylisedCluster stylisedCluster)
        {
            Debug.WriteLine("ClusterPlot: " + stylisedCluster);

            // Reset the chart
            MChart.Plot plot = PrepareNewPlot(stylisedCluster != null && !stylisedCluster.IsPreview, (stylisedCluster != null) ? stylisedCluster.Cluster : null);

            // Set the selected cluster
            Cluster p = stylisedCluster != null ? stylisedCluster.Cluster : null;
            Dictionary<Peak, LineInfo> colours = stylisedCluster != null ? stylisedCluster.Colours : null;
            SelectedCluster = stylisedCluster;

            if (stylisedCluster != null)
            {
                SetCaption(stylisedCluster.CaptionFormat, stylisedCluster.ActualElement, stylisedCluster.Source);
            }
            else
            {
                SetCaption("No plot displayed.");
            }

            var core = _core;
            var colourSettings = core.Options.Colours;

            // If there is nothing to plot then exit
            if (p == null)
            {
                CompleteNewPlot(plot);
                return;
            }

            // Sort the variables exemplars top-most
            List<Assignment> toPlot = new List<Assignment>(p.Assignments.List);

            // Too much to plot
            while (toPlot.Count > core.Options.MaxPlotVariables)
            {
                toPlot = toPlot.OrderBy(z => z.Peak.Index).ToList();
                toPlot.RemoveRange(core.Options.MaxPlotVariables, toPlot.Count - core.Options.MaxPlotVariables);
            }

            int numClusterCentres = 0;
            HashSet<GroupInfo> groups = p.Assignments.List.Select(z => z.Vector.Group).Unique(); // array containing groups or null
            bool isMultiGroup = groups.Count != 1;
            GroupInfo[] groupOrder = _core.Groups.OrderBy(GroupInfo.GroupOrderBy).ToArray();

            MChart.Series legendEntry = new MChart.Series();
            legendEntry.Name = "Input vector";
            legendEntry.Style.DrawLines = new Pen(Color.Black, 2);
            plot.LegendEntries.Add(legendEntry);

            // --- LEGEND ---
            var groupLegends = DrawLegend(plot, groupOrder);

            // --- PLOT CLUSTER ASSIGNMENTS ---
            // Iterate variables in cluster
            for (int assignmentIndex = 0; assignmentIndex < toPlot.Count; assignmentIndex++)
            {
                Assignment assignment = toPlot[assignmentIndex];
                Vector vec = assignment.Vector;
                Peak peak = vec.Peak;
                Dictionary<GroupInfo, MChart.Series> vecSeries = new Dictionary<GroupInfo, MChart.Series>();

                double[] vector = vec.Values;

                ///////////////////
                // PLOT THE POINT
                // Okay! Now plot the values pertinent to this peak & type
                if (vec.Conditions != null)
                {
                    IEnumerable<int> order = vec.Conditions.WhichOrder(ConditionInfo.GroupTimeOrder);

                    foreach (int index in order)
                    {
                        // Peak, by condition
                        ConditionInfo cond = vec.Conditions[index];
                        GroupInfo group = cond.Group;
                        MChart.Series series = GetOrCreateSeries(plot, vecSeries, group, vec, stylisedCluster, groupLegends, legendEntry);
                        int originalIndex = index;

                        int typeOffset = GetTypeOffset(groupOrder, group, isMultiGroup);

                        int x = typeOffset + cond.Time;
                        double v = vector[originalIndex];
                        MChart.DataPoint cdp = new MChart.DataPoint(x, v);
                        cdp.Tag = new IntensityInfo(cond.Time, null, cond.Group, v);
                        series.Points.Add(cdp);
                    }
                }
                else if (vec.Observations != null)
                {
                    IEnumerable<int> order = vec.Observations.WhichOrder(ObservationInfo.GroupTimeReplicateOrder);

                    foreach (int index in order)
                    {
                        // Peak, by observation
                        ObservationInfo obs = vec.Observations[index];
                        GroupInfo group = obs.Group;
                        MChart.Series series = GetOrCreateSeries(plot, vecSeries, group, vec, stylisedCluster, groupLegends, legendEntry);

                        int originalIndex = index;

                        int typeOffset = GetTypeOffset(groupOrder, group, isMultiGroup);

                        int x = typeOffset + obs.Time;
                        double v = vector[originalIndex];
                        MChart.DataPoint cdp = new MChart.DataPoint(x, v);
                        cdp.Tag = new IntensityInfo(obs.Time, obs.Rep, obs.Group, v);
                        series.Points.Add(cdp);
                    }
                }
            }

            // --- PLOT CLUSTER CENTRES ---
            if (!stylisedCluster.IsPreview && core.Options.ShowCentres && p.Centres.Count != 0 && p.Centres.Count < 100)
            {
                MChart.Series legendEntry2 = new MChart.Series();
                legendEntry2.Name = (p.Centres.Count == 1) ? "Cluster centre" : "Cluster centres";
                legendEntry2.Style.DrawLines = new Pen(colourSettings.ClusterCentre, 2);
                legendEntry2.Style.DrawLines.DashStyle = DashStyle.Dash;
                legendEntry2.Style.DrawPoints = new SolidBrush(colourSettings.ClusterCentre);
                legendEntry2.Style.DrawPointsSize = 8; // MarkerStyle.Diamond;
                plot.LegendEntries.Add(legendEntry2);

                var templateAssignment = p.Assignments.List.First();

                foreach (double[] centre in p.Centres)
                {
                    MChart.Series series = new MChart.Series();
                    series.ApplicableLegends.Add(legendEntry2);
                    plot.Series.Add(series);
                    series.Name = "Cluster centre #" + (++numClusterCentres);
                    series.Style.DrawLines = new Pen(colourSettings.ClusterCentre, 2);
                    series.Style.DrawLines.DashStyle = DashStyle.Dash;
                    series.Style.DrawPoints = new SolidBrush(colourSettings.ClusterCentre);
                    series.Style.DrawPointsSize = 8; // MarkerStyle.Diamond;

                    if (templateAssignment.Vector.Conditions != null)
                    {
                        IEnumerable<int> order = templateAssignment.Vector.Conditions.WhichOrder(ConditionInfo.GroupTimeOrder);

                        foreach (int index in order)
                        {
                            // Centre, by condition
                            ConditionInfo cond = templateAssignment.Vector.Conditions[index];
                            double dp = centre[index];
                            int x = GetTypeOffset(groupOrder, cond.Group, isMultiGroup) + cond.Time;
                            MChart.DataPoint cdp = new MChart.DataPoint(x, dp);
                            cdp.Tag = new IntensityInfo(cond.Time, null, cond.Group, dp);
                            series.Points.Add(cdp);
                        }
                    }
                    else
                    {
                        IEnumerable<int> order = templateAssignment.Vector.Observations.WhichOrder(ObservationInfo.GroupTimeReplicateOrder);

                        foreach (int index in order)
                        {
                            // Centre, by observation
                            ObservationInfo cond = templateAssignment.Vector.Observations[index];
                            double dp = centre[index];
                            int x = GetTypeOffset(groupOrder, cond.Group, isMultiGroup) + cond.Time;
                            MChart.DataPoint cdp = new MChart.DataPoint(x, dp);
                            cdp.Tag = new IntensityInfo(cond.Time, cond.Rep, cond.Group, dp);
                            series.Points.Add(cdp);
                        }
                    }
                }
            }

            // --- LABELS ---
            DrawLabels(plot, !isMultiGroup, groupOrder);

            // --- COMPLETE ---
            CompleteNewPlot(plot);
        }

        private MChart.Series GetOrCreateSeries(MChart.Plot plot, Dictionary<GroupInfo, MChart.Series> serieses, GroupInfo group, Vector vector, StylisedCluster sp, Dictionary<GroupInfoBase, MChart.Series> groupLegends, MChart.Series lineLegend)
        {
            MChart.Series series;

            if (serieses.TryGetValue(group, out series))
            {
                return series;
            }

            Peak peak = vector.Peak;
            Dictionary<Peak, LineInfo> colours = sp != null ? sp.Colours : null;

            // Each peak + condition gets its own series (yes we end up with lots of series)
            series = new MChart.Series();
            series.Name = vector.ToString() + " : " + group.Name;
            series.ApplicableLegends.Add(groupLegends[group]);
            series.ApplicableLegends.Add(lineLegend);
            plot.Series.Add(series);

            // If the parameters specify a colour for this peak use that, else use the default
            if (colours != null && colours.ContainsKey(peak))
            {
                series.Style.DrawLines = new Pen(colours[peak].Colour);
                series.Style.DrawLines.DashStyle = colours[peak].DashStyle;
                series.Name = colours[peak].SeriesName + " : " + group.Name;
            }
            else
            {
                series.Style.DrawLines = new Pen(group.Colour);
            }

            series.Style.DrawLines.Width = sp.Source == peak ? 2 : 1;
            series.Tag = peak;

            if (sp.Highlight != null)
            {
                foreach (var highlight in sp.Highlight)
                {
                    if (highlight.Peak == vector.Peak
                        && (highlight.Group == null || highlight.Group == group))
                    {
                        series.Style.DrawLines.Color = Color.Red;
                        break;
                    }
                }
            }

            serieses.Add(group, series);

            return series;
        }

        private int GetTypeOffset(IEnumerable<GroupInfo> order, GroupInfo type, bool groupMode)
        {
            if (groupMode)
            {
                return 0;
            }
            else
            {
                return base.GetTypeOffset(order, type);
            }
        }

        public static Bitmap CreatePlaceholderBitmap(IVisualisable x, Size sz)
        {
            int width = sz.Width;
            int height = sz.Height;
            Bitmap result = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.LightGray);
                g.DrawLine(Pens.Silver, 0, 0, width, height);
                g.DrawLine(Pens.Silver, width, 0, 0, height);
                Image icon = UiControls.GetImage(x.GetIcon(), true);
                Rectangle rect = new Rectangle(sz.Width / 2 - icon.Width / 2, sz.Height / 2 - icon.Height / 2, icon.Width, icon.Height);
                g.FillRectangle(Brushes.White, rect);
                g.DrawImage(icon, rect);
                g.DrawRectangle(Pens.Silver, rect);
            }

            return result;
        }
    }
}
