namespace Rovio
{
    partial class MainForm
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
            this.Predator_Button = new System.Windows.Forms.Button();
            this.Prey_Button = new System.Windows.Forms.Button();
            this.User_Button = new System.Windows.Forms.Button();
            this.Close_Button = new System.Windows.Forms.Button();
            this.MapViewer = new System.Windows.Forms.PictureBox();
            this.VideoViewer = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.MapViewer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // Predator_Button
            // 
            this.Predator_Button.Location = new System.Drawing.Point(12, 306);
            this.Predator_Button.Name = "Predator_Button";
            this.Predator_Button.Size = new System.Drawing.Size(81, 23);
            this.Predator_Button.TabIndex = 3;
            this.Predator_Button.Text = "Predator";
            this.Predator_Button.UseVisualStyleBackColor = true;
            this.Predator_Button.Click += new System.EventHandler(this.Predator_Button_Click);
            // 
            // Prey_Button
            // 
            this.Prey_Button.Location = new System.Drawing.Point(99, 306);
            this.Prey_Button.Name = "Prey_Button";
            this.Prey_Button.Size = new System.Drawing.Size(81, 23);
            this.Prey_Button.TabIndex = 4;
            this.Prey_Button.Text = "Prey";
            this.Prey_Button.UseVisualStyleBackColor = true;
            this.Prey_Button.Click += new System.EventHandler(this.Prey_Button_Click);
            // 
            // User_Button
            // 
            this.User_Button.Enabled = false;
            this.User_Button.Location = new System.Drawing.Point(186, 306);
            this.User_Button.Name = "User_Button";
            this.User_Button.Size = new System.Drawing.Size(81, 23);
            this.User_Button.TabIndex = 5;
            this.User_Button.Text = "User";
            this.User_Button.UseVisualStyleBackColor = true;
            this.User_Button.Click += new System.EventHandler(this.User_Button_Click);
            // 
            // Close_Button
            // 
            this.Close_Button.Location = new System.Drawing.Point(273, 306);
            this.Close_Button.Name = "Close_Button";
            this.Close_Button.Size = new System.Drawing.Size(81, 23);
            this.Close_Button.TabIndex = 6;
            this.Close_Button.Text = "Close";
            this.Close_Button.UseVisualStyleBackColor = true;
            this.Close_Button.Click += new System.EventHandler(this.Close_Button_Click);
            // 
            // MapViewer
            // 
            this.MapViewer.InitialImage = null;
            this.MapViewer.Location = new System.Drawing.Point(379, 12);
            this.MapViewer.Name = "MapViewer";
            this.MapViewer.Size = new System.Drawing.Size(260, 300);
            this.MapViewer.TabIndex = 2;
            this.MapViewer.TabStop = false;
            // 
            // VideoViewer
            // 
            this.VideoViewer.Location = new System.Drawing.Point(12, 12);
            this.VideoViewer.MaximumSize = new System.Drawing.Size(352, 288);
            this.VideoViewer.MinimumSize = new System.Drawing.Size(352, 288);
            this.VideoViewer.Name = "VideoViewer";
            this.VideoViewer.Size = new System.Drawing.Size(352, 288);
            this.VideoViewer.TabIndex = 0;
            this.VideoViewer.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 345);
            this.ControlBox = false;
            this.Controls.Add(this.Close_Button);
            this.Controls.Add(this.User_Button);
            this.Controls.Add(this.Prey_Button);
            this.Controls.Add(this.Predator_Button);
            this.Controls.Add(this.MapViewer);
            this.Controls.Add(this.VideoViewer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximumSize = new System.Drawing.Size(657, 373);
            this.MinimumSize = new System.Drawing.Size(657, 373);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MapViewer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox VideoViewer;
        private System.Windows.Forms.Button Predator_Button;
        private System.Windows.Forms.Button Prey_Button;
        private System.Windows.Forms.Button User_Button;
        private System.Windows.Forms.Button Close_Button;
        public System.Windows.Forms.PictureBox MapViewer;

    }
}

