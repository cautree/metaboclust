﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetaboliteLevels.Data.Algorithms.General;
using MetaboliteLevels.Data.Session.Associational;
using MetaboliteLevels.Data.Session.General;
using MetaboliteLevels.Gui.Controls.Lists;
using MetaboliteLevels.Gui.Forms.Selection;
using MetaboliteLevels.Utilities;
using MGui.Controls;
using MGui.Helpers;

namespace MetaboliteLevels.Gui.Forms.Activities
{                    
    internal partial class FrmActHeatMap : Form
    {                                  
        private readonly Column _source1D;
        private readonly DistanceMatrix _source2D;
        private readonly CtlAutoList _sourceList;
        private HeatPoint[,] _heatMap;
        private bool _ignoreScrollBarChanges;
        private Color _oorColour;
        private Color _nanColour;
        private Color _minColour;
        private Color _maxColour;
        private bool _sort;
        private readonly Core _core;
        private int _zoom = 1;

        /// <summary>
        /// Shows a 1D heatmap
        /// </summary>        
        public static void Show( Core core, CtlAutoList lvh, Column source )
        {
            FrmActHeatMap frm = new FrmActHeatMap( core, lvh, source, null );
            frm.Show( lvh.ListView.FindForm() );
        }

        /// <summary>
        /// Shows a 2D heatmap
        /// </summary>        
        public static void Show( Core core, CtlAutoList lvh, DistanceMatrix source )
        {
            FrmActHeatMap frm = new FrmActHeatMap( core, lvh, null, source);
            frm.Show( lvh.ListView.FindForm() );
        }

        private FrmActHeatMap( Core core, CtlAutoList lvh, Column source1D, DistanceMatrix source2D )
        {
            this.InitializeComponent();
            UiControls.SetIcon( this );

            this._maxColour = core.Options.HeatMapMaxColour;
            this._nanColour = core.Options.HeatMapNanColour;
            this._minColour = core.Options.HeatMapMinColour;
            this._oorColour = core.Options.HeatMapOorColour;
                              
            this._core= core;
            this._sourceList = lvh;
            this._source1D = source1D;
            this._source2D = source2D;

            if (source1D != null)
            {
                this.Text = UiControls.GetFileName( source1D.DisplayName );
            }
            else
            {
                this.Text = UiControls.GetFileName( "Distance matrix" );
            }

            this.ctlTitleBar1.Text = this.Text;

            this.GenerateHeat();
        }

        struct HeatPoint
        {
            public object XSource;
            public int XIndex;

            public object YSource;
            public int YIndex;

            public double ZFraction;
            public double ZValue;
            public Color ZColour;

            public bool IsEmpty => this.XSource == null && this.YSource == null;
        }

        private void GenerateHeat()
        {
            // Set legend colours
            this.toolStripMenuItem1.Image = UiControls.CreateSolidColourImage( null, this._maxColour, this._maxColour );
            this.minToolStripMenuItem.Image = UiControls.CreateSolidColourImage( null, this._minColour, this._minColour );
            this.notANumberToolStripMenuItem.Image = UiControls.CreateSolidColourImage( null, this._nanColour, this._nanColour );
            this.outOfRangeToolStripMenuItem.Image = UiControls.CreateSolidColourImage( null, this._oorColour, this._oorColour );

            if (this._source1D != null)
            {
                this.Generate1DHeatMap();
            }
            else
            {
                this.Generate2DHeatMap();
            }

            // Calculate min/max
            double min = double.MaxValue;
            double max = double.MinValue;
            int numberOfValids = 0;

            foreach(HeatPoint hp in this._heatMap)
            {
                double val = hp.ZValue;

                if (!double.IsNaN( val ) && !double.IsInfinity( val ))
                {
                    numberOfValids++;
                    min = Math.Min( hp.ZValue, min );
                    max = Math.Max( hp.ZValue, max );
                }
            }

            double range = max - min;

            // Set legend ranges
            this.toolStripMenuItem1.Text += " (" + Maths.SignificantDigits( max ) + ")";
            this.minToolStripMenuItem.Text += " (" + Maths.SignificantDigits( min ) + ")";

            // Set colours
            for (int x = 0; x < this._heatMap.GetLength( 0 ); x++)
            {
                for (int y = 0; y < this._heatMap.GetLength( 1 ); y++)
                {
                    double val = this._heatMap[x,y].ZValue;

                    if (!double.IsNaN( val ) && !double.IsInfinity( val ))
                    {
                        this._heatMap[x, y].ZFraction = (val - min) / range;
                        this._heatMap[x, y].ZColour = ColourHelper.Blend( this._minColour, this._maxColour, this._heatMap[x, y].ZFraction );
                    }
                    else
                    {
                        this._heatMap[x, y].ZColour = this._nanColour;
                    }
                }
            }

            // Low valid count warning
            if (numberOfValids <= (this._heatMap.Length / 2))
            {
                FrmMsgBox.ShowInfo( this, this.Text, "The majority of information in this column is not numeric, the heatmap may be missing information.", FrmMsgBox.EDontShowAgainId.HeatmapColumnNotNumerical );
            }

            // Generate bitmap
            this.GenerateBitmap();
        }

