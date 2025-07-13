namespace CubicGUI;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
        panel1 = new System.Windows.Forms.Panel();
        label4 = new System.Windows.Forms.Label();
        button4 = new System.Windows.Forms.Button();
        label3 = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        button1 = new System.Windows.Forms.Button();
        label1 = new System.Windows.Forms.Label();
        button2 = new System.Windows.Forms.Button();
        label5 = new System.Windows.Forms.Label();
        panel1.SuspendLayout();
        SuspendLayout();
        // 
        // panel1
        // 
        panel1.Controls.Add(label5);
        panel1.Controls.Add(label4);
        panel1.Controls.Add(button4);
        panel1.Controls.Add(label3);
        panel1.Controls.Add(label2);
        panel1.Controls.Add(button1);
        panel1.Controls.Add(label1);
        panel1.Location = new System.Drawing.Point(50, 14);
        panel1.Name = "panel1";
        panel1.Size = new System.Drawing.Size(697, 153);
        panel1.TabIndex = 0;
        // 
        // label4
        // 
        label4.Font = new System.Drawing.Font("Roboto Black", 13F);
        label4.Location = new System.Drawing.Point(353, 38);
        label4.Name = "label4";
        label4.Size = new System.Drawing.Size(258, 62);
        label4.TabIndex = 6;
        label4.Text = "- Convert from anything to anything";
        // 
        // button4
        // 
        button4.Location = new System.Drawing.Point(272, 38);
        button4.Name = "button4";
        button4.Size = new System.Drawing.Size(75, 20);
        button4.TabIndex = 5;
        button4.Text = "TextTools";
        button4.UseVisualStyleBackColor = true;
        button4.Click += button4_Click;
        // 
        // label3
        // 
        label3.Font = new System.Drawing.Font("Roboto Black", 15F);
        label3.Location = new System.Drawing.Point(272, 16);
        label3.Name = "label3";
        label3.Size = new System.Drawing.Size(175, 20);
        label3.TabIndex = 4;
        label3.Text = "Lightweight Tools";
        // 
        // label2
        // 
        label2.Font = new System.Drawing.Font("Roboto Black", 13F);
        label2.Location = new System.Drawing.Point(88, 37);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(189, 63);
        label2.TabIndex = 2;
        label2.Text = "- Get info about your system";
        // 
        // button1
        // 
        button1.Location = new System.Drawing.Point(15, 37);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(67, 20);
        button1.TabIndex = 1;
        button1.Text = "SysInfo";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // label1
        // 
        label1.Font = new System.Drawing.Font("Roboto Black", 15F);
        label1.Location = new System.Drawing.Point(15, 16);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(134, 20);
        label1.TabIndex = 0;
        label1.Text = "System Tools";
        // 
        // button2
        // 
        button2.Location = new System.Drawing.Point(758, 8);
        button2.Name = "button2";
        button2.Size = new System.Drawing.Size(30, 17);
        button2.TabIndex = 1;
        button2.Text = "X";
        button2.UseVisualStyleBackColor = true;
        button2.Click += button2_Click;
        // 
        // label5
        // 
        label5.Location = new System.Drawing.Point(19, 101);
        label5.Name = "label5";
        label5.Size = new System.Drawing.Size(130, 23);
        label5.TabIndex = 7;
        label5.Text = "lamps.lol/cubicwarn";
        label5.Click += label5_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.Snow;
        ClientSize = new System.Drawing.Size(800, 391);
        ControlBox = false;
        Controls.Add(button2);
        Controls.Add(panel1);
        Font = new System.Drawing.Font("Roboto Black", 9F);
        ShowIcon = false;
        Text = "Cubic";
        Load += Form1_Load;
        panel1.ResumeLayout(false);
        ResumeLayout(false);
    }

    private System.Windows.Forms.Label label5;

    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.Label label4;

    private System.Windows.Forms.Label label3;

    private System.Windows.Forms.Button button2;

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label label2;

    private System.Windows.Forms.Label label1;

    private System.Windows.Forms.Panel panel1;

    #endregion
}