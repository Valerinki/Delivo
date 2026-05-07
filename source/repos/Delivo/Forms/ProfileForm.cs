using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public class ProfileForm : Form
    {
        private readonly Color ColorPortocaliu = Color.FromArgb(255, 107, 0);
        private readonly Color ColorAlbastruInchis = Color.FromArgb(13, 27, 62);
        private readonly Color ColorAlbastruCard = Color.FromArgb(22, 32, 64);
        private readonly Color ColorAlb = Color.White;
        private readonly Color ColorTextSecundar = Color.FromArgb(136, 153, 187);

        public ProfileForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Delivo - Profilul meu";
            this.Size = new Size(420, 600);
            this.BackColor = ColorAlbastruInchis;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Avatar
            var pnlAvatar = new Panel { Size = new Size(80, 80), BackColor = ColorPortocaliu };
            RoundCorners(pnlAvatar, 40);
            pnlAvatar.Location = new Point(170, 30);
            string initiale = LoginForm.UtilizatorLogat.Length > 0
                ? LoginForm.UtilizatorLogat.Substring(0, 1).ToUpper() : "U";
            var lblInitiale = new Label { Text = initiale, Font = new Font("Poppins", 28, FontStyle.Bold), ForeColor = ColorAlb, AutoSize = false, Size = new Size(80, 80), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent };
            pnlAvatar.Controls.Add(lblInitiale);

            var lblNume = new Label { Text = LoginForm.UtilizatorLogat, Font = new Font("Poppins", 14, FontStyle.Bold), ForeColor = ColorAlb, AutoSize = true, Location = new Point(130, 120) };
            var lblRol = new Label { Text = LoginForm.RolLogat, Font = new Font("Poppins", 10), ForeColor = ColorPortocaliu, AutoSize = true, Location = new Point(170, 148) };

            // Statistici
            try
            {
                var comenzi = DatabaseHelper.GetComenziUtilizator(LoginForm.UtilizatorLogat);
                int total = comenzi.Count;
                int active = comenzi.FindAll(c => c.Status == "In livrare").Count;
                decimal cheltuiti = 0;
                foreach (var c in comenzi) cheltuiti += c.TotalPret;

                AddStatCard("Comenzi", total.ToString(), 30, 180);
                AddStatCard("Active", active.ToString(), 150, 180);
                AddStatCard("Lei chelt.", cheltuiti.ToString("F0"), 270, 180);
            }
            catch { }

            // Meniu optiuni
            string[] optiuni = { "Adrese salvate", "Notificari", "Schimba parola", "Suport clienti" };
            string[] emoji = { "📍", "🔔", "🔒", "📞" };
            for (int i = 0; i < optiuni.Length; i++)
            {
                var item = CreateMenuItem(optiuni[i], emoji[i], 280 + i * 52);
                this.Controls.Add(item);
            }

            // Buton deconectare
            var btnLogout = new Button
            {
                Text = "Deconectare",
                Font = new Font("Poppins", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 75, 74),
                BackColor = Color.FromArgb(30, 226, 75, 74),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(360, 42),
                Location = new Point(30, 530),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 1;
            btnLogout.FlatAppearance.BorderColor = Color.FromArgb(226, 75, 74);
            btnLogout.Click += (s, e) => {
                var result = MessageBox.Show("Sigur vrei sa te deconectezi?", "Deconectare",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    new LoginForm().Show();
                    Application.OpenForms[0]?.Close();
                    this.Close();
                }
            };

            this.Controls.Add(pnlAvatar);
            this.Controls.Add(lblNume);
            this.Controls.Add(lblRol);
            this.Controls.Add(btnLogout);
        }

        private void AddStatCard(string label, string valoare, int x, int y)
        {
            var card = new Panel { Size = new Size(110, 60), Location = new Point(x, y), BackColor = ColorAlbastruCard };
            RoundCorners(card, 10);
            var lblVal = new Label { Text = valoare, Font = new Font("Poppins", 18, FontStyle.Bold), ForeColor = ColorPortocaliu, AutoSize = true, Location = new Point(10, 8), BackColor = Color.Transparent };
            var lblLbl = new Label { Text = label, Font = new Font("Poppins", 8), ForeColor = ColorTextSecundar, AutoSize = true, Location = new Point(10, 38), BackColor = Color.Transparent };
            card.Controls.Add(lblVal);
            card.Controls.Add(lblLbl);
            this.Controls.Add(card);
        }

        private Panel CreateMenuItem(string text, string emoji, int y)
        {
            var pnl = new Panel { Size = new Size(360, 44), Location = new Point(30, y), BackColor = ColorAlbastruCard, Cursor = Cursors.Hand };
            RoundCorners(pnl, 10);
            var lblEmoji = new Label { Text = emoji, Font = new Font("Segoe UI Emoji", 14), AutoSize = true, Location = new Point(10, 10), BackColor = Color.Transparent };
            var lblText = new Label { Text = text, Font = new Font("Poppins", 10), ForeColor = ColorAlb, AutoSize = true, Location = new Point(46, 12), BackColor = Color.Transparent };
            var lblArrow = new Label { Text = "›", Font = new Font("Poppins", 16), ForeColor = ColorTextSecundar, AutoSize = true, Location = new Point(320, 8), BackColor = Color.Transparent };
            pnl.Controls.Add(lblEmoji);
            pnl.Controls.Add(lblText);
            pnl.Controls.Add(lblArrow);
            return pnl;
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