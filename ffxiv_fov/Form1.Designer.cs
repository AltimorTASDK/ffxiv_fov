namespace ffxiv_fov
{
    partial class Form1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.label1 = new System.Windows.Forms.Label();
			this.fovInput = new System.Windows.Forms.TextBox();
			this.useHorFov = new System.Windows.Forms.RadioButton();
			this.useVertFov = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.statusBox = new System.Windows.Forms.TextBox();
			this.setFov = new System.Windows.Forms.Button();
			this.fovDisplay = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 86);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Current FOV";
			// 
			// fovInput
			// 
			this.fovInput.Location = new System.Drawing.Point(84, 109);
			this.fovInput.Name = "fovInput";
			this.fovInput.Size = new System.Drawing.Size(77, 20);
			this.fovInput.TabIndex = 6;
			// 
			// useHorFov
			// 
			this.useHorFov.AutoSize = true;
			this.useHorFov.Checked = true;
			this.useHorFov.Location = new System.Drawing.Point(6, 19);
			this.useHorFov.Name = "useHorFov";
			this.useHorFov.Size = new System.Drawing.Size(48, 17);
			this.useHorFov.TabIndex = 9;
			this.useHorFov.TabStop = true;
			this.useHorFov.Text = "Hor+";
			this.useHorFov.UseVisualStyleBackColor = true;
			this.useHorFov.CheckedChanged += new System.EventHandler(this.useHorFov_CheckedChanged);
			// 
			// useVertFov
			// 
			this.useVertFov.AutoSize = true;
			this.useVertFov.Location = new System.Drawing.Point(6, 42);
			this.useVertFov.Name = "useVertFov";
			this.useVertFov.Size = new System.Drawing.Size(60, 17);
			this.useVertFov.TabIndex = 10;
			this.useVertFov.Text = "Vertical";
			this.useVertFov.UseVisualStyleBackColor = true;
			this.useVertFov.CheckedChanged += new System.EventHandler(this.useVertFov_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.useHorFov);
			this.groupBox1.Controls.Add(this.useVertFov);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(149, 68);
			this.groupBox1.TabIndex = 11;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Mode";
			// 
			// statusBox
			// 
			this.statusBox.Location = new System.Drawing.Point(167, 12);
			this.statusBox.Multiline = true;
			this.statusBox.Name = "statusBox";
			this.statusBox.ReadOnly = true;
			this.statusBox.Size = new System.Drawing.Size(250, 117);
			this.statusBox.TabIndex = 12;
			// 
			// setFov
			// 
			this.setFov.Location = new System.Drawing.Point(10, 109);
			this.setFov.Name = "setFov";
			this.setFov.Size = new System.Drawing.Size(62, 20);
			this.setFov.TabIndex = 13;
			this.setFov.Text = "Set FOV";
			this.setFov.UseVisualStyleBackColor = true;
			this.setFov.Click += new System.EventHandler(this.setFov_Click);
			// 
			// fovDisplay
			// 
			this.fovDisplay.Location = new System.Drawing.Point(84, 83);
			this.fovDisplay.Name = "fovDisplay";
			this.fovDisplay.ReadOnly = true;
			this.fovDisplay.Size = new System.Drawing.Size(77, 20);
			this.fovDisplay.TabIndex = 14;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(429, 141);
			this.Controls.Add(this.fovDisplay);
			this.Controls.Add(this.setFov);
			this.Controls.Add(this.statusBox);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.fovInput);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form1";
			this.Text = "FFXIV Field of View";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox fovInput;
        private System.Windows.Forms.RadioButton useHorFov;
        private System.Windows.Forms.RadioButton useVertFov;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox statusBox;
		private System.Windows.Forms.Button setFov;
		private System.Windows.Forms.TextBox fovDisplay;
    }
}

