﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaboliteLevels.Data.Algorithms.Definitions.Statistics;
using MetaboliteLevels.Data.Session.Associational;
using MetaboliteLevels.Gui.Controls.Lists;
using MetaboliteLevels.Properties;
using MGui.Datatypes;
using MGui.Helpers;

namespace MetaboliteLevels.Data.Session.Main
{
    /// <summary>
    /// Filters (Peak and Observations)
    /// 
    /// IMMUTABLE (exc. names, comments and enabled status)
    /// </summary>
    [Serializable]
    internal abstract class Filter : Visualisable
    {         
        /// <summary>
        /// CONSTRUCTOR
        /// </summary>  
        protected Filter(string overrideDisplayName, string comment)
        {
            this.OverrideDisplayName = overrideDisplayName;
            this.Comment = comment;
        }

        #region IVisualisable             

        /// <summary>
        /// IMPLEMENTS IVisualisable.
        /// </summary>
        [XColumn]
        public override string DefaultDisplayName => this.ParamsAsString();

        /// <summary>
        /// IMPLEMENTS IVisualisable.
        /// </summary>
        public override Image Icon=> Resources.MnuFilter;    

        #endregion

        /// <summary>
        /// Describes the filter.
        /// </summary>          
        [XColumn("Parameters")]
        public abstract string ParamsAsString();

        public enum EStatOperator
        {
            [Name("<")]
            LessThan,

            [Name("≤")]
            LessThanEq,

            [Name(">")]
            MoreThan,

            [Name("≥")]
            MoreThanEq,
        }

        [Flags]
        public enum ESetOperator
        {
            [Name("_Not")]
            Negate = 1,

            [Name("any of")]
            Any = 2,

            [Name( "all of" )]
            All = 4,

            [Name("limited to")]
            Limited = 8,

            [Name( "exactly" )]
            Exact = 16,

            [Name("none of")]
            NotAny = Any | Negate,

            [Name("not all of")]
            NotAll = All | Negate,

            [Name( "not limited to" )]
            NotLimited = Limited | Negate,

            [Name( "not exactly" )]
            NotExact = Exact | Negate,

            [Name("_BasicMask")]
            BasicMask = Any | All | Limited | Exact
        }

        [Flags]
        public enum ELimitedSetOperator
        {
            [Name( "_Not" )]
            Negate = 1,

            [Name( "any of" )]
            Any = 2,

            [Name( "all of" )]
            All = 4,   

            [Name( "none of" )]
            NotAny = Any | Negate,

            [Name( "not all of" )]
            NotAll = All | Negate,

            [Name( "_BasicMask" )]
            BasicMask = Any | All
        }

        public enum ELogicOperator
        {
            [Name("and")]
            And,

            [Name("or")]
            Or,
        }

        public enum EElementOperator
        {
            [Name("is")]
            Is,

            [Name("is not")]
            IsNot,
        }      
    }

    /// <summary>
    /// Filters (Peak and Observations)
    /// 
    /// IMMUTABLE (exc. names, comments and enabled status)
    /// </summary>
    /// <typeparam name="T">Type to filter (currently Peak or ObservationInfo)</typeparam>
    [Serializable]
    internal abstract class Filter<T> : Filter
    {
        /// <summary>
        /// Conditions of the filter.
        /// </summary>
        public readonly ReadOnlyCollection<ConditionBase> Conditions;

        /// <summary>
        /// Ctor
        /// </summary>
        protected Filter(string overrideDisplayName, string comment, IEnumerable<ConditionBase> filters)
            : base(overrideDisplayName, comment)
        {
            if (filters != null)
            {
                this.Conditions = new ReadOnlyCollection<ConditionBase>(filters.ToList());
            }
            else
            {
                this.Conditions = new ReadOnlyCollection<ConditionBase>(new List<ConditionBase>());
            }
        }

        /// <summary>
        /// Base class for condition
        /// </summary>
        [Serializable]
        public abstract class ConditionBase : Visualisable
        {
            public readonly ELogicOperator CombiningOperator;
            public readonly bool Negate;         

            protected ConditionBase(ELogicOperator op, bool negate)
            {                             
                this.Negate = negate;
                this.CombiningOperator = op;
            }                                        

            public abstract override string DefaultDisplayName { get; }              

            public override Image Icon=> Resources.MnuFilter;

            public bool Preview(T target)
            {
                return (this.Test(target) ^ this.Negate);
            }                                   

