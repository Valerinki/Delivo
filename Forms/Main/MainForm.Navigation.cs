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
            using var confirmDialog = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(22, 32, 64),
                Width = 380,
                Height = 200,
                ShowInTaskbar = false,
                TopMost = true
            };

            // Colțuri rotunde + bordură portocalie
            confirmDialog.Paint += (ds, de) =>
            {
                var gp = new GraphicsPath();
                int r = 20;
                var rect = confirmDialog.ClientRectangle;
                gp.AddArc(rect.Left, rect.Top, r * 2, r * 2, 180, 90);
                gp.AddArc(rect.Right - r * 2, rect.Top, r * 2, r * 2, 270, 90);
                gp.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                gp.AddArc(rect.Left, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                gp.CloseFigure();
                confirmDialog.Region = new Region(gp);
                de.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(C_Orange, 2);
                de.Graphics.DrawPath(pen, gp);
            };

            // Titlu
            var lblTitle = new Label
            {
                Text = "🔓  Deconectare",
                Font = new Font("Poppins", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(24, 18),
                AutoSize = true
            };
            confirmDialog.Controls.Add(lblTitle);

            // Separator
            var separator = new Panel
            {
                BackColor = C_Orange,
                Height = 2,
                Width = confirmDialog.ClientSize.Width - 48,
                Location = new Point(24, 52)
            };
            confirmDialog.Controls.Add(separator);

            // Mesaj întrebare
            var lblQuestion = new Label
            {
                Text = "Sigur vrei să te deconectezi?",
                Font = new Font("Poppins", 10),
                ForeColor = Color.FromArgb(200, 215, 240),
                AutoSize = true,
                Location = new Point(24, 80)
            };
            confirmDialog.Controls.Add(lblQuestion);

            // Butoane
            int btnW = 100, btnH = 36;
            var btnYes = new Button
            {
                Text = "Da",
                FlatStyle = FlatStyle.Flat,
                BackColor = C_Orange,
                ForeColor = Color.White,
                Font = new Font("Poppins", 9, FontStyle.Bold),
                Size = new Size(btnW, btnH),
                Location = new Point(confirmDialog.ClientSize.Width - btnW * 2 - 20, confirmDialog.ClientSize.Height - 60),
                Cursor = Cursors.Hand
            };
            btnYes.FlatAppearance.BorderSize = 0;
            Round(btnYes, 18);
            btnYes.Click += (_, _) =>
            {
                confirmDialog.Close();
                // Repornește aplicația (deschide LoginForm și închide cea curentă)
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                Application.Exit();
            };

            var btnNo = new Button
            {
                Text = "Nu",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 55, 85),
                ForeColor = Color.White,
                Font = new Font("Poppins", 9, FontStyle.Bold),
                Size = new Size(btnW, btnH),
                Location = new Point(confirmDialog.ClientSize.Width - btnW - 10, confirmDialog.ClientSize.Height - 60),
                Cursor = Cursors.Hand
            };
            btnNo.FlatAppearance.BorderSize = 0;
            Round(btnNo, 18);
            btnNo.Click += (_, _) => confirmDialog.Close();

            confirmDialog.Controls.Add(btnYes);
            confirmDialog.Controls.Add(btnNo);

            // Ajustare la redimensionare
            confirmDialog.Resize += (_, _) =>
            {
                separator.Width = confirmDialog.ClientSize.Width - 48;
                btnYes.Location = new Point(confirmDialog.ClientSize.Width - btnW * 2 - 20, confirmDialog.ClientSize.Height - 60);
                btnNo.Location = new Point(confirmDialog.ClientSize.Width - btnW - 10, confirmDialog.ClientSize.Height - 60);
            };

            confirmDialog.ShowDialog(this);
        }

        private void ToggleTheme()
        {
            _darkMode = !_darkMode;

            // Actualizează toate fundalurile principale
            this.BackColor = C_Bg;
            pnlScroll.BackColor = C_Bg;
            if (pnlHeader != null) pnlHeader.BackColor = C_Bg;
            if (pnlCatW != null) pnlCatW.BackColor = C_Bg;
            if (flpCats != null) flpCats.BackColor = C_Bg;
            if (flpProducts != null) flpProducts.BackColor = C_Bg;
            if (pnlNavbar != null)
                pnlNavbar.BackColor = _darkMode ? Color.FromArgb(8, 18, 48) : Color.FromArgb(255, 235, 220); // portocaliu pentru navbar

            // Actualizează culorile categoriilor (dicționarul pentru cercuri)
            UpdateCategoryColorsForTheme();

            // Actualizează culoarea eroilor (rămâne portocaliu)
            if (pnlHero != null)
            {
                pnlHero.BackColor = C_Orange;
                pnlHero.Refresh(); // forțează redesenarea curbei
            }

            // Actualizează culorile textelor și ale câmpului de căutare
            if (lblGreet != null) lblGreet.ForeColor = C_Muted;
            if (txtSearch != null)
            {
                txtSearch.BackColor = _darkMode ? Color.White : Color.FromArgb(255, 248, 240);
                txtSearch.ForeColor = _darkMode ? Color.Black : Color.FromArgb(60, 60, 70);
            }
            if (lblPop != null) lblPop.ForeColor = _darkMode ? Color.White : Color.Black;
            if (lblCat != null) lblCat.ForeColor = _darkMode ? Color.White : Color.Black;
            // Actualizează culorile categoriilor existente (fără a le recrea)
            UpdateExistingCategoriesColors();

            // Reîncarcă produsele pentru a aplica noile culori cardurilor
            RefreshCards();

            // Actualizează culoarea butoanelor de navigare
            for (int i = 0; i < _navBtns.Length; i++)
                _navBtns[i].ForeColor = i == GetCurrentNavIndex() ? C_Orange : C_Muted;

            // Redesenare forțată
            this.Refresh();
            pnlScroll.Refresh();
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