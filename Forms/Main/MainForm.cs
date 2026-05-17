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
        private Color C_Bg => _darkMode ? Color.FromArgb(13, 27, 62) : Color.FromArgb(235, 240, 255);
        private Color C_Card => _darkMode ? Color.FromArgb(22, 32, 64) : Color.White;
        private Color C_Text => _darkMode ? Color.White : Color.FromArgb(15, 25, 55);
        private Color C_Muted => _darkMode ? Color.FromArgb(136, 153, 187) : Color.FromArgb(100, 115, 150);
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

        public MainForm()
        {
            InitFonts();
            BuildUI();
            LoadCats();
            LoadProds();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }
    }
}