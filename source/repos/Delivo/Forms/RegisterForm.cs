using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public class RegisterForm : Form
    {
        private readonly Color ColorPortocaliu = Color.FromArgb(255, 107, 0);
        private readonly Color ColorAlbastruInchis = Color.FromArgb(13, 27, 62);
        private readonly Color ColorAlbastruCard = Color.FromArgb(22, 32, 64);
        private readonly Color ColorAlb = Color.White;
        private readonly Color ColorTextSecundar = Color.FromArgb(136, 153, 187);
        private readonly Color ColorVerde = Color.FromArgb(29, 158, 117);
        private readonly Color ColorRosu = Color.FromArgb(226, 75, 74);

        private TextBox txtNume, txtUsername, txtTelefon, txtParola, txtConfirmParola;
        private Label lblValidare;
        private Panel pnlRules;
        private Label[] ruleLabels = new Label[4];

        public RegisterForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Delivo - Inregistrare";
            this.Size = new Size(420, 620);
            this.BackColor = ColorAlbastruInchis;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // === HERO portocaliu ===
            var pnlHero = new Panel
            {
                Size = new Size(420, 130),
                Location = new Point(0, 0),
                BackColor = ColorPortocaliu
            };
            pnlHero.Paint += (s, e) => {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = new GraphicsPath();
                path.AddArc(-50, pnlHero.Height - 60, pnlHero.Width + 100, 120, 0, 180);
                path.AddLine(pnlHero.Width + 50, pnlHero.Height, -50, pnlHero.Height);
                path.CloseFigure();
                using var brush = new SolidBrush(ColorAlbastruInchis);
                g.FillPath(brush, path);
            };

            var lblLogo = new Label
            {
                Text = "DELIVO",
                Font = new Font("Showcard Gothic", 26, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true
            };
            lblLogo.Location = new Point((420 - lblLogo.PreferredWidth) / 2, 22);

            var lblSub = new Label
            {
                Text = "Creeaza cont nou",
                Font = new Font("Poppins", 9),
                ForeColor = Color.FromArgb(220, 255, 255, 255),
                AutoSize = true,
                Location = new Point(140, 72)
            };

            pnlHero.Controls.Add(lblLogo);
            pnlHero.Controls.Add(lblSub);

            // === CARD formular ===
            var pnlCard = new Panel
            {
                Size = new Size(350, 440),
                Location = new Point(35, 110),
                BackColor = ColorAlbastruCard
            };
            RoundCorners(pnlCard, 14);

            int y = 20;

            // Nume complet
            AddLabel(pnlCard, "Nume complet", 20, y); y += 22;
            txtNume = AddTextBox(pnlCard, 20, y, "Ex: Valeria Lysyi"); y += 42;

            // Username
            AddLabel(pnlCard, "Utilizator", 20, y); y += 22;
            txtUsername = AddTextBox(pnlCard, 20, y, "Alege un username"); y += 42;

            // Telefon
            AddLabel(pnlCard, "Numar de telefon", 20, y); y += 22;
            txtTelefon = AddTextBox(pnlCard, 20, y, "+373 XX XXX XXX"); y += 42;

            // Parola
            AddLabel(pnlCard, "Parola", 20, y); y += 22;
            txtParola = AddTextBox(pnlCard, 20, y, "Minim 8 caractere", true);
            txtParola.TextChanged += TxtParola_TextChanged;
            y += 38;

            // Reguli parola
            pnlRules = new Panel
            {
                Location = new Point(20, y),
                Size = new Size(310, 22),
                BackColor = Color.Transparent
            };
            string[] rules = { "min 8 car.", "majuscula", "cifra", "simbol" };
            int rx = 0;
            for (int i = 0; i < 4; i++)
            {
                ruleLabels[i] = new Label
                {
                    Text = rules[i],
                    Font = new Font("Poppins", 8),
                    ForeColor = ColorPortocaliu,
                    BackColor = Color.FromArgb(30, 255, 107, 0),
                    AutoSize = false,
                    Size = new Size(68, 18),
                    Location = new Point(rx, 2),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                RoundCorners(ruleLabels[i], 8);
                pnlRules.Controls.Add(ruleLabels[i]);
                rx += 74;
            }
            pnlCard.Controls.Add(pnlRules);
            y += 30;

            // Confirma parola
            AddLabel(pnlCard, "Confirma parola", 20, y); y += 22;
            txtConfirmParola = AddTextBox(pnlCard, 20, y, "Repeta parola", true); y += 42;

            // Label validare
            lblValidare = new Label
            {
                Text = "",
                Font = new Font("Poppins", 9),
                ForeColor = ColorRosu,
                AutoSize = true,
                Location = new Point(20, y),
                BackColor = Color.Transparent
            };
            pnlCard.Controls.Add(lblValidare);
            y += 22;

            // Buton inregistrare
            var btnRegister = new Button
            {
                Text = "CREEAZA CONT",
                Font = new Font("Poppins", 10, FontStyle.Bold),
                ForeColor = ColorAlb,
                BackColor = ColorPortocaliu,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(310, 42),
                Location = new Point(20, y),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            RoundCorners(btnRegister, 10);
            btnRegister.Click += BtnRegister_Click;

            pnlCard.Controls.Add(btnRegister);

            // Link login
            var lblLogin = new Label
            {
                Text = "Ai deja cont? Autentifica-te",
                Font = new Font("Poppins", 9),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point(80, 575),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            lblLogin.Click += (s, e) => { new LoginForm().Show(); this.Close(); };

            this.Controls.Add(pnlHero);
            this.Controls.Add(pnlCard);
            this.Controls.Add(lblLogin);
        }

        // ===== VALIDARE PAROLA in timp real =====
        private void TxtParola_TextChanged(object sender, EventArgs e)
        {
            string p = txtParola.Text;
            bool r1 = p.Length >= 8;
            bool r2 = Regex.IsMatch(p, "[A-Z]");
            bool r3 = Regex.IsMatch(p, "[0-9]");
            bool r4 = Regex.IsMatch(p, "[^a-zA-Z0-9]");

            bool[] ok = { r1, r2, r3, r4 };
            for (int i = 0; i < 4; i++)
            {
                ruleLabels[i].ForeColor = ok[i] ? ColorVerde : ColorPortocaliu;
                ruleLabels[i].BackColor = ok[i]
                    ? Color.FromArgb(30, 29, 158, 117)
                    : Color.FromArgb(30, 255, 107, 0);
            }
        }

        // ===== BUTON INREGISTRARE =====
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string nume = txtNume.Text.Trim();
            string user = txtUsername.Text.Trim();
            string tel = txtTelefon.Text.Trim();
            string pass = txtParola.Text;
            string conf = txtConfirmParola.Text;

            // Validari
            if (string.IsNullOrEmpty(nume) || string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(conf))
            {
                lblValidare.ForeColor = ColorRosu;
                lblValidare.Text = "Completeaza toate campurile!";
                return;
            }
            if (pass.Length < 8)
            {
                lblValidare.ForeColor = ColorRosu;
                lblValidare.Text = "Parola trebuie sa aiba minim 8 caractere!";
                return;
            }
            if (!Regex.IsMatch(pass, "[A-Z]"))
            {
                lblValidare.ForeColor = ColorRosu;
                lblValidare.Text = "Parola trebuie sa contina o litera majuscula!";
                return;
            }
            if (!Regex.IsMatch(pass, "[0-9]"))
            {
                lblValidare.ForeColor = ColorRosu;
                lblValidare.Text = "Parola trebuie sa contina o cifra!";
                return;
            }
            if (!Regex.IsMatch(pass, "[^a-zA-Z0-9]"))
            {
                lblValidare.ForeColor = ColorRosu;
                lblValidare.Text = "Parola trebuie sa contina un simbol (!@#$)!";
                return;
            }
            if (pass != conf)
            {
                lblValidare.ForeColor = ColorRosu;
                lblValidare.Text = "Parolele nu coincid!";
                return;
            }

            try
            {
                bool succes = DatabaseHelper.InregistreazaUtilizator(user, pass, nume, tel);
                if (!succes)
                {
                    lblValidare.ForeColor = ColorRosu;
                    lblValidare.Text = "Username-ul este deja folosit!";
                    return;
                }
                MessageBox.Show("Cont creat cu succes!\nTe poti autentifica acum.",
                    "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                new LoginForm().Show();
                this.Close();
            }
            catch (Exception ex)
            {
                lblValidare.ForeColor = ColorRosu;
                lblValidare.Text = "Eroare: " + ex.Message;
            }
        }

        // ===== HELPER METODE =====
        private void AddLabel(Panel parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Poppins", 9),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point(x, y),
                BackColor = Color.Transparent
            });
        }

        private TextBox AddTextBox(Panel parent, int x, int y, string placeholder, bool isPassword = false)
        {
            var tb = new TextBox
            {
                Font = new Font("Poppins", 10),
                ForeColor = ColorAlb,
                BackColor = ColorAlbastruInchis,
                BorderStyle = BorderStyle.None,
                Location = new Point(x, y),
                Size = new Size(310, 28),
                UseSystemPasswordChar = isPassword
            };
            // Placeholder
            tb.Text = placeholder;
            tb.ForeColor = ColorTextSecundar;
            tb.GotFocus += (s, e) => {
                if (tb.Text == placeholder) { tb.Text = ""; tb.ForeColor = ColorAlb; }
            };
            tb.LostFocus += (s, e) => {
                if (string.IsNullOrEmpty(tb.Text)) { tb.Text = placeholder; tb.ForeColor = ColorTextSecundar; }
            };

            // Linie de jos decorativa
            var pnlLine = new Panel
            {
                Location = new Point(x, y + 30),
                Size = new Size(310, 1),
                BackColor = Color.FromArgb(60, 255, 107, 0)
            };
            parent.Controls.Add(tb);
            parent.Controls.Add(pnlLine);
            return tb;
        }

        private void RoundCorners(Control ctrl, int radius)
        {
            if (ctrl.Width < 1 || ctrl.Height < 1) return;
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(ctrl.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(ctrl.Width - radius * 2, ctrl.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, ctrl.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            ctrl.Region = new Region(path);
        }
    }
}