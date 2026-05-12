using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class LoginForm : Form
    {
        public static string UtilizatorLogat { get; set; } = "";
        public static string RolLogat { get; set; } = "";

        // Paleta de culori originală
        private readonly Color C_Dark = Color.FromArgb(13, 27, 62);
        private readonly Color C_Card = Color.FromArgb(22, 32, 64);
        private readonly Color C_Orange = Color.FromArgb(255, 107, 0);
        private readonly Color C_White = Color.White;
        private readonly Color C_Muted = Color.FromArgb(136, 153, 187);
        private readonly Color C_Green = Color.FromArgb(29, 158, 117);
        private readonly Color C_Red = Color.FromArgb(226, 75, 74);
        private readonly Color C_Input = Color.FromArgb(8, 18, 48);

        private Panel pnlStep1, pnlStep2;
        private TextBox txtUser1, txtPass1, txtPassConfirm;
        private Label lblError1, lblMatchMsg;
        private string _usernameTemp = "";

        public LoginForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            BuildUI();
        }

        private void BuildUI()
        {
            // Setări Fereastră - Maximizat conform imaginii
            this.Text = "Delivo - Autentificare";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = C_Dark;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- DESIGN: CURBA PORTOCALIE (Hero) ---
            var pnlHero = new Panel { Dock = DockStyle.Top, Height = 220, BackColor = Color.Transparent };
            pnlHero.Paint += (s, e) => {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    float w = pnlHero.Width;
                    float h = pnlHero.Height - 80;
                    path.AddLine(0, 0, w, 0);
                    path.AddLine(w, 0, w, h);
                    path.AddBezier(new PointF(w, h), new PointF(w * 0.75f, h + 100), new PointF(w * 0.25f, h - 100), new PointF(0, h));
                    path.AddLine(0, h, 0, 0);
                    path.CloseFigure();
                    using var br = new LinearGradientBrush(new PointF(0, 0), new PointF(0, pnlHero.Height), C_Orange, Color.FromArgb(255, 80, 0));
                    g.FillPath(br, path);
                }
            };

            // Logo-ul DELIVO centrat în curbă
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

            this.Controls.Add(pnlHero);

            // Container pentru Pași (pentru a-i centra sub curbă)
            var pnlContainer = new Panel
            {
                Size = new Size(420, 500),
                BackColor = Color.Transparent
            };
            this.Resize += (s, e) => {
                pnlContainer.Location = new Point((this.ClientSize.Width - pnlContainer.Width) / 2, 300);
            };
            this.Controls.Add(pnlContainer);

            // Inițializăm pașii în container
            BuildStep1(pnlContainer);
            BuildStep2(pnlContainer);

            pnlStep1.Visible = true;
            pnlStep2.Visible = false;
        }

        private void BuildStep1(Panel container)
        {
            pnlStep1 = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            var pnlCard = CreateCard(420);

            // UI conform cerinței (Identic cu imaginea)
            var dot1 = MakeDot(true); dot1.Location = new Point(175, 20);
            var dot2 = MakeDot(false); dot2.Location = new Point(205, 20);

            var lblT = new Label
            {
                Text = "AUTENTIFICARE",
                Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold),
                ForeColor = C_White,
                AutoSize = true,
                Location = new Point(110, 50)
            };

            int curY = 110;
            AddLabel(pnlCard, "UTILIZATOR", 30, curY); curY += 25;
            txtUser1 = AddInput(pnlCard, 30, curY, 360); curY += 55;

            AddLabel(pnlCard, "PAROLĂ", 30, curY); curY += 25;
            txtPass1 = AddInput(pnlCard, 30, curY, 360, true); curY += 45;

            lblError1 = new Label { Text = "", Font = new Font("Segoe UI", 8), ForeColor = C_Red, AutoSize = true, Location = new Point(30, curY) }; curY += 30;

            var btnContinue = MakeButton("Continuă →", 30, curY, 360);
            btnContinue.Click += (s, e) => Step1_Continue(); curY += 65;

            var lblReg = new Label
            {
                Text = "Nu ai cont? Înregistrează-te",
                Font = new Font("Segoe UI", 9),
                ForeColor = C_Orange,
                AutoSize = true,
                Cursor = Cursors.Hand,
                Location = new Point(120, curY)
            };
            lblReg.Click += (s, e) => OpenRegister();

            pnlCard.Controls.AddRange(new Control[] { dot1, dot2, lblT, lblError1, btnContinue, lblReg });
            pnlStep1.Controls.Add(pnlCard);
            container.Controls.Add(pnlStep1);
        }

        private Label lblUserChipName, lblUserChipRole;

        private void BuildStep2(Panel container)
        {
            pnlStep2 = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            var pnlCard = CreateCard(420);

            var dot1 = MakeDot(false); dot1.Location = new Point(175, 20);
            var dot2 = MakeDot(true); dot2.Location = new Point(205, 20);

            var pnlChip = new Panel { BackColor = C_Input, Size = new Size(360, 60), Location = new Point(30, 60) };
            RoundCtrl(pnlChip, 15);

            lblUserChipName = new Label { Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = C_White, Location = new Point(15, 10), AutoSize = true };
            lblUserChipRole = new Label { Font = new Font("Segoe UI", 9), ForeColor = C_Orange, Location = new Point(15, 32), AutoSize = true };
            pnlChip.Controls.AddRange(new Control[] { lblUserChipName, lblUserChipRole });

            int curY = 140;
            AddLabel(pnlCard, "CONFIRMĂ PAROLA PENTRU SECURITATE", 30, curY); curY += 25;
            txtPassConfirm = AddInput(pnlCard, 30, curY, 360, true);
            txtPassConfirm.TextChanged += (s, e) => CheckPasswordMatch(); curY += 45;

            lblMatchMsg = new Label { Text = "", Font = new Font("Segoe UI", 8), AutoSize = true, Location = new Point(30, curY) }; curY += 35;

            var btnLogin = MakeButton("Finalizează Logarea", 30, curY, 360);
            btnLogin.Click += (s, e) => Step2_Login(); curY += 60;

            var btnBack = new Label
            {
                Text = "← Înapoi la identificare",
                Font = new Font("Segoe UI", 9),
                ForeColor = C_Muted,
                AutoSize = true,
                Cursor = Cursors.Hand,
                Location = new Point(135, curY)
            };
            btnBack.Click += (s, e) => { pnlStep2.Visible = false; pnlStep1.Visible = true; };

            pnlCard.Controls.AddRange(new Control[] { dot1, dot2, pnlChip, lblMatchMsg, btnLogin, btnBack });
            pnlStep2.Controls.Add(pnlCard);
            container.Controls.Add(pnlStep2);
        }

        // --- LOGICA ORIGINALĂ PĂSTRATĂ ---
        private void Step1_Continue()
        {
            string u = txtUser1.Text.Trim(); string p = txtPass1.Text;
            if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p)) { lblError1.Text = "⚠ Introdu datele de acces!"; return; }
            string rol = DatabaseHelper.VerificaAutentificare(u, p);
            if (rol != null)
            {
                _usernameTemp = u; RolLogat = rol;
                lblUserChipName.Text = u.ToUpper();
                lblUserChipRole.Text = "Acces nivel: " + rol;
                pnlStep1.Visible = false; pnlStep2.Visible = true;
            }
            else { lblError1.Text = "⚠ Utilizator sau parolă incorectă!"; }
        }

        private void CheckPasswordMatch()
        {
            if (txtPassConfirm.Text == txtPass1.Text) { lblMatchMsg.ForeColor = C_Green; lblMatchMsg.Text = "✓ Confirmare validă"; }
            else { lblMatchMsg.ForeColor = C_Red; lblMatchMsg.Text = "✗ Parola nu corespunde"; }
        }

        private void Step2_Login()
        {
            if (txtPassConfirm.Text == txtPass1.Text)
            {
                UtilizatorLogat = _usernameTemp;
                if (RolLogat == "admin") new AdminForm().Show();
                else new MainForm().Show();
                this.Hide();
            }
        }

        private void OpenRegister() { new RegisterForm().Show(); this.Hide(); }

        // --- HELPERS REVIZUIȚI PENTRU LOOK-UL NOU ---
        private Panel CreateCard(int h)
        {
            var p = new Panel { BackColor = C_Card, Width = 420, Height = h, Location = new Point(0, 0) };
            RoundCtrl(p, 25);
            return p;
        }

        private void AddLabel(Panel p, string txt, int x, int y)
        {
            p.Controls.Add(new Label { Text = txt, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = C_Muted, AutoSize = true, Location = new Point(x, y), BackColor = Color.Transparent });
        }

        private TextBox AddInput(Panel parent, int x, int y, int w, bool isPass = false)
        {
            var tb = new TextBox { Font = new Font("Segoe UI", 12), ForeColor = C_White, BackColor = Color.FromArgb(12, 24, 56), BorderStyle = BorderStyle.None, Location = new Point(x + 5, y), Size = new Size(w - 10, 25), UseSystemPasswordChar = isPass };
            var line = new Panel { Location = new Point(x, y + 28), Size = new Size(w, 1), BackColor = Color.FromArgb(60, C_White) };
            tb.Enter += (s, e) => line.BackColor = C_Orange;
            tb.Leave += (s, e) => line.BackColor = Color.FromArgb(60, C_White);
            parent.Controls.Add(tb); parent.Controls.Add(line);
            return tb;
        }

        private Button MakeButton(string txt, int x, int y, int w)
        {
            var btn = new Button { Text = txt, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = C_White, BackColor = C_Orange, Size = new Size(w, 45), Location = new Point(x, y), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0; RoundCtrl(btn, 12);
            return btn;
        }

        private Label MakeDot(bool active) => new Label { Size = new Size(active ? 22 : 12, 6), BackColor = active ? C_Orange : Color.FromArgb(60, 255, 255, 255), Text = "" };

        private void RoundCtrl(Control c, int r)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(0, 0, r, r, 180, 90);
            gp.AddArc(c.Width - r, 0, r, r, 270, 90);
            gp.AddArc(c.Width - r, c.Height - r, r, r, 0, 90);
            gp.AddArc(0, c.Height - r, r, r, 90, 90);
            c.Region = new Region(gp);
        }
    }
}