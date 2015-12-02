﻿namespace MetaboliteLevels.Controls
{
    partial class CtlTitleBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._lblTitle = new System.Windows.Forms.Label();
            this._lblSubTitle = new System.Windows.Forms.Label();
            this._btnHelp = new System.Windows.Forms.Button();
            this._btnWarning = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.CornflowerBlue;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this._lblTitle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._lblSubTitle, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._btnHelp, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this._btnWarning, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.ForeColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(8);
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(256, 77);
            this.tableLayoutPanel1.TabIndex = 24;
            // 
            // _lblTitle
            // 
            this._lblTitle.AutoSize = true;
            this._lblTitle.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblTitle.Location = new System.Drawing.Point(11, 8);
            this._lblTitle.Name = "_lblTitle";
            this._lblTitle.Size = new System.Drawing.Size(138, 40);
            this._lblTitle.TabIndex = 1;
            this._lblTitle.Text = "Main title";
            // 
            // _lblSubTitle
            // 
            this._lblSubTitle.AutoSize = true;
            this._lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblSubTitle.Location = new System.Drawing.Point(11, 48);
            this._lblSubTitle.Name = "_lblSubTitle";
            this._lblSubTitle.Padding = new System.Windows.Forms.Padding(32, 0, 0, 0);
            this._lblSubTitle.Size = new System.Drawing.Size(94, 21);
            this._lblSubTitle.TabIndex = 1;
            this._lblSubTitle.Text = "Subtitle";
            // 
            // _btnHelp
            // 
            this._btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnHelp.AutoSize = true;
            this._btnHelp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnHelp.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this._btnHelp.FlatAppearance.BorderSize = 0;
            this._btnHelp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this._btnHelp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this._btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnHelp.Image = global::MetaboliteLevels.Properties.Resources.IcoHelp;
            this._btnHelp.Location = new System.Drawing.Point(223, 11);
            this._btnHelp.Name = "_btnHelp";
            this.tableLayoutPanel1.SetRowSpan(this._btnHelp, 2);
            this._btnHelp.Size = new System.Drawing.Size(22, 22);
            this._btnHelp.TabIndex = 2;
            this._btnHelp.UseVisualStyleBackColor = false;
            this._btnHelp.Visible = false;
            this._btnHelp.Click += new System.EventHandler(this._btnHelp_Click);
            // 
            // _btnWarning
            // 
            this._btnWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnWarning.AutoSize = true;
            this._btnWarning.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnWarning.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnWarning.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this._btnWarning.FlatAppearance.BorderSize = 0;
            this._btnWarning.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this._btnWarning.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this._btnWarning.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnWarning.Image = global::MetaboliteLevels.Properties.Resources._109_AllAnnotations_Warning_16x16_72;
            this._btnWarning.Location = new System.Drawing.Point(195, 11);
            this._btnWarning.Name = "_btnWarning";
            this._btnWarning.Size = new System.Drawing.Size(22, 22);
            this._btnWarning.TabIndex = 2;
            this._btnWarning.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this._btnWarning.UseVisualStyleBackColor = false;
            this._btnWarning.Visible = false;
            this._btnWarning.Click += new System.EventHandler(this._btnWarning_Click);
            this._btnWarning.MouseEnter += new System.EventHandler(this._btnWarning_MouseEnter);
            this._btnWarning.MouseLeave += new System.EventHandler(this._btnWarning_MouseLeave);
            // 
            // CtlTitleBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(256, 0);
            this.Name = "CtlTitleBar";
            this.Size = new System.Drawing.Size(256, 77);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label _lblTitle;
        private System.Windows.Forms.Label _lblSubTitle;
        private System.Windows.Forms.Button _btnHelp;
        private System.Windows.Forms.Button _btnWarning;

    }
}
