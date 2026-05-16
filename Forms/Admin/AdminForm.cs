using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Delivo.Forms
{
    public partial class AdminForm : Form
    {
        // === Paletă DELIVO 2025 ===
        private readonly Color ColorPortocaliu = Color.FromArgb(255, 107, 0);
        private readonly Color ColorPortocaliuSoft = Color.FromArgb(255, 140, 60);
        private readonly Color ColorPortocaliuGlow = Color.FromArgb(48, 255, 107, 0);
        private readonly Color ColorAlbastruInchis = Color.FromArgb(13, 27, 62);   // #0D1B3E
        private readonly Color ColorSidebar = Color.FromArgb(9, 21, 45);           // #09152D
        private readonly Color ColorAlbastruCard = Color.FromArgb(23, 32, 64);   // #172040
        private readonly Color ColorHeaderBg = Color.FromArgb(10, 20, 43);
        private readonly Color ColorInputBg = Color.FromArgb(15, 27, 58);
        private readonly Color ColorAlb = Color.White;
        private readonly Color ColorTextSecundar = Color.FromArgb(136, 153, 187);  // #8899BB
        private readonly Color ColorAccentGreen = Color.FromArgb(52, 211, 128);
        private readonly Color ColorAccentSky = Color.FromArgb(100, 180, 255);
        private readonly Color ColorDanger = Color.FromArgb(255, 90, 90);
        private readonly Color ColorDangerBg = Color.FromArgb(38, 180, 50, 50);

        // === Layout principal ===
        private Panel pnlContent = null!;
        private Panel pnlSidebar = null!;
        private Panel pnlHeader = null!;
        private readonly List<Button> _navButtons = new();
        private Button? currentNavButton;

        // === Status comenzi (admin) ===
        internal static readonly string[] StatusComenziAdmin =
        {
            "În așteptare",
            "În preparare",
            "În livrare",
            "Livrată"
        };

        private enum ProductListFocus { None, Edit, Delete }
        private enum ProductFormMode { Add, Edit }

        public AdminForm()
        {
            EnableDoubleBuffer(this);
            BuildUI();
            LoadDashboard();
        }

        private void SetActiveNavButton(Button btn)
        {
            foreach (var b in _navButtons)
            {
                b.BackColor = Color.Transparent;
                b.ForeColor = ColorTextSecundar;
                b.FlatAppearance.BorderSize = 0;
                b.Padding = new Padding(50, 0, 12, 0);
            }

            currentNavButton = btn;
            btn.BackColor = ColorPortocaliuGlow;
            btn.ForeColor = ColorPortocaliu;
            btn.FlatAppearance.BorderColor = ColorPortocaliu;
            btn.FlatAppearance.BorderSize = 1;
            btn.Padding = new Padding(54, 0, 12, 0);
        }

        private void NavigateToSection(int index, ProductListFocus productFocus = ProductListFocus.None)
        {
            if (index >= 0 && index < _navButtons.Count)
                SetActiveNavButton(_navButtons[index]);

            switch (index)
            {
                case 0: LoadDashboard(); break;
                case 1: LoadProduse(productFocus); break;
                case 2: LoadComenzi(); break;
                case 3: LoadUtilizatori(); break;
            }
        }
    }
}
