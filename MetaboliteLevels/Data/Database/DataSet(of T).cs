﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetaboliteLevels.Data.Session.Associational;
using MetaboliteLevels.Data.Session.General;
using MetaboliteLevels.Gui.Controls;
using MetaboliteLevels.Gui.Controls.Lists;
using MetaboliteLevels.Gui.Forms.Editing;
using MetaboliteLevels.Gui.Forms.Selection;
using MetaboliteLevels.Utilities;
using MGui.Controls;
using MGui.Datatypes;
using MGui.Helpers;

namespace MetaboliteLevels.Data.Database
{
    /// <summary>
    /// Holds a list of objects alongside additional information on how to represent that list
    /// objects in the GUI.
    /// 
    /// Concrete implementations of this class can be found in the static <see cref="DataSet"/> helper class.
    /// 
    /// This is a somewhat large list of properties, depending upon which GUI elements need to be displayed,
    /// however most are special cases, the only mandatory properties to set are:
    ///     * Title                      (list title)
    ///     * Source                     (the actual list)
    ///     * ItemNameProvider           (how items are named)
    ///     * ItemDescriptionProvider    (how items are described)
    ///     * Core                       (backreference to Core)
    /// </summary>
    internal class DataSet<T> : IDataSet
    {
        /// <summary>
        /// Backing field for <see cref="Icon"/>.
        /// </summary>
        private Image _icon;

        public delegate bool ComparatorDelegate(string name, T item);
        public delegate bool RetrieverDelegate(string name, out T item);

        public Type DataType { get; set; }

        /// <summary>
        /// (MANDATORY) Title of the dataset
        /// </summary>
        public string ListTitle { get; set; }

        /// <summary>
        /// (OPTIONAL) Subtitle of the dataset
        /// </summary>
        public string ListDescription { get; set; }

        /// <summary>
        /// (MANDATORY) Where to find the items
        /// </summary>
        public IEnumerable<T> ListSource { private get; set; }

        /// <summary>
        /// (MANDATORY) How the items should be named in simple lists
        /// </summary>
        public Converter<T, string> ItemTitle { get; set; }

        /// <summary>
        /// (MANDATORY) How the items should be described in simple lists
        /// 
        /// This is mandatory but may return NULL.
        /// </summary>
        public Converter<T, string> ItemDescription { get; set; }    

        /// <summary>
        /// (MANDATORY) The core (only a convenience for data which needs a reference to the core to be meaningful).
        /// </summary>
        public Core Core { get; set; }                                           

        /// <summary>
        /// (OPTIONAL) Called after the source list is changed via a call to <see cref="HandleCommit"/>.
        /// 
        /// Options are unaffected.
        /// </summary>
        public Action<AfterApplyArgs> HandleFinished { get; set; }

        /// <summary>
        /// (OPTIONAL) When set list updates will be performed DURING list editing.
        /// This is used when list items require references the list itself as it is being modified.
        /// 
        /// The <see cref="ApplyArgs.Transient"/> field of the <see cref="HandleCommit"/> call will be TRUE during transient updates.
        /// The original list will be restored if the user cancels with another call to <see cref="HandleCommit"/>.
        /// </summary>
        public bool ItemsReferenceList { get; set; }

        /// <summary>
        /// Called to apply changes to the source list.
        /// <para/>
        /// NULL if the source list cannot be modified (e.g. temporary or fixed lists).
        /// <para/>
        /// Options Add/Remove require this or ListChangesOnEdit.
        /// </summary>
        public Action<ApplyArgs> HandleCommit { get; set; }

        /// <summary>
        /// When set signifies the list changes when items are edited.
        /// ListChangeApplicator is ignored and changes to the list cannot be cancelled.
        /// 
        /// Options Add/Remove require this or ListChangeApplicator.
        /// </summary>
        public bool ListIsSelfUpdating { get; set; }