        private void Generate2DHeatMap()
        {   
            int n = this._source2D.ValueMatrix.NumRows;
            this._heatMap = new HeatPoint[n, n];

            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    HeatPoint hp = new HeatPoint(  );
                    hp.XIndex = x;
                    hp.XSource = this._source2D.ValueMatrix.Vectors[x].Peak;
                    hp.YIndex = y;
                    hp.YSource = this._source2D.ValueMatrix.Vectors[y].Peak;

                    hp.ZValue = this._source2D.Values[x, y];

                    this._heatMap[x, y] = hp;
                }
            }
        }

        private void Generate1DHeatMap()
        {
            // Get source list
            object[] source = this._sourceList.GetVisible().ToArray();

            // Generate heatmap values
            HeatPoint[] tsrc = new HeatPoint[source.Length];

            for (int n = 0; n < source.Length; n++)
            {
                var vis = source[n];
                HeatPoint heat = new HeatPoint();
                heat.XSource = vis;
                heat.XIndex = n;
                heat.ZValue = this.GetRow( vis );
                tsrc[n] = heat;
            }

            // Sort if specified
            if (this._sort)
            {
                tsrc = tsrc.OrderBy( z => z.ZValue ).ToArray();
            }

            this._heatMap = new HeatPoint[tsrc.Length,1];

            for (int n = 0; n < tsrc.Length; n++)
            {
                this._heatMap[n, 0] = tsrc[n];
            }
        }

        private void GenerateBitmap()
        {
            // Set checks
            this.sameAsListToolStripMenuItem.Checked = !this._sort;
            this.orderedToolStripMenuItem.Checked = this._sort;

            this._ignoreScrollBarChanges = true;
            this.hScrollBar1.Value = 0;
            this.vScrollBar1.Value = 0;
            this._ignoreScrollBarChanges = false;

            this.hScrollBar1.Maximum = this._heatMap.GetLength( 0 ) - 1;
            this.vScrollBar1.Maximum = this._heatMap.GetLength( 1 ) - 1;

            this.hScrollBar1.Visible = this.hScrollBar1.Maximum != 0;
            this.vScrollBar1.Visible = this.vScrollBar1.Maximum != 0;

            this.pictureBox1.Rerender();
        }             

        private double GetRow( object arg )
        {
            object r = this._source1D.GetRow( arg );             
            return Column.AsDouble( r );
        }

        HeatPoint ScreenToHeatMap( Point p )
        {
            int x = p.X / this._zoom;
            int y = p.Y / this._zoom;

            x += this.hScrollBar1.Value;
            y += this.vScrollBar1.Value;

            if (this._heatMap.GetLength( 1 ) == 1)
            {
                y = 0;
            }

            if (x < 0 || x >= this._heatMap.GetLength( 0 )
                || y < 0 || y >= this._heatMap.GetLength(1))
            {
                return new HeatPoint();
            }

            return this._heatMap[x, y];
        }

        private void pictureBox1_MouseMove( object sender, MouseEventArgs e )
        {
            HeatPoint h = this.ScreenToHeatMap( e.Location );

            if (h.IsEmpty)
            {
                this._lblSelection.Visible = false;
                this.toolStripStatusLabel3.Visible = false;
                return;
            }

            if (h.YSource == null)
            {
                this._lblSelection.Text = h.XSource.ToString() + " = (" + h.XIndex + ", " + h.ZValue + ")";
            }
            else
            {
                this._lblSelection.Text = "{" + h.XSource.ToString() + ", " + h.YSource.ToString() + "} ( {" + h.XIndex + ", " + h.YIndex + " }, " + h.ZValue + ")";
            }

            this.toolStripStatusLabel3.BackColor = h.ZColour;
            this._lblSelection.Visible = true;
            this.toolStripStatusLabel3.Visible = true;
        }

        private void pictureBox1_MouseUp( object sender, MouseEventArgs e )
        {
            HeatPoint h = this.ScreenToHeatMap( e.Location );

            if (h.IsEmpty)
            {
                this._lblSelection.Visible = false;
                this.toolStripStatusLabel3.Visible = false;
                return;
            }

            if (h.YSource == null || h.YSource== h.XSource)
            {
                this._sourceList.ActivateItem( h.XSource );
            }
            else
            {
                this.alphaToolStripMenuItem.Text = h.XSource.ToString();
                this.alphaToolStripMenuItem.Tag = h.XSource;
                this.betaToolStripMenuItem.Text = h.YSource.ToString();
                this.betaToolStripMenuItem.Tag = h.YSource;
                this.contextMenuStrip1.Show( this.pictureBox1, e.Location );
            }
        }

        private void pictureBox1_MouseDown( object sender, MouseEventArgs e )
        {

        }  

        private void sameAsListToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this._sort = false;
            this.GenerateHeat();
        }

        private void orderedToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this._sort = true;
            this.GenerateHeat();
        }

        private void FrmActHeatMap_Resize( object sender, EventArgs e )
        {
            //GenerateBitmap();
        }           

        private void eitherScrollBar_Scroll( object sender, ScrollEventArgs e )
        {
            if (!this._ignoreScrollBarChanges)
            {
                this.pictureBox1.Rerender();
            }               
        }

        private void legendToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            

        }

        private void toolStripMenuItem1_Click( object sender, EventArgs e )
        {
            if (ColourHelper.EditColor( ref this._maxColour ))
            {
                this._core.Options.HeatMapMaxColour = this._maxColour;
                this.GenerateHeat();
            }
        }

        private void minToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if (ColourHelper.EditColor( ref this._minColour ))
            {
                this._core.Options.HeatMapMinColour = this._minColour;
                this.GenerateHeat();
            }
        }

        private void notANumberToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if (ColourHelper.EditColor( ref this._nanColour ))
            {
                this._core.Options.HeatMapNanColour = this._nanColour;
                this.GenerateHeat();
            }
        }

        private void outOfRangeToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if (ColourHelper.EditColor( ref this._oorColour ))
            {
                this._core.Options.HeatMapOorColour = this._oorColour;
                this.GenerateHeat();
            }
        }

        private void pictureBox1_MouseLeave( object sender, EventArgs e )
        {
            this._lblSelection.Visible = false;
            this.toolStripStatusLabel3.Visible = false;
        }       

        private void alphaToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            Visualisable vis = (Visualisable)tsmi.Tag;
            this._sourceList.ActivateItem( vis );
        }

        private void pictureBox1_Render( object sender, RenderEventArgs e )
        {   
            int w = this._heatMap.GetLength( 0 );
            int h = this._heatMap.GetLength( 1 );
            Bitmap bmp = e.Bitmap;
            Rectangle all = new Rectangle( 0, 0, bmp.Width, bmp.Height);

            BitmapData bdata = bmp.LockBits( all, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
            int size = bdata.Height * bdata.Stride;
            byte[] data = new byte[size];              

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    int i = (x * 3) + (y * bdata.Stride);

                    HeatPoint hm = this.ScreenToHeatMap( new Point( x, y ) );

                    Color c = !hm.IsEmpty ? hm.ZColour : this._oorColour;

                    data[i + 0] = c.B;
                    data[i + 1] = c.G;
                    data[i + 2] = c.R;
                }
            }

            // Marshal to avoid unsafe code...
            Marshal.Copy( data, 0, bdata.Scan0, data.Length );

            bmp.UnlockBits( bdata );             
        }

        private void defaultToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this._zoom = 1;
            this.pictureBox1.Rerender();
        }

        private void zoomInToolStripMenuItem1_Click( object sender, EventArgs e )
        {
            this._zoom += 1;
            this.pictureBox1.Rerender();
        }

        private void zoomout1ToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this._zoom -= 1;

            if (this._zoom < 1)
            {
                this._zoom = 1;
            }

            this.pictureBox1.Rerender();
        }
    }
}
