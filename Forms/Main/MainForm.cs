using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class MainForm : Form
    {
        // Tema
        private bool _darkMode = true;
        private Color C_Bg => _darkMode ? Color.FromArgb(13, 27, 62) : Color.White;
        private Color C_Card => _darkMode ? Color.FromArgb(22, 32, 64) : Color.FromArgb(250, 250, 252);
        private Color C_Text => _darkMode ? Color.White : Color.FromArgb(30, 30, 40);
        private Color C_Muted => _darkMode ? Color.FromArgb(136, 153, 187) : Color.FromArgb(110, 110, 120);
        private Color C_Border => _darkMode ? Color.FromArgb(40, 255, 255, 255) : Color.FromArgb(200, 200, 200);
        private readonly Color C_Orange = Color.FromArgb(255, 107, 0);
        private readonly Color C_NavBg = Color.FromArgb(8, 18, 48);

        // Fonturi
        private Font F_Logo, F_H1, F_Bold, F_Normal, F_Small, F_Tiny;

        // Controale principale
        private Panel pnlHero, pnlNavbar, pnlScroll;
        private FlowLayoutPanel flpCats, flpProducts;
        private TextBox txtSearch;
        private Button[] _navBtns = new Button[4];
        private Panel? _menuPanel;
        private Label lblGreet;
        private Panel pnlHeader, pnlCatW;
        private Label lblPop;
        private string _selectedCategory = null;

        // Date
        private List<(int Id, string Nume, string Descriere, decimal Pret, string Categorie)> _all = new();
        private List<(int Id, string Nume, decimal Pret, int Qty)> _cart = new();

        // Statice pentru categorii
        internal static readonly Dictionary<string, string> CatEmoji = new()
        {
            {"Pizza","🍕"},{"Burgeri","🍔"},{"Sushi","🍣"},
            {"Deserturi","🍰"},{"Bauturi","🥤"},{"Băuturi","🥤"}
        };
        internal static readonly Dictionary<string, Color> CatColor = new()
        {
            {"Pizza",    Color.FromArgb(220,70,20)},
            {"Burgeri",  Color.FromArgb(180,90,10)},
            {"Sushi",    Color.FromArgb(20,110,180)},
            {"Deserturi",Color.FromArgb(180,40,110)},
            {"Bauturi",  Color.FromArgb(20,140,90)},
            {"Băuturi",  Color.FromArgb(20,140,90)}
        };
        private void UpdateCategoryColorsForTheme()
        {
            if (_darkMode)
            {
                // Culorile originale (închise)
                CatColor["Pizza"] = Color.FromArgb(220, 70, 20);
                CatColor["Burgeri"] = Color.FromArgb(180, 90, 10);
                CatColor["Sushi"] = Color.FromArgb(20, 110, 180);
                CatColor["Deserturi"] = Color.FromArgb(180, 40, 110);
                CatColor["Bauturi"] = Color.FromArgb(20, 140, 90);
                CatColor["Băuturi"] = Color.FromArgb(20, 140, 90);
            }
            else
            {
                // Mod light: toate cercurile devin o nuanță uniformă de gri deschis
                Color lightGray = Color.FromArgb(200, 200, 210);
                CatColor["Pizza"] = lightGray;
                CatColor["Burgeri"] = lightGray;
                CatColor["Sushi"] = lightGray;
                CatColor["Deserturi"] = lightGray;
                CatColor["Bauturi"] = lightGray;
                CatColor["Băuturi"] = lightGray;
            }
        }
        private void UpdateExistingCategoriesColors()
        {
            foreach (Control item in flpCats.Controls)
            {
                if (item is Panel itemPanel)
                {
                    Panel? circle = null;
                    Label? lblN = null;
                    foreach (Control child in itemPanel.Controls)
                    {
                        if (child is Panel p && p.Size == new Size(95, 95))
                            circle = p;
                        else if (child is Label l && l.Location.Y == 110)
                            lblN = l;
                    }
                    if (circle != null && lblN != null)
                    {
                        string catName = lblN.Text;
                        Color newColor = CatColor.ContainsKey(catName) ? CatColor[catName] : C_Orange;
                        circle.BackColor = newColor;
                        lblN.ForeColor = Color.White; // ← forțează alb
                    }
                }
            }
        }

        public MainForm()
        {
            InitFonts();
            BuildUI();
            LoadCats();
            LoadProds();
            LoadCartFromFile();
            UpdateCartButton();
        }
        private class CartItem
        {
            public int Id { get; set; }
            public string Nume { get; set; } = "";
            public decimal Pret { get; set; }
            public int Qty { get; set; }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }
    }
}