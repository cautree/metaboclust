﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetaboliteLevels.Algorithms.Statistics.Configurations;
using MetaboliteLevels.Controls;
using MetaboliteLevels.Data.Session;
using MetaboliteLevels.Data.Visualisables;
using MetaboliteLevels.Forms.Generic;
using MetaboliteLevels.Utilities;
using MetaboliteLevels.Algorithms;
using MetaboliteLevels.Algorithms.Statistics.Arguments;
using MetaboliteLevels.Data.DataInfo;
using MetaboliteLevels.Settings;
using MetaboliteLevels.Viewers.Charts;
using MetaboliteLevels.Data;

namespace MetaboliteLevels.Forms.Wizards
{
    public partial class FrmClusterWizard : Form
    {
        CtlWizard wizard;
        private Core _core;
        Peak _lowestPeak;
        Peak _highestPeak;
        Peak current;
        private EditableComboBox<Settings.ObsFilter> _ecbFilter;
        private readonly ChartHelperForPeaks _chart;

        internal static bool Show(Form owner, Core core)
        {
            using (FrmClusterWizard frm = new FrmClusterWizard(core, FrmMain.SearchForSelectedPeak(owner)))
            {
                return UiControls.ShowWithDim(owner, frm) == DialogResult.OK;
            }
        }

        public FrmClusterWizard()
        {
            InitializeComponent();
            UiControls.SetIcon(this);
        }

        private FrmClusterWizard(Core core, Peak current)
            : this()
        {
            this._core = core;

            _chart = new ChartHelperForPeaks(null, core, panel1);

            _ecbFilter = EditableComboBox.ForObsFilter(_lstFilters, _btnEditFilters, core);
            _lstGroups.Items.AddRange(NamedItem.GetRange(core.Groups, z => z.Name).ToArray());
            _lstGroups.SelectedIndex = 0;

            ctlStatistics1.Bind(core);

            this.current = current;
            _lblSeedCurrent.Text = current != null ? current.DisplayName : "None";
            _radSeedCurrent.Enabled = current != null;
            _lblSeedCurrent.Enabled = current != null;

            _lstStat.Items.AddRange(NamedItem.GetRange(core.ActiveStatistics, GetStatName).ToArray());

            wizard = CtlWizard.BindNew(this, tabControl1, CtlWizardOptions.DEFAULT | CtlWizardOptions.DialogResultCancel | CtlWizardOptions.HandleBasicChanges);
            wizard.OkClicked += wizard_OkClicked;
            wizard.TitleHelpText = "@k-means++";
            wizard.PermitAdvance += this.ValidatePage;
            wizard.Revalidate();

            UpdateStatBox();

            UiControls.CompensateForVisualStyles(this);
        }

        private string GetStatName(ConfigurationStatistic input)
        {
            return input.ToString();
        }

        private void UpdateStatBox()
        {
            ConfigurationStatistic stat = NamedItem<ConfigurationStatistic>.Extract(_lstStat.SelectedItem);
            bool b = stat != null;

            if (b)
            {
                _lowestPeak = _core.Peaks.FindLowest(λ => λ.GetStatistic(stat));
                _highestPeak = _core.Peaks.FindHighest(λ => λ.GetStatistic(stat));
                _lblSeedStudent.Text = _lowestPeak.DisplayName;
                _lblSeedPearson.Text = _highestPeak.DisplayName;
            }
            else
            {
                _lblSeedStudent.Text = "None";
                _lblSeedPearson.Text = "None";
            }

            _radSeedLowest.Enabled = b;
            _radSeedHighest.Enabled = b;
            _lblSeedStudent.Enabled = b;
            _lblSeedPearson.Enabled = b;
        }

        private void _lblSeedStudent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {                                                 
            _radSeedLowest.Checked = true;
        }

        private void _lblSeedPearson_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {                                                 
            _radSeedHighest.Checked = true;
        }

        private void _lblSeedCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {   
            _radSeedCurrent.Checked = true;
        }

        private bool ValidatePage(int p)
        {
            double tmpd;
            int tmpi;

            switch (p)
            {
                case 0:
                    return _ecbFilter.HasSelection;

                case 1:
                    return ((_radSeedLowest.Checked || _radSeedHighest.Checked)
                        && (_lstStat.SelectedItem != null) || _radSeedCurrent.Checked)
                        && _lstGroups.SelectedItem != null;

                case 2:
                    return ctlStatistics1.IsValid();

                case 3:
                    return (_radStopN.Checked && int.TryParse(_txtStopN.Text, out tmpi))
                        || (_radStopD.Checked && double.TryParse(_txtStopD.Text, out tmpd));

                case 4:
                    return _radFinishK.Checked || _radFinishStop.Checked;

                case 5:
                    return true;

                default:
                    return false;
            }
        }

