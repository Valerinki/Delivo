using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class MainForm : Form
    {
        private bool _darkMode = true;
        private Color C_Bg => _darkMode ? Color.FromArgb(13, 27, 62) : Color.FromArgb(235, 240, 255);
        private Color C_Card => _darkMode ? Color.FromArgb(22, 32, 64) : Color.White;
        private Color C_Text => _darkMode ? Color.White : Color.FromArgb(15, 25, 55);
        private Color C_Muted => _darkMode ? Color.FromArgb(136, 153, 187) : Color.FromArgb(100, 115, 150);
        private readonly Color C_Orange = Color.FromArgb(255, 107, 0);
        private readonly Color C_NavBg = Color.FromArgb(8, 18, 48);

        private Font F_Logo, F_H1, F_Bold, F_Normal, F_Small, F_Tiny;
        private Panel pnlHero, pnlNavbar, pnlScroll;
        private FlowLayoutPanel flpCats, flpProducts;
        private TextBox txtSearch;
        private Button[] _navBtns = new Button[4];
        private Panel? _menuPanel;
        private Label lblGreet;

        private List<(int Id, string Nume, string Descriere, decimal Pret, string Categorie)> _all = new();
        private List<(int Id, string Nume, decimal Pret, int Qty)> _cart = new();

        private static readonly Dictionary<string, string> CatEmoji = new()
        { {"Pizza","🍕"},{"Burgeri","🍔"},{"Sushi","🍣"},{"Deserturi","🍰"},{"Bauturi","🥤"},{"Băuturi","🥤"} };
        private static readonly Dictionary<string, Color> CatColor = new()
        {
            {"Pizza",    Color.FromArgb(220,70,20)},
            {"Burgeri",  Color.FromArgb(180,90,10)},
            {"Sushi",    Color.FromArgb(20,110,180)},
            {"Deserturi",Color.FromArgb(180,40,110)},
            {"Bauturi",  Color.FromArgb(20,140,90)},
            {"Băuturi",  Color.FromArgb(20,140,90)}
        };

        public MainForm()
        {
            InitFonts();
            InitializeComponent();
            BuildUI();
            LoadCats();
            LoadProds();
        }

        private void InitFonts()
        {
            // Segoe UI Semibold - profesional si aerisit
            string fn = "Segoe UI";
            F_Logo = new Font("Showcard Gothic", 24, FontStyle.Bold);
            F_H1 = new Font(fn, 26, FontStyle.Bold);
            F_Bold = new Font(fn, 12, FontStyle.Bold);
            F_Normal = new Font(fn, 11, FontStyle.Regular);
            F_Small = new Font(fn, 10, FontStyle.Regular);
            F_Tiny = new Font(fn, 9, FontStyle.Regular);
        }
        private bool IsFontOk(string n) { try { var f = new Font(n, 10); bool ok = f.Name == n; f.Dispose(); return ok; } catch { return false; } }

        private void BuildUI()
        {
            Text = "Delivo"; WindowState = FormWindowState.Maximized;
            BackColor = C_Bg; MinimumSize = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterScreen;

            // ── NAVBAR JOS ──────────────────────────────
            pnlNavbar = new Panel { Dock = DockStyle.Bottom, Height = 68, BackColor = C_NavBg };
            pnlNavbar.Paint += (s, e) => {
                using var p = new Pen(Color.FromArgb(40, 255, 255, 255), 1);
                e.Graphics.DrawLine(p, 0, 0, pnlNavbar.Width, 0);
            };
            string[] nL = { "Acasă", "Caută", "Comenzi", "Profil" };
            string[] nE = { "🏠", "🔍", "📋", "👤" };
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                var b = new Button
                {
                    Text = nE[i] + "\n" + nL[i],
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
                _navBtns[i] = b; pnlNavbar.Controls.Add(b);
            }

            // ── PANEL SCROLL PRINCIPAL ───────────────────
            pnlScroll = new Panel { Dock = DockStyle.Fill, BackColor = C_Bg, AutoScroll = true };

            // ── HEADER ──────────────────────────────────
            var pnlHeader = new Panel { Location = new Point(0, 0), Height = 66, BackColor = C_Bg };

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

            // Buton hamburger ≡
            var btnHam = new Button
            {
                Text = "≡",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = C_Orange,
                BackColor = Color.FromArgb(30, 255, 107, 0),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(44, 44),
                Cursor = Cursors.Hand
            };
            btnHam.FlatAppearance.BorderSize = 1;
            btnHam.FlatAppearance.BorderColor = Color.FromArgb(80, 255, 107, 0);
            Round(btnHam, 10);
            btnHam.Click += OpenMenu;

            pnlHeader.Controls.Add(lblLogo);
            pnlHeader.Controls.Add(lblGreet);
            pnlHeader.Controls.Add(btnHam);
            pnlHeader.Resize += (s, e) => {
                lblGreet.Location = new Point(pnlHeader.Width - lblGreet.PreferredWidth - 62, 24);
                btnHam.Location = new Point(pnlHeader.Width - 54, 11);
            };

            // ── HERO PORTOCALIU ──────────────────────────
            // Inaltime = ~33% din ecran, minim 220px
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

            // Search bar — spatiu stanga pentru icon
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
                Location = new Point(52, 13),   // 52px de la stanga = dupa icon
                Text = "   Caută pizza, burgeri, sushi..."
            };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text.TrimStart().StartsWith("Caută")) { txtSearch.Text = ""; txtSearch.ForeColor = Color.Black; } };
            txtSearch.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "   Caută pizza, burgeri, sushi..."; txtSearch.ForeColor = Color.FromArgb(130, 130, 130); } };
            txtSearch.TextChanged += (s, e) => FilterSearch(txtSearch.Text);
            pnlSrch.Controls.Add(lblIco);
            pnlSrch.Controls.Add(txtSearch);

            pnlHero.Controls.Add(lblH1);
            pnlHero.Controls.Add(lblSub);
            pnlHero.Controls.Add(pnlSrch);

            pnlHero.Resize += (s, e) => {
                int w = pnlHero.Width, h = pnlHero.Height;
                int midY = (h - 60) / 2;   // centru vertical fara zona semicercului
                lblH1.Left = (w - lblH1.PreferredWidth) / 2; lblH1.Top = midY - 50;
                lblSub.Left = (w - lblSub.PreferredWidth) / 2; lblSub.Top = midY - 4;
                pnlSrch.Left = (w - pnlSrch.Width) / 2; pnlSrch.Top = midY + 46;
                // Redimensionam search bar
                pnlSrch.Width = Math.Min(560, w - 80);
                txtSearch.Width = pnlSrch.Width - 60;
            };

            // ── CATEGORII ────────────────────────────────
            var pnlCatW = new Panel { BackColor = C_Bg };
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
                Height = 110,
                BackColor = C_Bg,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false
            };
            pnlCatW.Controls.Add(lblCat);
            pnlCatW.Controls.Add(flpCats);

            // ── LABEL POPULARE ───────────────────────────
            var lblPop = new Label
            {
                Text = "🔥  Populare",
                Font = F_Bold,
                ForeColor = C_Text,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // ── GRILA PRODUSE ────────────────────────────
            flpProducts = new FlowLayoutPanel
            {
                BackColor = C_Bg,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = false,
                Padding = new Padding(4)
            };

            pnlScroll.Controls.Add(flpProducts);
            pnlScroll.Controls.Add(lblPop);
            pnlScroll.Controls.Add(pnlCatW);
            pnlScroll.Controls.Add(pnlHero);
            pnlScroll.Controls.Add(pnlHeader);

            Controls.Add(pnlNavbar);
            Controls.Add(pnlScroll);

            Resize += (s, e) => Relayout(pnlHeader, pnlCatW, lblPop);
            Relayout(pnlHeader, pnlCatW, lblPop);
        }

        private void Relayout(Panel hdr, Panel catW, Label lblPop)
        {
            int w = pnlScroll.ClientSize.Width;
            int heroH = Math.Max(260, ClientSize.Height / 3);

            hdr.Width = w;
            hdr.Location = new Point(0, 0);

            pnlHero.Width = w;
            pnlHero.Height = heroH;
            pnlHero.Location = new Point(0, 66);

            // =========================
            // CATEGORII
            // =========================
            int catTop = 66 + heroH + 25;
            catW.Width = w;
            catW.Height = 170;
            catW.Location = new Point(0, catTop);

            // Calculăm cât spațiu ocupă toate cercurile (5 categorii * lățime + margini)
            int totalCatsWidth = (140 + 30) * 5; // 140px lățime + 30px margini totale
            int leftPadding = (w - totalCatsWidth) / 2;

            flpCats.Width = w;
            flpCats.Location = new Point(0, 55);
            // Dacă ecranul e mic, lăsăm padding minim 25, altfel centrăm
            flpCats.Padding = new Padding(Math.Max(25, leftPadding), 5, 25, 5);

            // =========================
            // POPULARE
            // =========================
            int popTop = catTop + 185;

            lblPop.Location = new Point(32, popTop);

            // =========================
            // PRODUSE
            // =========================
            flpProducts.Location = new Point(0, popTop + 55);

            flpProducts.Width = w;
            flpProducts.Height = Math.Max(700, ClientSize.Height);

            // CENTRARE
            flpProducts.Padding = new Padding(35, 10, 35, 40);

            // SPATIU INTRE CARDURI
            flpProducts.Margin = new Padding(0);

            // =========================
            // NAVBAR
            // =========================
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

        // ── HERO PAINT ───────────────────────────────────
        private void PaintHero(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = pnlHero.Width;
            int h = pnlHero.Height;

            // WAVE / SEMICERC GLovo STYLE
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddBezier(
                    0, h - 40,
                    w / 4, h + 50,
                    w / 2, h - 120,
                    w, h - 20
                );

                path.AddLine(w, h + 100, 0, h + 100);
                path.CloseFigure();

                using (SolidBrush br = new SolidBrush(C_Bg))
                {
                    g.FillPath(br, path);
                }
            }

            // Cerc decorativ dreapta
            using (SolidBrush br2 = new SolidBrush(Color.FromArgb(25, 255, 255, 255)))
            {
                g.FillEllipse(br2, w - 220, -80, 280, 280);
                g.FillEllipse(br2, -100, h - 120, 180, 180);
            }
        }

        // ── CATEGORII ────────────────────────────────────
        private void LoadCats()
        {
            flpCats.Controls.Clear();

            var cats = new[]
            {
        ("🍕", "Pizza"),
        ("🍔", "Burgeri"),
        ("🍣", "Sushi"),
        ("🍰", "Deserturi"),
        ("🥤", "Băuturi")
    };

            flpCats.FlowDirection = FlowDirection.LeftToRight;
            flpCats.WrapContents = false;
            flpCats.AutoScroll = false;
            flpCats.Padding = new Padding(25, 5, 25, 5);

            foreach (var (em, nm) in cats)
            {
                Color bg = CatColor.ContainsKey(nm) ? CatColor[nm] : C_Orange;

                var item = new Panel
                {
                    Width = 130,
                    Height = 145,
                    BackColor = Color.Transparent,
                    Margin = new Padding(12, 0, 12, 0),
                    Cursor = Cursors.Hand
                };

                var circle = new Panel
                {
                    Width = 95,
                    Height = 95,
                    BackColor = bg,
                    Location = new Point(17, 5),
                    Cursor = Cursors.Hand
                };

                Round(circle, 47);

                // SHADOW EFFECT
                circle.Paint += (s, pe) =>
                {
                    pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    using var br = new SolidBrush(Color.FromArgb(35, Color.White));
                    pe.Graphics.FillEllipse(br, 8, 8, 35, 35);
                };

                var lblE = new Label
                {
                    Text = em,
                    Font = new Font("Segoe UI Emoji", 34),
                    AutoSize = false,
                    Size = new Size(95, 95),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent
                };

                var lblN = new Label
                {
                    Text = nm,
                    Font = new Font(F_Normal.FontFamily, 11, FontStyle.Bold),
                    ForeColor = C_Text,
                    Width = 130,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 108),
                    BackColor = Color.Transparent
                };

                circle.Controls.Add(lblE);

                item.Controls.Add(circle);
                item.Controls.Add(lblN);

                string n = nm;

                void FC(object? s, EventArgs ev) => FilterCat(n);

                item.Click += FC;
                circle.Click += FC;
                lblE.Click += FC;

                // HOVER EFFECT
                item.MouseEnter += (s, e) =>
                {
                    circle.Size = new Size(102, 102);
                    circle.Location = new Point(14, 2);
                };

                item.MouseLeave += (s, e) =>
                {
                    circle.Size = new Size(95, 95);
                    circle.Location = new Point(17, 5);
                };

                flpCats.Controls.Add(item);
            }
        }

        // ── PRODUSE ──────────────────────────────────────
        private void LoadProds()
        {
            try { _all = DatabaseHelper.GetProduse(); }
            catch
            {
                _all = new List<(int, string, string, decimal, string)>{
                    (1,"Pizza Margherita","Sos rosii, mozzarella, busuioc",89,"Pizza"),
                    (2,"Pizza Pepperoni","Sos rosii, mozzarella, pepperoni",99,"Pizza"),
                    (3,"Burger Classic","Carne de vita, salata, rosii",75,"Burgeri"),
                    (4,"Burger BBQ","Carne de vita, sos BBQ, ceapa",85,"Burgeri"),
                    (5,"Tiramisu","Desert italian clasic",45,"Deserturi")
                };
            }
            RefreshCards();
        }

        private void RefreshCards()
        {
            if (flpProducts == null) return;

            flpProducts.Controls.Clear();

            // LATIME DISPONIBILA
            int availableWidth = flpProducts.Width - 120;

            // CARDURI MAI MARI
            int cardWidth = 420;

            // DACA E ECRAN MIC
            if (availableWidth < 900)
                cardWidth = (availableWidth / 2) - 20;

            foreach (var p in _all)
            {
                var card = MakeCard(
                    p.Id,
                    p.Nume,
                    p.Descriere,
                    p.Pret,
                    p.Categorie,
                    cardWidth
                );

                // SPATIU FRUMOS INTRE ELE
                card.Margin = new Padding(18, 10, 18, 22);

                flpProducts.Controls.Add(card);
            }
        }

        private Panel MakeCard(int id, string nume, string desc, decimal pret, string cat, int w)
        {
            Color acc = CatColor.ContainsKey(cat) ? CatColor[cat] : C_Orange;
            string em = CatEmoji.ContainsKey(cat) ? CatEmoji[cat] : "🍽";

            var card = new Panel
            {
                Width = w,
                Height = 270,
                BackColor = C_Card,
                Margin = new Padding(18, 10, 18, 24),
                Cursor = Cursors.Hand
            };
            Round(card, 16);

            var pnlImg = new Panel
            {
                Width = w,
                Height = 145,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(30, acc)
            };
            pnlImg.Paint += (s, pe) => {
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var lgb = new LinearGradientBrush(new Point(0, 0), new Point(w, 120),
                    Color.FromArgb(70, acc), Color.FromArgb(15, Color.Black));
                pe.Graphics.FillRectangle(lgb, 0, 0, w, 120);
            };
            var lblEm = new Label
            {
                Text = em,
                Font = new Font("Segoe UI Emoji", 64),
                AutoSize = false,
                Size = new Size(w, 120),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            var lblBadge = new Label
            {
                Text = " " + cat + " ",
                Font = new Font(F_Tiny.FontFamily, 9, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(170, acc),
                AutoSize = true,
                Location = new Point(10, 10),
                Padding = new Padding(5, 2, 5, 2)
            };
            Round(lblBadge, 8);
            pnlImg.Controls.Add(lblEm); pnlImg.Controls.Add(lblBadge);

            var lblNume = new Label
            {
                Text = nume,
                Font = new Font(F_Normal.FontFamily, 11, FontStyle.Bold),
                ForeColor = C_Text,
                AutoSize = false,
                Width = w - 20,
                Height = 44,
                Location = new Point(16, 155),
                BackColor = Color.Transparent
            };
            var lblDesc = new Label
            {
                Text = desc,
                Font = new Font(F_Tiny.FontFamily, 9),
                ForeColor = C_Muted,
                AutoSize = false,
                Width = w - 20,
                Height = 18,
                Location = new Point(16, 198),
                BackColor = Color.Transparent
            };
            var lblPret = new Label
            {
                Text = pret.ToString("F0") + " lei",
                Font = new Font(F_Bold.FontFamily, 13, FontStyle.Bold),
                ForeColor = C_Orange,
                AutoSize = true,
                Location = new Point(16, 225),
                BackColor = Color.Transparent
            };
            var btnAdd = new Button
            {
                Text = "+",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = C_Orange,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 40),
                Location = new Point(w - 62, 218),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0; Round(btnAdd, 10);
            btnAdd.Click += (s, e) => AddCart(id, nume, pret);

            card.Controls.Add(pnlImg); card.Controls.Add(lblNume);
            card.Controls.Add(lblDesc); card.Controls.Add(lblPret); card.Controls.Add(btnAdd);
            card.MouseEnter += (s, e) => card.BackColor = _darkMode ? Color.FromArgb(28, 40, 76) : Color.FromArgb(225, 230, 250);
            card.MouseLeave += (s, e) => card.BackColor = C_Card;
            return card;
        }

        // ── FILTRARE ─────────────────────────────────────
        private void FilterCat(string cat)
        {
            var f = _all.FindAll(p => p.Categorie.Equals(cat, StringComparison.OrdinalIgnoreCase) ||
                p.Categorie.Equals(cat == "Băuturi" ? "Bauturi" : cat, StringComparison.OrdinalIgnoreCase));
            ShowList(f.Count > 0 ? f : _all);
        }
        private void FilterSearch(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.TrimStart().StartsWith("Caută")) { RefreshCards(); return; }
            ShowList(_all.FindAll(p => p.Nume.ToLower().Contains(q.ToLower()) || p.Categorie.ToLower().Contains(q.ToLower())));
        }
        private void ShowList(List<(int Id, string Nume, string Descriere, decimal Pret, string Categorie)> list)
        {
            flpProducts.Controls.Clear();
            int cw = Math.Max(200, (flpProducts.Width - 24) / 2);
            foreach (var p in list) flpProducts.Controls.Add(MakeCard(p.Id, p.Nume, p.Descriere, p.Pret, p.Categorie, cw));
        }

        // ── COS ──────────────────────────────────────────
        private void AddCart(int id, string nume, decimal pret)
        {
            int i = _cart.FindIndex(x => x.Id == id);
            if (i >= 0) { var t = _cart[i]; _cart[i] = (t.Id, t.Nume, t.Pret, t.Qty + 1); }
            else _cart.Add((id, nume, pret, 1));
            decimal tot = 0; foreach (var x in _cart) tot += x.Pret * x.Qty;
            _navBtns[2].Text = "🛒\nCoș(" + _cart.Count + ")"; _navBtns[2].ForeColor = C_Orange;
            MessageBox.Show("✓ " + nume + " adăugat!\nTotal: " + tot.ToString("F0") + " lei",
                "Coș", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ── NAVIGARE ─────────────────────────────────────
        private void NavClick(int idx)
        {
            foreach (var b in _navBtns) b.ForeColor = C_Muted;
            _navBtns[idx].ForeColor = C_Orange;
            switch (idx)
            {
                case 0: RefreshCards(); break;
                case 1: txtSearch.Focus(); break;
                case 2: ShowCart(); break;
                case 3: new ProfileForm().ShowDialog(); break;
            }
        }

        // ── MENIU HAMBURGER ──────────────────────────────
        private void OpenMenu(object? sender, EventArgs e)
        {
            if (_menuPanel != null && Controls.Contains(_menuPanel))
            {
                Controls.Remove(_menuPanel); _menuPanel = null; return;
            }

            int mw = 320;
            _menuPanel = new Panel
            {
                Width = mw,
                Height = ClientSize.Height - pnlNavbar.Height,
                Location = new Point(ClientSize.Width - mw, 0),
                BackColor = Color.FromArgb(245, 10, 22, 54),   // albastru foarte inchis semitransparent
            };
            // Border stanga portocaliu
            _menuPanel.Paint += (s, pe) => {
                using var pen = new Pen(Color.FromArgb(120, 255, 107, 0), 2);
                pe.Graphics.DrawLine(pen, 0, 0, 0, _menuPanel.Height);
                // fundal gradient
                using var lgb = new LinearGradientBrush(new Point(0, 0), new Point(mw, 0),
                    Color.FromArgb(250, 10, 22, 54), Color.FromArgb(250, 18, 32, 70));
                pe.Graphics.FillRectangle(lgb, 1, 0, mw - 1, _menuPanel.Height);
            };

            // Logo
            var lblMLogo = new Label
            {
                Text = "DELIVO",
                Font = F_Logo,
                ForeColor = C_Orange,
                AutoSize = true,
                Location = new Point(20, 20),
                BackColor = Color.Transparent
            };

            // Buton X
            var btnX = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = C_Orange,
                BackColor = Color.FromArgb(30, 255, 107, 0),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(38, 38),
                Location = new Point(mw - 50, 16),
                Cursor = Cursors.Hand
            };
            btnX.FlatAppearance.BorderSize = 1;
            btnX.FlatAppearance.BorderColor = Color.FromArgb(80, 255, 107, 0);
            Round(btnX, 8);
            btnX.Click += (s, ev) => { Controls.Remove(_menuPanel); _menuPanel = null; };

            // Separator
            var sep = new Panel { Height = 1, Width = mw - 40, Location = new Point(20, 68), BackColor = Color.FromArgb(40, 255, 107, 0) };

            // Iteme meniu
            int my = 90;
            var items = new[] { ("🔍", "Caută"), ("📋", "Comenzile mele"), ("", "---"), ("🌙", "Schimbă tema") };
            foreach (var (ico, txt) in items)
            {
                if (txt == "---")
                {
                    var div = new Panel { Height = 1, Width = mw - 40, Location = new Point(20, my), BackColor = Color.FromArgb(30, 255, 255, 255) };
                    _menuPanel.Controls.Add(div); my += 16; continue;
                }
                var row = new Panel
                {
                    Width = mw - 40,
                    Height = 52,
                    Location = new Point(20, my),
                    BackColor = Color.FromArgb(20, 255, 255, 255),
                    Cursor = Cursors.Hand
                };
                Round(row, 12);
                var lIco = new Label { Text = ico, Font = new Font("Segoe UI Emoji", 16), AutoSize = true, Location = new Point(14, 12), BackColor = Color.Transparent };
                var lTxt = new Label
                {
                    Text = txt,
                    Font = new Font(F_Normal.FontFamily, 11, FontStyle.Bold),
                    ForeColor = C_Text,
                    AutoSize = true,
                    Location = new Point(50, 16),
                    BackColor = Color.Transparent
                };
                var lArr = new Label { Text = "›", Font = new Font("Segoe UI", 16), ForeColor = C_Muted, AutoSize = true, Location = new Point(mw - 80, 12), BackColor = Color.Transparent };

                row.Controls.Add(lIco); row.Controls.Add(lTxt); row.Controls.Add(lArr);
                row.MouseEnter += (s, ev) => row.BackColor = Color.FromArgb(40, 255, 107, 0);
                row.MouseLeave += (s, ev) => row.BackColor = Color.FromArgb(20, 255, 255, 255);

                string t2 = txt;
                row.Click += (s, ev) => {
                    Controls.Remove(_menuPanel); _menuPanel = null;
                    if (t2 == "Caută") txtSearch.Focus();
                    else if (t2 == "Comenzile mele") new OrdersForm().ShowDialog();
                    else if (t2 == "Schimbă tema") { _darkMode = !_darkMode; BackColor = C_Bg; pnlScroll.BackColor = C_Bg; RefreshCards(); LoadCats(); }
                };

                _menuPanel.Controls.Add(row); my += 62;
            }

            _menuPanel.Controls.Add(lblMLogo);
            _menuPanel.Controls.Add(btnX);
            _menuPanel.Controls.Add(sep);

            Controls.Add(_menuPanel);
            _menuPanel.BringToFront();
        }

        private void ShowCart()
        {
            if (_cart.Count == 0) { MessageBox.Show("Coșul este gol!", "Coș", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            string msg = "🛒 Coș:\n\n"; decimal tot = 0;
            foreach (var x in _cart) { msg += $"• {x.Nume} x{x.Qty} = {x.Pret * x.Qty:F0} lei\n"; tot += x.Pret * x.Qty; }
            if (MessageBox.Show(msg + $"\n💰 TOTAL: {tot:F0} lei\n\nPlasezi comanda?", "Coș",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                PlaceOrder(tot);
        }
        private void PlaceOrder(decimal tot)
        {
            string adr = Microsoft.VisualBasic.Interaction.InputBox("Adresa livrare:", "Comandă", "");
            if (string.IsNullOrWhiteSpace(adr)) return;
            try
            {
                int uid = DatabaseHelper.GetUserId(LoginForm.UtilizatorLogat);
                int oid = DatabaseHelper.PlaseazaComanda(uid, tot, adr);
                _cart.Clear(); _navBtns[2].Text = "📋\nComenzi"; _navBtns[2].ForeColor = C_Muted;
                MessageBox.Show($"✅ Comanda #{oid} plasată!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message); }
        }

        // ── HELPERS ──────────────────────────────────────
        private void Round(Control c, int r)
        {
            if (c.Width < 1 || c.Height < 1) return;
            var p = new GraphicsPath();
            p.AddArc(0, 0, r * 2, r * 2, 180, 90); p.AddArc(c.Width - r * 2, 0, r * 2, r * 2, 270, 90);
            p.AddArc(c.Width - r * 2, c.Height - r * 2, r * 2, r * 2, 0, 90); p.AddArc(0, c.Height - r * 2, r * 2, r * 2, 90, 90);
            p.CloseFigure(); c.Region = new Region(p);
        }
        private Color Lighten(Color c, int a) => Color.FromArgb(Math.Min(255, c.R + a), Math.Min(255, c.G + a), Math.Min(255, c.B + a));

        protected override void OnFormClosed(FormClosedEventArgs e) { base.OnFormClosed(e); Application.Exit(); }
    }
}