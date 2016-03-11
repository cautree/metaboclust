﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetaboliteLevels.Data.Session;
using MetaboliteLevels.Settings;
using MetaboliteLevels.Utilities;

namespace MetaboliteLevels.Forms.Editing
{
    internal partial class FrmOptions2 : Form
    {
        public static bool Show(Form owner, Core core)
        {
            using (FrmOptions2 frm = new FrmOptions2(core.Options))
            {
                switch (frm.ShowDialog())
                {
                    case DialogResult.OK:
                        return true;

                    case DialogResult.Cancel:
                        return false;

                    case DialogResult.Yes:
                        FrmOptions.Show(owner, core);
                        return true;

                    default:
                        throw new SwitchException();
                }
            }
        }

        public FrmOptions2(CoreOptions target)
        {
            InitializeComponent();
            UiControls.SetIcon(this);

            _target = target;

            Bind(_txtClusterXAxis, nameof(CoreOptions.ClusterDisplay), nameof(CoreOptions.PlotSetup.AxisX));
            Bind(_txtClusterYAxis, nameof(CoreOptions.ClusterDisplay), nameof(CoreOptions.PlotSetup.AxisY));
            Bind(_txtClusterInfo, nameof(CoreOptions.ClusterDisplay), nameof(CoreOptions.PlotSetup.Information));
            Bind(_txtClusterSubtitle, nameof(CoreOptions.ClusterDisplay), nameof(CoreOptions.PlotSetup.SubTitle));
            Bind(_txtClusterTitle, nameof(CoreOptions.ClusterDisplay), nameof(CoreOptions.PlotSetup.Title));

            Bind(_txtEvalFilename, nameof(CoreOptions.ClusteringEvaluationResultsFileName));

            Bind(_btnColourAxisTitle, nameof(CoreOptions.Colours), nameof(CoreColourSettings.AxisTitle));
            Bind(_btnColourCentre, nameof(CoreOptions.Colours), nameof(CoreColourSettings.ClusterCentre));
            Bind(_btnColourUntypedElements, nameof(CoreOptions.Colours), nameof(CoreColourSettings.InputVectorJoiners));
            Bind(_btnColourMajorGrid, nameof(CoreOptions.Colours), nameof(CoreColourSettings.MajorGrid));
            Bind(_btnColourMinorGrid, nameof(CoreOptions.Colours), nameof(CoreColourSettings.MinorGrid));
            Bind(_btnColourHighlight, nameof(CoreOptions.Colours), nameof(CoreColourSettings.NotableHighlight));
            Bind(_btnColourBackground, nameof(CoreOptions.Colours), nameof(CoreColourSettings.PlotBackground));
            Bind(_btnColourPreviewBackground, nameof(CoreOptions.Colours), nameof(CoreColourSettings.PreviewBackground));
            Bind(_btnColourSeries, nameof(CoreOptions.Colours), nameof(CoreColourSettings.SelectedSeries));

            Bind(_txtCompXAxis, nameof(CoreOptions.CompoundDisplay), nameof(CoreOptions.PlotSetup.AxisX));
            Bind(_txtCompYAxis, nameof(CoreOptions.CompoundDisplay), nameof(CoreOptions.PlotSetup.AxisY));
            Bind(_txtCompInfo, nameof(CoreOptions.CompoundDisplay), nameof(CoreOptions.PlotSetup.Information));
            Bind(_txtCompSubtitle, nameof(CoreOptions.CompoundDisplay), nameof(CoreOptions.PlotSetup.SubTitle));
            Bind(_txtCompTitle, nameof(CoreOptions.CompoundDisplay), nameof(CoreOptions.PlotSetup.Title));

            Bind(_lstPeakPlotting, nameof(CoreOptions.ConditionsSideBySide));
            Bind(_chkPeakFlag, nameof(CoreOptions.EnablePeakFlagging));
            Bind(_numClusterMaxPlot, nameof(CoreOptions.MaxPlotVariables));
            Bind(_numSizeLimit, nameof(CoreOptions.ObjectSizeLimit));

            Bind(_txtPathXAxis, nameof(CoreOptions.PathwayDisplay), nameof(CoreOptions.PlotSetup.AxisX));
            Bind(_txtPathYAxis, nameof(CoreOptions.PathwayDisplay), nameof(CoreOptions.PlotSetup.AxisY));
            Bind(_txtPathInfo, nameof(CoreOptions.PathwayDisplay), nameof(CoreOptions.PlotSetup.Information));
            Bind(_txtPathSubtitle, nameof(CoreOptions.PathwayDisplay), nameof(CoreOptions.PlotSetup.SubTitle));
            Bind(_txtPathTitle, nameof(CoreOptions.PathwayDisplay), nameof(CoreOptions.PlotSetup.Title));

            Bind(_txtPeakXAxis, nameof(CoreOptions.PeakDisplay), nameof(CoreOptions.PlotSetup.AxisX));
            Bind(_txtPeakYAxis, nameof(CoreOptions.PeakDisplay), nameof(CoreOptions.PlotSetup.AxisY));
            Bind(_txtPeakInfo, nameof(CoreOptions.PeakDisplay), nameof(CoreOptions.PlotSetup.Information));
            Bind(_txtPeakSubtitle, nameof(CoreOptions.PeakDisplay), nameof(CoreOptions.PlotSetup.SubTitle));
            Bind(_txtPeakTitle, nameof(CoreOptions.PeakDisplay), nameof(CoreOptions.PlotSetup.Title));

            Bind(numericUpDown2, nameof(CoreOptions.PopoutThumbnailSize));
            Bind(_chkClusterCentres, nameof(CoreOptions.ShowCentres));
            Bind(_chkPeakData, nameof(CoreOptions.ShowPoints));
            Bind(_chkPeakTrend, nameof(CoreOptions.ShowTrend));
            Bind(_chkPeakMean, nameof(CoreOptions.ShowVariableMean));
            Bind(_chkPeakRanges, nameof(CoreOptions.ShowVariableRanges));

            Bind(_numThumbnail, nameof(CoreOptions.ThumbnailSize));

            Bind(_lstPeakOrder, nameof(CoreOptions.ViewAcquisition));

            Bind(_lstPeakData, nameof(CoreOptions.ViewAlternativeObservations));

            //Bind(_aaa, nameof(CoreOptions.PeakFlags));
            //Bind(_xyz, nameof(CoreOptions.ViewTypes));

            foreach (var kvp in props)
            {
                ControlSetValue(kvp.Key, kvp.Value);
            }

            UiControls.CompensateForVisualStyles(this);
        }

