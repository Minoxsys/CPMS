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
            this.GenerateCompletedEvents = new System.Windows.Forms.Button();
            this.GenerateSpecialtiesHospitals = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPeriodCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPathwayCount = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // GeneratePatients
            // 
            this.GeneratePatients.Location = new System.Drawing.Point(12, 33);
            this.GeneratePatients.Name = "GeneratePatients";
            this.GeneratePatients.Size = new System.Drawing.Size(213, 23);
            this.GeneratePatients.TabIndex = 0;
            this.GeneratePatients.Text = "Generate Patients";
            this.GeneratePatients.UseVisualStyleBackColor = true;
            this.GeneratePatients.Click += new System.EventHandler(this.GeneratePatients_Click);
            // 
            // GenerateClinicians
            // 
            this.GenerateClinicians.Location = new System.Drawing.Point(12, 119);
            this.GenerateClinicians.Name = "GenerateClinicians";
            this.GenerateClinicians.Size = new System.Drawing.Size(213, 23);
            this.GenerateClinicians.TabIndex = 1;
            this.GenerateClinicians.Text = "Generate Clinicians";
            this.GenerateClinicians.UseVisualStyleBackColor = true;
            this.GenerateClinicians.Click += new System.EventHandler(this.GenerateClinicians_Click);
            // 
            // GeneratePathways
            // 
            this.GeneratePathways.Location = new System.Drawing.Point(12, 161);
            this.GeneratePathways.Name = "GeneratePathways";
            this.GeneratePathways.Size = new System.Drawing.Size(213, 23);
            this.GeneratePathways.TabIndex = 2;
            this.GeneratePathways.Text = "Generate Pathways";
            this.GeneratePathways.UseVisualStyleBackColor = true;
            this.GeneratePathways.Click += new System.EventHandler(this.GeneratePathways_Click);
            // 
            // GenerateCompletedEvents
            // 
            this.GenerateCompletedEvents.Location = new System.Drawing.Point(12, 202);
            this.GenerateCompletedEvents.Name = "GenerateCompletedEvents";
            this.GenerateCompletedEvents.Size = new System.Drawing.Size(213, 23);
            this.GenerateCompletedEvents.TabIndex = 3;
            this.GenerateCompletedEvents.Text = "Generate Completed Events";
            this.GenerateCompletedEvents.UseVisualStyleBackColor = true;
            this.GenerateCompletedEvents.Click += new System.EventHandler(this.GenerateCompletedEvents_Click);
            // 
            // GenerateSpecialtiesHospitals
            // 
            this.GenerateSpecialtiesHospitals.Location = new System.Drawing.Point(12, 76);
            this.GenerateSpecialtiesHospitals.Name = "GenerateSpecialtiesHospitals";
            this.GenerateSpecialtiesHospitals.Size = new System.Drawing.Size(213, 23);
            this.GenerateSpecialtiesHospitals.TabIndex = 4;
            this.GenerateSpecialtiesHospitals.Text = "Generate Specialties and Hospitals";
            this.GenerateSpecialtiesHospitals.UseVisualStyleBackColor = true;
            this.GenerateSpecialtiesHospitals.Click += new System.EventHandler(this.GenerateSpecialtiesHospitals_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(237, 207);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "for";
            // 
            // txtPeriodCount
            // 
            this.txtPeriodCount.Location = new System.Drawing.Point(262, 204);
            this.txtPeriodCount.Name = "txtPeriodCount";
            this.txtPeriodCount.Size = new System.Drawing.Size(57, 20);
            this.txtPeriodCount.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(325, 207);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "periods (max 2095)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(237, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "number of pathways";
            // 
            // txtPathwayCount
            // 
            this.txtPathwayCount.Location = new System.Drawing.Point(345, 166);
            this.txtPathwayCount.Name = "txtPathwayCount";
            this.txtPathwayCount.Size = new System.Drawing.Size(57, 20);
            this.txtPathwayCount.TabIndex = 9;
            // 
            // GenerateData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 261);
            this.Controls.Add(this.txtPathwayCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPeriodCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GenerateSpecialtiesHospitals);
            this.Controls.Add(this.GenerateCompletedEvents);
            this.Controls.Add(this.GeneratePathways);
            this.Controls.Add(this.GenerateClinicians);
            this.Controls.Add(this.GeneratePatients);
            this.Name = "GenerateData";
            this.Text = "Generate Data";
            this.Load += new System.EventHandler(this.GenerateData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GeneratePatients;
        private System.Windows.Forms.Button GenerateClinicians;
        private System.Windows.Forms.Button GeneratePathways;
        private System.Windows.Forms.Button GenerateCompletedEvents;
        private System.Windows.Forms.Button GenerateSpecialtiesHospitals;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPeriodCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPathwayCount;
    }
}

