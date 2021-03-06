﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetaboliteLevels.Data.Algorithms.Definitions.Clusterers;
using MetaboliteLevels.Data.Algorithms.Definitions.Statistics;
using MetaboliteLevels.Data.Database;
using MetaboliteLevels.Data.Session.Associational;
using MetaboliteLevels.Data.Session.General;
using MetaboliteLevels.Data.Session.Main;
using MetaboliteLevels.Gui.Forms.Selection;
using MetaboliteLevels.Properties;
using MGui.Datatypes;
using MGui.Helpers;
using RDotNet;

namespace MetaboliteLevels.Data.Algorithms.General
{
    /// <summary>
    /// Contains the concrete implementations of <see cref="IAlgoParameterType"/>.
    /// </summary>
    internal static class AlgoParameterTypes
    {
        public static IAlgoParameterType Integer = new _Integer();
        public static IAlgoParameterType Boolean = new _Boolean();
        public static IAlgoParameterType Double = new _Double();
        public static IAlgoParameterType WeakRefStatisticArray = new _WeakRefStatisticArray();
        public static IAlgoParameterType WeakRefPeak = new _WeakRefPeak();
        public static IAlgoParameterType Group = new _Group();
        public static IAlgoParameterType WeakRefConfigurationClusterer = new _WeakRefConfigurationClusterer();
        public static IAlgoParameterType WeakRefClusterArray = new _WeakRefClusterArray();

        public static Dictionary<string, IAlgoParameterType> GetKeys()
        {
            Dictionary<string, IAlgoParameterType> result = new Dictionary<string, IAlgoParameterType>();

            foreach (IAlgoParameterType x in GetAll())
            {
                foreach (string name in x.Aliases)
                {
                    result.Add( name.ToUpper(), x );
                }
            }

            return result;
        }

        /// <summary>
        /// Converts a single parameter to a string.
        /// </summary>                                    
        /// <param name="param">Value to convert</param>
        /// <returns>Value as a string</returns>
        internal static string ToString( object param )
        {
            return GetAll().Select( z => z.TryToString( param ) ).FirstOrDefault( z => z != null );
        }

        public static IEnumerable<IAlgoParameterType> GetAll()
        {
            yield return Integer;
            yield return Boolean;
            yield return Double;
            yield return WeakRefStatisticArray;
            yield return WeakRefPeak;
            yield return Group;
            yield return WeakRefConfigurationClusterer;
            yield return WeakRefClusterArray;
        }

        private abstract class _AlgoParameterType<T> : IAlgoParameterType
        {
            public virtual string Name => Aliases.First();

            public abstract IEnumerable<string> Aliases { get; }

            public WeakReference[] TrackChanges( object param )
            {
                return this.OnTrackChanges( (T)param );
            }

            protected virtual WeakReference[] OnTrackChanges( T param )
            {
                return null;
            }

            public object FromString( FromStringArgs args )
            {
                return OnFromString( args );
            }

            public object Browse( Form owner, Core core, object value )
            {
                return OnBrowse( owner, core, value );
            }

            public virtual void SetSymbol( REngine rEngine, string name, object value )
            {
                throw new InvalidOperationException( "ApplyArgs: " + ToString() + " on " + name );
            }

            protected abstract object OnBrowse( Form owner, Core core, object value );

            protected abstract object OnFromString( FromStringArgs args );

            public override string ToString()
            {
                return Name.ToSmallCaps();
            }

            public string TryToString( object x )
            {
                if (x is T)
                {
                    return OnTryToString( (T)x );
                }

                return null;
            }

            internal abstract string OnTryToString( T x );
        }

        private class _Boolean : _AlgoParameterType<bool>
        {
            public override IEnumerable<string> Aliases => new[] { "Boolean", "Bool" };

