namespace Delivo.Forms

{
    partial class LoginForm
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblLogo = new Label();
            txtUsername = new TextBox();
            btnLogin = new Button();
            label1 = new Label();
            label2 = new Label();
            txtPassword = new TextBox();
            SuspendLayout();
            // 
            // lblLogo
            // 
            lblLogo.AutoSize = true;
            lblLogo.Font = new Font("Showcard Gothic", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLogo.ForeColor = Color.White;
            lblLogo.Location = new Point(306, 72);
            lblLogo.Name = "lblLogo";
            lblLogo.Size = new Size(167, 50);
            lblLogo.TabIndex = 0;
            lblLogo.Text = "DELIVO";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(289, 179);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(192, 27);
            txtUsername.TabIndex = 1;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.DarkOrange;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Tahoma", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 204);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(298, 304);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(175, 42);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "AUTENTIFICARE";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += this.btnLogin_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Tahoma", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.ForeColor = SystemColors.GrayText;
            label1.Location = new Point(343, 155);
            label1.Name = "label1";
            label1.Size = new Size(89, 21);
            label1.TabIndex = 3;
            label1.Text = "Utilizator";
            label1.Click += this.label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Tahoma", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label2.ForeColor = SystemColors.GrayText;
            label2.Location = new Point(354, 219);
            label2.Name = "label2";
            label2.Size = new Size(64, 21);
            label2.TabIndex = 4;
            label2.Text = "Parolă";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(289, 243);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(192, 27);
            txtPassword.TabIndex = 6;
            txtPassword.UseSystemPasswordChar = false;
            txtPassword.PasswordChar = '*';
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Navy;
            ClientSize = new Size(800, 450);
            Controls.Add(txtPassword);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnLogin);
            Controls.Add(txtUsername);
            Controls.Add(lblLogo);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "LoginForm";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLogo;
        private TextBox txtUsername;
        private Button btnLogin;
        private Label label1;
        private Label label2;
        private TextBox txtPassword;
    }
}