        private void _btnOk_Click(object sender, EventArgs e)
        {
            SaveAndClose(DialogResult.OK);
        }

        private void SaveAndClose(DialogResult result)
        {
            foreach (var kvp in props)
            {
                PropertySetValue(kvp.Value, ControlGetValue(kvp.Key));
            }

            DialogResult = result;
        }

        private object ControlGetValue(Control c)
        {
            if (c is TextBox)
            {
                return ((TextBox)c).Text;
            }
            else if (c is CheckBox)
            {
                return ((CheckBox)c).Checked;
            }
            else if (c is ComboBox)
            {
                return ((ComboBox)c).SelectedIndex != 0;
            }
            else if (c is NumericUpDown)
            {
                return (int)((NumericUpDown)c).Value;
            }
            else if (c is Button)
            {
                return ((Button)c).BackColor;
            }
            else
            {
                throw new SwitchException(c);
            }
        }

        private void ControlSetValue(Control c, string[] propertyPath)
        {
            PropertyInfo property = null;
            object value = _target;

            foreach (string name in propertyPath)
            {
                property = value.GetType().GetProperty(name);
                value = property.GetValue(value);
            }

            StringBuilder sb = new StringBuilder();
            CategoryAttribute cat = property.GetCustomAttribute<CategoryAttribute>();
            DisplayNameAttribute namer = property.GetCustomAttribute<DisplayNameAttribute>();
            DescriptionAttribute desc = property.GetCustomAttribute<DescriptionAttribute>();

            if (cat != null)
            {
                sb.Append(cat.Category.ToBold() + ": ");
            }

            if (namer != null)
            {
                sb.Append(namer.DisplayName.ToBold());
            }
            else
            {
                sb.Append(property.Name.ToBold());
            }

            if (desc != null)
            {
                sb.AppendLine();
                sb.Append(desc.Description);
            }

            toolTip1.SetToolTip(c, sb.ToString());

            if (c is TextBox)
            {
                ((TextBox)c).Text = value != null ? value.ToString() : "";
            }
            else if (c is CheckBox)
            {
                ((CheckBox)c).Checked = (bool)value;
            }
            else if (c is ComboBox)
            {
                ((ComboBox)c).SelectedIndex = ((bool)value ? 0 : 1);
            }
            else if (c is NumericUpDown)
            {
                ((NumericUpDown)c).Value = (int)value;
            }
            else if (c is Button)
            {
                ((Button)c).BackColor = (Color)value;
            }
            else
            {
                throw new SwitchException(c);
            }
        }

        private void PropertySetValue(string[] values, object v)
        {
            object target = _target;

            for (int n = 0; n < values.Length; n++)
            {
                string value = values[n];
                PropertyInfo property = target.GetType().GetProperty(value);

                if (n == values.Length - 1)
                {
                    if (property.PropertyType == typeof(ParseElementCollection))
                    {
                        v = new ParseElementCollection((string)v);
                    }

                    property.SetValue(target, v);
                }
                else
                {
                    target = property.GetValue(target);
                }
            }
        }

        Dictionary<Control, string[]> props = new Dictionary<Control, string[]>();
        private CoreOptions _target;

        private void Bind(Control control, params string[] properties)
        {
            props.Add(control, properties);
        }

        private void _btnColourCentre_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = ((Button)sender).BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                ((Button)sender).BackColor = colorDialog1.Color;
            }
        }

        private void _btnEditFlags_Click(object sender, EventArgs e)
        {
            FrmOptions.Show(this, "Peak Flags", _target.PeakFlags);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveAndClose(DialogResult.Yes);
        }
    }
}