            public bool Test(bool previousResult, T target)
            {
                if (this.Hidden)
                {
                    return true;
                }

                switch (this.CombiningOperator)
                {
                    case ELogicOperator.And:
                        return previousResult && (this.Test(target) ^ this.Negate);

                    case ELogicOperator.Or:
                        return previousResult || (this.Test(target) ^ this.Negate);

                    default:
                        throw new SwitchException(this.CombiningOperator);
                }
            }

            protected abstract bool Test(T target);
        }

        /// <summary>
        /// Returns the conditions of the filter as a string (non reversable).
        /// </summary>
        public sealed override string ParamsAsString()
        {
            if (this.IsEmpty)
            {
                return "No filter";
            }

            StringBuilder sb = new StringBuilder();
            bool firstTest = true;

            for (int index = 0; index < this.Conditions.Count; index++)
            {
                ConditionBase test = this.Conditions[index];

                if (test.Hidden)
                {
                    continue;
                }

                if (firstTest)
                {
                    firstTest = false;
                }
                else
                {
                    switch (test.CombiningOperator)
                    {
                        case ELogicOperator.And:
                            sb.Append(" ⋏ ");
                            break;

                        case ELogicOperator.Or:
                            sb.Append(" ⋎ ");
                            break;
                    }
                }

                if (test.Negate)
                {
                    sb.Append("¬");
                }

                if (this.Conditions.Count != 1)
                {
                    sb.Append("(");
                }

                sb.Append(test.ToString());

                if (this.Conditions.Count != 1)
                {
                    sb.Append(")");
                }
            }

            if (firstTest)
            {
                return "No filter";
            }

            return sb.ToString();
        }     

