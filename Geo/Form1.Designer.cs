namespace Geo
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.Topography = new System.Windows.Forms.Button();
            this.DisplayPoints = new System.Windows.Forms.FlowLayoutPanel();
            this.DisplayTraces = new System.Windows.Forms.FlowLayoutPanel();
            this.labelTopo = new System.Windows.Forms.Label();
            this.labelTrace = new System.Windows.Forms.Label();
            this.RecordTrace = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gMapControl1
            // 
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(13, 13);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 2;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomEnabled = true;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(469, 425);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 0D;
            // 
            // Topography
            // 
            this.Topography.Location = new System.Drawing.Point(13, 497);
            this.Topography.Name = "Topography";
            this.Topography.Size = new System.Drawing.Size(105, 36);
            this.Topography.TabIndex = 2;
            this.Topography.Text = "Topography";
            this.Topography.UseVisualStyleBackColor = true;
            this.Topography.Click += new System.EventHandler(this.Topography_Click);
            // 
            // DisplayPoints
            // 
            this.DisplayPoints.AutoScroll = true;
            this.DisplayPoints.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.DisplayPoints.Location = new System.Drawing.Point(832, 62);
            this.DisplayPoints.Name = "DisplayPoints";
            this.DisplayPoints.Size = new System.Drawing.Size(242, 522);
            this.DisplayPoints.TabIndex = 3;
            this.DisplayPoints.WrapContents = false;
            this.DisplayPoints.Paint += new System.Windows.Forms.PaintEventHandler(this.DisplayPoints_Paint);
            // 
            // DisplayTraces
            // 
            this.DisplayTraces.Location = new System.Drawing.Point(1080, 62);
            this.DisplayTraces.Name = "DisplayTraces";
            this.DisplayTraces.Size = new System.Drawing.Size(241, 522);
            this.DisplayTraces.TabIndex = 4;
            this.DisplayTraces.Paint += new System.Windows.Forms.PaintEventHandler(this.DisplayTraces_Paint);
            // 
            // labelTopo
            // 
            this.labelTopo.AutoSize = true;
            this.labelTopo.Location = new System.Drawing.Point(914, 26);
            this.labelTopo.Name = "labelTopo";
            this.labelTopo.Size = new System.Drawing.Size(82, 16);
            this.labelTopo.TabIndex = 5;
            this.labelTopo.Text = "Topography";
            this.labelTopo.Click += new System.EventHandler(this.labelTopo_Click);
            // 
            // labelTrace
            // 
            this.labelTrace.AutoSize = true;
            this.labelTrace.Location = new System.Drawing.Point(1175, 26);
            this.labelTrace.Name = "labelTrace";
            this.labelTrace.Size = new System.Drawing.Size(43, 16);
            this.labelTrace.TabIndex = 6;
            this.labelTrace.Text = "Trace";
            this.labelTrace.Click += new System.EventHandler(this.labelTrace_Click);
            // 
            // RecordTrace
            // 
            this.RecordTrace.Location = new System.Drawing.Point(177, 497);
            this.RecordTrace.Name = "RecordTrace";
            this.RecordTrace.Size = new System.Drawing.Size(113, 36);
            this.RecordTrace.TabIndex = 7;
            this.RecordTrace.Text = "Trace";
            this.RecordTrace.UseVisualStyleBackColor = true;
            this.RecordTrace.Click += new System.EventHandler(this.RecordTrace_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1333, 596);
            this.Controls.Add(this.RecordTrace);
            this.Controls.Add(this.labelTrace);
            this.Controls.Add(this.labelTopo);
            this.Controls.Add(this.DisplayTraces);
            this.Controls.Add(this.DisplayPoints);
            this.Controls.Add(this.Topography);
            this.Controls.Add(this.gMapControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Button Topography;
        private System.Windows.Forms.FlowLayoutPanel DisplayPoints;
        private System.Windows.Forms.FlowLayoutPanel DisplayTraces;
        private System.Windows.Forms.Label labelTopo;
        private System.Windows.Forms.Label labelTrace;
        private System.Windows.Forms.Button RecordTrace;
    }
}

