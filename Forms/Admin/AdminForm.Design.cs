using System;
using System.Drawing;
using System.Windows.Forms;

namespace Delivo.Forms
{
    public partial class AdminForm
    {
        private void BuildUI()
        {
            Text = "Delivo Admin Pro — Food Delivery";
            MinimumSize = new Size(1280, 760);
            BackColor = ColorAlbastruInchis;
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Font = UiFont(11f);
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);
            UpdateStyles();

            BuildHeader();
            BuildSidebar();
            BuildContentArea();

            Controls.Add(pnlContent);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlHeader);
        }

        private void BuildHeader()
        {
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,                          // ✅ mai compact
                BackColor = ColorHeaderBg,
                Padding = new Padding(24, 0, 24, 0)
            };
            EnableDoubleBuffer(pnlHeader);

            // ── STÂNGA: Logo ──────────────────────────────────────
            var left = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = Color.Transparent
            };

            var lblLogo = new Label
            {
                Text = "DELIVO",
                ForeColor = ColorPortocaliu,
                Font = UiFont(26f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 14),          // ✅ centrat vertical
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text = "ADMIN PANEL",
                ForeColor = ColorTextSecundar,
                Font = UiFont(8.5f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(3, 50),          // ✅ sub logo, nu peste
                BackColor = Color.Transparent
            };

            left.Controls.Add(lblLogo);
            left.Controls.Add(lblSub);

            // ── DREAPTA: Admin card + Status ─────────────────────
            var right = new Panel
            {
                Dock = DockStyle.Right,
                Width = 370,                          // ✅ mai lat → aproape de margine
                BackColor = Color.Transparent
            };

            var adminCard = new Panel
            {
                Size = new Size(210, 56),
                BackColor = ColorAlbastruCard,
                Location = new Point(0, 12)           // ✅ aliniat sus
            };
            ApplyRoundedRegion(adminCard, 12);
            adminCard.Resize += (_, _) => ApplyRoundedRegion(adminCard, 12);

            var avatar = new Panel
            {
                Size = new Size(40, 40),
                Location = new Point(10, 8),
                BackColor = ColorPortocaliu
            };
            ApplyRoundedRegion(avatar, 20);
            avatar.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var f = UiFont(15f, FontStyle.Bold);
                var sz = e.Graphics.MeasureString("A", f);
                e.Graphics.DrawString("A", f, Brushes.White,
                    18 - sz.Width / 2, 18 - sz.Height / 2);
            };

            var lblName = new Label
            {
                Text = "admin",
                ForeColor = ColorAlb,
                Font = UiFont(10.5f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(60, 10),
                BackColor = Color.Transparent
            };

            var lblRole = new Label
            {
                Text = "Administrator",
                ForeColor = ColorTextSecundar,
                Font = UiFont(8.5f),
                AutoSize = true,
                Location = new Point(60, 32),
                BackColor = Color.Transparent
            };

            adminCard.Controls.Add(avatar);
            adminCard.Controls.Add(lblName);
            adminCard.Controls.Add(lblRole);

            var statusBadge = new Panel
            {
                Size = new Size(148, 36),
                Location = new Point(222, 22),        // ✅ aliniat vertical centrat
                BackColor = Color.FromArgb(255, 18, 38, 32)
            };
            ApplyRoundedRegion(statusBadge, 10);
            statusBadge.Resize += (_, _) => ApplyRoundedRegion(statusBadge, 10);

            var dot = new Panel
            {
                Size = new Size(9, 9),
                Location = new Point(12, 14),
                BackColor = ColorAccentGreen
            };
            ApplyRoundedRegion(dot, 5);

            var lblStatus = new Label
            {
                Text = "Sistem activ",
                ForeColor = ColorAccentGreen,
                Font = UiFont(9f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(28, 9),
                BackColor = Color.Transparent
            };

            statusBadge.Controls.Add(dot);
            statusBadge.Controls.Add(lblStatus);

            right.Controls.Add(adminCard);
            right.Controls.Add(statusBadge);

            pnlHeader.Controls.Add(right);
            pnlHeader.Controls.Add(left);

            pnlHeader.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.FromArgb(40, 255, 107, 0), 1);
                e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1,
                    pnlHeader.Width, pnlHeader.Height - 1);
            };
        }

        private void BuildSidebar()
        {
            pnlSidebar = new Panel
            {
                Width = 230,                          // ✅ sidebar mai compact
                Dock = DockStyle.Left,
                BackColor = ColorSidebar,
                Padding = new Padding(0, 8, 0, 8)
            };
            EnableDoubleBuffer(pnlSidebar);

            // ── Profil ────────────────────────────────────────────
            var profile = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(16, 12, 16, 8),
                BackColor = Color.Transparent
            };

            var pAvatar = new Panel
            {
                Size = new Size(48, 48),
                Location = new Point(16, 20),
                BackColor = ColorPortocaliu
            };
            ApplyRoundedRegion(pAvatar, 24);
            pAvatar.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var f = UiFont(16f, FontStyle.Bold);
                var sz = e.Graphics.MeasureString("A", f);
                e.Graphics.DrawString("A", f, Brushes.White,
                    24 - sz.Width / 2, 24 - sz.Height / 2);
            };

            var lblPName = new Label
            {
                Text = "Administrator",
                ForeColor = ColorAlb,
                Font = UiFont(11f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(74, 24),
                BackColor = Color.Transparent
            };

            var lblPEmail = new Label
            {
                Text = "admin@delivo.md",
                ForeColor = ColorTextSecundar,
                Font = UiFont(8.5f),
                AutoSize = true,
                Location = new Point(74, 48),
                BackColor = Color.Transparent
            };

            profile.Controls.Add(pAvatar);
            profile.Controls.Add(lblPName);
            profile.Controls.Add(lblPEmail);

            var sep = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(35, 255, 255, 255)
            };

            // ── Nav buttons ───────────────────────────────────────
            var navHost = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 12, 12, 8),
                BackColor = Color.Transparent
            };

            // ✅ Iconițe clare și vizibile (text Unicode simplu)
            string[] menuItems = { "Dashboard", "Produse", "Comenzi", "Utilizatori" };
            string[] icons = { "🏠", "🍕", "📦", "👥" };

            _navButtons.Clear();
            int y = 8;
            for (int i = 0; i < menuItems.Length; i++)
            {
                int idx = i;
                var btn = BuildSidebarNavButton(icons[i], menuItems[i], y);
                y += 60;
                btn.Click += (_, _) =>
                {
                    SetActiveNavButton(btn);
                    NavigateToSection(idx);
                };
                navHost.Controls.Add(btn);
                _navButtons.Add(btn);
            }

            if (_navButtons.Count > 0)
                SetActiveNavButton(_navButtons[0]);

            var btnLogout = BuildLogoutButton();

            pnlSidebar.Controls.Add(btnLogout);
            pnlSidebar.Controls.Add(navHost);
            pnlSidebar.Controls.Add(sep);
            pnlSidebar.Controls.Add(profile);
        }

        private void BuildContentArea()
        {
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorAlbastruInchis,
                Padding = new Padding(24, 20, 24, 20),
                AutoScroll = true
            };
            EnableDoubleBuffer(pnlContent);
        }

        private Button BuildSidebarNavButton(string icon, string label, int y)
        {
            var btn = new Button
            {
                Size = new Size(216, 52),
                Location = new Point(0, y),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Transparent,
                Font = UiFont(11f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Text = $"{icon}  {label}",
                UseVisualStyleBackColor = false,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;

            btn.Paint += (_, e) =>
            {
                // ✅ Șterge complet fundalul — previne dublarea textului
                e.Graphics.Clear(ColorSidebar);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                bool isActive = btn == currentNavButton;
                bool isHover = btn.ClientRectangle.Contains(btn.PointToClient(Cursor.Position));

                Color bg = isActive
                    ? ColorPortocaliu
                    : isHover
                        ? Color.FromArgb(40, 255, 255, 255)
                        : Color.Transparent;

                if (bg != Color.Transparent)
                {
                    using var brush = new SolidBrush(bg);
                    var rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
                    int r = 12;
                    using var path = new System.Drawing.Drawing2D.GraphicsPath();
                    path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
                    path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
                    path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                    path.CloseFigure();
                    e.Graphics.FillPath(brush, path);
                }

                Color textColor = isActive ? Color.White : ColorTextSecundar;
                TextRenderer.DrawText(
                    e.Graphics,
                    btn.Text,
                    btn.Font,
                    btn.ClientRectangle,
                    textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            };

            btn.MouseEnter += (_, _) => btn.Invalidate();
            btn.MouseLeave += (_, _) => btn.Invalidate();

            return btn;
        }

        private Button BuildLogoutButton()
        {
            // ✅ Același padding ca navHost (12px stânga și dreapta)
            var btnLogout = new Button
            {
                Text = "  ⏻  Deconectare",
                Height = 52,
                Dock = DockStyle.Bottom,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(255, 150, 150),
                Font = UiFont(11f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorDangerBg,
                Margin = new Padding(16, 0, 12, 16),  // ✅ margini egale
                Padding = new Padding(12, 0, 0, 0)
            };
            btnLogout.FlatAppearance.BorderColor = Color.FromArgb(80, 255, 80, 80);
            btnLogout.FlatAppearance.BorderSize = 1;

            var hover = Color.FromArgb(55, 220, 60, 60);
            btnLogout.MouseEnter += (_, _) => btnLogout.BackColor = hover;
            btnLogout.MouseLeave += (_, _) => btnLogout.BackColor = ColorDangerBg;

            ApplyRoundedRegion(btnLogout, 12);
            btnLogout.Resize += (_, _) => ApplyRoundedRegion(btnLogout, 12);

            btnLogout.Click += (_, _) =>
            {
                Hide();
                new LoginForm().Show();
            };

            return btnLogout;
        }
    }
}