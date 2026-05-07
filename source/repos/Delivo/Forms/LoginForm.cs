using System;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class LoginForm : Form
    {
        public static string UtilizatorLogat { get; private set; } = "";
        public static string RolLogat { get; private set; } = "";

        public LoginForm()
        {
            InitializeComponent();
            this.Text = "Delivo - Autentificare";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string parola = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(parola))
                {
                    MessageBox.Show("Completeaza username si parola!", "Atentie",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string rol = DatabaseHelper.VerificaAutentificare(username, parola);

                if (rol == null || rol == "")
                {
                    MessageBox.Show("Username sau parola gresita!",
                        "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                UtilizatorLogat = username;
                RolLogat = rol;

                if (rol == "admin")
                {
                    MessageBox.Show("Bine ai venit Admin!", "Succes",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                }
                else
                {
                    try
                    {
                        MainForm mainForm = new MainForm();
                        mainForm.Show();
                        this.Hide();
                    }
                    catch (Exception exMain)
                    {
                        MessageBox.Show("Eroare MainForm:\n" + exMain.Message +
                            "\n\nStackTrace:\n" + exMain.StackTrace,
                            "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message, "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
    }
}