﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MetaboliteLevels.Algorithms.Statistics;
using MetaboliteLevels.Algorithms.Statistics.Clusterers;
using MetaboliteLevels.Controls;
using MetaboliteLevels.Data.Session;
using MetaboliteLevels.Data.Visualisables;
using MetaboliteLevels.Utilities;
using MetaboliteLevels.Algorithms.Statistics.Configurations;
using MetaboliteLevels.Forms.Editing;
using MetaboliteLevels.Settings;
using MetaboliteLevels.Data.DataInfo;
using System.Diagnostics;
using MetaboliteLevels.Viewers.Lists;

namespace MetaboliteLevels.Forms.Generic
{
    internal partial class FrmList : Form
    {
        private IFormList _handler;
        private IDataSet opts;
        private int _numFields;
        private bool _multiSelect;
        private bool _allowNewEntries;

        private static IEnumerable<T> Show<T>(Form owner, IFormList handler, DataSet<T> opts, bool multiSelect, IEnumerable<T> defaultSelection, bool allowNewEntries)
        {
            using (FrmList frm = new FrmList(handler, opts, multiSelect, defaultSelection, allowNewEntries, opts.Core))
            {
                if (UiControls.ShowWithDim(owner, frm) == DialogResult.OK)
                {
                    return opts.TypedGetList(true).Corresponding(frm.GetStates());
                }

                return null;
            }
        }

        private FrmList(IFormList handler, IDataSet opts, bool multiSelect, IEnumerable defaultSelection, bool _allowNewEntries, Core core)
            : this()
        {
            this.Text = opts.Title;
            this.ctlTitleBar1.Text = opts.Title;
            this.ctlTitleBar1.SubText = opts.SubTitle;
            this._btnEdit.Visible = opts.ListSupportsChanges;

            this._handler = handler;
            this._numFields = opts.UntypedGetList(true).CountAll();
            this.opts = opts;
            this._multiSelect = multiSelect;
            this._allowNewEntries = _allowNewEntries;
            _flpSelectAll.Visible = multiSelect;

            this._handler.Initialise(this, core);

            RefreshList(defaultSelection);
        }

        interface IFormList
        {
            void Initialise(FrmList form, Core core);
            void ClearItems();
            void Ready();
            bool GetState(int n);
            void SetState(int n, bool state);
            void AddItem(object item, string text, string description);
        }

        class FormListBigListBox : IFormList
        {
            private ListView _listBox;
            ListViewHelper<IVisualisable> _lvh;
            List<IVisualisable> _list = new List<IVisualisable>();

            public void AddItem(object item, string text, string description)
            {
                _list.Add((IVisualisable)item);
            }

            public void ClearItems()
            {
                _lvh.Clear();
            }

            public bool GetState(int n)
            {
                return _lvh.SelectedIndex == n;
            }

            public void Initialise(FrmList form, Core core)
            {
                _listBox = new ListView();
                _listBox.Dock = DockStyle.Fill;
                _listBox.Margin = new Padding(8, 8, 8, 8);
                _listBox.Visible = true;

                form.panel1.Controls.Add(_listBox);
                _lvh = new ListViewHelper<IVisualisable>(_listBox, core, null, null);
            }

            public void Ready()
            {
                _lvh.DivertList(_list);
            }

            public void SetState(int n, bool state)
            {
                if (state)
                {
                    _lvh.SelectedIndex = n;
                }
            }
        }

        class FormListListBox : IFormList
        {
            protected ListView listBox;

            public virtual void Initialise(FrmList form, Core core)
            {
                listBox = new ListView();

                listBox.Columns.Add(new ColumnHeader());

                listBox.View = View.Details;
                listBox.HeaderStyle = ColumnHeaderStyle.None;
                listBox.FullRowSelect = true;
                listBox.GridLines = true;
                listBox.MultiSelect = false;
                listBox.ShowItemToolTips = true;

                listBox.Dock = DockStyle.Fill;
                listBox.Margin = new Padding(8, 8, 8, 8);
                listBox.Visible = true;

                form.panel1.Controls.Add(listBox);
            }

            void IFormList.Ready()
            {
                listBox.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }

            void IFormList.ClearItems()
            {
                listBox.Items.Clear();
            }

            bool IFormList.GetState(int n)
            {
                return listBox.Items[n].Selected;
            }

