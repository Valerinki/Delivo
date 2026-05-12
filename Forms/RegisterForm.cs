using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class RegisterForm : Form
    {
        // Paleta de culori Modern Dark
        private readonly Color C_Dark = Color.FromArgb(13, 27, 62);
        private readonly Color C_Card = Color.FromArgb(22, 32, 64);
        private readonly Color C_Orange = Color.FromArgb(255, 107, 0);
        private readonly Color C_White = Color.White;
        private readonly Color C_Muted = Color.FromArgb(136, 153, 187);
        private readonly Color C_Green = Color.FromArgb(29, 158, 117);
        private readonly Color C_Red = Color.FromArgb(226, 75, 74);

        private TextBox txtNume, txtUser, txtTel, txtPass, txtPassConf;
        private Label lblUserMsg, lblPassMatch;
        private Label[] ruleLabels = new Label[4];
        private System.Windows.Forms.Timer? _userCheckTimer;

        public RegisterForm()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Delivo - Înregistrare";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = C_Dark;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoScroll = true;

            // 1. Hero Panel - Valul și Logo-ul
            var pnlHero = new Panel { Dock = DockStyle.Top, Height = 220, BackColor = Color.Transparent };
            pnlHero.Paint += (s, e) => {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    float w = pnlHero.Width;
                    float h = pnlHero.Height - 80;
                    path.AddLine(new PointF(0, 0), new PointF(w, 0));
                    path.AddLine(new PointF(w, 0), new PointF(w, h));
                    path.AddBezier(new PointF(w, h), new PointF(w * 0.75f, h + 100), new PointF(w * 0.25f, h - 100), new PointF(0, h));
                    path.AddLine(new PointF(0, h), new PointF(0, 0));
                    path.CloseFigure();
                    using var br = new LinearGradientBrush(new PointF(0, 0), new PointF(0, pnlHero.Height), C_Orange, Color.FromArgb(255, 80, 0));
                    g.FillPath(br, path);
                }
            };

            var lblLogo = new Label
            {
                Text = "DELIVO",
                Font = new Font("Showcard Gothic", 42, FontStyle.Bold),
                ForeColor = C_White,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlHero.Controls.Add(lblLogo);
            pnlHero.Resize += (s, e) => {
                lblLogo.Location = new Point((pnlHero.Width - lblLogo.PreferredWidth) / 2, 40);
            };

            // 2. Cardul Central - Repoziționat și centrat
            var pnlCard = new Panel
            {
                BackColor = C_Card,
                Width = 420,
                Height = 480,
            };
            RoundCtrl(pnlCard, 25); // Colțuri puțin mai rotunjite pentru eleganță

            this.Resize += (s, e) => {
                // Coborât la 240 pentru a lăsa spațiu față de curba portocalie
                pnlCard.Location = new Point((this.ClientSize.Width - pnlCard.Width) / 2, 240);
            };

            // --- TITLU ÎNREGISTRARE CENTRAT ---
            var lblHeader = new Label
            {
                Text = "ÎNREGISTRARE",
                Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold),
                ForeColor = C_White,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlCard.Controls.Add(lblHeader);

            var lineHeader = new Panel
            {
                Size = new Size(60, 3),
                BackColor = C_Orange,
            };
            pnlCard.Controls.Add(lineHeader);

            // Centrare dinamică a titlului în interiorul cardului
            lblHeader.Location = new Point((pnlCard.Width - lblHeader.PreferredWidth) / 2, 30);
            lineHeader.Location = new Point((pnlCard.Width - lineHeader.Width) / 2, 65);

            int curY = 95;
            int inputW = 360;

            // Câmpuri Formular
            AddLbl(pnlCard, "NUME COMPLET", 30, curY); curY += 20;
            txtNume = AddInput(pnlCard, 30, curY, inputW); curY += 45;

            AddLbl(pnlCard, "UTILIZATOR", 30, curY); curY += 20;
            txtUser = AddInput(pnlCard, 30, curY, inputW);
            txtUser.TextChanged += TxtUser_Changed; curY += 32;

            lblUserMsg = new Label { Text = "", Font = new Font("Segoe UI", 7), AutoSize = true, Location = new Point(30, curY), BackColor = Color.Transparent };
            pnlCard.Controls.Add(lblUserMsg); curY += 15;

            AddLbl(pnlCard, "PAROLĂ", 30, curY); curY += 20;
            txtPass = AddInput(pnlCard, 30, curY, inputW, true);
            txtPass.TextChanged += TxtPass_Changed; curY += 35;

            var pnlRules = new FlowLayoutPanel { Location = new Point(30, curY), Size = new Size(inputW, 22), BackColor = Color.Transparent };
            string[] ruleNames = { "min 8", "A-Z", "0-9", "(!?#)" };
            for (int i = 0; i < 4; i++)
            {
                ruleLabels[i] = new Label
                {
                    Text = ruleNames[i],
                    Font = new Font("Segoe UI", 7, FontStyle.Bold),
                    ForeColor = C_Orange,
                    BackColor = Color.FromArgb(25, C_Orange),
                    Size = new Size(84, 18),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(0, 0, 4, 0)
                };
                RoundCtrl(ruleLabels[i], 5);
                pnlRules.Controls.Add(ruleLabels[i]);
            }
            pnlCard.Controls.Add(pnlRules); curY += 35;

            AddLbl(pnlCard, "CONFIRMĂ PAROLA", 30, curY); curY += 20;
            txtPassConf = AddInput(pnlCard, 30, curY, inputW, true);
            txtPassConf.TextChanged += (s, e) => CheckMatch(); curY += 32;

            lblPassMatch = new Label { Text = "", Font = new Font("Segoe UI", 7), AutoSize = true, Location = new Point(30, curY), BackColor = Color.Transparent };
            pnlCard.Controls.Add(lblPassMatch); curY += 25;

            var btnCreate = new Button
            {
                Text = "CREEAZĂ CONT",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = C_White,
                BackColor = C_Orange,
                Size = new Size(inputW, 42),
                Location = new Point(30, curY),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            RoundCtrl(btnCreate, 12);
            btnCreate.Click += BtnCreate_Click;
            pnlCard.Controls.Add(btnCreate); curY += 60;

            var lblAcc = new Label
            {
                Text = "Ai deja un cont? Autentifică-te",
                Font = new Font("Segoe UI", 9),
                ForeColor = C_Muted,
                AutoSize = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lblAcc.Click += (s, e) => { new LoginForm().Show(); this.Close(); };
            pnlCard.Controls.Add(lblAcc);

            // Centrare link
            lblAcc.Location = new Point((pnlCard.Width - lblAcc.PreferredWidth) / 2, curY);

            this.Controls.Add(pnlCard);
            this.Controls.Add(pnlHero);
        }

        // --- Logică Verificări ---

        private void TxtUser_Changed(object? sender, EventArgs e)
        {
            lblUserMsg.Text = "Verificare...";
            _userCheckTimer?.Stop();
            _userCheckTimer = new System.Windows.Forms.Timer { Interval = 600 };
            _userCheckTimer.Tick += (s, ev) => { _userCheckTimer.Stop(); CheckUsernameAvailability(); };
            _userCheckTimer.Start();
        }

        private void CheckUsernameAvailability()
        {
            string user = txtUser.Text.Trim();
            if (user.Length < 3) { lblUserMsg.Text = ""; return; }
            bool taken = DatabaseHelper.IsUsernameTaken(user);
            lblUserMsg.ForeColor = taken ? C_Red : C_Green;
            lblUserMsg.Text = taken ? "⚠ Username ocupat" : "✓ Username disponibil";
        }

        private void TxtPass_Changed(object? sender, EventArgs e)
        {
            string p = txtPass.Text;
            bool[] ok = { p.Length >= 8, Regex.IsMatch(p, "[A-Z]"), Regex.IsMatch(p, "[0-9]"), Regex.IsMatch(p, @"[^a-zA-Z0-9]") };
            for (int i = 0; i < 4; i++)
            {
                ruleLabels[i].ForeColor = ok[i] ? C_Green : C_Orange;
                ruleLabels[i].BackColor = ok[i] ? Color.FromArgb(40, C_Green) : Color.FromArgb(30, C_Orange);
            }
            CheckMatch();
        }

        private void CheckMatch()
        {
            if (string.IsNullOrEmpty(txtPassConf.Text)) { lblPassMatch.Text = ""; return; }
            bool match = txtPassConf.Text == txtPass.Text;
            lblPassMatch.ForeColor = match ? C_Green : C_Red;
            lblPassMatch.Text = match ? "✓ Parolele coincid" : "✗ Parolele nu coincid";
        }

        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            string nume = txtNume.Text.Trim();
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text;
            string conf = txtPassConf.Text;

            // 1. Validări tăcute (fără mesaje, doar oprim execuția dacă datele sunt invalide)
            if (string.IsNullOrEmpty(nume) || string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(pass) || pass != conf)
            {
                return;
            }

            try
            {
                
                bool succes = DatabaseHelper.InregistreazaUtilizator(nume, user, pass);

                if (succes)
                {
                    
                    LoginForm.UtilizatorLogat = user;
                    LoginForm.RolLogat = "client";

                    var main = new MainForm();
                    main.Show();

                    this.Hide();
                }
            }
            catch
            {
                
            }
        }

        // --- Helperi UI ---

        private void AddLbl(Panel p, string text, int x, int y)
        {
            p.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = C_Muted,
                AutoSize = true,
                Location = new Point(x, y)
            });
        }

        private TextBox AddInput(Panel parent, int x, int y, int w, bool isPass = false)
        {
            var tb = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                ForeColor = C_White,
                BackColor = Color.FromArgb(12, 24, 56),
                BorderStyle = BorderStyle.None,
                Location = new Point(x + 5, y),
                Size = new Size(w - 10, 25),
                UseSystemPasswordChar = isPass
            };
            var line = new Panel
            {
                Location = new Point(x, y + 28),
                Size = new Size(w, 1),
                BackColor = Color.FromArgb(60, C_White)
            };
            tb.Enter += (s, e) => line.BackColor = C_Orange;
            tb.Leave += (s, e) => line.BackColor = Color.FromArgb(60, C_White);

            parent.Controls.Add(tb);
            parent.Controls.Add(line);
            return tb;
        }

        private void RoundCtrl(Control c, int r)
        {
            var p = new GraphicsPath();
            p.AddArc(0, 0, r * 2, r * 2, 180, 90);
            p.AddArc(c.Width - r * 2, 0, r * 2, r * 2, 270, 90);
            p.AddArc(c.Width - r * 2, c.Height - r * 2, r * 2, r * 2, 0, 90);
            p.AddArc(0, c.Height - r * 2, r * 2, r * 2, 90, 90);
            p.CloseFigure();
            c.Region = new Region(p);
        }
    }
}