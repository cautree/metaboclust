﻿using MetaboliteLevels.Algorithms.Statistics;
using MetaboliteLevels.Controls;
using MetaboliteLevels.Data;
using MetaboliteLevels.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetaboliteLevels.Algorithms.Statistics.Clusterers;
using MetaboliteLevels.Algorithms.Statistics.Clusterers.Legacy;
using MetaboliteLevels.Algorithms.Statistics.Containers;
using MetaboliteLevels.Algorithms.Statistics.Corrections;
using MetaboliteLevels.Algorithms.Statistics.Metrics;
using MetaboliteLevels.Algorithms.Statistics.Statistics;
using MetaboliteLevels.Algorithms.Statistics.Trends;
using MetaboliteLevels.Utilities;

namespace MetaboliteLevels.Algorithms
{
    /// <summary>
    /// Holds the list of all algorithms available to the user.
    /// 
    /// Algorithms are stored with an ID so they can be retrieved without having to store then with all the user's saved data.
    /// </summary>
    class Algo
    {
        // IDs of some algorithms so we can retrieve them elsewhere in the program
        public const string ID_METRIC_EUCLIDEAN = @"EUCLIDEAN";
        public const string ID_METRIC_TTEST = @"T_TEST";
        public const string ID_METRIC_PEARSON = @"PEARSON";
        public const string ID_METRIC_PEARSONDISTANCE = @"PEARSON_DISTANCE";
        public const string ID_TREND_FLAT_MEAN = @"FLAT_MEAN";
        public const string ID_TREND_MOVING_MEDIAN = @"MOVING_MEDIAN";
        public const string ID_TREND_MOVING_MINIMUM = @"MOVING_MIN";
        public const string ID_TREND_MOVING_MAXIMUM = @"MOVING_MAX";
        public const string ID_STATS_MIN = @"STATS_MIN";
        public const string ID_STATS_MAX = @"STATS_MAX";
        public const string ID_STATS_ABSMAX = @"STATS_ABSMAX";
        public const string ID_KMEANS = @"KMEANS";
        public const string ID_KMEANSWIZ = @"KMEANSWIZ";
        public const string ID_DKMEANSPPWIZ = @"DKMEANSPPWIZ";
        public const string ID_PATFROMPATH = @"PATFROMPATH";

        // R scripts for some tests
        private const string SCRIPT_TTEST = @"t.test(a, b)$p.value";
        private const string SCRIPT_PEARSON = @"cor(a, b)";
        private const string SCRIPT_KMEANS = "## k = integer\r\nkmeans(x, k)$cluster";

        // Our stores of algorithms, by category
        public readonly StatCollection<AlgoBase> All = new StatCollection<AlgoBase>();                      // all stats
        public readonly StatCollection<MetricBase> Metrics = new StatCollection<MetricBase>();              // metrics which support quick calculate
        public readonly StatCollection<StatisticBase> Statistics = new StatCollection<StatisticBase>();     // stats and metrics
        public readonly StatCollection<ClustererBase> Clusterers = new StatCollection<ClustererBase>();         // cluster algos
        public readonly StatCollection<TrendBase> Trends = new StatCollection<TrendBase>();                 // trend algos
        public readonly StatCollection<CorrectionBase> Corrections = new StatCollection<CorrectionBase>();  // correction algos

        // Instance of this class
        public static Algo Instance { get; private set; }

        /// <summary>
        /// Initialises this singleton
        /// </summary>
        public static void Initialise()
        {
            Instance = new Algo();
        }

        /// <summary>
        /// Constructior
        /// </summary>
        private Algo()
        {
            Rebuild();
        }

