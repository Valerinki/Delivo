using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Delivo.Forms
{
    public partial class MainForm
    {
        private void BuildUI()
        {
            Text = "Delivo";
            WindowState = FormWindowState.Maximized;
            BackColor = C_Bg;
            // переместить выше в метод:
            pnlScroll = new Panel { Dock = DockStyle.Fill, BackColor = C_Bg, AutoScroll = true };
            MinimumSize = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterScreen;

            // NAVBAR JOS
            pnlNavbar = new Panel { Dock = DockStyle.Bottom, Height = 68, BackColor = C_NavBg };
            pnlNavbar.Paint += PnlNavbar_Paint;
            string[] btnLabels = { "Acasă", "Comenzi", "Coș", "Profil" };
            string[] btnIcons = { "🏠", "📋", "🛒", "👤" };
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                var b = new Button
                {
                    Text = btnIcons[i] + "\n" + btnLabels[i],
                    Font = new Font(F_Tiny.FontFamily, 9),
                    ForeColor = i == 0 ? C_Orange : C_Muted,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand,
                    Size = new Size(120, 58),
                    Location = new Point(i * 120, 5)
                };
                b.FlatAppearance.BorderSize = 0;
                b.FlatAppearance.MouseOverBackColor = Color.Transparent;
                b.Click += (s, e) => NavClick(idx);
                _navBtns[i] = b;
                pnlNavbar.Controls.Add(b);
            }

            // PANEL SCROLL PRINCIPAL
            pnlScroll = new Panel { Dock = DockStyle.Fill, BackColor = C_Bg, AutoScroll = true };

            // HEADER
            pnlHeader = new Panel { Location = new Point(0, 0), Height = 66, BackColor = C_Bg };
            var lblLogo = new Label
            {
                Text = "DELIVO",
                Font = F_Logo,
                ForeColor = C_Orange,
                AutoSize = true,
                Location = new Point(22, 18),
                BackColor = Color.Transparent
            };
            lblGreet = new Label
            {
                Text = "Bun venit, " + LoginForm.UtilizatorLogat + "!",
                Font = new Font(F_Small.FontFamily, 10, FontStyle.Bold),
                ForeColor = C_Muted,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var btnHam = new Button
            {
                Text = "≡",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = C_Orange,
                BackColor = Color.FromArgb(30, 255, 107, 0),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(44, 44),
                Cursor = Cursors.Hand,
                Padding = new Padding(0, -35, 0, 0),  // ridică liniile mai sus
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnHam.FlatAppearance.BorderSize = 1;
            btnHam.FlatAppearance.BorderColor = Color.FromArgb(80, 255, 107, 0);
            Round(btnHam, 10);
            btnHam.Click += OpenMenu;  // direct, nu mai e nevoie de BtnHam_Click

            pnlHeader.Controls.Add(lblLogo);
            pnlHeader.Controls.Add(lblGreet);
            pnlHeader.Controls.Add(btnHam);
            pnlHeader.Resize += PnlHeader_Resize;

            // HERO
            pnlHero = new Panel { Location = new Point(0, 66), BackColor = C_Orange };
            pnlHero.Paint += PaintHero;

            var lblH1 = new Label
            {
                Text = "Ce dorești azi?",
                Font = F_H1,
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            var lblSub = new Label
            {
                Text = "Livrare rapidă la ușa ta 🚀",
                Font = new Font(F_Normal.FontFamily, 13),
                ForeColor = Color.FromArgb(235, 255, 255, 255),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var pnlSrch = new Panel { Height = 50, Width = 560, BackColor = Color.White };
            Round(pnlSrch, 25);
            var lblIco = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI Emoji", 16),
                AutoSize = true,
                Location = new Point(14, 11),
                BackColor = Color.Transparent
            };
            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font(F_Normal.FontFamily, 13),
                ForeColor = Color.FromArgb(130, 130, 130),
                BackColor = Color.White,
                Location = new Point(67, 13),
                Text = "Caută pizza, burgeri, sushi...",  // fără spații la început
                TextAlign = HorizontalAlignment.Left,
                Padding = new Padding(30, 0, 0, 0)         // adaugă spațiu interior stânga
            };
            txtSearch.GotFocus += TxtSearch_GotFocus;
            txtSearch.LostFocus += TxtSearch_LostFocus;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlSrch.Controls.Add(lblIco);
            pnlSrch.Controls.Add(txtSearch);

            pnlHero.Controls.Add(lblH1);
            pnlHero.Controls.Add(lblSub);
            pnlHero.Controls.Add(pnlSrch);

            // Evenimentul de resize pentru Hero (păstrat inline pentru că folosește variabile locale)
            pnlHero.Resize += (s, e) =>
            {
                int w = pnlHero.Width, h = pnlHero.Height;
                int midY = (h - 60) / 2;
                lblH1.Left = (w - lblH1.PreferredWidth) / 2;
                // slight upward adjustment so the main question sits a bit higher above the subtitle
                lblH1.Top = midY - 65;
                lblSub.Left = (w - lblSub.PreferredWidth) / 2;
                lblSub.Top = midY - 4;
                pnlSrch.Left = (w - pnlSrch.Width) / 2;
                pnlSrch.Top = midY + 46;
                pnlSrch.Width = Math.Min(560, w - 80);
                txtSearch.Width = pnlSrch.Width - 60;
            };

            // CATEGORII
            pnlCatW = new Panel { BackColor = C_Bg };
            var lblCat = new Label
            {
                Text = "Categorii",
                Font = F_Bold,
                ForeColor = C_Text,
                AutoSize = true,
                Location = new Point(24, 14),
                BackColor = Color.Transparent
            };
            flpCats = new FlowLayoutPanel
            {
                Location = new Point(0, 44),
                // increased height so the circular icons and their labels are fully visible
                Height = 170,
                BackColor = C_Bg,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true
            };
            pnlCatW.Controls.Add(lblCat);
            pnlCatW.Controls.Add(flpCats);

            // POPULARE
            lblPop = new Label
            {
                Text = "🔥  Populare",
                Font = F_Bold,
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // GRILA PRODUSE
            flpProducts = new FlowLayoutPanel
            {
                BackColor = C_Bg,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                // enable internal scrolling so product cards can be scrolled when they overflow
                AutoScroll = true,
                Padding = new Padding(4)
            };

            pnlScroll.Controls.Add(flpProducts);
            pnlScroll.Controls.Add(lblPop);
            pnlScroll.Controls.Add(pnlCatW);
            pnlScroll.Controls.Add(pnlHero);
            pnlScroll.Controls.Add(pnlHeader);

            Controls.Add(pnlNavbar);
            Controls.Add(pnlScroll);

            Resize += MainForm_Resize;
            Relayout();
        }

        private void Relayout()
        {
            int w = pnlScroll.ClientSize.Width;
            int heroH = Math.Max(260, ClientSize.Height / 3);

            pnlHeader.Width = w;
            pnlHeader.Location = new Point(0, 0);

            pnlHero.Width = w;
            pnlHero.Height = heroH;
            pnlHero.Location = new Point(0, 66);

            int catTop = 66 + heroH + 25;
            pnlCatW.Width = w;
            // give more vertical space for category icons and labels
            pnlCatW.Height = 200;
            pnlCatW.Location = new Point(0, catTop);

            int totalCatsWidth = (140 + 30) * 5;
            int leftPadding = (w - totalCatsWidth) / 2;
            flpCats.Width = w;
            flpCats.Location = new Point(0, 55);
            flpCats.Padding = new Padding(Math.Max(25, leftPadding), 5, 25, 5);

            // move the "Populare" section further down so category labels remain visible
            int popTop = catTop + 230;
            lblPop.Location = new Point(32, popTop);

            flpProducts.Location = new Point(0, popTop + 55);
            flpProducts.Width = w;
            flpProducts.Height = Math.Max(700, ClientSize.Height);
            flpProducts.Padding = new Padding(35, 10, 35, 40);
            flpProducts.Margin = new Padding(0);

            int bw = pnlNavbar.Width / 4;
            for (int i = 0; i < pnlNavbar.Controls.Count; i++)
            {
                if (pnlNavbar.Controls[i] is Button b)
                {
                    b.Width = bw;
                    b.Location = new Point(i * bw, 5);
                }
            }

            RefreshCards();
        }

        private void PaintHero(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int w = pnlHero.Width;
            int h = pnlHero.Height;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddBezier(0, h - 40, w / 4, h + 50, w / 2, h - 120, w, h - 20);
                path.AddLine(w, h + 100, 0, h + 100);
                path.CloseFigure();
                using (SolidBrush br = new SolidBrush(C_Bg))
                    g.FillPath(br, path);
            }

            using (SolidBrush br2 = new SolidBrush(Color.FromArgb(25, 255, 255, 255)))
            {
                g.FillEllipse(br2, w - 220, -80, 280, 280);
                g.FillEllipse(br2, -100, h - 120, 180, 180);
            }
        }

        private void LoadCats()
        {
            flpCats.Controls.Clear();
            var cats = new[] { ("🍕", "Pizza"), ("🍔", "Burgeri"), ("🍣", "Sushi"), ("🍰", "Deserturi"), ("🥤", "Băuturi") };
            flpCats.FlowDirection = FlowDirection.LeftToRight;
            flpCats.WrapContents = false;
            flpCats.AutoScroll = false;
            flpCats.Padding = new Padding(25, 5, 25, 5);

            foreach (var (em, nm) in cats)
            {
                Color bg = CatColor.ContainsKey(nm) ? CatColor[nm] : C_Orange;
                var item = new Panel { Width = 130, Height = 160, BackColor = Color.Transparent, Margin = new Padding(12, 0, 12, 0), Cursor = Cursors.Hand };
                // move circle slightly higher so the category name below remains visible
                var circle = new Panel { Width = 95, Height = 95, BackColor = bg, Location = new Point(17, 0), Cursor = Cursors.Hand };
                Round(circle, 47);
                circle.Paint += (s, pe) => { pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using var br = new SolidBrush(Color.FromArgb(35, Color.White)); pe.Graphics.FillEllipse(br, 8, 8, 35, 35); };
                var lblE = new Label { Text = em, Font = new Font("Segoe UI Emoji", 34), AutoSize = false, Size = new Size(95, 95), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent };
                var lblN = new Label
                {
                    Text = nm,
                    Font = new Font(F_Normal.FontFamily, 11, FontStyle.Bold),
                    ForeColor = Color.White,       // ← modificat: forțează alb
                    Width = 130,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 110),
                    BackColor = Color.Transparent
                };
                circle.Controls.Add(lblE);
                item.Controls.Add(circle);
                item.Controls.Add(lblN);
                // ensure the category label is visible above the circle
                lblN.BringToFront();
                void FC(object? s, EventArgs ev) => FilterCat(nm);
                item.Click += FC;
                circle.Click += FC;
                lblE.Click += FC;
                // reduced hover expansion so the circle doesn't overlap the label
                item.MouseEnter += (s, e) => { circle.Size = new Size(99, 99); circle.Location = new Point(16, -1); };
                item.MouseLeave += (s, e) => { circle.Size = new Size(95, 95); circle.Location = new Point(17, 0); };
                flpCats.Controls.Add(item);
            }
        }
    }
}