            void IFormList.SetState(int n, bool state)
            {
                listBox.Items[n].Selected = state;
            }

            void IFormList.AddItem(object item, string text, string description)
            {
                ListViewItem lvi = new ListViewItem(text);
                lvi.ToolTipText = description;
                listBox.Items.Add(lvi);
            }
        }

        class FormListCheckList : FormListListBox, IFormList
        {
            public override void Initialise(FrmList form, Core core)
            {
                base.Initialise(form, core);
                base.listBox.CheckBoxes = true;
            }

            bool IFormList.GetState(int n)
            {
                return listBox.Items[n].Checked;
            }

            void IFormList.SetState(int n, bool state)
            {
                listBox.Items[n].Checked = state;
            }
        }

        abstract class FormListControlArray<T> : IFormList
            where T : Control, new()
        {
            FlowLayoutPanel listBox;
            ToolTip toolTip;
            protected List<T> checkBoxes = new List<T>();

            void IFormList.Initialise(FrmList form, Core core)
            {
                listBox = new FlowLayoutPanel();
                listBox.FlowDirection = FlowDirection.TopDown;
                listBox.Dock = DockStyle.Fill;
                listBox.WrapContents = false;
                listBox.AutoScroll = true;
                listBox.Location = new System.Drawing.Point(0, 0);
                listBox.Margin = new Padding(0, 0, 0, 0);
                listBox.Visible = true;

                toolTip = new ToolTip();
                toolTip.Active = true;

                form.panel1.Controls.Add(listBox);
            }

            void IFormList.ClearItems()
            {
                listBox.Controls.Clear();
                checkBoxes.Clear();
            }

            void IFormList.Ready()
            {
                // NA
            }

            bool IFormList.GetState(int n)
            {
                return GetState(checkBoxes[n]);
            }

            void IFormList.SetState(int n, bool value)
            {
                SetState(checkBoxes[n], value);
            }

            public abstract bool GetState(T control);
            public abstract void SetState(T control, bool state);

            void IFormList.AddItem(object item, string text, string description)
            {
                T cb = new T();
                cb.AutoSize = true;
                cb.Text = text;
                cb.Visible = true;
                cb.Margin = new Padding(8, 8, 8, 0);
                toolTip.SetToolTip(cb, text);
                listBox.Controls.Add(cb);
                checkBoxes.Add(cb);
                InitialiseItem(cb);

                Label lb = new Label();
                lb.Text = description;
                lb.AutoSize = true;
                lb.Margin = new Padding(64, 0, 8, 8);
                lb.ForeColor = System.Drawing.Color.SteelBlue;
                lb.Font = new System.Drawing.Font(lb.Font.FontFamily.Name, 8);
                listBox.Controls.Add(lb);
            }

            protected virtual void InitialiseItem(T item)
            {
                // NA
            }
        }

        class FormListCheckBoxArray : FormListControlArray<CheckBox>
        {
            public override bool GetState(CheckBox control)
            {
                return control.Checked;
            }

            public override void SetState(CheckBox control, bool state)
            {
                control.Checked = state;
            }
        }

        class FormListButtonArray : FormListControlArray<Button>
        {
            public override bool GetState(Button control)
            {
                return control.Tag != null;
            }

            public override void SetState(Button control, bool state)
            {
                // NA
            }

            protected override void InitialiseItem(Button control)
            {
                control.FlatStyle = FlatStyle.Flat;
                control.FlatAppearance.BorderSize = 0;
                control.DialogResult = DialogResult.OK;
                control.ForeColor = System.Drawing.Color.Blue;
                control.Click += cb_Click;
                control.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            }

            void cb_Click(object sender, EventArgs e)
            {
                ((Button)sender).Tag = sender;
            }
        }

        class FormListRadioButtonArray : FormListControlArray<RadioButton>
        {
            public override bool GetState(RadioButton control)
            {
                return control.Checked;
            }

            public override void SetState(RadioButton control, bool state)
            {
                control.Checked = state;
            }
        }

