namespace GenerateData
{
    partial class GenerateData
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
            this.GeneratePatients = new System.Windows.Forms.Button();
            this.GenerateClinicians = new System.Windows.Forms.Button();
            this.GeneratePathways = new System.Windows.Forms.Button();
            this.GenerateEvents = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GeneratePatients
            // 
            this.GeneratePatients.Location = new System.Drawing.Point(12, 33);
            this.GeneratePatients.Name = "GeneratePatients";
            this.GeneratePatients.Size = new System.Drawing.Size(138, 23);
            this.GeneratePatients.TabIndex = 0;
            this.GeneratePatients.Text = "Generate Patients";
            this.GeneratePatients.UseVisualStyleBackColor = true;
            this.GeneratePatients.Click += new System.EventHandler(this.GeneratePatients_Click);
            // 
            // GenerateClinicians
            // 
            this.GenerateClinicians.Location = new System.Drawing.Point(12, 73);
            this.GenerateClinicians.Name = "GenerateClinicians";
            this.GenerateClinicians.Size = new System.Drawing.Size(138, 23);
            this.GenerateClinicians.TabIndex = 1;
            this.GenerateClinicians.Text = "Generate Clinicians";
            this.GenerateClinicians.UseVisualStyleBackColor = true;
            this.GenerateClinicians.Click += new System.EventHandler(this.GenerateClinicians_Click);
            // 
            // GeneratePathways
            // 
            this.GeneratePathways.Location = new System.Drawing.Point(12, 115);
            this.GeneratePathways.Name = "GeneratePathways";
            this.GeneratePathways.Size = new System.Drawing.Size(138, 23);
            this.GeneratePathways.TabIndex = 2;
            this.GeneratePathways.Text = "Generate Pathways";
            this.GeneratePathways.UseVisualStyleBackColor = true;
            this.GeneratePathways.Click += new System.EventHandler(this.GeneratePathways_Click);
            // 
            // GenerateEvents
            // 
            this.GenerateEvents.Location = new System.Drawing.Point(12, 159);
            this.GenerateEvents.Name = "GenerateEvents";
            this.GenerateEvents.Size = new System.Drawing.Size(138, 23);
            this.GenerateEvents.TabIndex = 3;
            this.GenerateEvents.Text = "Generate Events";
            this.GenerateEvents.UseVisualStyleBackColor = true;
            this.GenerateEvents.Click += new System.EventHandler(this.GenerateEvents_Click);
            // 
            // GenerateData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.GenerateEvents);
            this.Controls.Add(this.GeneratePathways);
            this.Controls.Add(this.GenerateClinicians);
            this.Controls.Add(this.GeneratePatients);
            this.Name = "GenerateData";
            this.Text = "Generate Data";
            this.Load += new System.EventHandler(this.GenerateData_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button GeneratePatients;
        private System.Windows.Forms.Button GenerateClinicians;
        private System.Windows.Forms.Button GeneratePathways;
        private System.Windows.Forms.Button GenerateEvents;
    }
}