        /// <summary>
        /// How to edit items in the list
        /// NULL if items cannot be modified.
        /// 
        /// Options Add/Edit/View require this.
        /// </summary>
        public Converter<EditItemArgs, T> HandleEdit { get; set; }

        /// <summary>
        /// Gets or sets the Integerbehaviour (for conversion to/from text only).
        /// When set the user can enter integer ranges (e.g. 5-7) instead of individual items (5, 6, 7) in text entry.
        /// </summary>
        public bool IntegerBehaviour { get; set; }

        /// <summary>
        /// Used to denote an empty selection in single-selection mode.
        /// This is usually NULL.
        /// </summary>
        public T CancelValue { get; set; }

        /// <summary>
        /// Identifies if a string corresponds to an item for text-lookups.
        /// NOTE: This is only for cases where string->x is not the same as x->string.
        ///       x.ToString() and Namer(x) are always checked regardless.
        /// </summary>
        public ComparatorDelegate StringComparator { get; set; }

        /// <summary>
        /// Retrieves an item with the specified name.
        /// NOTE: This is only for cases where the item is not in the source list and therefore
        /// allows users to specify new entries.
        ///       x.ToString(), Namer(x) and StringComparator are NOT checked.
        /// </summary>
        public RetrieverDelegate DynamicItemRetriever { get; set; }

        /// <summary>
        /// Returns if there is a <see cref="DynamicItemRetriever"/>.
        /// </summary>
        public bool DynamicEntries => this.DynamicItemRetriever != null;

        /// <summary>
        /// When set signifies the list can be reordered.
        /// This makes no sense if set with ListChangesOnEdit since the user cannot control the list.
        /// 
        /// Required for Up/Down.
        /// </summary>
        public bool ListSupportsReorder { get; set; }

        /// <summary>
        /// When items are added, replaced or removed in the list this is called.
        /// If ListChangesOnEdit is true this can handle remove events.
        /// </summary>                                                           
        public Action<Form, T, T> BeforeItemChanged { get; set; }      

        public class ApplyArgs
        {
            /// <summary>
            /// New list to apply
            /// </summary>
            public readonly IEnumerable<T> List;

            /// <summary>
            /// Where to send progress reports to.
            /// </summary>
            public readonly ProgressReporter Progress;

            public bool Transient; 

            public ApplyArgs(IEnumerable<T> newList, ProgressReporter prog, bool transient)
            {
                this.List = newList;
                this.Progress = prog;
                this.Transient = transient;
            }
        }

        public class AfterApplyArgs
        {
            /// <summary>
            /// Form to show dialogues on
            /// </summary>
            public readonly Form Owner;

            /// <summary>
            /// Status of the list
            /// </summary>
            public readonly IEnumerable<T> List;    

            public AfterApplyArgs(Form owner, IEnumerable<T> list)
            {
                this.Owner = owner;
                this.List = list;          
            }
        }

        public class EditItemArgs
        {
            /// <summary>
            /// Form to show dialogues on
            /// </summary>
            public readonly Form Owner;

            /// <summary>
            /// Default value
            /// </summary>
            public readonly T DefaultValue;

            /// <summary>
            /// Whether to disallow editing
            /// </summary>
            public readonly bool ReadOnly;                   

            /// <summary>
            /// When set the caller intends to create a new item
            /// -- the callee is responsible to ensure work is carried out on a copy, not the original.
            /// </summary>
            public readonly bool WorkOnCopy;

            public EditItemArgs(Form owner, T @default, bool readOnly, bool workOnCopy )
            {
                this.Owner = owner;
                this.DefaultValue = @default;
                this.ReadOnly = readOnly;
                this.WorkOnCopy = workOnCopy;               
            }

            public DataSet<T2>.EditItemArgs Cast<T2>()
            {
                return new DataSet<T2>.EditItemArgs( this.Owner, (T2)(object)this.DefaultValue, this.ReadOnly, this.WorkOnCopy );
            }
        }

