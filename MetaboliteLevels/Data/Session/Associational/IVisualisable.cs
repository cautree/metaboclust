﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using MetaboliteLevels.Data.Session;
using MetaboliteLevels.Utilities;
using MetaboliteLevels.Viewers.Lists;

namespace MetaboliteLevels.Data.Visualisables
{
    /// <summary>
    /// Stuff the user can add comments to and change the name of.
    /// </summary>
    interface INameable
    {
        /// <summary>
        /// Displayed name of this item.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Assigned name of this item.
        /// </summary>
        string DefaultDisplayName { get; }

        /// <summary>
        /// User overriden name of this item
        /// </summary>
        string OverrideDisplayName { get; set; }

        /// <summary>
        /// Comments applied to this item.
        /// </summary>
        string Comment { get; set; }

        /// <summary>
        /// Is this object enabled?
        /// </summary>
        bool Enabled { get; set; }
    }

    /// <summary>
    /// Stuff that shows in lists.
    /// </summary>
    interface IVisualisable : INameable
    {
        /// <summary>
        /// Icon for this item (as an index).
        /// </summary>
        UiControls.ImageListOrder GetIcon();

        /// <summary>
        /// STATIC
        /// Gets columns
        /// </summary>
        IEnumerable<Column> GetColumns(Core core);
    }

    /// <summary>
    /// Stuff that can have associations.
    /// Peaks...clusters...adducts...metabolites...pathways.
    /// </summary>
    interface IAssociational : IVisualisable
    {
        /// <summary>
        /// Gets related items.
        /// </summary>
        void RequestContents( ContentsRequest list );

        /// <summary>
        /// VisualClass of IVisualisable
        /// </summary>
        VisualClass VisualClass { get; }
    }

    /// <summary>
    /// Classes able to provide previews for IVisualisable derivatives.
    /// </summary>
    interface IPreviewProvider
    {
        /// <summary>
        /// Provides a preview for the specified IVisualisable.
        /// </summary>
        Image ProvidePreview(Size size, object target, object highlight );
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
        public static ContentsRequest GetContents(this IAssociational self, Core core, VisualClass type)
        {
            ContentsRequest cl = new ContentsRequest(core, self, type);
            self.RequestContents(cl);
            return cl;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>    
        public static string FormatDisplayName(INameable visualisable)
        {
            return string.IsNullOrEmpty(visualisable.OverrideDisplayName) ? visualisable.DefaultDisplayName : visualisable.OverrideDisplayName;
        }

        /// <summary>
        /// (EXTENSION) (MJR) Gets the display name, or "nothing" if null.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string SafeGetDisplayName(this INameable self )
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
        public static IEnumerable<T> WhereEnabled<T>(this IEnumerable<T> self)
            where T : INameable
        {
            return self.Where(z => z.Enabled);
        }

        /// <summary>
        /// (EXTENSION) (MJR) Gets the enabled elements of an IVisualisable enumerable.
        /// </summary>     
        public static IEnumerable<T> WhereEnabled<T>(this IEnumerable<T> self, bool onlyEnabled)
            where T : INameable
        {
            return onlyEnabled ? WhereEnabled(self) : self;
        }

        /// <summary>
        /// (EXTENSION) (MJR) Gets a list of valid values to pass to [QueryProperty::property]
        /// </summary>                     
        public static List<string> QueryProperties(this IVisualisable self, Core core)
        {
            List<string> result = new List<string>();
            result.AddRange(self.GetType().GetProperties().Select(z => SYMBOL_PROPERTY + z.Name));
            return result;
        }

        /// <summary>
        /// (EXTENSION) (MJR) Gets field called [property] from the [self]'s available columns..
        /// </summary>
        public static object QueryProperty(this IVisualisable self, string property, Core core)
        {
            if (self == null)
            {
                return null;
            }

            bool isProperty = property.StartsWith(SYMBOL_PROPERTY);

            if (isProperty)
            {
                property = property.Substring(SYMBOL_PROPERTY.Length);
            }

            if (isProperty)
            {
                PropertyInfo p = self.GetType().GetProperty(property);

                if (p != null)
                {
                    return p.GetValue(self);
                }

                FieldInfo f = self.GetType().GetField(property);

                if (f != null)
                {
                    return f.GetValue(self);
                }
            }

            Column column = ColumnManager.GetColumns(core, self).FirstOrDefault(z => z.Id.ToUpper() == property.ToUpper());

            if (column != null)
            {
                return column.GetRow(self);
            }

            return "{Missing: " + property + "}";
        }

        public static bool SupportsDisable( this INameable v )
        {
            bool originalState = v.Enabled;
            v.Enabled = !originalState;
            bool canEnable = v.Enabled != originalState;
            v.Enabled = originalState;
            return canEnable;
        }

        const string TEST_VALUE = "{EF304B9C-738D-4E2A-88D5-BFA1A72766EC}";

        public static bool SupportsRename( this INameable v )
        {
            string originalState = v.OverrideDisplayName;
            v.OverrideDisplayName = TEST_VALUE;
            bool canEnable = v.OverrideDisplayName == TEST_VALUE;
            v.OverrideDisplayName = originalState;
            return canEnable;
        }

        public static bool SupportsComment( this IVisualisable v )
        {
            string originalState = v.Comment;
            v.Comment = TEST_VALUE;
            bool canEnable = v.Comment == TEST_VALUE;
            v.Comment = originalState;
            return canEnable;
        }
    }