        /// <summary>
        /// Returns if the filter allows everything through.
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                return this.Conditions.Count == 0;
            }
        }

        /// <summary>
        /// Results of testing a list.
        /// 
        /// IMMUTABLE
        /// </summary>
        public class Results
        {
            public readonly ReadOnlyCollection<T> Passed;
            public readonly ReadOnlyCollection<T> Failed;

            public Results(ReadOnlyCollection<T> passed, ReadOnlyCollection<T> failed)
            {
                this.Passed = passed;
                this.Failed = failed;
            }
        }

        /// <summary>
        /// Tests a target against the filter.
        /// </summary>
        public bool Test(T target)
        {
            bool test = true;

            foreach (ConditionBase t in this.Conditions)
            {
                test = t.Test(test, target);
            }

            return test;
        }

        /// <summary>
        /// Tests a set of targets against the filter.
        /// </summary>
        public Results Test(IEnumerable<T> targets)
        {
            List<T> passed = new List<T>();
            List<T> failed = new List<T>();

            foreach (T p in targets)
            {
                if (this.Test(p))
                {
                    passed.Add(p);
                }
                else
                {
                    failed.Add(p);
                }
            }

            return new Results(passed.AsReadOnly(), failed.AsReadOnly());
        }

        protected static bool CompareSets<U>(ESetOperator op, IEnumerable<U> test, IEnumerable<U> target)
        {
            bool negate = op.HasFlag(ESetOperator.Negate);

            switch (op & ESetOperator.BasicMask)
            {
                case ESetOperator.Any:
                    return target.Any( test.Contains ) ^ negate;

                case ESetOperator.All:
                    return target.All( test.Contains ) ^ negate;

                case ESetOperator.Limited:
                    return test.All( target.Contains ) ^ negate;

                case ESetOperator.Exact:
                    return (target.All(test.Contains) && test.All( target.Contains )) ^ negate;

                default:
                    throw new SwitchException(op);
            }
        }

        protected static bool CompareElement<U>(EElementOperator op, U p, IEnumerable<U> compareTo)
        {
            switch (op)
            {
                case EElementOperator.Is:
                    return compareTo.Contains(p);

                case EElementOperator.IsNot:
                    return !compareTo.Contains(p);

                default:
                    throw new SwitchException(op);
            }
        }
    }

    [Serializable]
    internal class ObsFilter : Filter<ObservationInfo>
    {
        public static ObsFilter Empty = new ObsFilter(null, null, null);

        public ObsFilter(string overrideDisplayName, string comment, IEnumerable<Condition> filters)
            : base(overrideDisplayName, comment, filters)
        {
            // NA
        }

        [Serializable]
        public abstract class Condition : ConditionBase
        {
            public Condition(ELogicOperator op, bool negate)
                : base(op, negate)
            {
                // NA
            }
        }

        [Serializable]
        public sealed class ConditionGroup : Condition
        {
            public readonly ReadOnlyCollection<GroupInfo> Possibilities;
            public readonly EElementOperator Operator;

            public ConditionGroup(ELogicOperator lop, bool negate, EElementOperator op, IEnumerable<GroupInfo> enu)
                : base(lop, negate)
            {
                this.Operator = op;
                this.Possibilities = enu.ToList().AsReadOnly();
            }

            protected override bool Test(ObservationInfo ci)
            {
                return CompareElement(this.Operator, ci.Group, this.Possibilities);
            }

            public override string DefaultDisplayName
            {
                get
                {
                    return "Gʀᴏᴜᴘ " + this.Operator.ToUiString() + " {" + StringHelper.ArrayToString( this.Possibilities, z => z.DisplayShortName ) + "}";
                }
            }  
        }

        [Serializable]
        public sealed class ConditionRep : Condition
        {
            public readonly ReadOnlyCollection<int> Possibilities;
            public readonly EElementOperator Operator;

            public ConditionRep(ELogicOperator lop, bool negate, EElementOperator op, IEnumerable<int> enu)
                : base(lop, negate)
            {
                this.Operator = op;
                this.Possibilities = enu.ToList().AsReadOnly();
            }

            protected override bool Test(ObservationInfo ci)
            {
                return CompareElement(this.Operator, ci.Rep, this.Possibilities);
            }

            public override string DefaultDisplayName
            {
                get
                {
                    return "Rᴇᴘʟɪᴄᴀᴛᴇ " + this.Operator.ToUiString() + " {" + StringHelper.ArrayToStringInt( this.Possibilities ) + "}";
                }
            }     
        }

        [Serializable]
        public sealed class ConditionBatch : Condition
        {
            public readonly ReadOnlyCollection<BatchInfo> Possibilities;
            public readonly EElementOperator Operator;

            public ConditionBatch(ELogicOperator lop, bool negate, EElementOperator op, IEnumerable<BatchInfo> enu)
                : base(lop, negate)
            {
                this.Operator = op;
                this.Possibilities = enu.ToList().AsReadOnly();
            }

            protected override bool Test(ObservationInfo ci)
            {
                return CompareElement(this.Operator, ci.Batch, this.Possibilities);
            }

            public override string DefaultDisplayName
            {
                get
                {
                    return "Bᴀᴛᴄʜ " + this.Operator.ToUiString() + " {" + StringHelper.ArrayToString( this.Possibilities, z => z.DisplayShortName.ToString() ) + "}";
                }
            }  
        }

        [Serializable]
        public sealed class ConditionAcquisition : Condition
        {
            public readonly ReadOnlyCollection<int> Possibilities;
            public readonly EElementOperator Operator;

            public ConditionAcquisition(ELogicOperator lop, bool negate, EElementOperator op, IEnumerable<int> enu)
                : base(lop, negate)
            {
                this.Operator = op;
                this.Possibilities = enu.ToList().AsReadOnly();
            }

            protected override bool Test(ObservationInfo ci)
            {
                return CompareElement(this.Operator, ci.Order, this.Possibilities);
            }

            public override string DefaultDisplayName
            {
                get
                {
                    return "Aᴄǫᴜɪsɪᴛɪᴏɴ " + this.Operator.ToUiString() + " {" + StringHelper.ArrayToStringInt( this.Possibilities ) + "}";
                }
            }       
        }

        [Serializable]
        public sealed class ConditionTime : Condition
        {
            public readonly ReadOnlyCollection<int> Possibilities;
            public readonly EElementOperator Operator;

            public ConditionTime(ELogicOperator lop, bool negate, EElementOperator op, IEnumerable<int> enu)
                : base(lop, negate)
            {
                this.Operator = op;
                this.Possibilities = enu.ToList().AsReadOnly();
            }

            protected override bool Test(ObservationInfo ci)
            {
                return CompareElement(this.Operator, ci.Time, this.Possibilities);
            }

            public override string DefaultDisplayName
            {
                get
                {
                    return "Tɪᴍᴇ " + this.Operator.ToUiString() + " {" + StringHelper.ArrayToStringInt( this.Possibilities ) + "}";
                }
            }     
        }

        [Serializable]
        public sealed class ConditionObservation : Condition
        {
            public readonly ReadOnlyCollection<ObservationInfo> Possibilities;
            public readonly EElementOperator Operator;

            public ConditionObservation(ELogicOperator lop, bool negate, EElementOperator op, IEnumerable<ObservationInfo> enu)
                : base(lop, negate)
            {
                this.Operator = op;
                this.Possibilities = enu.ToList().AsReadOnly();
            }

            protected override bool Test(ObservationInfo ci)
            {
                return CompareElement(this.Operator, ci, this.Possibilities);
            }

            public override string DefaultDisplayName
            {
                get
                {
                    return "Oʙsᴇʀᴠᴀᴛɪᴏɴ " + this.Operator.ToUiString() + " {" + StringHelper.ArrayToString( this.Possibilities ) + "}";
                }
            }
        }

        [Serializable]
        public sealed class ConditionCondition : Condition
        {
            public readonly ReadOnlyCollection<ObservationInfo> Possibilities;
            public readonly EElementOperator Operator;

            public ConditionCondition( ELogicOperator lop, bool negate, EElementOperator op, IEnumerable<ObservationInfo> enu )
                : base( lop, negate )
            {
                this.Operator = op;
                this.Possibilities = enu.ToList().AsReadOnly();
            }

            protected override bool Test( ObservationInfo ci )
            {
                return CompareElement( this.Operator, ci, this.Possibilities );
            }

            public override string DefaultDisplayName
            {
                get
                {
                    return "Oʙsᴇʀᴠᴀᴛɪᴏɴ " + this.Operator.ToUiString() + " {" + StringHelper.ArrayToString( this.Possibilities ) + "}";
                }
            }            
        }

        [Serializable]
        public sealed class ConditionFilter : Condition
        {
            public readonly bool FilterOp;
            public readonly ObsFilter Filter;

            public ConditionFilter( ELogicOperator op, bool negate, ObsFilter filter, bool filterOp )
                : base( op, negate )
            {
                if (filter == null)
                {
                    throw new ArgumentNullException( nameof( filter ) );
                }

                this.Filter = filter;
                this.FilterOp = filterOp;
            }

            protected override bool Test( ObservationInfo p )
            {
                return this.Filter.Test( p ) == this.FilterOp;
            }

            public override string DefaultDisplayName
            {
                get
                {
                    return "Fɪʟᴛᴇʀ {" + this.Filter + "} = " + this.FilterOp;
                }
            }
        }        
    }         

    /// <summary>
    /// Significance filter.
    /// 
    /// IMMUTABLE (exc. names and comments)
    /// </summary>
    [Serializable]
    class PeakFilter : Filter<Peak>
    {
        public static readonly PeakFilter Empty = new PeakFilter(null, null, null);

        public PeakFilter(string name, string comments, IEnumerable<Condition> filters)
            : base(name, comments, filters)
        {
            // NA
        }

        [Serializable]
        public abstract class Condition : ConditionBase
        {
            protected Condition(ELogicOperator op, bool negate)
                : base(op, negate)
            {
                // NA
            }
        }

        [Serializable]
        public sealed class ConditionStatistic : Condition
        {
            public readonly EStatOperator StatisticOp;
            public readonly WeakReference<ConfigurationStatistic> Statistic;
            public readonly double StatisticValue;

            public ConditionStatistic(ELogicOperator op, bool negate, EStatOperator statOp, ConfigurationStatistic stat, double statisticValue)
                : base(op, negate)
            {
                this.StatisticOp = statOp;
                this.StatisticValue = statisticValue;
                this.Statistic = new WeakReference<ConfigurationStatistic>(stat);
            }

            protected override bool Test(Peak p)
            {
                double v;

                ConfigurationStatistic stat = this.Statistic.GetTarget();

                if (stat == null)
                {
                    throw new InvalidOperationException("Statistic required for significance detection but statistic is no longer available.");
                }

                if (!stat.Results.Results.TryGetValue( p, out v))
                {
                    throw new InvalidOperationException("Statistic required (" + this.Statistic + ") for significance detection but statistic is no longer available.");
                }

                switch (this.StatisticOp)
                {
                    case EStatOperator.LessThan:
                        return v < this.StatisticValue;

                    case EStatOperator.LessThanEq:
                        return v <= this.StatisticValue;

                    case EStatOperator.MoreThan:
                        return v > this.StatisticValue;

                    case EStatOperator.MoreThanEq:
                        return v >= this.StatisticValue;

                    default:
                        throw new InvalidOperationException("Invalid switch: " + this.StatisticOp);
                }
            }        

            public override string DefaultDisplayName
            {
                get
                {
                    return this.Statistic.SafeToString() + " " + this.StatisticOp.ToUiString() + " " + this.StatisticValue;
                }
            }
        }

        [Serializable]
        public sealed class ConditionFilter : Condition
        {
            public readonly bool FilterOp;
            public readonly PeakFilter Filter;

            public ConditionFilter(ELogicOperator op, bool negate, PeakFilter filter, bool filterOp)
                : base(op, negate)
            {
                this.Filter = filter;
                this.FilterOp = filterOp;
            }

            protected override bool Test(Peak p)
            {
                return this.Filter.Test(p) == this.FilterOp;
            }       
            public override string DefaultDisplayName
            {
                get
                {
                    return "Fɪʟᴛᴇʀ {" + this.Filter.ToString() + "} = " + this.FilterOp;
                }
            }
        }

        [Serializable]
        public sealed class ConditionPeak : Condition
        {
            public readonly EElementOperator PeaksOp;
            public readonly ReadOnlyCollection<WeakReference<Peak>> Peaks;

            public ConditionPeak(ELogicOperator op, bool negate, IEnumerable<Peak> peaks, EElementOperator peaksOp)
                : base(op, negate)
            {
                this.Peaks = new ReadOnlyCollection<WeakReference<Peak>>(peaks.Select(WeakReferenceHelper.ToWeakReference).ToList());
                this.PeaksOp = peaksOp;
            }

            protected override bool Test(Peak p)
            {
                return CompareElement(this.PeaksOp, p, this.Peaks.Select(z => z.GetTargetOrThrow()));
            }       

            public override string DefaultDisplayName
            {
                get
                {
                    return "Pᴇᴀᴋ " + this.PeaksOp.ToUiString() + " {" + StringHelper.ArrayToString( this.Peaks, z => z.DisplayName ) + "}";
                }
            }
        }

        [Serializable]
        public sealed class ConditionCluster : Condition
        {
            public readonly ELimitedSetOperator ClustersOp;
            public readonly ReadOnlyCollection<WeakReference<Cluster>> Clusters;

            public ConditionCluster(ELogicOperator op, bool negate, ELimitedSetOperator clusterOp, IEnumerable<Cluster> clusters)
                : base(op, negate)
            {
                this.ClustersOp = clusterOp;
                this.Clusters = new ReadOnlyCollection<WeakReference<Cluster>>(clusters.Select(WeakReferenceHelper.ToWeakReference).ToList());
            }

            protected override bool Test(Peak p)
            {
                bool result;

                switch (this.ClustersOp & ELimitedSetOperator.BasicMask)
                {                             
                    case ELimitedSetOperator.Any:
                        result = false;

                        foreach (var c in this.Clusters.Select( z => z.GetTargetOrThrow() ))
                        {
                            if (c.Assignments.Peaks.Contains( p ))
                            {
                                result = true;
                                break;
                            }
                        }
                        break;
                    case ELimitedSetOperator.All:
                        result = true;

                        foreach (var c in this.Clusters.Select( z => z.GetTargetOrThrow() ))
                        {
                            if (c.Assignments.Peaks.Contains( p ))
                            {
                                result = false;
                                break;
                            }
                        }
                        break;      
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (this.ClustersOp.Has( ELimitedSetOperator.Negate ))
                {
                    result = !result;
                }

                return result;
            }

            public override string DefaultDisplayName
            {
                get
                {
                    return "Cʟᴜsᴛᴇʀ " + this.ClustersOp.ToUiString() + " {" + StringHelper.ArrayToString( this.Clusters, z => z.DisplayName ) + "}";
                }
            }
        }

        [Serializable]
        public sealed class ConditionFlags : Condition
        {
            public readonly ESetOperator FlagsOp;
            public readonly ReadOnlyCollection<WeakReference<UserFlag>> Flags;

            public ConditionFlags( ELogicOperator op, bool negate, ESetOperator flagsOp, IEnumerable<UserFlag> flags ) : base( op, negate )
            {
                this.FlagsOp = flagsOp;
                this.Flags = new ReadOnlyCollection<WeakReference<UserFlag>>( flags.Select( WeakReferenceHelper.ToWeakReference ).ToList() );
            }

            protected override bool Test( Peak p )
            {
                return CompareSets( this.FlagsOp, p.UserFlags, this.Flags.Select( z => z.GetTargetOrThrow() ) );
            }            

            public override string DefaultDisplayName
            {
                get
                {
                    return "Fʟᴀɢ " + this.FlagsOp.ToUiString() + " {" + StringHelper.ArrayToString( this.Flags, z => z.DisplayName ) + "}";
                }
            }
        }
    }
}