            protected override object OnBrowse( Form owner, Core core, object value )
            {
                MsgBoxButton[] btns =
               {
                    new MsgBoxButton( "YES", Resources.MnuUp, DialogResult.Yes ),
                    new MsgBoxButton( "NO", Resources.MnuDown, DialogResult.No ),
                    new MsgBoxButton( "Cancel", Resources.MnuCancel, DialogResult.Cancel )
                };

                switch (FrmMsgBox.Show( owner, "Select Value", null, "Select a value", Resources.MsgHelp, btns, DialogResult.Cancel, DialogResult.Cancel ))
                {
                    case DialogResult.Yes:
                        return (object)true;

                    case DialogResult.No:
                        return (object)false;

                    default:
                        return null;
                }
            }

            protected override object OnFromString( FromStringArgs args )
            {
                switch (args.Text.Trim().ToUpper())
                {
                    case "1":
                    case "YES":
                    case "Y":
                    case "TRUE":
                    case "T":
                        return true;

                    case "0":
                    case "NO":
                    case "N":
                    case "FALSE":
                    case "F":
                        return false;

                    default:
                        return null;
                }
            }

            internal override string OnTryToString( bool x )
            {
                return x ? "Yes" : "No";
            }

            public override void SetSymbol( REngine rEngine, string name, object value )
            {
                rEngine.SetSymbol( name, rEngine.CreateLogical( (bool)value ) );
            }
        }

        private class _Integer : _AlgoParameterType<int>
        {
            public override IEnumerable<string> Aliases => new[] { "Integer", "Int" };

            protected override object OnFromString( FromStringArgs args )
            {
                int vi;

                if (args.Text == "MAX")
                {
                    return int.MaxValue;
                }
                else if (args.Text == "MIN")
                {
                    return int.MinValue;
                }
                else if (int.TryParse( args.Text, out vi ))
                {
                    return vi;
                }
                else
                {
                    args.Error = $"{{{args.Text}}} is not a valid integer.";
                    return null;
                }
            }

            internal override string OnTryToString( int x )
            {
                switch (x)
                {
                    case int.MaxValue:
                        return "MAX";

                    case int.MinValue:
                        return "MIN";

                    default:
                        return x.ToString();
                }
            }

            protected override object OnBrowse( Form owner, Core core, object value )
            {
                MsgBoxButton[] btns =
                {
                    new MsgBoxButton( "MAX", Resources.MnuUp, DialogResult.Yes ),
                    new MsgBoxButton( "MIN", Resources.MnuDown, DialogResult.No ),
                    new MsgBoxButton( "Cancel", Resources.MnuCancel, DialogResult.Cancel )
                };

                switch (FrmMsgBox.Show( owner, "Select Integer", null, "Select a value or enter a custom value into the textbox", Resources.MsgHelp, btns, DialogResult.Cancel, DialogResult.Cancel ))
                {
                    case DialogResult.Yes:
                        return (object)int.MaxValue;

                    case DialogResult.No:
                        return (object)int.MinValue;

                    default:
                        return null;
                }
            }

            public override void SetSymbol( REngine rEngine, string name, object value )
            {
                rEngine.SetSymbol( name, rEngine.CreateInteger( (int)value ) );
            }
        }

        private class _Double : _AlgoParameterType<double>
        {
            public override IEnumerable<string> Aliases => new[] { "Double", "Numeric", "Real" };

            protected override object OnFromString( FromStringArgs args )
            {
                double vd;

                if (args.Text == "MAX")
                {
                    return double.MaxValue;
                }
                else if (args.Text == "MIN")
                {
                    return double.MinValue;
                }
                else if (double.TryParse( args.Text, out vd ))
                {
                    return vd;
                }
                else
                {
                    args.Error = $"{{{args.Text}}} is not a valid number.";
                    return null;
                }
            }

            internal override string OnTryToString( double x )
            {
                if (x == double.MaxValue)
                {
                    return "MAX";
                }
                else if (x == double.MinValue)
                {
                    return "MIN";
                }
                else
                {
                    return x.ToString();
                }
            }

            public override void SetSymbol( REngine rEngine, string name, object value )
            {
                rEngine.SetSymbol( name, rEngine.CreateNumeric( (double)value ) );
            }

