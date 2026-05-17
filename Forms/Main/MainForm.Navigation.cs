using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Delivo.Forms
{
    public partial class MainForm
    {
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

        private void OpenMenu(object? sender, EventArgs e)
        {
            if (_menuPanel != null && Controls.Contains(_menuPanel))
            {
                Controls.Remove(_menuPanel);
                _menuPanel = null;
                return;
            }

            int mw = 320;
            _menuPanel = new Panel
            {
                Width = mw,
                Height = ClientSize.Height - pnlNavbar.Height,
                Location = new Point(ClientSize.Width - mw, 0),
                BackColor = Color.FromArgb(245, 10, 22, 54),
            };
            _menuPanel.Paint += (s, pe) => {
                using var pen = new Pen(Color.FromArgb(120, 255, 107, 0), 2);
                pe.Graphics.DrawLine(pen, 0, 0, 0, _menuPanel.Height);
                using var lgb = new LinearGradientBrush(new Point(0, 0), new Point(mw, 0),
                    Color.FromArgb(250, 10, 22, 54), Color.FromArgb(250, 18, 32, 70));
                pe.Graphics.FillRectangle(lgb, 1, 0, mw - 1, _menuPanel.Height);
            };

            var lblMLogo = new Label
            {
                Text = "DELIVO",
                Font = F_Logo,
                ForeColor = C_Orange,
                AutoSize = true,
                Location = new Point(20, 20),
                BackColor = Color.Transparent
            };

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

            var sep = new Panel { Height = 1, Width = mw - 40, Location = new Point(20, 68), BackColor = Color.FromArgb(40, 255, 107, 0) };

            int my = 90;
            var items = new[] { ("🔍", "Caută"), ("📋", "Comenzile mele"), ("", "---"), ("🌙", "Schimbă tema") };
            foreach (var (ico, txt) in items)
            {
                if (txt == "---")
                {
                    var div = new Panel { Height = 1, Width = mw - 40, Location = new Point(20, my), BackColor = Color.FromArgb(30, 255, 255, 255) };
                    _menuPanel.Controls.Add(div);
                    my += 16;
                    continue;
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
                var lTxt = new Label { Text = txt, Font = new Font(F_Normal.FontFamily, 11, FontStyle.Bold), ForeColor = C_Text, AutoSize = true, Location = new Point(50, 16), BackColor = Color.Transparent };
                var lArr = new Label { Text = "›", Font = new Font("Segoe UI", 16), ForeColor = C_Muted, AutoSize = true, Location = new Point(mw - 80, 12), BackColor = Color.Transparent };
                row.Controls.Add(lIco);
                row.Controls.Add(lTxt);
                row.Controls.Add(lArr);
                row.MouseEnter += (s, ev) => row.BackColor = Color.FromArgb(40, 255, 107, 0);
                row.MouseLeave += (s, ev) => row.BackColor = Color.FromArgb(20, 255, 255, 255);
                string t2 = txt;
                row.Click += (s, ev) =>
                {
                    Controls.Remove(_menuPanel);
                    _menuPanel = null;
                    if (t2 == "Caută") txtSearch.Focus();
                    else if (t2 == "Comenzile mele") new OrdersForm().ShowDialog();
                    else if (t2 == "Schimbă tema") { _darkMode = !_darkMode; BackColor = C_Bg; pnlScroll.BackColor = C_Bg; RefreshCards(); LoadCats(); }
                };
                _menuPanel.Controls.Add(row);
                my += 62;
            }

            _menuPanel.Controls.Add(lblMLogo);
            _menuPanel.Controls.Add(btnX);
            _menuPanel.Controls.Add(sep);
            Controls.Add(_menuPanel);
            _menuPanel.BringToFront();
        }
    }
}