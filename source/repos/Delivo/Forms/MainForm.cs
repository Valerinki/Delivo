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
        // Culorile principale Delivo
        private readonly Color ColorPortocaliu = Color.FromArgb(255, 107, 0);
        private readonly Color ColorAlbastruInchis = Color.FromArgb(13, 27, 62);
        private readonly Color ColorAlbastruCard = Color.FromArgb(22, 32, 64);
        private readonly Color ColorAlb = Color.White;
        private readonly Color ColorTextSecundar = Color.FromArgb(136, 153, 187);

        // Fontul Poppins (sau Segoe UI ca fallback)
        private Font FontTitlu;
        private Font FontNormal;
        private Font FontMic;
        private Font FontBold;

        // Paneluri principale
        private Panel pnlHero;
        private Panel pnlCategories;
        private Panel pnlProducts;
        private Panel pnlNavbar;
        private Label lblLogo;
        private Label lblGreeting;
        private TextBox txtSearch;
        private FlowLayoutPanel flpCategories;
        private FlowLayoutPanel flpProducts;
        private Button btnNavAcasa, btnNavCauta, btnNavComenzi, btnNavProfil;

        public MainForm()
        {
            InitializeFonts();
            InitializeComponent();
            BuildUI();
            LoadCategories();
            LoadProducts();
        }

        private void InitializeFonts()
        {
            // Incercam Poppins, daca nu e instalat folosim Segoe UI
            string fontName = "Poppins";
            try
            {
                var testFont = new Font(fontName, 10);
                if (testFont.Name != fontName) fontName = "Segoe UI";
                testFont.Dispose();
            }
            catch { fontName = "Segoe UI"; }

            FontTitlu = new Font(fontName, 16, FontStyle.Bold);
            FontBold = new Font(fontName, 11, FontStyle.Bold);
            FontNormal = new Font(fontName, 10, FontStyle.Regular);
            FontMic = new Font(fontName, 8, FontStyle.Regular);
        }

        private void BuildUI()
        {
            // === FEREASTRA PRINCIPALA ===
            this.Text = "Delivo";
            this.Size = new Size(420, 750);
            this.MinimumSize = new Size(380, 650);
            this.BackColor = ColorAlbastruInchis;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // === HEADER ===
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorAlbastruInchis,
                Padding = new Padding(16, 10, 16, 0)
            };

            lblLogo = new Label
            {
                Text = "DELIVO",
                Font = FontTitlu,
                ForeColor = ColorPortocaliu,
                AutoSize = true,
                Location = new Point(16, 15)
            };

            lblGreeting = new Label
            {
                Text = $"Bun venit, {LoginForm.UtilizatorLogat}!",
                Font = FontMic,
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point(16, 38)
            };

            pnlHeader.Controls.Add(lblLogo);
            pnlHeader.Controls.Add(lblGreeting);

            // === HERO (portocaliu semicercular) ===
            pnlHero = new Panel
            {
                Dock = DockStyle.Top,
                Height = 140,
                BackColor = ColorPortocaliu
            };
            pnlHero.Paint += PnlHero_Paint;

            var lblHeroTitle = new Label
            {
                Text = "Ce dorești azi?",
                Font = FontBold,
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(0, 18),
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblHeroTitle.Left = (pnlHero.Width - lblHeroTitle.PreferredWidth) / 2;

            var lblHeroSub = new Label
            {
                Text = "Livrare rapidă la ușa ta",
                Font = FontMic,
                ForeColor = Color.FromArgb(220, 255, 255, 255),
                AutoSize = true,
                Location = new Point(0, 42)
            };

            // Search bar
            var pnlSearch = new Panel
            {
                Height = 38,
                Width = 280,
                BackColor = ColorAlb,
                Location = new Point(0, 72)
            };
            MakeRound(pnlSearch, 19);

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = FontNormal,
                ForeColor = Color.FromArgb(80, 80, 80),
                BackColor = ColorAlb,
                Location = new Point(36, 9),
                Width = 220,
                Text = "Caută mâncare..."
            };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Caută mâncare...") txtSearch.Text = ""; };
            txtSearch.LostFocus += (s, e) => { if (string.IsNullOrEmpty(txtSearch.Text)) txtSearch.Text = "Caută mâncare..."; };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            var lblSearchIcon = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI Emoji", 12),
                AutoSize = true,
                Location = new Point(8, 7),
                BackColor = ColorAlb
            };

            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(lblSearchIcon);

            pnlHero.Controls.Add(lblHeroTitle);
            pnlHero.Controls.Add(lblHeroSub);
            pnlHero.Controls.Add(pnlSearch);
            pnlHero.Resize += (s, e) =>
            {
                lblHeroTitle.Left = (pnlHero.Width - lblHeroTitle.PreferredWidth) / 2;
                lblHeroSub.Left = (pnlHero.Width - lblHeroSub.PreferredWidth) / 2;
                pnlSearch.Left = (pnlHero.Width - pnlSearch.Width) / 2;
            };

            // === CATEGORII ===
            pnlCategories = new Panel
            {
                Dock = DockStyle.Top,
                Height = 115,
                BackColor = ColorAlbastruInchis,
                Padding = new Padding(16, 10, 16, 0)
            };

            var lblCatTitle = new Label
            {
                Text = "Categorii",
                Font = FontBold,
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(16, 10)
            };

            flpCategories = new FlowLayoutPanel
            {
                Location = new Point(16, 36),
                Width = 370,
                Height = 75,
                BackColor = ColorAlbastruInchis,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false
            };

            pnlCategories.Controls.Add(lblCatTitle);
            pnlCategories.Controls.Add(flpCategories);

            // === PRODUSE (scrollabil) ===
            var pnlProductsWrapper = new Panel
            {
                BackColor = ColorAlbastruInchis,
                Padding = new Padding(16, 8, 16, 80)
            };

            var lblProdTitle = new Label
            {
                Text = "Populare",
                Font = FontBold,
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(16, 8)
            };

            flpProducts = new FlowLayoutPanel
            {
                Location = new Point(16, 36),
                BackColor = ColorAlbastruInchis,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Padding = new Padding(0, 0, 0, 80)
            };

            pnlProductsWrapper.Controls.Add(lblProdTitle);
            pnlProductsWrapper.Controls.Add(flpProducts);

            // === NAVBAR (jos, fix) ===
            pnlNavbar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 65,
                BackColor = ColorAlbastruInchis
            };
            pnlNavbar.Paint += PnlNavbar_Paint;

            string[] navLabels = { "Acasă", "Caută", "Comenzi", "Profil" };
            string[] navEmoji = { "🏠", "🔍", "📋", "👤" };
            Button[] navBtns = new Button[4];

            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                var btn = new Button
                {
                    Text = navEmoji[i] + "\n" + navLabels[i],
                    Font = FontMic,
                    ForeColor = ColorTextSecundar,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(90, 55),
                    Location = new Point(5 + i * 98, 5),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.Transparent;

                if (i == 0) // Acasa activ
                {
                    btn.ForeColor = ColorPortocaliu;
                }

                btn.Click += (s, e) => NavBtn_Click(idx, navBtns);
                navBtns[i] = btn;
                pnlNavbar.Controls.Add(btn);
            }

            btnNavAcasa = navBtns[0];
            btnNavCauta = navBtns[1];
            btnNavComenzi = navBtns[2];
            btnNavProfil = navBtns[3];

            // === ASAMBLA FEREASTRA ===
            // Ordinea Dock conteaza!
            this.Controls.Add(pnlNavbar);       // jos fix
            this.Controls.Add(flpProducts);     // mijloc scrollabil
            this.Controls.Add(lblProdTitle);
            this.Controls.Add(pnlCategories);   // sus
            this.Controls.Add(pnlHero);         // sus
            this.Controls.Add(pnlHeader);       // sus

            // Ajustam dimensiunile la resize
            this.Resize += MainForm_Resize;
            MainForm_Resize(null, null);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            int w = this.ClientSize.Width;
            int topUsed = 60 + 140 + 115 + 30; // header + hero + categorii + label produse
            int bottomUsed = 65; // navbar

            // Produse ocupa restul spatiului
            flpProducts.Location = new Point(16, topUsed);
            flpProducts.Width = w - 32;
            flpProducts.Height = this.ClientSize.Height - topUsed - bottomUsed - 10;

            // Label "Populare"
            var lblProd = this.Controls.Find("", false);
            foreach (Control c in this.Controls)
            {
                if (c is Label lbl && lbl.Text == "Populare")
                {
                    lbl.Location = new Point(16, topUsed - 28);
                    break;
                }
            }

            // Recalculeaza latimea cardurilor
            LoadProductCards(null);
        }

        // === HERO - desenam semicercul jos ===
        private void PnlHero_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int w = pnlHero.Width;
            int h = pnlHero.Height;

            // Semicerc jos (fundalul albastru "musca" din portocaliu)
            using var path = new GraphicsPath();
            path.AddArc(-50, h - 60, w + 100, 120, 0, 180);
            path.AddLine(w + 50, h - 60 + 60, w + 50, h + 10);
            path.AddLine(w + 50, h + 10, -50, h + 10);
            path.CloseFigure();

            using var brush = new SolidBrush(ColorAlbastruInchis);
            g.FillPath(brush, path);
        }

        // === NAVBAR - linie sus ===
        private void PnlNavbar_Paint(object sender, PaintEventArgs e)
        {
            using var pen = new Pen(Color.FromArgb(40, 255, 255, 255), 1);
            e.Graphics.DrawLine(pen, 0, 0, pnlNavbar.Width, 0);
        }

        // === INCARCA CATEGORII din baza de date ===
        private void LoadCategories()
        {
            flpCategories.Controls.Clear();

            // Categorii hardcodate (se pot lua din BD mai tarziu)
            var categorii = new List<(string emoji, string nume)>
            {
                ("🍕", "Pizza"),
                ("🍔", "Burgeri"),
                ("🍣", "Sushi"),
                ("🍰", "Deserturi"),
                ("🥤", "Băuturi")
            };

            foreach (var (emoji, nume) in categorii)
            {
                var catPanel = CreateCategoryItem(emoji, nume);
                flpCategories.Controls.Add(catPanel);
            }
        }

        private Panel CreateCategoryItem(string emoji, string nume)
        {
            var pnl = new Panel
            {
                Width = 65,
                Height = 72,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Margin = new Padding(4, 0, 4, 0)
            };

            // Cercul alb
            var circle = new Panel
            {
                Width = 54,
                Height = 54,
                Location = new Point(5, 0),
                BackColor = ColorAlb,
                Cursor = Cursors.Hand
            };
            MakeRound(circle, 27);

            var lblEmoji = new Label
            {
                Text = emoji,
                Font = new Font("Segoe UI Emoji", 22),
                AutoSize = false,
                Size = new Size(54, 54),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var lblNume = new Label
            {
                Text = nume,
                Font = FontMic,
                ForeColor = ColorTextSecundar,
                AutoSize = false,
                Width = 65,
                Height = 16,
                Location = new Point(0, 56),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            circle.Controls.Add(lblEmoji);
            pnl.Controls.Add(circle);
            pnl.Controls.Add(lblNume);

            // Hover effect
            circle.MouseEnter += (s, e) => circle.BackColor = Color.FromArgb(255, 240, 220);
            circle.MouseLeave += (s, e) => circle.BackColor = ColorAlb;
            lblEmoji.MouseEnter += (s, e) => circle.BackColor = Color.FromArgb(255, 240, 220);
            lblEmoji.MouseLeave += (s, e) => circle.BackColor = ColorAlb;

            // Click pe categorie - filtreaza produsele
            circle.Click += (s, e) => FilterByCategory(nume);
            lblEmoji.Click += (s, e) => FilterByCategory(nume);

            return pnl;
        }

        // === INCARCA PRODUSE din baza de date ===
        private List<(int Id, string Nume, string Descriere, decimal Pret, string Categorie)> _allProducts = new();

        private void LoadProducts()
        {
            try
            {
                _allProducts = DatabaseHelper.GetProduse();
                LoadProductCards(_allProducts);
            }
            catch
            {
                // Daca BD nu e disponibila, afisam produse demo
                _allProducts = new List<(int, string, string, decimal, string)>
                {
                    (1, "Pizza Margherita", "Sos rosii, mozzarella", 89, "Pizza"),
                    (2, "Pizza Pepperoni",  "Sos rosii, pepperoni",  99, "Pizza"),
                    (3, "Burger Classic",   "Carne, salata, rosii",  75, "Burgeri"),
                    (4, "Burger BBQ",       "Carne, sos BBQ",        85, "Burgeri"),
                    (5, "Tiramisu",         "Desert italian",        45, "Deserturi")
                };
                LoadProductCards(_allProducts);
            }
        }

        private Dictionary<string, string> _categoryEmoji = new()
        {
            {"Pizza", "🍕"}, {"Burgeri", "🍔"}, {"Sushi", "🍣"},
            {"Deserturi", "🍰"}, {"Bauturi", "🥤"}, {"Băuturi", "🥤"}
        };

        private void LoadProductCards(List<(int Id, string Nume, string Descriere, decimal Pret, string Categorie)> products)
        {
            if (products == null) products = _allProducts;

            flpProducts.Controls.Clear();
            int cardWidth = (flpProducts.Width - 12) / 2;
            if (cardWidth < 140) cardWidth = 140;

            foreach (var p in products)
            {
                var card = CreateProductCard(p.Id, p.Nume, p.Descriere, p.Pret, p.Categorie, cardWidth);
                flpProducts.Controls.Add(card);
            }
        }

        private Panel CreateProductCard(int id, string nume, string descriere, decimal pret, string categorie, int width)
        {
            var card = new Panel
            {
                Width = width,
                Height = 165,
                BackColor = ColorAlbastruCard,
                Margin = new Padding(0, 0, 8, 10),
                Cursor = Cursors.Hand
            };
            MakeRound(card, 12);

            // Zona imaginii (emoji mare)
            var pnlImg = new Panel
            {
                Width = width,
                Height = 85,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(30, 45, 84)
            };

            string emoji = _categoryEmoji.ContainsKey(categorie) ? _categoryEmoji[categorie] : "🍽";
            var lblEmoji = new Label
            {
                Text = emoji,
                Font = new Font("Segoe UI Emoji", 32),
                AutoSize = false,
                Size = new Size(width, 85),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlImg.Controls.Add(lblEmoji);

            // Info produs
            var lblNume = new Label
            {
                Text = nume,
                Font = FontNormal,
                ForeColor = ColorAlb,
                AutoSize = false,
                Width = width - 16,
                Height = 36,
                Location = new Point(10, 90),
                BackColor = Color.Transparent
            };

            var lblPret = new Label
            {
                Text = $"{pret} lei",
                Font = FontBold,
                ForeColor = ColorPortocaliu,
                AutoSize = true,
                Location = new Point(10, 130),
                BackColor = Color.Transparent
            };

            // Buton +
            var btnAdd = new Button
            {
                Text = "+",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorAlb,
                BackColor = ColorPortocaliu,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(30, 30),
                Location = new Point(width - 40, 127),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            MakeRound(btnAdd, 6);
            btnAdd.Click += (s, e) => AddToCart(id, nume, pret);

            card.Controls.Add(pnlImg);
            card.Controls.Add(lblNume);
            card.Controls.Add(lblPret);
            card.Controls.Add(btnAdd);

            // Hover
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(30, 42, 80);
            card.MouseLeave += (s, e) => card.BackColor = ColorAlbastruCard;

            return card;
        }

        // === FILTRARE dupa categorie ===
        private void FilterByCategory(string categorie)
        {
            var filtered = _allProducts.FindAll(p =>
                p.Categorie.Equals(categorie, StringComparison.OrdinalIgnoreCase));

            if (filtered.Count == 0)
                filtered = _allProducts;

            LoadProductCards(filtered);
        }

        // === CAUTARE ===
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string query = txtSearch.Text.ToLower();
            if (query == "caută mâncare..." || string.IsNullOrWhiteSpace(query))
            {
                LoadProductCards(_allProducts);
                return;
            }

            var filtered = _allProducts.FindAll(p =>
                p.Nume.ToLower().Contains(query) ||
                p.Categorie.ToLower().Contains(query));

            LoadProductCards(filtered);
        }

        // === COS DE CUMPARATURI ===
        private List<(int Id, string Nume, decimal Pret, int Cantitate)> _cart = new();

        private void AddToCart(int id, string nume, decimal pret)
        {
            var existing = _cart.FindIndex(x => x.Id == id);
            if (existing >= 0)
            {
                var item = _cart[existing];
                _cart[existing] = (item.Id, item.Nume, item.Pret, item.Cantitate + 1);
            }
            else
            {
                _cart.Add((id, nume, pret, 1));
            }

            decimal total = 0;
            foreach (var item in _cart) total += item.Pret * item.Cantitate;

            // Actualizare buton Comenzi
            btnNavComenzi.Text = $"📋\nCoș ({_cart.Count})";
            btnNavComenzi.ForeColor = ColorPortocaliu;

            MessageBox.Show($"✓ {nume} adăugat în coș!\nTotal: {total} lei",
                "Adăugat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // === NAVIGARE ===
        private void NavBtn_Click(int index, Button[] btns)
        {
            foreach (var btn in btns)
                btn.ForeColor = ColorTextSecundar;

            btns[index].ForeColor = ColorPortocaliu;

            switch (index)
            {
                case 0: // Acasa
                    LoadProducts();
                    break;
                case 1: // Cauta
                    txtSearch.Focus();
                    break;
                case 2: // Comenzi
                    ShowCart();
                    break;
                case 3: // Profil
                    ShowProfile();
                    break;
            }
        }

        private void ShowCart()
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Coșul tău este gol.\nAdaugă produse din meniu!",
                    "Coș gol", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string msg = "Produse în coș:\n\n";
            decimal total = 0;
            foreach (var item in _cart)
            {
                msg += $"• {item.Nume} x{item.Cantitate} = {item.Pret * item.Cantitate} lei\n";
                total += item.Pret * item.Cantitate;
            }
            msg += $"\nTOTAL: {total} lei";

            var result = MessageBox.Show(msg + "\n\nDorești să plasezi comanda?",
                "Coșul meu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                PlaceOrder(total);
        }

        private void PlaceOrder(decimal total)
        {
            try
            {
                string adresa = Microsoft.VisualBasic.Interaction.InputBox(
                    "Introdu adresa de livrare:", "Adresa livrare", "");

                if (string.IsNullOrWhiteSpace(adresa)) return;

                // Gasim ID-ul utilizatorului
                int userId = DatabaseHelper.GetUserId(LoginForm.UtilizatorLogat);
                int orderId = DatabaseHelper.PlaseazaComanda(userId, total, adresa);

                _cart.Clear();
                btnNavComenzi.Text = "📋\nComenzi";
                btnNavComenzi.ForeColor = ColorTextSecundar;

                MessageBox.Show($"Comanda #{orderId} a fost plasată cu succes!\nEste în curs de livrare.",
                    "Comandă confirmată", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la plasarea comenzii: {ex.Message}",
                    "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowProfile()
        {
            MessageBox.Show(
                $"Utilizator: {LoginForm.UtilizatorLogat}\nRol: {LoginForm.RolLogat}",
                "Profilul meu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // === HELPER: border-radius pe Panel ===
        private void MakeRound(Control ctrl, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(ctrl.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(ctrl.Width - radius * 2, ctrl.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, ctrl.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            ctrl.Region = new Region(path);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }
    }
}