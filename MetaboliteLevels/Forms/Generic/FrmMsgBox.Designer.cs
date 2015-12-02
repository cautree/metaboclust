﻿namespace MetaboliteLevels.Forms.Generic
{
    partial class FrmMsgBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMsgBox));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ctlTitleBar1 = new MetaboliteLevels.Controls.CtlTitleBar();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._btn1 = new MetaboliteLevels.Controls.CtlButton();
            this._btn2 = new MetaboliteLevels.Controls.CtlButton();
            this._btn3 = new MetaboliteLevels.Controls.CtlButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ctlTitleBar1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(695, 194);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.Image = global::MetaboliteLevels.Properties.Resources.MsgAccept;
            this.pictureBox1.Location = new System.Drawing.Point(16, 82);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(16, 16, 8, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // ctlTitleBar1
            // 
            this.ctlTitleBar1.AutoSize = true;
            this.ctlTitleBar1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.ctlTitleBar1, 2);
            this.ctlTitleBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ctlTitleBar1.HelpText = null;
            this.ctlTitleBar1.Location = new System.Drawing.Point(0, 0);
            this.ctlTitleBar1.Margin = new System.Windows.Forms.Padding(0);
            this.ctlTitleBar1.MinimumSize = new System.Drawing.Size(384, 0);
            this.ctlTitleBar1.Name = "ctlTitleBar1";
            this.ctlTitleBar1.Size = new System.Drawing.Size(695, 66);
            this.ctlTitleBar1.SubText = "";
            this.ctlTitleBar1.TabIndex = 0;
            this.ctlTitleBar1.Text = "ctlTitleBar1";
            this.ctlTitleBar1.WarningText = null;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 79);
            this.label1.Margin = new System.Windows.Forms.Padding(12, 13, 12, 13);
            this.label1.MaximumSize = new System.Drawing.Size(600, 2048);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(599, 42);
            this.label1.TabIndex = 1;
            this.label1.Text = "This is a message telling the user something. If you are seeing this there is an " +
    "error or the programmer forgot to put the message text in. Sorry about that.";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this._btn1);
            this.flowLayoutPanel1.Controls.Add(this._btn2);
            this.flowLayoutPanel1.Controls.Add(this._btn3);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(263, 138);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(432, 56);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // _btn1
            // 
            this._btn1.Image = ((System.Drawing.Image)(resources.GetObject("_btn1.Image")));
            this._btn1.Location = new System.Drawing.Point(8, 8);
            this._btn1.Margin = new System.Windows.Forms.Padding(8);
            this._btn1.Name = "_btn1";
            this._btn1.Size = new System.Drawing.Size(128, 40);
            this._btn1.TabIndex = 3;
            this._btn1.Text = "Button 1";
            this._btn1.UseVisualStyleBackColor = true;
            this._btn1.Visible = false;
            // 
            // _btn2
            // 
            this._btn2.Image = ((System.Drawing.Image)(resources.GetObject("_btn2.Image")));
            this._btn2.Location = new System.Drawing.Point(152, 8);
            this._btn2.Margin = new System.Windows.Forms.Padding(8);
            this._btn2.Name = "_btn2";
            this._btn2.Size = new System.Drawing.Size(128, 40);
            this._btn2.TabIndex = 4;
            this._btn2.Text = "Button 2";
            this._btn2.UseVisualStyleBackColor = true;
            this._btn2.Visible = false;
            // 
            // _btn3
            // 
            this._btn3.Image = ((System.Drawing.Image)(resources.GetObject("_btn3.Image")));
            this._btn3.Location = new System.Drawing.Point(296, 8);
            this._btn3.Margin = new System.Windows.Forms.Padding(8);
            this._btn3.Name = "_btn3";
            this._btn3.Size = new System.Drawing.Size(128, 40);
            this._btn3.TabIndex = 5;
            this._btn3.Text = "Button 3";
            this._btn3.UseVisualStyleBackColor = true;
            this._btn3.Visible = false;
            // 
            // FrmMsgBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(851, 275);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrmMsgBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.CtlTitleBar ctlTitleBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Controls.CtlButton _btn1;
        private Controls.CtlButton _btn2;
        private Controls.CtlButton _btn3;
    }
}