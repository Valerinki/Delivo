using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json;
using System.Windows.Forms;
using System.Linq;
using System.IO;

namespace Delivo.Forms
{
    public partial class MainForm
    {
        private void InitFonts()
        {
            string fn = "Segoe UI";
            F_Logo = new Font("Showcard Gothic", 24, FontStyle.Bold);
            F_H1 = new Font(fn, 26, FontStyle.Bold);
            F_Bold = new Font(fn, 12, FontStyle.Bold);
            F_Normal = new Font(fn, 11, FontStyle.Regular);
            F_Small = new Font(fn, 10, FontStyle.Regular);
            F_Tiny = new Font(fn, 9, FontStyle.Regular);
        }
        private Button CreateRoundButton(string text, Color bgColor, Color fgColor, int width, int height)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = fgColor,
                BackColor = bgColor,
                FlatStyle = FlatStyle.Flat,
                Width = width,
                Height = height,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            // Colțuri rotunde
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = 20;
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
            btn.Region = new Region(path);
            return btn;
        }

        private Button CreateRoundButton(string text, int width, int height)
        {
            return CreateRoundButton(text, Color.FromArgb(35, 45, 75), Color.White, width, height);
        }

        private void Round(Control c, int r)
        {
            if (c.Width < 1 || c.Height < 1) return;
            var p = new GraphicsPath();
            p.AddArc(0, 0, r * 2, r * 2, 180, 90);
            p.AddArc(c.Width - r * 2, 0, r * 2, r * 2, 270, 90);
            p.AddArc(c.Width - r * 2, c.Height - r * 2, r * 2, r * 2, 0, 90);
            p.AddArc(0, c.Height - r * 2, r * 2, r * 2, 90, 90);
            p.CloseFigure();
            c.Region = new Region(p);
        }
        private static string CartFilePath => Path.Combine(Application.UserAppDataPath, "cart.json");

        private void SaveCartToFile()
        {
            try
            {
                Directory.CreateDirectory(Application.UserAppDataPath);
                var toSave = _cart.Select(c => new CartItem { Id = c.Id, Nume = c.Nume, Pret = c.Pret, Qty = c.Qty }).ToList();
                var json = JsonSerializer.Serialize(toSave);
                File.WriteAllText(CartFilePath, json);
            }
            catch { }
        }

        private void LoadCartFromFile()
        {
            try
            {
                if (File.Exists(CartFilePath))
                {
                    var json = File.ReadAllText(CartFilePath);
                    var loaded = JsonSerializer.Deserialize<List<CartItem>>(json);
                    if (loaded != null)
                    {
                        _cart.Clear();
                        foreach (var item in loaded)
                            _cart.Add((item.Id, item.Nume, item.Pret, item.Qty));
                    }
                }
            }
            catch { _cart = new List<(int, string, decimal, int)>(); }
        }
        private void UpdateCartButton()
        {
            _navBtns[2].Text = _cart.Count == 0 ? "\U0001f6d2\nCoș" : $"🛒\nCoș({_cart.Count})";
            _navBtns[2].ForeColor = _cart.Count == 0 ? C_Muted : C_Orange;
        }
    }
}