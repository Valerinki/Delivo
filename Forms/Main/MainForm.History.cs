using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class MainForm
    {
        private void ShowOrderHistory()
        {
            using var dlg = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(22, 32, 64),
                Width = 860,  // redus
                Height = 640, // redus
                ShowInTaskbar = false,
                TopMost = true
            };
            // Colțuri rotunde + bordură
            dlg.Paint += (s, e) =>
            {
                var gp = new GraphicsPath();
                int r = 20;
                var rect = dlg.ClientRectangle;
                gp.AddArc(rect.Left, rect.Top, r * 2, r * 2, 180, 90);
                gp.AddArc(rect.Right - r * 2, rect.Top, r * 2, r * 2, 270, 90);
                gp.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                gp.AddArc(rect.Left, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                gp.CloseFigure();
                dlg.Region = new Region(gp);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(C_Orange, 2);
                e.Graphics.DrawPath(pen, gp);
            };

            // Titlu
            var lblTitle = new Label
            {
                Text = "📋  Istoric comenzi",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(24, 18),
                AutoSize = true
            };
            dlg.Controls.Add(lblTitle);
            var separator = new Panel { BackColor = C_Orange, Height = 2, Width = dlg.ClientSize.Width - 48, Location = new Point(24, 52) };
            dlg.Controls.Add(separator);

            // Panel scrollabil pentru comenzi (redus ca dimensiune)
            var pnlScroll = new Panel
            {
                Location = new Point(24, 68),
                Size = new Size(dlg.ClientSize.Width - 48, dlg.ClientSize.Height - 120), // redus înălțimea
                BackColor = Color.FromArgb(15, 25, 55),
                AutoScroll = true
            };
            dlg.Controls.Add(pnlScroll);

            // Încarcă datele
            var comenzi = DatabaseHelper.GetComenziUtilizatorCuDetalii(LoginForm.UtilizatorLogat);

            if (comenzi.Count == 0)
            {
                var lblEmpty = new Label
                {
                    Text = "Nu ai plasat nicio comandă încă. 🛒",
                    Font = new Font("Segoe UI", 12f),
                    ForeColor = Color.FromArgb(200, 215, 240),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };
                pnlScroll.Controls.Add(lblEmpty);
            }
            else
            {
                int y = 12;
                foreach (var comanda in comenzi)
                {
                    var card = BuildOrderCard(comanda);
                    card.Location = new Point(12, y);
                    pnlScroll.Controls.Add(card);
                    y += card.Height + 16;
                }
            }

            // Buton închidere rotunjit
            var btnClose = new Button
            {
                Text = "Închide",
                FlatStyle = FlatStyle.Flat,
                BackColor = C_Orange,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Size = new Size(90, 32),
                Location = new Point(dlg.ClientSize.Width - 110, dlg.ClientSize.Height - 46),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            Round(btnClose, 16); // rotunjire adăugată
            btnClose.Click += (_, _) => dlg.Close();
            dlg.Controls.Add(btnClose);

            dlg.Resize += (_, _) =>
            {
                separator.Width = dlg.ClientSize.Width - 48;
                pnlScroll.Size = new Size(dlg.ClientSize.Width - 48, dlg.ClientSize.Height - 120);
                btnClose.Location = new Point(dlg.ClientSize.Width - 110, dlg.ClientSize.Height - 46);
            };

            dlg.ShowDialog(this);
        }

        private Panel BuildOrderCard(DatabaseHelper.UserOrder order)
        {
            const int cardWidth = 780;
            const int cardHeight = 180;
            var card = new Panel
            {
                Size = new Size(cardWidth, cardHeight),
                BackColor = Color.FromArgb(28, 38, 68),
                Padding = new Padding(12)
            };
            // Colțuri rotunde
            var path = new GraphicsPath();
            int r = 14;
            path.AddArc(0, 0, r * 2, r * 2, 180, 90);
            path.AddArc(cardWidth - r * 2, 0, r * 2, r * 2, 270, 90);
            path.AddArc(cardWidth - r * 2, cardHeight - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(0, cardHeight - r * 2, r * 2, r * 2, 90, 90);
            card.Region = new Region(path);

            // Header comandă (stânga)
            var lblId = new Label
            {
                Text = $"Comanda #{order.Id}",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = C_Orange,
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblData = new Label
            {
                Text = order.Data.ToString("dd MMM yyyy, HH:mm"),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(170, 190, 220),
                AutoSize = true,
                Location = new Point(12, 34)
            };

            // Preț (dreapta sus)
            var lblTotal = new Label
            {
                Text = $"{order.Total:F0} lei",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(cardWidth - 100, 10),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Produse (lista scurtă, în stânga)
            var produseStr = string.Join(", ", order.Produse.Take(3).Select(p => $"{p.NumeProdus} x{p.Cantitate}"));
            if (order.Produse.Count > 3) produseStr += $"... +{order.Produse.Count - 3}";
            var lblProduse = new Label
            {
                Text = produseStr,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(200, 210, 240),
                AutoSize = true,
                Location = new Point(12, 62),
                MaximumSize = new Size(cardWidth - 200, 40)
            };

            // Adresa (sub produse, în stânga) - POZIȚIA INIȚIALĂ
            var lblAdresa = new Label
            {
                Text = $"📍 {order.Adresa}",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(150, 170, 200),
                AutoSize = true,
                Location = new Point(12, lblProduse.Bottom + 6)
            };

            card.Controls.AddRange(new Control[] { lblId, lblData, lblTotal, lblProduse, lblAdresa });

            // Axa (doar dacă nu e livrată)
            bool isLivrata = order.Status.Equals("Livrată", StringComparison.OrdinalIgnoreCase);
            if (!isLivrata)
            {
                var steps = new[] { "În așteptare", "În preparare", "În livrare", "Livrată" };
                int currentStep = Array.IndexOf(steps, order.Status);
                if (currentStep < 0) currentStep = 0;

                int stepWidth = (cardWidth - 80) / 4;
                int startX = 45;
                int yAxis = 145; // poziționare mai sus pentru a evita suprapunerea

                card.Paint += (sender, e) =>
                {
                    var g = e.Graphics;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var penGray = new Pen(Color.FromArgb(80, 255, 255, 255), 2))
                        g.DrawLine(penGray, startX, yAxis + 10, startX + stepWidth * 4, yAxis + 10);
                    using (var penOrange = new Pen(C_Orange, 2))
                        g.DrawLine(penOrange, startX, yAxis + 10, startX + stepWidth * (currentStep + 1), yAxis + 10);
                    for (int i = 0; i < steps.Length; i++)
                    {
                        int x = startX + stepWidth * i;
                        bool isActive = i <= currentStep;
                        Color fill = isActive ? C_Orange : Color.FromArgb(100, 100, 130);
                        using (var brush = new SolidBrush(fill))
                            g.FillEllipse(brush, x - 6, yAxis + 2, 12, 12);
                        using (var penBorder = new Pen(Color.White, 1))
                            g.DrawEllipse(penBorder, x - 6, yAxis + 2, 12, 12);
                        var label = steps[i];
                        var size = g.MeasureString(label, new Font("Segoe UI", 7.5f));
                        g.DrawString(label, new Font("Segoe UI", 7.5f), new SolidBrush(isActive ? C_Orange : Color.FromArgb(150, 170, 200)),
                            x - (int)(size.Width / 2), yAxis - 14);
                    }
                };
            }
            else
            {
                var lblLivrat = new Label
                {
                    Text = "✅ Livrată",
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(80, 200, 80),
                    AutoSize = true,
                    Location = new Point(12, 110)
                };
                card.Controls.Add(lblLivrat);
            }

            // Buton Detalii (rotunjit)
            var btnDetalii = new Button
            {
                Text = "🔍 Detalii",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 55, 85),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f),
                Size = new Size(90, 28),
                Location = new Point(cardWidth - 110, cardHeight - 38),
                Cursor = Cursors.Hand
            };
            btnDetalii.FlatAppearance.BorderSize = 0;
            Round(btnDetalii, 14);
            btnDetalii.Click += (_, _) => ShowOrderDetails(order);
            card.Controls.Add(btnDetalii);

            return card;
        }

        private void ShowOrderDetails(DatabaseHelper.UserOrder order)
        {
            using var dlg = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(22, 32, 64),
                Width = 550,
                Height = 460,
                ShowInTaskbar = false,
                TopMost = true
            };
            // Colțuri rotunde + bordură
            dlg.Paint += (s, e) =>
            {
                var gp = new GraphicsPath();
                int r = 20;
                var rect = dlg.ClientRectangle;
                gp.AddArc(rect.Left, rect.Top, r * 2, r * 2, 180, 90);
                gp.AddArc(rect.Right - r * 2, rect.Top, r * 2, r * 2, 270, 90);
                gp.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                gp.AddArc(rect.Left, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                gp.CloseFigure();
                dlg.Region = new Region(gp);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(C_Orange, 2);
                e.Graphics.DrawPath(pen, gp);
            };

            var lblTitle = new Label
            {
                Text = $"Comanda #{order.Id}",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(24, 18),
                AutoSize = true
            };
            dlg.Controls.Add(lblTitle);

            var lblData = new Label
            {
                Text = order.Data.ToString("dd MMM yyyy, HH:mm"),
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(200, 215, 240),
                Location = new Point(24, 48),
                AutoSize = true
            };
            dlg.Controls.Add(lblData);

            var lblStatus = new Label
            {
                Text = $"Status: {order.Status}",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = order.Status == "Livrată" ? Color.FromArgb(80, 200, 80) : C_Orange,
                Location = new Point(24, 78),
                AutoSize = true
            };
            dlg.Controls.Add(lblStatus);

            var lblProd = new Label
            {
                Text = "Produse comandate:",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(24, 110),
                AutoSize = true
            };
            dlg.Controls.Add(lblProd);

            var panelProd = new Panel
            {
                Location = new Point(24, 136),
                Size = new Size(dlg.ClientSize.Width - 48, 180),
                BackColor = Color.FromArgb(15, 25, 55),
                AutoScroll = true
            };
            int y = 6;
            foreach (var prod in order.Produse)
            {
                var lbl = new Label
                {
                    Text = $"• {prod.NumeProdus}  x{prod.Cantitate}  =  {prod.PretUnitar * prod.Cantitate:F0} lei",
                    Font = new Font("Segoe UI", 9f),
                    ForeColor = Color.FromArgb(210, 220, 245),
                    AutoSize = true,
                    Location = new Point(12, y)
                };
                panelProd.Controls.Add(lbl);
                y += 28;
            }
            dlg.Controls.Add(panelProd);

            var lblAdresa = new Label
            {
                Text = $"📍 Adresă livrare: {order.Adresa}",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(180, 200, 230),
                AutoSize = true,
                Location = new Point(24, panelProd.Bottom + 12)
            };
            dlg.Controls.Add(lblAdresa);

            var btnClose = new Button
            {
                Text = "Închide",
                FlatStyle = FlatStyle.Flat,
                BackColor = C_Orange,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Size = new Size(90, 32),
                Location = new Point(dlg.ClientSize.Width - 110, dlg.ClientSize.Height - 46),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            Round(btnClose, 16);
            btnClose.Click += (_, _) => dlg.Close();
            dlg.Controls.Add(btnClose);

            dlg.Resize += (_, _) =>
            {
                btnClose.Location = new Point(dlg.ClientSize.Width - 110, dlg.ClientSize.Height - 46);
                panelProd.Size = new Size(dlg.ClientSize.Width - 48, dlg.ClientSize.Height - 280);
            };

            dlg.ShowDialog(this);
        }
    }
}