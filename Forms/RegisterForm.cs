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
        private readonly Color C_Dark = Color.FromArgb(13, 27, 62);
        private readonly Color C_Card = Color.FromArgb(22, 32, 64);
        private readonly Color C_Orange = Color.FromArgb(255, 107, 0);
        private readonly Color C_White = Color.White;
        private readonly Color C_Muted = Color.FromArgb(136, 153, 187);
        private readonly Color C_Green = Color.FromArgb(29, 158, 117);
        private readonly Color C_Red = Color.FromArgb(226, 75, 74);
        private readonly Color C_InputBg = Color.FromArgb(12, 24, 56);

        private TextBox txtNume = null!, txtUser = null!, txtTel = null!,
                        txtPass = null!, txtPassConf = null!, txtEmail = null!;
        private Label lblUserMsg = null!, lblPassMatch = null!, lblEmailMsg = null!, lblTelMsg = null!;
        private Label[] ruleLabels = new Label[4];
        private System.Windows.Forms.Timer? _userCheckTimer;
        private System.Windows.Forms.Timer? _emailCheckTimer;
        private System.Windows.Forms.Timer? _telCheckTimer;

        public RegisterForm()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            Text = "Delivo - Înregistrare";
            WindowState = FormWindowState.Maximized;
            BackColor = C_Dark;
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = true;

            // ── Hero ──────────────────────────────────────────────
            var pnlHero = new Panel { Dock = DockStyle.Top, Height = 180, BackColor = Color.Transparent };
            pnlHero.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = new GraphicsPath();
                float w = pnlHero.Width, h = pnlHero.Height - 60;
                path.AddLine(0, 0, w, 0);
                path.AddLine(w, 0, w, h);
                path.AddBezier(w, h, w * 0.75f, h + 80, w * 0.25f, h - 80, 0, h);
                path.CloseFigure();
                using var br = new LinearGradientBrush(
                    new PointF(0, 0), new PointF(0, pnlHero.Height),
                    C_Orange, Color.FromArgb(200, 80, 0));
                g.FillPath(br, path);
            };

            var lblLogo = new Label
            {
                Text = "DELIVO",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = C_White,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlHero.Controls.Add(lblLogo);
            pnlHero.Resize += (s, e) =>
                lblLogo.Location = new Point((pnlHero.Width - lblLogo.PreferredWidth) / 2, 28);

            // ── Card ──────────────────────────────────────────────
            var pnlCard = new Panel
            {
                BackColor = C_Card,
                Width = 460,
                Height = 760       // înălțime mărită pentru câmpuri noi
            };
            RoundCtrl(pnlCard, 24);

            Resize += (s, e) =>
                pnlCard.Location = new Point(
                    (ClientSize.Width - pnlCard.Width) / 2,
                    Math.Max(200, (ClientSize.Height - pnlCard.Height) / 2 + 60));

            // ── Titlu ─────────────────────────────────────────────
            var lblHeader = new Label
            {
                Text = "ÎNREGISTRARE",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = C_White,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(0, 28)
            };
            pnlCard.Controls.Add(lblHeader);
            lblHeader.Location = new Point((pnlCard.Width - lblHeader.PreferredWidth) / 2, 28);

            var line = new Panel { Size = new Size(50, 3), BackColor = C_Orange };
            pnlCard.Controls.Add(line);
            line.Location = new Point((pnlCard.Width - line.Width) / 2, 58);

            int curY = 82;
            const int inputW = 390;
            const int x = 35;

            // ── Câmpuri ───────────────────────────────────────────
            AddLbl(pnlCard, "NUME COMPLET", x, curY); curY += 20;
            txtNume = AddInput(pnlCard, x, curY, inputW); curY += 52;

            AddLbl(pnlCard, "UTILIZATOR", x, curY); curY += 20;
            txtUser = AddInput(pnlCard, x, curY, inputW);
            txtUser.TextChanged += TxtUser_Changed; curY += 36;
            lblUserMsg = AddMsg(pnlCard, x, curY); curY += 22;

            AddLbl(pnlCard, "EMAIL  (@delivo.com)", x, curY); curY += 20;
            // ── Container email cu sufix fix ──────────────────────
            var pnlEmail = new Panel
            {
                Location = new Point(x, curY),
                Size = new Size(inputW, 28),
                BackColor = Color.Transparent
            };
            txtEmail = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                ForeColor = C_White,
                BackColor = C_InputBg,
                BorderStyle = BorderStyle.None,
                Location = new Point(5, 0),
                Size = new Size(270, 25)              // ✅ lățime fixă mai mică
            };
            var lblSuffix = new Label
            {
                Text = "@delivo.com",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = C_Orange,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(272, 3)          // ✅ imediat după câmpul de input
            };
            var lineEmail = new Panel
            {
                Location = new Point(0, 28),
                Size = new Size(inputW, 1),
                BackColor = Color.FromArgb(60, C_White)
            };
            txtEmail.Enter += (s, e) => lineEmail.BackColor = C_Orange;
            txtEmail.Leave += (s, e) => lineEmail.BackColor = Color.FromArgb(60, C_White);
            txtEmail.TextChanged += TxtEmail_Changed;
            pnlEmail.Controls.Add(txtEmail);
            pnlEmail.Controls.Add(lblSuffix);
            pnlEmail.Controls.Add(lineEmail);
            pnlCard.Controls.Add(pnlEmail);
            curY += 38;
            lblEmailMsg = AddMsg(pnlCard, x, curY); curY += 22;

            AddLbl(pnlCard, "TELEFON", x, curY); curY += 20;
            txtTel = AddInput(pnlCard, x, curY, inputW);
            txtTel.TextChanged += TxtTel_Changed; curY += 36;
            lblTelMsg = AddMsg(pnlCard, x, curY); curY += 22;

            AddLbl(pnlCard, "PAROLĂ", x, curY); curY += 20;
            txtPass = AddInput(pnlCard, x, curY, inputW, true);
            txtPass.TextChanged += TxtPass_Changed; curY += 36;

            var pnlRules = new FlowLayoutPanel
            {
                Location = new Point(x, curY),
                Size = new Size(inputW, 22),
                BackColor = Color.Transparent
            };
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
            pnlCard.Controls.Add(pnlRules); curY += 34;

            AddLbl(pnlCard, "CONFIRMĂ PAROLA", x, curY); curY += 20;
            txtPassConf = AddInput(pnlCard, x, curY, inputW, true);
            txtPassConf.TextChanged += (s, e) => CheckMatch(); curY += 36;
            lblPassMatch = AddMsg(pnlCard, x, curY); curY += 28;

            // ── Buton ─────────────────────────────────────────────
            var btnCreate = new Button
            {
                Text = "CREEAZĂ CONT",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = C_White,
                BackColor = C_Orange,
                Size = new Size(inputW, 44),
                Location = new Point(x, curY),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 140, 60);
            RoundCtrl(btnCreate, 12);
            btnCreate.Click += BtnCreate_Click;
            pnlCard.Controls.Add(btnCreate); curY += 56;

            var lblAcc = new Label
            {
                Text = "Ai deja un cont? Autentifică-te",
                Font = new Font("Segoe UI", 9),
                ForeColor = C_Muted,
                AutoSize = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lblAcc.Click += (s, e) => { new LoginForm().Show(); Close(); };
            pnlCard.Controls.Add(lblAcc);
            lblAcc.Location = new Point((pnlCard.Width - lblAcc.PreferredWidth) / 2, curY);

            // Ajustează înălțimea cardului la conținut
            pnlCard.Height = curY + 40;

            Controls.Add(pnlCard);
            Controls.Add(pnlHero);
        }

        // ── Verificări ────────────────────────────────────────────

        private void TxtUser_Changed(object? sender, EventArgs e)
        {
            lblUserMsg.Text = "Verificare...";
            lblUserMsg.ForeColor = C_Muted;
            _userCheckTimer?.Stop();
            _userCheckTimer = new System.Windows.Forms.Timer { Interval = 600 };
            _userCheckTimer.Tick += (s, ev) => { _userCheckTimer.Stop(); CheckUsername(); };
            _userCheckTimer.Start();
        }

        private void CheckUsername()
        {
            string user = txtUser.Text.Trim();
            if (user.Length < 3) { lblUserMsg.Text = ""; return; }
            bool taken = DatabaseHelper.IsUsernameTaken(user);
            lblUserMsg.ForeColor = taken ? C_Red : C_Green;
            lblUserMsg.Text = taken ? "✗ Username ocupat" : "✓ Username disponibil";
        }

        private void TxtEmail_Changed(object? sender, EventArgs e)
        {
            lblEmailMsg.Text = "Verificare...";
            lblEmailMsg.ForeColor = C_Muted;
            _emailCheckTimer?.Stop();
            _emailCheckTimer = new System.Windows.Forms.Timer { Interval = 600 };
            _emailCheckTimer.Tick += (s, ev) => { _emailCheckTimer.Stop(); CheckEmail(); };
            _emailCheckTimer.Start();
        }

        private void CheckEmail()
        {
            string prefix = txtEmail.Text.Trim();
            if (prefix.Length < 2) { lblEmailMsg.Text = ""; return; }
            string fullEmail = prefix + "@delivo.com";
            bool taken = DatabaseHelper.IsEmailTaken(fullEmail);
            lblEmailMsg.ForeColor = taken ? C_Red : C_Green;
            lblEmailMsg.Text = taken ? "✗ Email ocupat" : "✓ Email disponibil";
        }

        private void TxtTel_Changed(object? sender, EventArgs e)
        {
            lblTelMsg.Text = "";
            string tel = txtTel.Text.Trim();
            if (tel.Length < 8) return;
            _telCheckTimer?.Stop();
            _telCheckTimer = new System.Windows.Forms.Timer { Interval = 600 };
            _telCheckTimer.Tick += (s, ev) => { _telCheckTimer.Stop(); CheckTelefon(); };
            _telCheckTimer.Start();
        }

        private void CheckTelefon()
        {
            string tel = txtTel.Text.Trim();
            if (string.IsNullOrEmpty(tel)) return;
            bool taken = DatabaseHelper.IsPhoneTaken(tel);
            lblTelMsg.ForeColor = taken ? C_Red : C_Green;
            lblTelMsg.Text = taken ? "✗ Telefon deja înregistrat" : "✓ Telefon disponibil";
        }

        private void TxtPass_Changed(object? sender, EventArgs e)
        {
            string p = txtPass.Text;
            bool[] ok =
            {
                p.Length >= 8,
                Regex.IsMatch(p, "[A-Z]"),
                Regex.IsMatch(p, "[0-9]"),
                Regex.IsMatch(p, @"[^a-zA-Z0-9]")
            };
            for (int i = 0; i < 4; i++)
            {
                ruleLabels[i].ForeColor = ok[i] ? C_Green : C_Orange;
                ruleLabels[i].BackColor = ok[i]
                    ? Color.FromArgb(40, C_Green)
                    : Color.FromArgb(30, C_Orange);
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
            string emailPfx = txtEmail.Text.Trim();
            string tel = txtTel.Text.Trim();
            string pass = txtPass.Text;
            string conf = txtPassConf.Text;

            if (string.IsNullOrEmpty(nume) || string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(emailPfx) || string.IsNullOrEmpty(tel) ||
                string.IsNullOrEmpty(pass) || pass != conf)
                return;

            string fullEmail = emailPfx + "@delivo.com";

            try
            {
                // ✅ Ordinea corectă: numeComplet, username, parola, telefon, email
                bool succes = DatabaseHelper.InregistreazaUtilizator(
                    nume, user, pass, tel, fullEmail);

                if (succes)
                {
                    LoginForm.UtilizatorLogat = user;
                    LoginForm.RolLogat = "client";
                    new MainForm().Show();
                    Hide();
                }
            }
            catch { }
        }

        // ── Helperi UI ────────────────────────────────────────────

        private void AddLbl(Panel p, string text, int x, int y)
        {
            p.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = C_Muted,
                AutoSize = true,
                Location = new Point(x, y),
                BackColor = Color.Transparent
            });
        }

        private Label AddMsg(Panel p, int x, int y)
        {
            var lbl = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 7.5f),
                AutoSize = true,
                Location = new Point(x, y),
                BackColor = Color.Transparent
            };
            p.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddInput(Panel parent, int x, int y, int w, bool isPass = false)
        {
            var tb = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                ForeColor = C_White,
                BackColor = C_InputBg,
                BorderStyle = BorderStyle.None,
                Location = new Point(x + 5, y),
                Size = new Size(w - 10, 25),
                UseSystemPasswordChar = isPass
            };
            var underline = new Panel
            {
                Location = new Point(x, y + 28),
                Size = new Size(w, 1),
                BackColor = Color.FromArgb(60, C_White)
            };
            tb.Enter += (s, ev) => underline.BackColor = C_Orange;
            tb.Leave += (s, ev) => underline.BackColor = Color.FromArgb(60, C_White);
            parent.Controls.Add(tb);
            parent.Controls.Add(underline);
            return tb;
        }

        private void RoundCtrl(Control c, int r)
        {
            if (c.Width <= 0 || c.Height <= 0) return;
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