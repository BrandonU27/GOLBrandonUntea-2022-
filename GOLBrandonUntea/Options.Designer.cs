
namespace GOLBrandonUntea
{
    partial class Options
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelMilli = new System.Windows.Forms.Label();
            this.labelCellWidth = new System.Windows.Forms.Label();
            this.labelCellHeight = new System.Windows.Forms.Label();
            this.numericUpDownMilli = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMilli)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(50, 139);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(165, 139);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelMilli
            // 
            this.labelMilli.AutoSize = true;
            this.labelMilli.Location = new System.Drawing.Point(28, 23);
            this.labelMilli.Name = "labelMilli";
            this.labelMilli.Size = new System.Drawing.Size(142, 13);
            this.labelMilli.TabIndex = 2;
            this.labelMilli.Text = "Timer Interval in Milliseconds";
            // 
            // labelCellWidth
            // 
            this.labelCellWidth.AutoSize = true;
            this.labelCellWidth.Location = new System.Drawing.Point(42, 56);
            this.labelCellWidth.Name = "labelCellWidth";
            this.labelCellWidth.Size = new System.Drawing.Size(128, 13);
            this.labelCellWidth.TabIndex = 3;
            this.labelCellWidth.Text = "Width of Universe in Cells";
            // 
            // labelCellHeight
            // 
            this.labelCellHeight.AutoSize = true;
            this.labelCellHeight.Location = new System.Drawing.Point(39, 89);
            this.labelCellHeight.Name = "labelCellHeight";
            this.labelCellHeight.Size = new System.Drawing.Size(131, 13);
            this.labelCellHeight.TabIndex = 4;
            this.labelCellHeight.Text = "Height of Universe in Cells";
            // 
            // numericUpDownMilli
            // 
            this.numericUpDownMilli.Location = new System.Drawing.Point(177, 22);
            this.numericUpDownMilli.Name = "numericUpDownMilli";
            this.numericUpDownMilli.Size = new System.Drawing.Size(63, 20);
            this.numericUpDownMilli.TabIndex = 5;
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Location = new System.Drawing.Point(177, 55);
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(64, 20);
            this.numericUpDownWidth.TabIndex = 6;
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.Location = new System.Drawing.Point(177, 88);
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Size = new System.Drawing.Size(64, 20);
            this.numericUpDownHeight.TabIndex = 7;
            // 
            // Options
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(277, 184);
            this.Controls.Add(this.numericUpDownHeight);
            this.Controls.Add(this.numericUpDownWidth);
            this.Controls.Add(this.numericUpDownMilli);
            this.Controls.Add(this.labelCellHeight);
            this.Controls.Add(this.labelCellWidth);
            this.Controls.Add(this.labelMilli);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMilli)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelMilli;
        private System.Windows.Forms.Label labelCellWidth;
        private System.Windows.Forms.Label labelCellHeight;
        private System.Windows.Forms.NumericUpDown numericUpDownMilli;
        private System.Windows.Forms.NumericUpDown numericUpDownWidth;
        private System.Windows.Forms.NumericUpDown numericUpDownHeight;
    }
}