    /// <summary>
    /// Request for contents from an IVisualisable.
    /// </summary>
    internal class ContentsRequest
    {
        /// <summary>
        /// Empty request.
        /// </summary>
        public static readonly ContentsRequest Empty = new ContentsRequest(null, null, (VisualClass)(-1));

        /// <summary>
        /// Request - Core
        /// </summary>
        public readonly Core Core;

        /// <summary>
        /// Request - Called upon
        /// </summary>
        public readonly IVisualisable Owner;

        /// <summary>
        /// Request - Type of results to get.
        /// </summary>
        public readonly VisualClass Type;

        /// <summary>
        /// Response - Title of the results.
        /// </summary>
        public string Text;

        /// <summary>
        /// Response - OBJECT and OPTIONAL EXTRA COLUMNS
        /// </summary>
        public readonly Dictionary<IVisualisable, object[]> Contents = new Dictionary<IVisualisable, object[]>();

        /// <summary>
        /// Response - titles of OPTIONAL EXTRA COLUMNS 
        /// </summary>
        private readonly List<ExtraColumn> _headers = new List<ExtraColumn>();

        /// <summary>
        /// CONSTRUCTOR
        /// </summary> 
        public ContentsRequest(Core core, IVisualisable owner, VisualClass type)
        {
            this.Core = core;
            this.Owner = owner;
            this.Type = type;
        }

        /// <summary>
        /// Adds an extra column header - pass the actual values to [Add::extraColumnData].
        /// </summary>           
        public void AddExtraColumn(string header, string description)
        {
            _headers.Add(new ExtraColumn(header, description));
        }

        /// <summary>
        /// Adds an [item], additional columns should be passed in the [extraColumnData].
        /// </summary>   
        public void Add(IVisualisable item, params object[] extraColumnData)
        {
            Contents[item] = extraColumnData;
        }

        /// <summary>
        /// Returns the column items.
        /// </summary>
        public IEnumerable<IVisualisable> Keys
        {
            get { return Contents.Keys; }
        }

        /// <summary>
        /// Adds a range of items (this can't be done if there are unique columns).
        /// </summary>                                                             
        public void AddRange(IEnumerable<IVisualisable> items)
        {
            if (items != null)
            {
                foreach (IVisualisable item in items)
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Gets the list of extra columns.
        /// </summary>
        public IList<ExtraColumn> ExtraColumns
        {
            get
            {
                return this._headers;
            }
        }

        /// <summary>
        /// Adds a range of items, with their counts in the first extra column (there must just be one extra column).
        /// </summary>                                                  
        public void AddRangeWithCounts<T>(Counter<T> counts)
            where T : IVisualisable
        {
            foreach (var kvp in counts.Counts)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Columns (returned in <see cref="ContentsRequest"/>) in addition to those normally on the items.
        /// </summary>
        public class ExtraColumn
        {
            /// <summary>
            /// Column name
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// Column description
            /// </summary>
            public readonly string Description;

            /// <summary>
            /// CONSTRUCTOR
            /// </summary> 
            public ExtraColumn(string header, string description)
            {
                this.Name = header;
                this.Description = description;
            }
        }
    }

    /// <summary>
    /// Types of IVisualisable.
    /// </summary>
    public enum VisualClass
    {
        None,
        Peak,
        Cluster,
        Compound,
        Adduct,
        Pathway,
        Assignment,
        Annotation,
    }
}