        /// <summary>
        /// Clone method.
        /// </summary>
        public DataSet<T> Clone()
        {
            return (DataSet<T>)this.MemberwiseClone();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataSet()
        {
            this.ItemTitle = z => z.ToString();
            this.ItemDescription = z => null;
            this.DataType = typeof( T );
            this.CancelValue = this.DataType == typeof( int ) ? (T)(object)int.MinValue : default( T );
        }

        /// <summary>
        /// Wrapper to untyped object.
        /// </summary>
        IEnumerable IDataSet.UntypedGetList(bool onlyEnabled)
        {
            return this.TypedGetList(onlyEnabled);
        }

        /// <summary>
        /// Wrapper to untyped object.
        /// </summary>
        public IEnumerable<T> TypedGetList(bool onlyEnabled)
        {
            if (onlyEnabled)
            {
                return this.ListSource.Where(this.EnabledFitler);
            }

            return this.ListSource;
        }

        /// <summary>
        /// Helper: Only allows enabled items
        /// </summary>                       
        private bool EnabledFitler(T arg)
        {
            var x = arg as Visualisable;

            return x == null || !x.Hidden;
        }

        /// <summary>
        /// Wrapper to untyped Namer delegate.
        /// </summary>
        string IDataSet.UntypedName(object x)
        {
            return this.ItemTitle((T)x);
        }

        /// <summary>
        /// Wrapper to untyped Describer delegate.
        /// </summary>
        string IDataSet.UntypedDescription(object x)
        {
            return this.ItemDescription((T)x);
        }

        /// <summary>
        /// Shows the big list (FrmBigList).
        /// </summary>         
        public BigListResult<T> ShowListEditor(Form owner, FrmBigList.EShow show, object automaticAddTemplate)
        {
            return FrmBigList.Show( owner, this.Core, this, show, automaticAddTemplate )?.Cast<T>();
        }   

        /// <summary>
        /// Shows the big list (FrmBigList).
        /// The list is editable if a ListChangeAcceptor is set.
        /// </summary>        
        /// <returns>If the list was modified</returns>
        public BigListResult<object> ShowListEditor(Form owner)
        {
            return FrmBigList.Show( owner, this.Core, this, FrmBigList.EShow.Default, null );
        }

        /// <summary>
        /// Shows the list selection form (FrmList, single selection).
        /// </summary>
        public T ShowList(Form owner, T defaultSelection)
        {
            return FrmSelectList.ShowList(owner, this, defaultSelection);
        }

        /// <summary>
        /// Shows the radio button selection form (FrmList, single selection).
        /// </summary>
        public T ShowRadio(Form owner, T defaultSelection)
        {
            return FrmSelectList.ShowRadio(owner, this, defaultSelection);
        }

        /// <summary>
        /// Shows the button selection form (FrmList, single selection).
        /// </summary>
        public T ShowButtons(Form owner, T defaultSelection)
        {
            return FrmSelectList.ShowButtons(owner, this, defaultSelection);
        }

        /// <summary>
        /// Shows the list selection form (FrmList, multiple selection).
        /// </summary>
        public IEnumerable<T> ShowCheckList(Form owner, IEnumerable<T> defaultSelection)
        {
            return FrmSelectList.ShowCheckList(owner, this, defaultSelection);
        }

        /// <summary>
        /// Shows the checkbox selection form (FrmList, single selection).
        /// </summary>
        public IEnumerable<T> ShowCheckBox(Form owner, IEnumerable<T> defaultSelection)
        {
            return FrmSelectList.ShowCheckBox(owner, this, defaultSelection);
        }

        /// <summary>
        /// Creates a ConditionBox (textbox and browse button) from the list.
        /// </summary>
        public ConditionBox<T> CreateConditionBox(CtlTextBox textBox, Button button)
        {
            return new ConditionBox<T>(this, textBox, button);
        }

        /// <summary>
        /// Creates a ComboBox (dropdown list and edit button) from the list.
        /// </summary>        
        public EditableComboBox<T> CreateComboBox(ComboBox l, Button b, EditableComboBox.EFlags flags)
        {
            return new EditableComboBox<T>(l, b, this, flags );
        }

        public EditableComboBox UntypedCreateComboBox( ComboBox l, Button b, EditableComboBox.EFlags flags )
        {
            return this.CreateComboBox( l, b, flags );
        }

        /// <summary>
        /// Fluent interface for [SubTitle].
        /// </summary>
        internal DataSet<T> IncludeMessage(string subTitle)
        {
            this.ListDescription = subTitle;
            return this;
        }

        /// <summary>
        /// IMPLEMENTS IListValueSet.
        /// </summary>               
        object IDataSet.UntypedEdit(Form owner, object @default, bool readOnly, bool workOnCopy)
        {
            return this.HandleEdit(new EditItemArgs(owner, (T)@default, readOnly, workOnCopy) );
        }        

        /// <summary>
        /// IMPLEMENTS IListValueSet.
        /// </summary>               
        void IDataSet.UntypedAfterApply(Form owner, IEnumerable list)
        {
            if (this.HandleFinished == null)
            {
                return;
            }

            this.HandleFinished(new AfterApplyArgs(owner, list.Cast<T>()));
        }

        /// <summary>
        /// IMPLEMENTS IListValueSet.
        /// </summary>               
        void IDataSet.UntypedApplyChanges(IEnumerable list, ProgressReporter prog, bool transient)
        {
            if (this.HandleCommit == null)
            {
                return;
            }

            this.HandleCommit(new ApplyArgs(list.Cast<T>(), prog, transient));
        }

        /// <summary>
        /// IMPLEMENTS IListValueSet.
        /// </summary>    
        string IDataSet.Title
        {
            get
            {
                return this.ListTitle;
            }
        }

        /// <summary>
        /// IMPLEMENTS IListValueSet.
        /// </summary>      
        string IDataSet.SubTitle
        {
            get
            {
                return this.ListDescription;
            }
        }

        /// <summary>
        /// IMPLEMENTS IListValueSet.
        /// </summary>    
        [XColumn(EColumn.Visible, "List editable")]
        bool IDataSet.ListSupportsChanges
        {
            get
            {
                return this.HandleCommit != null;
            }
        }

        /// <summary>
        /// IMPLEMENTS IListValueSet.
        /// </summary>    
        [XColumn( EColumn.Visible, "Items editable" )]
        bool IDataSet.HasItemEditor
        {
            get
            {
                return this.HandleEdit != null;
            }
        }

        /// <summary>
        /// IMPLEMENTS IListValueSet.
        /// </summary>    
        void IDataSet.UntypedBeforeReplace(Form owner, object remove, object create)
        {
            if (this.BeforeItemChanged != null)
            {
                this.BeforeItemChanged(owner, (T)remove, (T)create);
            }
        }
                      
        /// <summary>
        /// IMPLEMENTS <see cref="IIconProvider"/>
        /// Can also set the icon
        /// </summary>
        public Image Icon
        {
            get
            {
                return this._icon ?? UiControls.GetImage( this.ListSource.FirstOrDefault() );
            }
            set
            {
                this._icon = value;
            }
        }

        public ISpreadsheet ExportData()
        {      
            T[] src = this.ListSource.ToArray();
            var c = ColumnManager.GetColumns( this.DataType, this.Core ).ToArray();

            Spreadsheet<object> ss = new Spreadsheet<object>( src.Length, c.Length );

            ss.Title = this.ListTitle;
            ss.RowNames.Set( this.ListSource.Select( z => z.ToString() ) );
            ss.ColNames.Set( c.Select( z => z.Id ) );

            for (int row = 0; row < src.Length; ++row)
            {
                object rowValue = src[row];

                for (int col = 0; col < c.Length; ++col)
                {
                    ss[row,col] = c[col].GetRowAsString( rowValue );
                }
            }

            return ss;
        }

        public override string ToString()
        {
            return this.ListTitle;
        }
    }
}