            protected override object OnBrowse( Form owner, Core core, object value )
            {
                {
                    MsgBoxButton[] btns =
                    {
                        new MsgBoxButton( "MAX", Resources.MnuUp, DialogResult.Yes ),
                        new MsgBoxButton( "MIN", Resources.MnuDown, DialogResult.No ),
                        new MsgBoxButton( "Cancel", Resources.MnuCancel, DialogResult.Cancel )
                    };

                    switch (FrmMsgBox.Show( owner, "Select Double", null, "Select a value or enter a custom value into the textbox", Resources.MsgHelp, btns, DialogResult.Cancel, DialogResult.Cancel ))
                    {
                        case DialogResult.Yes:
                            return (object)double.MaxValue;

                        case DialogResult.No:
                            return (object)double.MinValue;

                        default:
                            return null;
                    }
                }
            }
        }

        /// <summary>
        /// Used for converting a list of parameters to and from the individual parameter values.
        /// </summary>
        public static SpreadsheetReader ExternalConvertor = new SpreadsheetReader()
        {
            OpenQuote = '{',
            CloseQuote = '}',
            Delimiter = ',',
            WriteSpaces = 1,
        };

        /// <summary>
        /// Used for converting a single array-type parameter to and from an array.
        /// </summary>
        public static SpreadsheetReader InternalConvertor = new SpreadsheetReader()
        {
            OpenQuote = '"',
            CloseQuote = '"',
            Delimiter = ',',
            WriteSpaces = 1,
        };

        private static string ElementToString<T>( WeakReference<T> arg )
                 where T : Visualisable
        {
            var arga = arg.GetTarget();

            if (arga == null)
            {
                return "(missing)";
            }

            return arga.DisplayName;
        }

        [Name( "Statistic[]" )]
        private class _WeakRefStatisticArray : _AlgoParameterType<WeakReference<ConfigurationStatistic>[]>
        {
            public override IEnumerable<string> Aliases => new[] { "Statistic[]", "ConfigurationStatistic[]", "WeakReference<ConfigurationStatistic>[]" };

            protected override WeakReference[] OnTrackChanges( WeakReference<ConfigurationStatistic>[] param )
            {
                return param.Select( z => z.GetTargetOrThrow().Results.ToWeakReferenceO() ).ToArray();
            }

            internal override string OnTryToString( WeakReference<ConfigurationStatistic>[] x )
            {
                return InternalConvertor.WriteFields( x.Select( ElementToString ) );
            }

            protected override object OnFromString( FromStringArgs args )
            {
                string[] e2 = InternalConvertor.ReadFields( args.Text );

                WeakReference<ConfigurationStatistic>[] r = new WeakReference<ConfigurationStatistic>[e2.Length];
                var opts = args.Core.Statistics;

                for (int n = 0; n < e2.Length; n++)
                {
                    var x = opts.FirstOrDefault( z => z.DisplayName == e2[n] );

                    if (x == null)
                    {
                        args.Error = $"There is no statistic with the name {{{e2[n]}}}.";
                        return null;
                    }

                    r[n] = new WeakReference<ConfigurationStatistic>( x );
                }

                return r;
            }

            protected override object OnBrowse( Form owner, Core core, object value )
            {
                WeakReference<ConfigurationStatistic>[] typedValue = (WeakReference<ConfigurationStatistic>[])value;
                IEnumerable<ConfigurationStatistic> validSelection = typedValue?.Select( z => z.GetTarget() ).Where( z => z != null );
                IEnumerable<ConfigurationStatistic> newSelection = DataSet.ForStatistics( core ).ShowCheckList( owner, validSelection );

                if (newSelection == null)
                {
                    return null;
                }

                return newSelection.Select( z => new WeakReference<ConfigurationStatistic>( z ) ).ToArray();
            }
        }

        private class _WeakRefPeak : _AlgoParameterType<WeakReference<Peak>>
        {
            public override IEnumerable<string> Aliases => new[] { "Peak", "WeakReference<Peak>" };

            protected override object OnBrowse( Form owner, Core core, object value )
            {
                var sel = DataSet.ForPeaks( core ).ShowList( owner, ((WeakReference<Peak>)value).GetTarget() );

                if (sel == null)
                {
                    return null;
                }

                return new WeakReference<Peak>( sel );
            }

