
namespace sag_rod_plugin_app
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
            this.button1 = new System.Windows.Forms.Button();
            this.cb_plate1 = new System.Windows.Forms.ComboBox();
            this.cb_plate2 = new System.Windows.Forms.ComboBox();
            this.cb_plate4 = new System.Windows.Forms.ComboBox();
            this.cb_plate3 = new System.Windows.Forms.ComboBox();
            this.cb_sinleordouble = new System.Windows.Forms.ComboBox();
            this.tx_spacings = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(53, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cb_plate1
            // 
            this.cb_plate1.FormattingEnabled = true;
            this.cb_plate1.Items.AddRange(new object[] {
            "yes",
            "no"});
            this.cb_plate1.Location = new System.Drawing.Point(235, 12);
            this.cb_plate1.Name = "cb_plate1";
            this.cb_plate1.Size = new System.Drawing.Size(62, 21);
            this.cb_plate1.TabIndex = 1;
            // 
            // cb_plate2
            // 
            this.cb_plate2.FormattingEnabled = true;
            this.cb_plate2.Items.AddRange(new object[] {
            "yes",
            "no"});
            this.cb_plate2.Location = new System.Drawing.Point(235, 39);
            this.cb_plate2.Name = "cb_plate2";
            this.cb_plate2.Size = new System.Drawing.Size(62, 21);
            this.cb_plate2.TabIndex = 1;
            // 
            // cb_plate4
            // 
            this.cb_plate4.FormattingEnabled = true;
            this.cb_plate4.Items.AddRange(new object[] {
            "yes",
            "no"});
            this.cb_plate4.Location = new System.Drawing.Point(235, 93);
            this.cb_plate4.Name = "cb_plate4";
            this.cb_plate4.Size = new System.Drawing.Size(62, 21);
            this.cb_plate4.TabIndex = 1;
            // 
            // cb_plate3
            // 
            this.cb_plate3.FormattingEnabled = true;
            this.cb_plate3.Items.AddRange(new object[] {
            "yes",
            "no"});
            this.cb_plate3.Location = new System.Drawing.Point(235, 66);
            this.cb_plate3.Name = "cb_plate3";
            this.cb_plate3.Size = new System.Drawing.Size(62, 21);
            this.cb_plate3.TabIndex = 1;
            // 
            // cb_sinleordouble
            // 
            this.cb_sinleordouble.FormattingEnabled = true;
            this.cb_sinleordouble.Items.AddRange(new object[] {
            "single",
            "double"});
            this.cb_sinleordouble.Location = new System.Drawing.Point(53, 102);
            this.cb_sinleordouble.Name = "cb_sinleordouble";
            this.cb_sinleordouble.Size = new System.Drawing.Size(121, 21);
            this.cb_sinleordouble.TabIndex = 2;
            // 
            // tx_spacings
            // 
            this.tx_spacings.Location = new System.Drawing.Point(53, 164);
            this.tx_spacings.Name = "tx_spacings";
            this.tx_spacings.Size = new System.Drawing.Size(259, 20);
            this.tx_spacings.TabIndex = 3;
            this.tx_spacings.Text = "70 50 60 20";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 257);
            this.Controls.Add(this.tx_spacings);
            this.Controls.Add(this.cb_sinleordouble);
            this.Controls.Add(this.cb_plate3);
            this.Controls.Add(this.cb_plate4);
            this.Controls.Add(this.cb_plate2);
            this.Controls.Add(this.cb_plate1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cb_plate1;
        private System.Windows.Forms.ComboBox cb_plate2;
        private System.Windows.Forms.ComboBox cb_plate4;
        private System.Windows.Forms.ComboBox cb_plate3;
        private System.Windows.Forms.ComboBox cb_sinleordouble;
        private System.Windows.Forms.TextBox tx_spacings;
    }
}