        private void RefreshList(IEnumerable selectedItems)
        {
            _handler.ClearItems();

            int n = 0;
            IEnumerable<object> source = opts.UntypedGetList(true).Cast<object>();
            IEnumerable<object> selected = selectedItems != null ? selectedItems.Cast<object>() : new object[0];

            foreach (object item in source)
            {
                _handler.AddItem(item, opts.UntypedName(item), opts.UntypedDescription(item));

                _handler.SetState(n, selected.Contains(item));

                n++;
            }

            foreach (object item in selected)
            {
                if (!source.Contains(item))
                {
                    if (_allowNewEntries)
                    {
                        _handler.AddItem(item, opts.UntypedName(item), opts.UntypedDescription(item));

                        _handler.SetState(n, true);

                        n++;
                    }
                    else
                    {
                        throw new InvalidOperationException("Item present in selection but not in list.");
                    }
                }
            }

            UiControls.CompensateForVisualStyles(this);

            _handler.Ready();
        }

        private IEnumerable<bool> GetStates()
        {
            bool[] r = new bool[_numFields];

            for (int n = 0; n < _numFields; n++)
            {
                r[n] = _handler.GetState(n);
            }

            return r;
        }

        private FrmList()
        {
            InitializeComponent();

            _flpSelectAll.BackColor = UiControls.BackColour;
            _flpSelectAll.ForeColor = UiControls.ForeColour;

            UiControls.SetIcon(this);
        }

        private void _btnEdit_Click(object sender, EventArgs e)
        {
            opts.ShowListEditor(this);

            RefreshList(null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!_multiSelect)
            {
                if (!GetStates().Any(z => z))
                {
                    FrmMsgBox.ShowError(this, "Please make a selection before continuing.");
                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        internal static T ShowList<T>(Form owner, DataSet<T> listValueSet, T defaultSelection, bool allowNewEntries)
        {
            if (typeof(IVisualisable).IsAssignableFrom(typeof(T)))
            {
                return Show<T>(owner, new FormListBigListBox(), listValueSet, false, AsArray(defaultSelection, listValueSet), allowNewEntries).FirstOrDefault(listValueSet.CancelValue);
            }
            else
            {
                return Show<T>(owner, new FormListListBox(), listValueSet, false, AsArray(defaultSelection, listValueSet), allowNewEntries).FirstOrDefault(listValueSet.CancelValue);
            }
        }

        private static IEnumerable<T> AsArray<T>(T defaultSelection, DataSet<T> listValueSet)
        {
            if (defaultSelection == null || defaultSelection.Equals(listValueSet.CancelValue))
            {
                return new T[0];
            }
            else
            {
                return new T[] { defaultSelection };
            }
        }

        public static T ShowRadio<T>(Form owner, DataSet<T> listValueSet, T defaultSelection, bool allowNewEntries)
        {
            Debug.Assert(listValueSet.TypedGetList(true).Count() < 50, "When list count is large you might be better using a different view method.");
            return Show<T>(owner, new FormListRadioButtonArray(), listValueSet, false, AsArray(defaultSelection, listValueSet), allowNewEntries).FirstOrDefault(listValueSet.CancelValue);
        }

        public static T ShowButtons<T>(Form owner, DataSet<T> listValueSet, T defaultSelection, bool allowNewEntries)
        {
            Debug.Assert(listValueSet.TypedGetList(true).Count() < 50, "When list count is large you might be better using a different view method.");
            return Show<T>(owner, new FormListButtonArray(), listValueSet, false, AsArray(defaultSelection, listValueSet), allowNewEntries).FirstOrDefault(listValueSet.CancelValue);
        }

        public static IEnumerable<T> ShowCheckList<T>(Form owner, DataSet<T> listValueSet, IEnumerable<T> defaultSelection, bool allowNewEntries)
        {
            return Show<T>(owner, new FormListCheckList(), listValueSet, true, defaultSelection, allowNewEntries);
        }

        public static IEnumerable<T> ShowCheckBox<T>(Form owner, DataSet<T> listValueSet, IEnumerable<T> defaultSelection, bool allowNewEntries)
        {
            Debug.Assert(listValueSet.TypedGetList(true).Count() < 50, "When list count is large you might be better using a different view method.");
            return Show<T>(owner, new FormListCheckBoxArray(), listValueSet, true, defaultSelection, allowNewEntries);
        }

        private void _btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int n = 0; n < _numFields; n++)
            {
                _handler.SetState(n, true);
            }
        }

        private void _btnSelectNone_Click(object sender, EventArgs e)
        {
            for (int n = 0; n < _numFields; n++)
            {
                _handler.SetState(n, false);
            }
        }
    }
}