            protected override object OnFromString( FromStringArgs args )
            {
                string peakName = args.Text;

                Peak peak = args.Core.Peaks.FirstOrDefault( z => z.DisplayName == peakName );

                if (peak != null)
                {
                    return new WeakReference<Peak>( peak );
                }
                else
                {
                    args.Error = $"There is no peak with the name {{{peakName}}}.";
                    return null;
                }
            }

            internal override string OnTryToString( WeakReference<Peak> x )
            {
                var xx = x.GetTarget();

                if (xx == null)
                {
                    return "(Missing)";
                }

                return xx.DisplayName;
            }
        }

        private class _Group : _AlgoParameterType<GroupInfo>
        {
            public override IEnumerable<string> Aliases => new[] { "Group", "GroupInfo" };

            protected override object OnFromString( FromStringArgs args )
            {
                string el = args.Text.Trim();
                var res = args.Core.Groups.FirstOrDefault( z => z.Id == el );

                if (res == null)
                {
                    args.Error = $"There is no group with the name {{{el}}}";
                }

                return res;
            }

            protected override object OnBrowse( Form owner, Core core, object value )
            {
                return DataSet.ForGroups( core ).ShowList( owner, (GroupInfo)value );
            }

            internal override string OnTryToString( GroupInfo x )
            {
                return x.Id;
            }
        }

        private class _WeakRefConfigurationClusterer : _AlgoParameterType<WeakReference<ConfigurationClusterer>>
        {
            public override IEnumerable<string> Aliases => new[] { "Clusterer", "ConfigurationClusterer", "WeakReference<ConfigurationClusterer>" };

            protected override WeakReference[] OnTrackChanges( WeakReference<ConfigurationClusterer> param )
            {
                return new[] { param.GetTargetOrThrow().Results.ToWeakReferenceO() };
            }

            protected override object OnBrowse( Form owner, Core core, object value )
            {
                ConfigurationClusterer def = ((WeakReference<ConfigurationClusterer>)value).GetTarget();
                var sel = DataSet.ForClusterers( core ).ShowList( owner, def );

                if (sel == null)
                {
                    return null;
                }

                return new WeakReference<ConfigurationClusterer>( sel );
            }

            protected override object OnFromString( FromStringArgs args )
            {
                var x = args.Core.Clusterers.FirstOrDefault( z => z.DisplayName == args.Text );

                if (x == null)
                {
                    args.Error = $"There is no clusterer with the name {{{args.Text}}}.";
                    return null;
                }

                return new WeakReference<ConfigurationClusterer>( x );
            }

            internal override string OnTryToString( WeakReference<ConfigurationClusterer> x )
            {
                var xx = x.GetTarget();

                if (xx == null)
                {
                    return "(Missing)";
                }

                return xx.DisplayName;
            }
        }

        private class _WeakRefClusterArray : _AlgoParameterType<WeakReference<Cluster>[]>
        {
            public override IEnumerable<string> Aliases => new[] { "Cluster[]", "WeakReference<Cluster>[]" };

            internal override string OnTryToString( WeakReference<Cluster>[] x )
            {
                return InternalConvertor.WriteFields( x.Select( ElementToString ) );
            }

            protected override object OnFromString( FromStringArgs args )
            {
                string[] elems = InternalConvertor.ReadFields( args.Text );

                WeakReference<Cluster>[] r = new WeakReference<Cluster>[elems.Length];

                for (int n = 0; n < elems.Length; ++n)
                {
                    Cluster c = args.Core.Clusters.FirstOrDefault( z => z.DisplayName == elems[n] );

                    if (c == null)
                    {
                        args.Error = $"There is no cluster with the name {{{elems[n]}}}.";
                        return null;
                    }

                    r[n] = new WeakReference<Cluster>( c );
                }

                return r;
            }

            protected override object OnBrowse( Form owner, Core core, object value )
            {
                Cluster def = ((WeakReference<Cluster>)value).GetTarget();
                var sel = DataSet.ForClusters( core ).ShowList( owner, def );

                if (sel == null)
                {
                    return null;
                }

                return new WeakReference<Cluster>( sel );
            }
        }
    }
}
