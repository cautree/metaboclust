﻿using MetaboliteLevels.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetaboliteLevels.Data.DataInfo;
using MetaboliteLevels.Forms;
using System.Drawing;
using MetaboliteLevels.Data.Session;
using MetaboliteLevels.Data.Visualisables;
using MetaboliteLevels.Forms.Generic;
using MetaboliteLevels.Utilities;
using MetaboliteLevels.Settings;

namespace MetaboliteLevels.Controls
{
    /// <summary>
    /// See ConditionBox.
    /// </summary>
    internal class ConditionBox<T>
    {
        private readonly DataSet<T> _args;
        private readonly bool _integerBehaviour;
        private readonly Color _defaultColour;

        public readonly TextBox TextBox;
        public readonly Button Button;

        private HashSet<T> _selected = new HashSet<T>();
        private HashSet<T> _lastValidSelection;

        bool _allowNewEntries;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConditionBox(DataSet<T> args, TextBox textBox, Button button, bool allowNewEntries)
        {
            this._args = args;
            this._integerBehaviour = args.IntegerBehaviour && typeof(T) == typeof(int);
            this.TextBox = textBox;
            this.Button = button;

            _defaultColour = textBox.BackColor;
            _allowNewEntries = allowNewEntries;

            textBox.Validating += _textBox_Validating;
            button.Click += button_Click;
        }

        /// <summary>
        /// Show list button clicked.
        /// </summary>
        void button_Click(object sender, EventArgs e)
        {
            // Get available options
            DataSet<T> sel = _args.Clone();
                                       
            IEnumerable<T> newSelected;

            if (_args.TypedGetList(true).Count() < 10)
            {
                newSelected = sel.ShowCheckBox(TextBox.FindForm(), _lastValidSelection, _allowNewEntries);
            }
            else
            {
                newSelected = sel.ShowCheckList(TextBox.FindForm(), _lastValidSelection, _allowNewEntries);
            }

            if (newSelected != null)
            {
                _selected = new HashSet<T>(newSelected);
                _lastValidSelection = _selected;

                UpdateText();
            }
        }

        /// <summary>
        /// Handler.
        /// </summary>
        void _textBox_Validating(object sender, CancelEventArgs e)
        {
            if (UpdateList())
            {
                UpdateText();
            }
        }

        /// <summary>
        /// Gets a copy of the selected items set, or throws an error for an invalid selection.
        /// </summary>
        public HashSet<T> GetSelectedItemsE()
        {
            if (_selected == null)
            {
                throw new InvalidOperationException("Invalid selection.");
            }

            return new HashSet<T>(_selected);
        }

        /// <summary>
        /// Gets a copy of the selection items set, or returns NULL if the selection is invalid.
        /// </summary>
        public HashSet<T> GetSelection()
        {
            if (_selected == null)
            {
                return null;
            }

            return new HashSet<T>(_selected);
        }

        /// <summary>
        /// Is the selection valid?
        /// </summary>
        public bool SelectionValid
        {
            get { return _selected != null; }
        }

        /// <summary>
        /// Gets or sets the most recent valid selected items.
        /// 
        /// Returns NULL if the selection is invalid.
        /// Setting this to NULL will clear the selection.
        /// </summary>
        public IEnumerable<T> SelectedItems
        {
            get
            {
                return _selected;
            }
            set
            {
                if (value == null)
                {
                    _selected = new HashSet<T>();
                }
                else
                {
                    _selected = new HashSet<T>(value);
                }

                _lastValidSelection = _selected;

                UpdateText();
            }
        }

        /// <summary>
        /// Updates the textbox text to reflect the current selection.
        /// </summary>
        private void UpdateText()
        {
            if (_integerBehaviour)
            {
                TextBox.Text = StringHelper.ArrayToStringInt(_lastValidSelection.Cast<int>());
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                HashSet<T> sel = new HashSet<T>(_lastValidSelection);

                foreach (var choice in _args.TypedGetList(true))
                {
                    if (sel.Contains(choice))
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(", ");
                        }

                        sb.Append(GetText(choice));
                        sel.Remove(choice);
                    }
                }

                // Choices not in list
                foreach (T extraChoice in sel)
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(", ");
                    }

                    sb.Append(GetText(extraChoice));
                }

                TextBox.Text = sb.ToString();
            }
        }

        /// <summary>
        /// Returns the text for the specified choice.
        /// </summary>
        private string GetText(T choice)
        {                        
            return _args.ItemNameProvider(choice);
        }

        /// <summary>
        /// Updates the selection list to reflect the current text.
        /// </summary>
        private bool UpdateList()
        {
            bool r = DoUpdateList();

            if (r)
            {
                this.TextBox.BackColor = _defaultColour;
            }
            else
            {
                this.TextBox.BackColor = Color.Red;
            }

            return r;
        }

        /// <summary>
        /// Updates the selection list to reflect the current text.
        /// </summary>
        private bool DoUpdateList()
        {
            var choices = _args.TypedGetList(true);

            if (_integerBehaviour)
            {
                List<int> ints = StringHelper.StringToArrayInt(TextBox.Text, EErrorHandler.ReturnNull);

                if (ints == null || ints.Any(z => !choices.Cast<int>().Contains(z)))
                {
                    _selected = null;
                    return false;
                }

                _selected = new HashSet<T>(ints.Cast<T>());
                _lastValidSelection = _selected;
                return true;
            }
            else
            {
                string[] ee = TextBox.Text.Split(",".ToCharArray());

                HashSet<T> result = new HashSet<T>();

                foreach (string e in ee)
                {
                    if (!string.IsNullOrWhiteSpace(e))
                    {
                        T choice;

                        if (!TryGetChoice(choices, e, out choice))
                        {
                            _selected = null;
                            return false;
                        }

                        if (!_allowNewEntries && !choices.Contains(choice))
                        {
                            _selected = null;
                            return false;
                        }

                        result.Add(choice);
                    }
                }

                _selected = result;
                _lastValidSelection = _selected;
                return true;
            }
        }

        /// <summary>
        /// Trys to determine the choice based on the text.
        /// </summary>
        private bool TryGetChoice(IEnumerable<T> choices, string name, out T result)
        {
            name = name.ToUpper().Trim();

            if (_args.NewItemRetriever != null)
            {
                if (_args.NewItemRetriever(name, out result))
                {
                    return true;
                }
            }

            if (_args.StringComparator != null)
            {
                foreach (var i in choices)
                {
                    if (_args.StringComparator(name, i))
                    {
                        result = i;
                        return true;
                    }
                }
            }

            foreach (var choice in choices)
            {
                if (_args.ItemNameProvider != null && _args.ItemNameProvider(choice).ToUpper() == name)
                {
                    result = choice;
                    return true;
                }
                else if (choice.ToString().ToUpper() == name)
                {
                    result = choice;
                    return true;
                }
            }

            result = default(T);
            return false;
        }

        public bool Enabled
        {
            get
            {
                return TextBox.Enabled && Button.Enabled;
            }
            set
            {
                TextBox.Enabled = value;
                Button.Enabled = value;
            }
        }

        public bool Visible
        {
            get
            {
                return TextBox.Visible && Button.Visible;
            }
            set
            {
                TextBox.Visible = value;
                Button.Visible = value;
            }
        }
    }
}
