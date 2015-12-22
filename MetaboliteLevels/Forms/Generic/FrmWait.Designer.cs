﻿namespace MetaboliteLevels.Forms.Generic
{
    partial class FrmWait
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmWait));
            this.ctlTitleBar1 = new MetaboliteLevels.Controls.CtlTitleBar();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._chkStop = new System.Windows.Forms.CheckBox();
            this._chkDeprioritise = new System.Windows.Forms.CheckBox();
            this._chkPrioritise = new System.Windows.Forms.CheckBox();
            this._chkSuspend = new System.Windows.Forms.CheckBox();
            this._chkLazy = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctlTitleBar1
            // 
            this.ctlTitleBar1.AutoSize = true;
            this.ctlTitleBar1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ctlTitleBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ctlTitleBar1.HelpText = resources.GetString("ctlTitleBar1.HelpText");
            this.ctlTitleBar1.Location = new System.Drawing.Point(0, 0);
            this.ctlTitleBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ctlTitleBar1.MinimumSize = new System.Drawing.Size(384, 0);
            this.ctlTitleBar1.Name = "ctlTitleBar1";
            this.ctlTitleBar1.Size = new System.Drawing.Size(749, 87);
            this.ctlTitleBar1.SubText = "Doing something";
            this.ctlTitleBar1.TabIndex = 0;
            this.ctlTitleBar1.Text = "Please wait";
            this.ctlTitleBar1.WarningText = null;
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 2);
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar1.Location = new System.Drawing.Point(78, 45);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.progressBar1.MarqueeAnimationSpeed = 200;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(590, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 87);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(749, 115);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.ForeColor = System.Drawing.Color.Silver;
            this.label1.Location = new System.Drawing.Point(667, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 21);
            this.label1.TabIndex = 3;
            this.label1.Text = "0 seconds";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this._chkStop);
            this.flowLayoutPanel1.Controls.Add(this._chkDeprioritise);
            this.flowLayoutPanel1.Controls.Add(this._chkPrioritise);
            this.flowLayoutPanel1.Controls.Add(this._chkSuspend);
            this.flowLayoutPanel1.Controls.Add(this._chkLazy);
            this.flowLayoutPanel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(160, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(425, 34);
            this.flowLayoutPanel1.TabIndex = 4;
            this.flowLayoutPanel1.Visible = false;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // _chkStop
            // 
            this._chkStop.AutoSize = true;
            this._chkStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._chkStop.Location = new System.Drawing.Point(3, 3);
            this._chkStop.Name = "_chkStop";
            this._chkStop.Size = new System.Drawing.Size(72, 25);
            this._chkStop.TabIndex = 0;
            this._chkStop.Text = "Cancel";
            this._chkStop.UseVisualStyleBackColor = true;
            this._chkStop.CheckedChanged += new System.EventHandler(this._chkStop_CheckedChanged);
            // 
            // _chkDeprioritise
            // 
            this._chkDeprioritise.AutoSize = true;
            this._chkDeprioritise.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._chkDeprioritise.Location = new System.Drawing.Point(81, 3);
            this._chkDeprioritise.Name = "_chkDeprioritise";
            this._chkDeprioritise.Size = new System.Drawing.Size(107, 25);
            this._chkDeprioritise.TabIndex = 1;
            this._chkDeprioritise.Text = "Deprioritise";
            this._chkDeprioritise.UseVisualStyleBackColor = true;
            this._chkDeprioritise.CheckedChanged += new System.EventHandler(this._chkDeprioritise_CheckedChanged);
            // 
            // _chkPrioritise
            // 
            this._chkPrioritise.AutoSize = true;
            this._chkPrioritise.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._chkPrioritise.Location = new System.Drawing.Point(194, 3);
            this._chkPrioritise.Name = "_chkPrioritise";
            this._chkPrioritise.Size = new System.Drawing.Size(88, 25);
            this._chkPrioritise.TabIndex = 3;
            this._chkPrioritise.Text = "Prioritise";
            this._chkPrioritise.UseVisualStyleBackColor = true;
            this._chkPrioritise.CheckedChanged += new System.EventHandler(this._chkPrioritise_CheckedChanged);
            // 
            // _chkSuspend
            // 
            this._chkSuspend.AutoSize = true;
            this._chkSuspend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._chkSuspend.Location = new System.Drawing.Point(288, 3);
            this._chkSuspend.Name = "_chkSuspend";
            this._chkSuspend.Size = new System.Drawing.Size(67, 25);
            this._chkSuspend.TabIndex = 2;
            this._chkSuspend.Text = "Pause";
            this._chkSuspend.UseVisualStyleBackColor = true;
            this._chkSuspend.CheckedChanged += new System.EventHandler(this._chkSuspend_CheckedChanged);
            // 
            // _chkLazy
            // 
            this._chkLazy.AutoSize = true;
            this._chkLazy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._chkLazy.Location = new System.Drawing.Point(361, 3);
            this._chkLazy.Name = "_chkLazy";
            this._chkLazy.Size = new System.Drawing.Size(57, 25);
            this._chkLazy.TabIndex = 4;
            this._chkLazy.Text = "Lazy";
            this._chkLazy.UseVisualStyleBackColor = true;
            this._chkLazy.CheckedChanged += new System.EventHandler(this._chkLazy_CheckedChanged);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.ForeColor = System.Drawing.Color.Silver;
            this.label2.Location = new System.Drawing.Point(3, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "0 bytes";
            // 
            // FrmWait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(749, 202);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.ctlTitleBar1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrmWait";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Busy";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.CtlTitleBar ctlTitleBar1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox _chkStop;
        private System.Windows.Forms.CheckBox _chkDeprioritise;
        private System.Windows.Forms.CheckBox _chkSuspend;
        private System.Windows.Forms.CheckBox _chkPrioritise;
        private System.Windows.Forms.CheckBox _chkLazy;
        private System.Windows.Forms.Label label2;
    }
}