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
                Height = 104,
                BackColor = ColorHeaderBg,
                Padding = new Padding(32, 0, 32, 0)
            };
            EnableDoubleBuffer(pnlHeader);

            var left = new Panel
            {
                Dock = DockStyle.Left,
                Width = 360,
                BackColor = Color.Transparent
            };

            var lblLogo = new Label
            {
                Text = "DELIVO",
                ForeColor = ColorPortocaliu,
                Font = UiFont(28f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 18),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text = "ADMIN PANEL",
                ForeColor = ColorTextSecundar,
                Font = UiFont(9.5f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(2, 60),
                BackColor = Color.Transparent
            };

            left.Controls.Add(lblLogo);
            left.Controls.Add(lblSub);

            var right = new Panel
            {
                Dock = DockStyle.Right,
                Width = 460,
                BackColor = Color.Transparent
            };

            var adminCard = new Panel
            {
                Size = new Size(218, 62),
                BackColor = ColorAlbastruCard,
                Location = new Point(0, 21)
            };
            ApplyRoundedRegion(adminCard, 14);
            adminCard.Resize += (_, _) => ApplyRoundedRegion(adminCard, 14);

            var avatar = new Panel
            {
                Size = new Size(44, 44),
                Location = new Point(12, 9),
                BackColor = ColorPortocaliu
            };
                ApplyRoundedRegion(avatar, 22);
            avatar.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var f = UiFont(16f, FontStyle.Bold);
                var sz = e.Graphics.MeasureString("A", f);
                e.Graphics.DrawString("A", f, Brushes.White,
                    20 - sz.Width / 2, 20 - sz.Height / 2);
            };

            var lblName = new Label
            {
                Text = "admin",
                ForeColor = ColorAlb,
                Font = UiFont(11f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(68, 14),
                BackColor = Color.Transparent
            };

            var lblRole = new Label
            {
                Text = "Administrator",
                ForeColor = ColorTextSecundar,
                Font = UiFont(9f),
                AutoSize = true,
                Location = new Point(68, 36),
                BackColor = Color.Transparent
            };

            adminCard.Controls.Add(avatar);
            adminCard.Controls.Add(lblName);
            adminCard.Controls.Add(lblRole);

            var statusBadge = new Panel
            {
                Size = new Size(158, 40),
                Location = new Point(232, 32),
                BackColor = Color.FromArgb(255, 18, 38, 32)
            };
            ApplyRoundedRegion(statusBadge, 10);
            statusBadge.Resize += (_, _) => ApplyRoundedRegion(statusBadge, 10);

            var dot = new Panel
            {
                Size = new Size(10, 10),
                Location = new Point(14, 15),
                BackColor = ColorAccentGreen
            };
            ApplyRoundedRegion(dot, 5);

            var lblStatus = new Label
            {
                Text = "Sistem activ",
                ForeColor = ColorAccentGreen,
                Font = UiFont(9.5f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(32, 11),
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
                e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            };
        }

        private void BuildSidebar()
        {
            pnlSidebar = new Panel
            {
                Width = 304,
                Dock = DockStyle.Left,
                BackColor = ColorSidebar,
                Padding = new Padding(0, 12, 0, 12)
            };
            EnableDoubleBuffer(pnlSidebar);

            var profile = new Panel
            {
                Dock = DockStyle.Top,
                Height = 116,
                Padding = new Padding(22, 18, 22, 10),
                BackColor = Color.Transparent
            };

            var pAvatar = new Panel
            {
                Size = new Size(54, 54),
                Location = new Point(22, 24),
                BackColor = ColorPortocaliu
            };
            ApplyRoundedRegion(pAvatar, 27);
            pAvatar.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var f = UiFont(18f, FontStyle.Bold);
                var sz = e.Graphics.MeasureString("A", f);
                e.Graphics.DrawString("A", f, Brushes.White, 27 - sz.Width / 2, 27 - sz.Height / 2);
            };

            var lblPName = new Label
            {
                Text = "Administrator",
                ForeColor = ColorAlb,
                Font = UiFont(12f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(90, 27),
                BackColor = Color.Transparent
            };

            var lblPEmail = new Label
            {
                Text = "admin@delivo.md",
                ForeColor = ColorTextSecundar,
                Font = UiFont(9f),
                AutoSize = true,
                Location = new Point(90, 52),
                BackColor = Color.Transparent
            };

            profile.Controls.Add(pAvatar);
            profile.Controls.Add(lblPName);
            profile.Controls.Add(lblPEmail);

            var sep = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(35, 255, 255, 255),
                Margin = new Padding(20, 0, 20, 0)
            };

            var navHost = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 16, 14, 8),
                BackColor = Color.Transparent
            };

            string[] menuItems = { "Dashboard", "Produse", "Comenzi", "Utilizatori" };
            string[] icons = { "⌂", "▦", "□", "◉" };

            _navButtons.Clear();
            int y = 8;
            for (int i = 0; i < menuItems.Length; i++)
            {
                int idx = i;
                var btn = BuildSidebarNavButton(icons[i], menuItems[i], y);
                y += 64;
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
                Padding = new Padding(28, 24, 28, 28),
                AutoScroll = true
            };
            EnableDoubleBuffer(pnlContent);
        }

        private Button BuildSidebarNavButton(string icon, string label, int y)
        {
            var btn = new Button
            {
                Text = $"   {icon}     {label}",
                Size = new Size(264, 56),
                Location = new Point(6, y),
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorTextSecundar,
                Font = UiFont(11.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(50, 0, 12, 0),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            btn.FlatAppearance.BorderSize = 0;

            var hoverBg = Color.FromArgb(26, 255, 255, 255);
            btn.MouseEnter += (_, _) =>
            {
                if (btn != currentNavButton)
                    btn.BackColor = hoverBg;
            };
            btn.MouseLeave += (_, _) =>
            {
                if (btn != currentNavButton)
                    btn.BackColor = Color.Transparent;
            };

            ApplyRoundedRegion(btn, 14);
            btn.Resize += (_, _) => ApplyRoundedRegion(btn, 14);
            return btn;
        }

        private Button BuildLogoutButton()
        {
            var btnLogout = new Button
            {
                Text = "   ⏻   Deconectare",
                Height = 58,
                Dock = DockStyle.Bottom,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(255, 150, 150),
                Font = UiFont(11f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(20, 0, 20, 18),
                BackColor = ColorDangerBg
            };
            btnLogout.FlatAppearance.BorderColor = Color.FromArgb(80, 255, 80, 80);
            btnLogout.FlatAppearance.BorderSize = 1;

            var hover = Color.FromArgb(55, 220, 60, 60);
            btnLogout.MouseEnter += (_, _) => btnLogout.BackColor = hover;
            btnLogout.MouseLeave += (_, _) => btnLogout.BackColor = ColorDangerBg;

            ApplyRoundedRegion(btnLogout, 14);
            btnLogout.Resize += (_, _) => ApplyRoundedRegion(btnLogout, 14);

            btnLogout.Click += (_, _) =>
            {
                Hide();
                new LoginForm().Show();
            };

            return btnLogout;
        }
    }
}