        void wizard_OkClicked(object sender, EventArgs e)
        {
            // Check
            if (!wizard.RevalidateAll())
            {
                FrmMsgBox.ShowError(this, "Not all options have been selected.");
                return;
            }

            var statistics = ctlStatistics1.Retrieve();

            int param1_numClusters = _radStopN.Checked ? int.Parse(_txtStopN.Text) : int.MinValue;
            double param2_distanceLimit = _radStopD.Checked ? double.Parse(_txtStopD.Text) : double.MinValue;
            WeakReference<Peak> param3_seedPeak = new WeakReference<Peak>(_radSeedCurrent.Checked ? current : _radSeedHighest.Checked ? _highestPeak : _lowestPeak);
            GroupInfo param4_seedGroup = NamedItem<GroupInfo>.Extract(_lstGroups.SelectedItem);
            int param5_doKMeans = _radFinishK.Checked ? 1 : 0;
            object[] parameters = { param1_numClusters, param2_distanceLimit, param3_seedPeak, param4_seedGroup, param5_doKMeans };
            string name = "DK";

            // Create a constraint that only allows overlapping timepoints
            HashSet<int> overlappingPoints = new HashSet<int>();
            var fil = _ecbFilter.SelectedItem ?? ObsFilter.Empty;
            var passed = fil.Test(_core.Conditions);
            HashSet<GroupInfo> groups = new HashSet<GroupInfo>(passed.Select(z => z.Group));
            bool needsExFilter = false;

            foreach (int ctp in _core.Times)
            {
                bool trueInAny = false;
                bool falseInAny = false;

                foreach (GroupInfo g in groups)
                {
                    if (passed.Any(z => z.Group == g && z.Time == ctp))
                    {
                        trueInAny = true;
                    }
                    else
                    {
                        falseInAny = true;
                    }
                }

                if (trueInAny && !falseInAny) // i.e. true in all          TT
                {
                    overlappingPoints.Add(ctp);
                }
                else if (trueInAny) // i.e. true in one but not all        TF
                {
                    needsExFilter = true;
                }
                //else if (falseInAny) //  False in all (accptable)        FT
                // else       //     No groups                             FF
            }

            ObsFilter trueFilter;

            if (needsExFilter)
            {
                List<ObsFilter.Condition> conditions = new List<ObsFilter.Condition>
                                                       {
                                                           new ObsFilter.ConditionFilter(Filter.ELogicOperator.And, false,
                                                                                         _ecbFilter.SelectedItem, true),
                                                           new ObsFilter.ConditionTime(Filter.ELogicOperator.And, false,
                                                                                       Filter.EElementOperator.Is,
                                                                                       overlappingPoints)
                                                       };


                trueFilter = new Settings.ObsFilter(null, null, conditions);
                _core.AddObsFilter(trueFilter);
            }
            else
            {
                trueFilter = _ecbFilter.SelectedItem;
            }

            ArgsClusterer args = new ArgsClusterer(statistics.PeakFilter,
                                                    statistics.DistanceMetric,
                                                    EAlgoSourceMode.Trend,
                                                    trueFilter,
                                                    _chkClusterIndividually.Checked,
                                                    EClustererStatistics.None,
                                                    parameters);

            ConfigurationClusterer config = new ConfigurationClusterer(name, "Generated using wizard", Algo.ID_DKMEANSPPWIZ, args);

            FrmWait.Show(this, "Clustering", null, z => _core.AddClusterer(config, z));
            DialogResult = DialogResult.OK;
        }

        private void OptionChanged(object sender, EventArgs e)
        {
            if (wizard != null)
            {
                wizard.Revalidate();
            }

            if (_radSeedCurrent.Checked)
            {
                _chart.Plot(new StylisedPeak(current));
            }
            else if (_radSeedHighest.Checked)
            {
                _chart.Plot(new StylisedPeak(_highestPeak));
            }
            else if (_radSeedLowest.Checked)
            {
                _chart.Plot(new StylisedPeak(_lowestPeak));
            }
        }

        private void _radStopN_CheckedChanged(object sender, EventArgs e)
        {
            _txtStopN.Enabled = _radStopN.Checked;
            OptionChanged(sender, e);
        }

        private void _radStopD_CheckedChanged(object sender, EventArgs e)
        {
            _txtStopD.Enabled = _radStopD.Checked;
            OptionChanged(sender, e);
        }

        private void _lstStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStatBox();
            OptionChanged(sender, e);
        }

        private void _chkClusterIndividually_CheckedChanged(object sender, EventArgs e)
        {
            _lstGroups.Enabled = _chkClusterIndividually.Checked;
        }
    }
}