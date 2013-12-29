namespace WinFormApp
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
            this.btnSingleThread = new System.Windows.Forms.Button();
            this.btnMultiThread = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSingleThread
            // 
            this.btnSingleThread.Location = new System.Drawing.Point(152, 53);
            this.btnSingleThread.Name = "btnSingleThread";
            this.btnSingleThread.Size = new System.Drawing.Size(75, 23);
            this.btnSingleThread.TabIndex = 0;
            this.btnSingleThread.Text = "单线程";
            this.btnSingleThread.UseVisualStyleBackColor = true;
            this.btnSingleThread.Click += new System.EventHandler(this.btnSingleThread_Click);
            // 
            // btnMultiThread
            // 
            this.btnMultiThread.Location = new System.Drawing.Point(152, 112);
            this.btnMultiThread.Name = "btnMultiThread";
            this.btnMultiThread.Size = new System.Drawing.Size(75, 23);
            this.btnMultiThread.TabIndex = 0;
            this.btnMultiThread.Text = "多线程";
            this.btnMultiThread.UseVisualStyleBackColor = true;
            this.btnMultiThread.Click += new System.EventHandler(this.btnMultiThread_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btnMultiThread);
            this.Controls.Add(this.btnSingleThread);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSingleThread;
        private System.Windows.Forms.Button btnMultiThread;
    }
}

