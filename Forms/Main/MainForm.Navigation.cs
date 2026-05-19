using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Delivo.Forms
{
    public partial class MainForm
    {
        private readonly Color ColorDanger = Color.FromArgb(226, 75, 74);
        private void NavClick(int idx)
        {
            switch (idx)
            {
                case 0: _selectedCategory = null; RefreshCards(); break;
                case 1: ShowOrderHistory(); break;
                case 2: ShowCart(); break;
                case 3: new ProfileForm().ShowDialog(); break;
            }
            for (int i = 0; i < _navBtns.Length; i++)
                _navBtns[i].ForeColor = i == idx ? C_Orange : C_Muted;
        }

        private void OpenMenu(object? sender, EventArgs e)
        {
            // Dacă meniul este deja deschis, îl închidem
            if (_menuPanel != null && Controls.Contains(_menuPanel))
            {
                Controls.Remove(_menuPanel);
                _menuPanel?.Dispose();
                _menuPanel = null;
                return;
            }

            int menuWidth = 340;
            int margin = 20;

            _menuPanel = new Panel
            {
                Width = menuWidth,
                Height = ClientSize.Height - pnlNavbar.Height,
                Location = new Point(ClientSize.Width - menuWidth, 0),
                BackColor = Color.FromArgb(245, 10, 22, 54),
            };

            // Bordură stânga portocalie
            _menuPanel.Paint += (s, pe) =>
            {
                using var pen = new Pen(Color.FromArgb(180, 255, 107, 0), 3);
                pe.Graphics.DrawLine(pen, 0, 0, 0, _menuPanel.Height);
                using var lgb = new LinearGradientBrush(new Point(0, 0), new Point(menuWidth, 0),
                    Color.FromArgb(250, 10, 22, 54), Color.FromArgb(250, 18, 32, 70));
                pe.Graphics.FillRectangle(lgb, 1, 0, menuWidth - 1, _menuPanel.Height);
            };

            // TableLayoutPanel pentru a plasa logout-ul jos
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                RowCount = 3,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Clear();
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80f)); // zona logo + buton X
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // zona dinamica (butoanele)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70f));  // zona deconectare
            _menuPanel.Controls.Add(mainLayout);

            // ========== ZONA DE SUS (logo + buton închidere) ==========
            var topPanel = new Panel { Height = 80, BackColor = Color.Transparent, Dock = DockStyle.Fill };
            var lblLogo = new Label
            {
                Text = "DELIVO",
                Font = F_Logo,
                ForeColor = C_Orange,
                AutoSize = true,
                Location = new Point(margin, 20),
                BackColor = Color.Transparent
            };
            var btnClose = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = C_Orange,
                BackColor = Color.FromArgb(30, 255, 107, 0),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(38, 38),
                Location = new Point(menuWidth - 58, 18),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 1;
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(80, 255, 107, 0);
            Round(btnClose, 10);
            btnClose.Click += (_, _) => CloseMenu();

            var sepTop = new Panel
            {
                Height = 1,
                Width = menuWidth - margin * 2,
                Location = new Point(margin, 72),
                BackColor = Color.FromArgb(60, 255, 107, 0)
            };
            topPanel.Controls.Add(lblLogo);
            topPanel.Controls.Add(btnClose);
            topPanel.Controls.Add(sepTop);
            mainLayout.Controls.Add(topPanel, 0, 0);

            // ========== ZONA DINAMICĂ (butoanele meniului) ==========
            var itemsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 10)
            };
            mainLayout.Controls.Add(itemsPanel, 0, 1);

            // Elementele meniului (fără deconectare)
            var menuItems = new (string icon, string text, Action action)[]
            {
        ("🔍", "Caută", () => { CloseMenu(); txtSearch.Focus(); }),
        ("📋", "Comenzile mele", () => { CloseMenu(); ShowOrderHistory(); }),
        ("🌙", "Schimbă tema", () => { CloseMenu(); ToggleTheme(); }),
            };

            int itemHeight = 52;
            int btnWidth = menuWidth - margin * 2;
            foreach (var item in menuItems)
            {
                var row = CreateMenuItem(item.icon, item.text, item.action, btnWidth, itemHeight);
                itemsPanel.Controls.Add(row);
            }

            // ========== ZONA DE JOS (deconectare) ==========
            var logoutPanel = new Panel { Height = 70, BackColor = Color.Transparent, Dock = DockStyle.Fill };
            var logoutRow = CreateMenuItem("🚪", "Deconectare", () => { CloseMenu(); PerformLogout(); }, btnWidth, itemHeight, isLogout: true);
            logoutRow.Margin = new Padding(margin, 8, margin, 12);
            logoutPanel.Controls.Add(logoutRow);
            mainLayout.Controls.Add(logoutPanel, 0, 2);

            void CloseMenu()
            {
                if (_menuPanel != null && Controls.Contains(_menuPanel))
                {
                    Controls.Remove(_menuPanel);
                    _menuPanel.Dispose();
                    _menuPanel = null;
                }
            }

            Controls.Add(_menuPanel);
            _menuPanel.BringToFront();
        }

        // Helper pentru a crea un element de meniu (refactorizat)
        private Panel CreateMenuItem(string icon, string text, Action action, int width, int height, bool isLogout = false)
        {
            var row = new Panel
            {
                Width = width,
                Height = height,
                BackColor = Color.FromArgb(20, 255, 255, 255),
                Cursor = Cursors.Hand,
                Margin = new Padding(20, 5, 20, 5)
            };
            Round(row, 14);

            var lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 16),
                ForeColor = isLogout ? ColorDanger : C_Orange,
                AutoSize = true,
                Location = new Point(16, (height - 22) / 2),
                BackColor = Color.Transparent
            };

            var lblText = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = isLogout ? ColorDanger : C_Text,
                AutoSize = true,
                Location = new Point(52, (height - 20) / 2),
                BackColor = Color.Transparent,
                MaximumSize = new Size(width - 100, 0)
            };

            var lblArrow = new Label
            {
                Text = "›",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = C_Muted,
                AutoSize = true,
                Location = new Point(row.Width - 30, (height - 20) / 2),
                BackColor = Color.Transparent
            };

            row.Controls.Add(lblIcon);
            row.Controls.Add(lblText);
            row.Controls.Add(lblArrow);

            row.MouseEnter += (_, _) => row.BackColor = Color.FromArgb(40, 255, 107, 0);
            row.MouseLeave += (_, _) => row.BackColor = Color.FromArgb(20, 255, 255, 255);
            row.Click += (_, _) => action?.Invoke();

            return row;
        }

        private void PerformLogout()
        {
            var result = MessageBox.Show("Sigur vrei să te deconectezi?", "Deconectare",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                new LoginForm().Show();
                Application.OpenForms[0]?.Close();
                this.Close();
            }
        }

        private void ToggleTheme()
        {
            _darkMode = !_darkMode;

            // Aplică tema la MainForm
            this.BackColor = C_Bg;
            pnlScroll.BackColor = C_Bg;
            if (pnlHero != null) pnlHero.BackColor = C_Orange; // hero rămâne portocaliu
            if (lblGreet != null) lblGreet.ForeColor = C_Muted;
            if (txtSearch != null) txtSearch.BackColor = _darkMode ? Color.White : Color.FromArgb(240, 240, 240);
            // Reîncarcă categoriile și produsele pentru a reaplica culorile
            LoadCats();
            RefreshCards();
            // Actualizează culoarea butoanelor de navigare
            for (int i = 0; i < _navBtns.Length; i++)
                _navBtns[i].ForeColor = i == GetCurrentNavIndex() ? C_Orange : C_Muted;
        }

        private int GetCurrentNavIndex()
        {
            // Determină care buton este activ (poți păstra un câmp _currentNavIndex)
            // Simplu: caută butonul cu culoarea portocalie
            for (int i = 0; i < _navBtns.Length; i++)
                if (_navBtns[i].ForeColor == C_Orange) return i;
            return 0;
        }
    }
}