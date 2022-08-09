namespace StandardNetCapB
{
    partial class Home
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
            this.label1 = new System.Windows.Forms.Label();
            this.txt_String = new System.Windows.Forms.TextBox();
            this.btn_next = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_Info = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(82, 51);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Insert Testing String";
            // 
            // txt_String
            // 
            this.txt_String.Location = new System.Drawing.Point(87, 94);
            this.txt_String.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_String.Name = "txt_String";
            this.txt_String.Size = new System.Drawing.Size(476, 26);
            this.txt_String.TabIndex = 1;
            this.txt_String.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txt_String_KeyUp);
            // 
            // btn_next
            // 
            this.btn_next.Location = new System.Drawing.Point(510, 163);
            this.btn_next.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new System.Drawing.Size(122, 48);
            this.btn_next.TabIndex = 2;
            this.btn_next.Text = "Start";
            this.btn_next.UseVisualStyleBackColor = true;
            this.btn_next.Click += new System.EventHandler(this.btn_next_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(380, 163);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 48);
            this.button1.TabIndex = 3;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_Info
            // 
            this.btn_Info.Location = new System.Drawing.Point(574, 94);
            this.btn_Info.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_Info.Name = "btn_Info";
            this.btn_Info.Size = new System.Drawing.Size(32, 31);
            this.btn_Info.TabIndex = 4;
            this.btn_Info.Text = "ℹ️";
            this.btn_Info.UseVisualStyleBackColor = true;
            this.btn_Info.Click += new System.EventHandler(this.btn_Info_Click);
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 203);
            this.Controls.Add(this.btn_Info);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_next);
            this.Controls.Add(this.txt_String);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(684, 259);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(684, 259);
            this.Name = "Home";
            this.Text = "NetworkLogger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_String;
        private System.Windows.Forms.Button btn_next;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_Info;
    }
}

