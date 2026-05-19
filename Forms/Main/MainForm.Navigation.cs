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
            if (_menuPanel != null && Controls.Contains(_menuPanel))
            {
                Controls.Remove(_menuPanel);
                _menuPanel?.Dispose();
                _menuPanel = null;
                return;
            }

            // ========== PARAMETRI REGLABILI MANUAL ==========
            int menuWidth = 340;                // lățimea meniului
            int margin = 20;                   // marginea stângă internă
            int menuRightOffset = -15;         // cu cât mai la stânga față de marginea ferestrei (negativ = spre stânga)
            int separatorTopY = 62;            // poziția liniei de separare (mai sus = valoare mai mică)
                                               // ===============================================

            int menuX = ClientSize.Width - menuWidth + menuRightOffset; // calcul poziție X

            _menuPanel = new Panel
            {
                Width = menuWidth,
                Height = ClientSize.Height - pnlNavbar.Height,
                Location = new Point(menuX, 0),
                BackColor = Color.FromArgb(245, 10, 22, 54),
            };

            _menuPanel.Paint += (s, pe) =>
            {
                using var pen = new Pen(Color.FromArgb(180, 255, 107, 0), 3);
                pe.Graphics.DrawLine(pen, 0, 0, 0, _menuPanel.Height);
                using var lgb = new LinearGradientBrush(new Point(0, 0), new Point(menuWidth, 0),
                    Color.FromArgb(250, 10, 22, 54), Color.FromArgb(250, 18, 32, 70));
                pe.Graphics.FillRectangle(lgb, 1, 0, menuWidth - 1, _menuPanel.Height);
            };

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                RowCount = 3,
                ColumnCount = 1,
                AutoScroll = true
            };
            mainLayout.RowStyles.Clear();
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70f));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70f));
            _menuPanel.Controls.Add(mainLayout);

            // ========== ZONA DE SUS ==========
            var topPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            var lblLogo = new Label
            {
                Text = "DELIVO",
                Font = F_Logo,
                ForeColor = C_Orange,
                AutoSize = true,
                Location = new Point(margin, (70 - F_Logo.Height) / 2),
                BackColor = Color.Transparent
            };
            var btnClose = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = C_Orange,
                BackColor = Color.FromArgb(30, 255, 107, 0),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(36, 36),
                Location = new Point(menuWidth - 48, (70 - 36) / 2),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 1;
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(80, 255, 107, 0);
            Round(btnClose, 10);
            btnClose.Click += (_, _) => CloseMenu();

            // Separator poziționat mai sus conform separatorTopY
            var sepTop = new Panel
            {
                Height = 1,
                Width = menuWidth - margin * 2,
                Location = new Point(margin, separatorTopY),
                BackColor = Color.FromArgb(60, 255, 107, 0)
            };
            topPanel.Controls.Add(lblLogo);
            topPanel.Controls.Add(btnClose);
            topPanel.Controls.Add(sepTop);
            mainLayout.Controls.Add(topPanel, 0, 0);

            // ========== ZONA DINAMICĂ ==========
            var itemsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 5, 0, 5)
            };
            mainLayout.Controls.Add(itemsPanel, 0, 1);

            var menuItems = new (string icon, string text, Action action)[]
            {
        ("🔍", "Caută", () => { CloseMenu(); txtSearch.Focus(); }),
        ("📋", "Comenzile mele", () => { CloseMenu(); ShowOrderHistory(); }),
        ("🌙", "Schimbă tema", () => { CloseMenu(); ToggleTheme(); }),
            };

            int itemHeight = 50;
            int btnWidth = menuWidth - margin * 2;
            foreach (var item in menuItems)
            {
                var row = CreateMenuItem(item.icon, item.text, item.action, btnWidth, itemHeight);
                itemsPanel.Controls.Add(row);
            }

            // ========== ZONA DE JOS (deconectare) ==========
            int logoutRightMargin = 8; // distanţa de la marginea dreaptă
            int logoutBtnWidth = btnWidth - logoutRightMargin;
            var logoutRow = CreateMenuItem("🚪", "Deconectare", () => { CloseMenu(); PerformLogout(); }, logoutBtnWidth, itemHeight, isLogout: true);
            int logoutMarginTop = (70 - itemHeight) / 2;
            logoutRow.Margin = new Padding(margin, logoutMarginTop, 0, logoutMarginTop);
            logoutRow.Location = new Point(margin, logoutMarginTop); // aliniere la stânga

            var logoutPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
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

        private Panel CreateMenuItem(string icon, string text, Action action, int width, int height, bool isLogout = false)
        {
            // ========== PARAMETRI REGLABILI ==========
            int iconLeft = 16;          // distanța iconiței de la marginea stângă
            int textLeft = 65;          // distanța textului de la marginea stângă (mărită pentru a nu fi acoperit)
            int arrowRight = 26;        // distanța săgeții de la marginea dreaptă
                                        // ========================================

            var row = new Panel
            {
                Width = width,
                Height = height,
                BackColor = Color.FromArgb(20, 255, 255, 255),
                Cursor = Cursors.Hand,
                Margin = new Padding(20, 4, 20, 4)
            };
            Round(row, 12);

            int centerY = (height - 22) / 2; // pentru iconiță și săgeată
            int textCenterY = (height - 20) / 2;

            var lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 16),
                ForeColor = isLogout ? ColorDanger : C_Orange,
                AutoSize = true,
                Location = new Point(iconLeft, centerY),
                BackColor = Color.Transparent
            };

            var lblText = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = isLogout ? ColorDanger : C_Text,
                AutoSize = true,
                Location = new Point(textLeft, textCenterY),
                BackColor = Color.Transparent,
                MaximumSize = new Size(width - textLeft - 40, 0)
            };

            var lblArrow = new Label
            {
                Text = "›",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = C_Muted,
                AutoSize = true,
                Location = new Point(row.Width - arrowRight, centerY),
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

            UpdateCategoryColorsForTheme();

            this.BackColor = C_Bg;
            pnlScroll.BackColor = C_Bg;
            if (pnlHero != null)
            {
                pnlHero.BackColor = C_Orange;
                pnlHero.Refresh();
            }
            if (lblGreet != null) lblGreet.ForeColor = C_Muted;
            if (txtSearch != null)
            {
                txtSearch.BackColor = _darkMode ? Color.White : Color.FromArgb(240, 240, 240);
                txtSearch.ForeColor = _darkMode ? Color.Black : Color.FromArgb(60, 60, 70);
            }
            if (lblPop != null)
                lblPop.ForeColor = _darkMode ? Color.White : Color.Black;
            UpdateExistingCategoriesColors();
            RefreshCards();
            for (int i = 0; i < _navBtns.Length; i++)
                _navBtns[i].ForeColor = i == GetCurrentNavIndex() ? C_Orange : C_Muted;

            this.Refresh();
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