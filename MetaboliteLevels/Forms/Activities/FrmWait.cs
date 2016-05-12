﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using MetaboliteLevels.Properties;
using MetaboliteLevels.Utilities;
using MGui.Helpers;

namespace MetaboliteLevels.Forms.Generic
{
    public partial class FrmWait : Form
    {
        private Callable function;
        private Stopwatch _operationTimer = Stopwatch.StartNew();
        private bool _allowClose;
        private Thread _thread;
        private Info _info;
        private ProgressReporter _prog;

        private class Info : IProgressReceiver
        {
            private readonly FrmWait form;                                  

            public Info(FrmWait frmWait)
            {
                this.form = frmWait;
            }

            void IProgressReceiver.ReportProgressDetails(ProgressReporter.ProgInfo info)
            {
                form.backgroundWorker1.ReportProgress(0, info);
            }
        }

        private abstract class Callable
        {
            public Exception Error;
            public object Result { get; private set; }

            internal void Invoke(ProgressReporter info)
            {
                Result = OnInvoke(info);
            }

            protected abstract object OnInvoke(ProgressReporter info);
        }

        private class Callable<TResult, TArgs> : Callable
        {
            public TArgs args;

            // Info and args
            public Func<ProgressReporter, TArgs, TResult> InfoArgsWithResult;
            public Action<ProgressReporter, TArgs> InfoArgsWithoutResult;

            // Info
            public Func<ProgressReporter, TResult> InfoWithResult;
            public Action<ProgressReporter> InfoWithoutResult;

            // Args
            public Func<TArgs, TResult> ArgsWithResult;
            public Action<TArgs> ArgsWithoutResult;

            // None
            public Func<TResult> WithResult;
            public Action WithoutResult;

            protected override object OnInvoke(ProgressReporter info)
            {
                if (InfoArgsWithResult != null)
                {
                    return InfoArgsWithResult(info, args);
                }
                else if (InfoArgsWithoutResult != null)
                {
                    InfoArgsWithoutResult(info, args);
                    return null;
                }
                else if (InfoWithResult != null)
                {
                    return InfoWithResult(info);
                }
                else if (InfoWithoutResult != null)
                {
                    InfoWithoutResult(info);
                    return null;
                }
                else if (ArgsWithResult != null)
                {
                    return ArgsWithResult(args);
                }
                else if (ArgsWithoutResult != null)
                {
                    ArgsWithoutResult(args);
                    return null;
                }
                else if (WithResult != null)
                {
                    return WithResult();
                }
                else if (WithoutResult != null)
                {
                    WithoutResult();
                    return null;
                }
                else
                {
                    throw new InvalidOperationException("No delegate provided.");
                }
            }
        }

        private class NA
        {
        }

        internal static void Show(Form owner, string title, string subtitle, Action action)
        {
            Show(owner, title, subtitle, new Callable<NA, NA> { WithoutResult = action });
        }

        internal static void Show(Form owner, string title, string subtitle, Action<ProgressReporter> action)
        {
            Show(owner, title, subtitle, new Callable<NA, NA> { InfoWithoutResult = action });
        }

        internal static void Show<TArgs>(Form owner, string title, string subtitle, Action<TArgs> action, TArgs args)
        {
            Show(owner, title, subtitle, new Callable<NA, TArgs> { ArgsWithoutResult = action, args = args });
        }

        internal static void Show<TArgs>(Form owner, string title, string subtitle, Action<ProgressReporter, TArgs> action, TArgs args)
        {
            Show(owner, title, subtitle, new Callable<NA, TArgs> { InfoArgsWithoutResult = action, args = args });
        }

        internal static TResult Show<TResult>(Form owner, string title, string subtitle, Func<TResult> action)
        {
            return (TResult)Show(owner, title, subtitle, new Callable<TResult, NA> { WithResult = action });
        }

        internal static TResult Show<TResult>(Form owner, string title, string subtitle, Func<ProgressReporter, TResult> action)
        {
            return (TResult)Show(owner, title, subtitle, new Callable<TResult, NA> { InfoWithResult = action });
        }

        internal static TResult Show<TResult, TArgs>(Form owner, string title, string subtitle, Func<TArgs, TResult> action, TArgs args)
        {
            return (TResult)Show(owner, title, subtitle, new Callable<TResult, TArgs> { ArgsWithResult = action, args = args });
        }

        internal static TResult Show<TResult, TArgs>(Form owner, string title, string subtitle, Func<ProgressReporter, TArgs, TResult> action, TArgs args)
        {
            return (TResult)Show(owner, title, subtitle, new Callable<TResult, TArgs> { InfoArgsWithResult = action, args = args });
        }

        private static object Show(Form owner, string title, string subtitle, Callable callable)
        {
            using (FrmWait frm = new FrmWait(title, subtitle, callable))
            {
                UiControls.ShowWithDim(owner, frm);

                if (callable.Error != null)
                {
                    throw new Exception(callable.Error.Message, callable.Error);
                }

                return callable.Result;
            }
        }

        public FrmWait()
        {
            InitializeComponent();
            UiControls.SetIcon(this);
        }

        private FrmWait(string title, string subtitle, Callable callable)
            : this()
        {
            this.ctlTitleBar1.Text = title;
            this.ctlTitleBar1.SubText = subtitle;
            this.function = callable;                           
            // UiControls.CompensateForVisualStyles(this);
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            _info = new Info(this);
            _thread = Thread.CurrentThread;
            _prog = new ProgressReporter(_info);

            // Give a chance for the form to show
            Thread.Sleep(1000);

            function.Invoke(_prog);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!_allowClose)
            {
                flowLayoutPanel1.Visible = true;
                e.Cancel = true;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressReporter.ProgInfo info = (ProgressReporter.ProgInfo)e.UserState;

            ctlTitleBar1.SubText = info.Text;

            if (info.Percent >= 0)
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Maximum = 100;
                progressBar1.Value = info.Percent;
            }
            else
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
            }

            label1.Text = StringHelper.TimeAsString(_operationTimer.Elapsed);

            if (info.CText != null)
            {
                label2.Text = info.CText;
                label2.Visible = true;
            }
            else
            {
                label2.Visible = false;
            }  
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _allowClose = true;

            if (e.Error != null)
            {
                function.Error = e.Error;
            }

            DialogResult = DialogResult.OK;
        }

        private void _chkStop_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkStop.Checked)
            {
                _prog.SetCancelAsync(true);
            }
            else
            {
                _prog.SetCancelAsync(false);
            }
        }

        private void _chkSuspend_CheckedChanged(object sender, EventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (_chkSuspend.Checked)
            {
                _thread.Suspend();
            }
            else
            {
                _thread.Resume();
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private void _chkDeprioritise_CheckedChanged(object sender, EventArgs e)
        {
            _chkPrioritise.Enabled = !_chkDeprioritise.Checked;

            if (_chkDeprioritise.Checked)
            {
                _thread.Priority = ThreadPriority.Lowest;
            }
            else
            {
                _thread.Priority = ThreadPriority.Normal;
            }
        }

        private void _chkPrioritise_CheckedChanged(object sender, EventArgs e)
        {
            _chkDeprioritise.Enabled = !_chkPrioritise.Checked;

            if (_chkPrioritise.Checked)
            {
                _thread.Priority = ThreadPriority.Highest;
            }
            else
            {
                _thread.Priority = ThreadPriority.Normal;
            }

        }

        private void _chkLazy_CheckedChanged(object sender, EventArgs e)
        {
            _prog.SetLazyModeAsync(_chkLazy.Checked);
        }
    }
}