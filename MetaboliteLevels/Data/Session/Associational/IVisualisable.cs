﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using MetaboliteLevels.Data.Session;
using MetaboliteLevels.Data.Visualisables;
using MetaboliteLevels.Utilities;
using MetaboliteLevels.Viewers.Lists;

namespace MetaboliteLevels.Data.Visualisables
{
    /// <summary>
    /// Stuff that shows in lists.
    /// </summary>
    [Serializable]
    internal abstract class Visualisable
    {
        /// <summary>
        /// Displayed name of this item.
        /// </summary>
        public virtual string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty( OverrideDisplayName ))
                {
                    return DefaultDisplayName;
                }
                else
                {
                    return OverrideDisplayName;
                }
            }
        }

        public sealed override string ToString() => DisplayName;

        /// <summary>
        /// Assigned name of this item.
        /// </summary>
        public abstract string DefaultDisplayName { get; }

        /// <summary>
        /// User overriden name of this item
        /// </summary>
        public virtual string OverrideDisplayName { get; set; }

        /// <summary>
        /// Comments applied to this item.
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Is this object hidden from view
        /// </summary>
        public virtual bool Hidden { get; set; }

        /// <summary>
        /// Icon for this item (as an index).
        /// </summary>
        public abstract UiControls.ImageListOrder Icon { get; }

        /// <summary>
        /// STATIC
        /// Gets columns
        /// </summary>
        public abstract IEnumerable<Column> GetColumns( Core core );
                                                 
        public virtual EPrevent SupportsHide => EPrevent.None;
    }

    [Flags]
    public enum EPrevent
    {
        None,
        Hide=1,
        Comment=2,
        Name =4,
    }

    /// <summary>
    /// Stuff that can have associations.
    /// Peaks...clusters...adducts...metabolites...pathways.
    /// </summary>
    [Serializable]
    internal abstract class Associational : Visualisable
    {
        /// <summary>
        /// Gets related items.
        /// </summary>
        public abstract void FindAssociations( ContentsRequest list );

        /// <summary>
        /// VisualClass of IVisualisable
        /// </summary>
        public abstract EVisualClass AssociationalClass { get; }
    }

    /// <summary>
    /// Classes able to provide previews for IVisualisable derivatives.
    /// </summary>
    interface IPreviewProvider
    {
        /// <summary>
        /// Provides a preview for the specified IVisualisable.
        /// </summary>
        Image ProvidePreview( Size size, object target );
    }

    interface IBackup
    {
        void Backup( BackupData data );
        void Restore( BackupData data );
    }

    class BackupData
    {
        private IBackup iBackup;

        public BackupData( IBackup iBackup )
        {
            this.iBackup = iBackup;
        }

        public void Restore()
        {
        // TODO
        }
    }

    /// <summary>
    /// Methods for IVisualisable.
    /// </summary>
    internal static class IVisualisableExtensions
    {
        /// <summary>
        /// Can be used with QueryProperty to search for internal properties.
        /// </summary>
        public const string SYMBOL_PROPERTY = "prop:";

        /// <summary>
        /// (EXTENSION) (MJR) Retrieves a request for contents with the specified parameters.
        /// </summary>
        public static ContentsRequest GetContents( this Associational self, Core core, EVisualClass type )
        {
            ContentsRequest cl = new ContentsRequest( core, self, type );
            self.FindAssociations( cl );
            return cl;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>    
        public static string FormatDisplayName( Visualisable visualisable )
        {
            return string.IsNullOrEmpty( visualisable.OverrideDisplayName ) ? visualisable.DefaultDisplayName : visualisable.OverrideDisplayName;
        }

        /// <summary>
        /// (EXTENSION) (MJR) Gets the display name, or "nothing" if null.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string SafeGetDisplayName( this Visualisable self )
        {
            if (self == null)
            {
                return "nothing";
            }

            return self.DisplayName;
        }

        /// <summary>
        /// (EXTENSION) (MJR) Gets the enabled elements of an IVisualisable enumerable.
        /// </summary>                                               
        public static IEnumerable<T> WhereEnabled<T>( this IEnumerable<T> self )
            where T : Visualisable
        {
            return self.Where( z => !z.Hidden );
        }

        /// <summary>
        /// (EXTENSION) (MJR) Gets the enabled elements of an IVisualisable enumerable.
        /// </summary>     
        public static IEnumerable<T> WhereEnabled<T>( this IEnumerable<T> self, bool onlyEnabled )
            where T : Visualisable
        {
            return onlyEnabled ? WhereEnabled( self ) : self;
        }

        /// <summary>
        /// (EXTENSION) (MJR) Gets a list of valid values to pass to [QueryProperty::property]
        /// </summary>                     
        public static List<string> QueryProperties( this Visualisable self, Core core )
        {
            List<string> result = new List<string>();
            result.AddRange( self.GetType().GetProperties().Select( z => SYMBOL_PROPERTY + z.Name ) );
            return result;
        }

        /// <summary>
        /// (EXTENSION) (MJR) Gets field called [property] from the [self]'s available columns..
        /// </summary>
        public static object QueryProperty( this Visualisable self, string property, Core core )
        {
            if (self == null)
            {
                return null;
            }

            string propertyUpperCase = property.ToUpper();

            switch (propertyUpperCase)
            {
                case "\\N":
                    return "\r\n";

                case "\\T":
                    return "\t";

                case "\\(":
                    return "{";

                case "\\)":
                    return "}";
            }

            bool isProperty = property.StartsWith( SYMBOL_PROPERTY );

            if (isProperty)
            {
                property = property.Substring( SYMBOL_PROPERTY.Length );
            }

            if (isProperty)
            {
                PropertyInfo p = self.GetType().GetProperty( property );

                if (p != null)
                {
                    return p.GetValue( self );
                }

                FieldInfo f = self.GetType().GetField( property );

                if (f != null)
                {
                    return f.GetValue( self );
                }
            }

            Column column = ColumnManager.GetColumns( core, self ).FirstOrDefault( z => z.Id.ToUpper() == propertyUpperCase );

            if (column != null)
            {
                return column.GetRow( self );
            }

            return "{Missing: " + property + "}";
        }

        public static bool SupportsDisable( this Visualisable v )
        {
            if (v == null)
            {
                return false;
            }

            bool originalState = v.Hidden;
            v.Hidden = !originalState;
            bool canEnable = v.Hidden != originalState;
            v.Hidden = originalState;
            return canEnable;
        }

        const string TEST_VALUE = "{EF304B9C-738D-4E2A-88D5-BFA1A72766EC}";

        public static bool SupportsRename( this Visualisable v )
        {
            if (v == null)
            {
                return false;
            }

            string originalState = v.OverrideDisplayName;
            v.OverrideDisplayName = TEST_VALUE;
            bool canEnable = v.OverrideDisplayName == TEST_VALUE;
            v.OverrideDisplayName = originalState;
            return canEnable;
        }

        public static bool SupportsComment( this Visualisable v )
        {
            if (v == null)
            {
                return false;
            }

            string originalState = v.Comment;
            v.Comment = TEST_VALUE;
            bool canEnable = v.Comment == TEST_VALUE;
            v.Comment = originalState;
            return canEnable;
        }
    }

    class Association : Visualisable
    {
        public readonly Associational WrappedValue;
        public Associational SourceValue => _owner.Owner;
        public readonly ContentsRequest _owner;
        private readonly object[] _extraColumns;

        public override string Comment
        {
            get { return WrappedValue.Comment; }

            set { WrappedValue.Comment = value; }
        }

        public Association( ContentsRequest source, Associational target, object[] extraColumns )
        {
            _owner = source;
            WrappedValue = target;
            _extraColumns = extraColumns;
        }

        public override string DefaultDisplayName => WrappedValue.DefaultDisplayName;

        public override string DisplayName => WrappedValue.DisplayName;

        public override bool Hidden
        {
            get { return WrappedValue.Hidden; }
            set { WrappedValue.Hidden = value; }
        }

        public override string OverrideDisplayName
        {
            get { return WrappedValue.OverrideDisplayName; }

            set { WrappedValue.OverrideDisplayName = value; }
        }

        public override IEnumerable<Column> GetColumns( Core core )
        {
            List<Column<Association>> results = new List<Column<Association>>();

            results.AddSubObject( core, "Target\\", z => z.WrappedValue );

            for (int n = 0; n < _owner.ExtraColumns.Count; ++n)
            {
                var closure = n;
                var c = _owner.ExtraColumns[n];

                results.Add( new Column<Association>( c.Item1, EColumn.Visible, c.Item2, z => z._extraColumns[closure], z => Color.Blue ) );

            }

            return results.Cast<Column>().Concat( GetExtraColumns( core ) );
        }

        protected virtual IEnumerable<Column> GetExtraColumns( Core core )
        {
            return new Column[0];
        }

        public override UiControls.ImageListOrder Icon => WrappedValue.Icon;
    }

    /// <summary>
    /// Request for contents from an IVisualisable.
    /// </summary>
    internal class ContentsRequest
    {
        /// <summary>
        /// Request - Core
        /// </summary>
        public readonly Core Core;

        /// <summary>
        /// Request - Called upon
        /// </summary>
        public readonly Associational Owner;

        /// <summary>
        /// Request - Type of results to get.
        /// </summary>
        public readonly EVisualClass Type;

        /// <summary>
        /// Response - Title of the results.
        /// </summary>
        public string Text;

        /// <summary>
        /// Response - List of results
        /// </summary>
        public readonly List<Association> Results = new List<Association>();

        /// <summary>
        /// Extra columns
        /// </summary>
        public readonly List<Tuple<string, string>> ExtraColumns = new List<Tuple<string, string>>();

        /// <summary>
        /// CONSTRUCTOR
        /// </summary> 
        public ContentsRequest( Core core, Associational owner, EVisualClass type )
        {
            this.Core = core;
            this.Owner = owner;
            this.Type = type;
        }

        public void Add<T>( T item, params object[] extraColumns )
            where T : Associational
        {
            Results.Add( new Association( this, item, extraColumns ) );
        }

        /// <summary>
        /// Adds a range of items (this can't be done if there are unique columns).
        /// </summary>                                                             
        public void AddRange( IEnumerable<Associational> items )
        {
            if (items != null)
            {
                foreach (Associational item in items)
                {
                    Add( item );
                }
            }
        }

        internal void AddExtraColumn( string title, string description )
        {
            ExtraColumns.Add( Tuple.Create( title, description ) );
        }

        internal void AddRangeWithCounts<T>( Counter<T> counts )
            where T : Associational
        {
            foreach (var kvp in counts.Counts)
            {
                Add( kvp.Key, kvp.Value );
            }
        }
    }

    /// <summary>
    /// Types of IVisualisable.
    /// </summary>
    public enum EVisualClass
    {
        None,
        Info,
        Statistic,
        Peak,
        Cluster,
        Compound,
        Adduct,
        Pathway,
        Assignment,
        Annotation,
    }
}