        /// <summary>
        /// Rebuilds the cache of statistics
        /// </summary>
        internal void Rebuild()
        {
            // Clear dat
            All.Clear();
            Statistics.Clear();
            Trends.Clear();
            Clusterers.Clear();
            Metrics.Clear();
            Corrections.Clear();

            // Metrics
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.Canberra));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.Chebyshev));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.Cosine));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.Euclidean, ID_METRIC_EUCLIDEAN, "Euclidean"));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.Hamming));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.Jaccard));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.MAE));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.Manhattan));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.MSE));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.Pearson, ID_METRIC_PEARSONDISTANCE, "Pearson (distance)"));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.SAD));
            Statistics.Add(new MetricInbuilt(MathNet.Numerics.Distance.SSD));
            Statistics.Add(new MetricInbuilt(Maths.Qian, @"QIAN", "Qian"));
            Statistics.Add(new MetricInbuilt(Maths.QianDistance, @"QIAN_DISTANCE", "Qian's S (-) (distance)"));

            Statistics.Add(new MetricScript(SCRIPT_TTEST, ID_METRIC_TTEST, "t-test") { Description = "Conducts a t-test and returns the p-value" });
            Statistics.Add(new MetricScript(SCRIPT_PEARSON, ID_METRIC_PEARSON, "Pearson"));

            // Statistics
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.InterquartileRange));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.Kurtosis));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.LowerQuartile));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.Maximum));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.Mean));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.Median));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.Minimum));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.RootMeanSquare));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.Skewness));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.StandardDeviation));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.UpperQuartile));
            Statistics.Add(new StatisticInbuilt(MathNet.Numerics.Statistics.Statistics.Variance));

            Statistics.Add(new StatisticPcaAnova(@"PCA_ANOVA", "PCA-ANOVA") { Description = "Constructs a matrix representing a CONDITION and REPLICATE for each row (TIME for each column) and uses PCA to reduce this to 1 dimension. Uses ANOVA to determine if there is a difference between groups of replicates for each CONDITION. It is recommended to constrain this method to CONDITIONS of interest and only use REPLICATES with comparable sets (CONDITIONs and TIMEs) of data since missing values are guessed based on the average of the other replicates." });

            // Derived statistics
            Statistics.Add(new StatisticConsumer(Maths.Mean, "STATS_MEAN", "Mean:"));
            Statistics.Add(new StatisticConsumer(Maths.Median, "STATS_MEDIAN", "Median:"));
            Statistics.Add(new StatisticConsumer(Maths.AbsMax, ID_STATS_ABSMAX, "Absolute Maximum:"));
            Statistics.Add(new StatisticConsumer(Maths.AbsMin, "STATS_ABSMIN", "Absolute Minimum:"));
            Statistics.Add(new StatisticConsumer(Maths.Max, ID_STATS_MAX, "Maximum:"));
            Statistics.Add(new StatisticConsumer(Maths.Min, ID_STATS_MIN, "Minimum:"));

            // Trends
            Trends.Add(new TrendFlatLine(Maths.Mean, ID_TREND_FLAT_MEAN, "Straight line across mean"));
            Trends.Add(new TrendFlatLine(Maths.Median, "FLAT_MEDIAN", "Straight line across median"));

            Trends.Add(new TrendAverage(Maths.Median, ID_TREND_MOVING_MEDIAN, "Moving median"));
            Trends.Add(new TrendAverage(Maths.Mean, @"MOVING_MEAN", "Moving mean"));
            Trends.Add(new TrendAverage(Maths.Min, ID_TREND_MOVING_MINIMUM, "Moving minimum"));
            Trends.Add(new TrendAverage(Maths.Max, ID_TREND_MOVING_MAXIMUM, "Moving maximum"));

            // Corrections
            Corrections.Add(new CorrectionScript(@"scale(y)", @"SCALE", "UV Scale and centre"));
            Corrections.Add(new CorrectionScript(@"scale(y, center = FALSE)", @"SCALE_NO_C", "UV Scale"));
            Corrections.Add(new CorrectionScript(@"scale(y, scale = FALSE)", @"CENTRE_NO_S", "Center"));
            Corrections.Add(new CorrectionDirtyRectify(@"ZERO_MISSING", "Zero invalid values"));

            // Clusterers
            Clusterers.Add(new ClustererScript(SCRIPT_KMEANS, ID_KMEANS, "k-means"));
            Clusterers.Add(new LegacyKMeansClusterer(ID_KMEANSWIZ, "k-means (legacy)"));
            Clusterers.Add(new LegacyDKMeansPPClusterer(ID_DKMEANSPPWIZ, "d-k-means++ (legacy)"));
            Clusterers.Add(new LegacyPathwayClusterer(ID_PATFROMPATH, "pathways (legacy)"));
            Clusterers.Add(new ClustererReclusterer("RECLUSTERER", "Cluster to existing clusters"));
            Clusterers.Add(new ClustererUniqueness("UNIQCLUST", "Find unique clusters"));

            // From files
            PopulateFiles(Statistics, UiControls.EInitialFolder.FOLDER_STATISTICS, (txt, id, name) => new StatisticScript(txt, id, name));
            PopulateFiles(Statistics, UiControls.EInitialFolder.FOLDER_METRICS, (txt, id, name) => new MetricScript(txt, id, name));
            PopulateFiles(Trends, UiControls.EInitialFolder.FOLDER_TRENDS, (txt, id, name) => new TrendScript(txt, id, name));
            PopulateFiles(Clusterers, UiControls.EInitialFolder.FOLDER_CLUSTERERS, (txt, id, name) => new ClustererScript(txt, id, name));
            PopulateFiles(Corrections, UiControls.EInitialFolder.FOLDER_CORRECTIONS, (txt, id, name) => new CorrectionScript(txt, id, name));

            Metrics.AddRange(Statistics.Where(z => z is MetricBase && z.GetParams().SupportsQuickCalculate).Cast<MetricBase>());
            All.AddRange(Statistics);
            All.AddRange(Trends);
            All.AddRange(Clusterers);
            All.AddRange(Corrections);
        }   

private static void PopulateFiles<T>(StatCollection<T> col, UiControls.EInitialFolder folder, Func<string, string, string, T> func)
            where T : AlgoBase
        {
            foreach (string fn in Directory.GetFiles(UiControls.GetOrCreateFixedFolder(folder), "*.r"))
            {
                string id = GetId(folder, fn);
                string name = Path.GetFileName(fn);
                col.Add(func(File.ReadAllText(fn), id, name));
            }
        }

        /// <summary>
        /// Retrieves/generates the ID for a script based algorithm loaded from disk.
        /// </summary>
        /// <param name="folder">Folder the script is in (one of the FOLDER_* constants)</param>
        /// <param name="fileName">Full or partial filename of the script</param>
        /// <returns>The ID</returns>
        public static string GetId(UiControls.EInitialFolder folder, string fileName)
        {
            string id = "SCRIPT:" + folder.ToUiString().ToUpper() + "\\" + Path.GetFileNameWithoutExtension(fileName);
            return id;
        }
    }
}
