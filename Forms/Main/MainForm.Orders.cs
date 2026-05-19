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
        // ── ADAUGĂ ÎN COȘ ─────────────────────────────────────────────────
        private void AddCart(int id, string nume, decimal pret, string categorie = "")
        {
            int i = _cart.FindIndex(x => x.Id == id);
            if (i >= 0)
            {
                var t = _cart[i];
                _cart[i] = (t.Id, t.Nume, t.Pret, t.Qty + 1);
            }
            else
                _cart.Add((id, nume, pret, 1));

            decimal total = _cart.Sum(x => x.Pret * x.Qty);
            _navBtns[2].Text = $"🛒\nCoș({_cart.Count})";
            _navBtns[2].ForeColor = C_Orange;

            if (string.IsNullOrEmpty(categorie))
            {
                var produs = _all.FirstOrDefault(p => p.Id == id);
                if (produs.Id != 0) categorie = produs.Categorie;
            }
            SaveCartToFile();
            ShowProductAddedPopup(nume, categorie, pret, total);
        }

        // ── ARATĂ COȘUL ─────────────────────────────────────────────────
        private void ShowCart()
        {
            if (_cart.Count == 0)
            {
                // Popup personalizat pentru coș gol, mai vizibil
                // Popup personalizat pentru coș gol
                // Popup personalizat pentru coș gol
                using var dlg = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor = Color.FromArgb(22, 32, 64),
                    Width = 380,
                    Height = 300,
                    ShowInTaskbar = false,
                    TopMost = true
                };

                // Colțuri rotunde + bordură portocalie
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

                // Iconiță coș (centrată orizontal, ușor mai sus)
                var lblIcon = new Label
                {
                    Text = "🛒",
                    Font = new Font("Segoe UI Emoji", 44),
                    ForeColor = C_Orange,
                    AutoSize = true,
                    Location = new Point((dlg.ClientSize.Width - 130) / 2, 40),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                dlg.Controls.Add(lblIcon);

                // Mesaj principal (imediat sub iconiță)
                var lblMsg = new Label
                {
                    Text = "Coșul este gol!",
                    Font = new Font("Poppins", 16, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point((dlg.ClientSize.Width - 180) / 2, 170),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                dlg.Controls.Add(lblMsg);

                // Subtitlu
                var lblSub = new Label
                {
                    Text = "Adaugă produse pentru a plasa o comandă.",
                    Font = new Font("Poppins", 10),
                    ForeColor = Color.FromArgb(200, 215, 240),
                    AutoSize = true,
                    Location = new Point((dlg.ClientSize.Width - 335) / 2, 210),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                dlg.Controls.Add(lblSub);

                // Buton OK (rotunjit, centrat)
                var btnOk = new Button
                {
                    Text = "OK",
                    FlatStyle = FlatStyle.Flat,
                    BackColor = C_Orange,
                    ForeColor = Color.White,
                    Font = new Font("Poppins", 10, FontStyle.Bold),
                    Size = new Size(100, 36),
                    Location = new Point((dlg.ClientSize.Width - 100) / 2, 250),
                    Cursor = Cursors.Hand
                };
                btnOk.FlatAppearance.BorderSize = 0;
                btnOk.Click += (_, _) => dlg.Close();

                // Rotunjim butonul
                var path = new GraphicsPath();
                int radius = 18;
                path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
                path.AddArc(btnOk.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
                path.AddArc(btnOk.Width - radius * 2, btnOk.Height - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(0, btnOk.Height - radius * 2, radius * 2, radius * 2, 90, 90);
                btnOk.Region = new Region(path);

                dlg.Controls.Add(btnOk);
                dlg.ShowDialog(this);
                return;
            }

            using var dialog = BuildOrderDialog();
            dialog.ShowDialog(this);
        }

        private Form BuildOrderDialog()
        {
            var workingCart = _cart.Select(item => new CartItem
            {
                Id = item.Id,
                Nume = item.Nume,
                Pret = item.Pret,
                Qty = item.Qty
            }).ToList();

            const int dialogWidth = 680;
            const int padding = 24;

            var dlg = MakePopupForm(dialogWidth, "🛒  Coșul tău");
            dlg.ClientSize = new Size(dialogWidth, 600);

            var pnlItems = new Panel
            {
                Location = new Point(padding, 74),
                Size = new Size(dialogWidth - padding * 2, 280),
                BackColor = Color.FromArgb(15, 25, 55),
                AutoScroll = true,
                BorderStyle = BorderStyle.None
            };

            void RenderItems()
            {
                pnlItems.Controls.Clear();
                int y = 8;
                foreach (var item in workingCart)
                {
                    var chk = new CheckBox { Checked = true, Size = new Size(24, 24), Location = new Point(8, y + 4), Tag = item };
                    var lblName = new Label { Text = item.Nume, Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(40, y + 6) };
                    var btnMinus = CreateRoundButton("-", 32, 32);
                    btnMinus.Location = new Point(380, y);
                    btnMinus.Click += (s, e) =>
                    {
                        if (item.Qty > 1) item.Qty--;
                        else workingCart.Remove(item);
                        RenderItems();
                        UpdateTotal();
                    };
                    var lblQty = new Label { Text = item.Qty.ToString(), Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = C_Orange, AutoSize = true, Location = new Point(420, y + 6), TextAlign = ContentAlignment.MiddleCenter, MinimumSize = new Size(30, 0) };
                    var btnPlus = CreateRoundButton("+", 32, 32);
                    btnPlus.Location = new Point(460, y);
                    btnPlus.Click += (s, e) => { item.Qty++; RenderItems(); UpdateTotal(); };
                    var lblPrice = new Label { Text = $"{item.Pret * item.Qty:F0} lei", Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = C_Orange, AutoSize = true, Location = new Point(520, y + 6), TextAlign = ContentAlignment.MiddleRight };
                    pnlItems.Controls.AddRange(new Control[] { chk, lblName, btnMinus, lblQty, btnPlus, lblPrice });
                    y += 48;
                }
                pnlItems.Height = Math.Max(200, y + 10);
            }

            var lblTotal = new Label
            {
                Text = "💰 TOTAL: 0 lei",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(13, 27, 62),
                AutoSize = false,
                Size = new Size(dialogWidth - padding * 2, 48),
                Location = new Point(padding, pnlItems.Bottom + 12),
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 12, 0)
            };

            var lblAddress = new Label
            {
                Text = "📍 Adresă livrare:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 215, 240),
                Location = new Point(padding, lblTotal.Bottom + 16),
                AutoSize = true
            };

            var txtAddress = new TextBox
            {
                Location = new Point(padding, lblAddress.Bottom + 6),
                Size = new Size(dialogWidth - padding * 2, 38),
                Font = new Font("Segoe UI", 11f),
                BackColor = Color.FromArgb(13, 27, 62),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Ex: Strada Principală, nr. 10, bloc A, scara 2, etaj 3"
            };

            void UpdateTotal()
            {
                decimal total = 0;
                foreach (Control ctrl in pnlItems.Controls)
                    if (ctrl is CheckBox chk && chk.Checked && chk.Tag is CartItem item)
                        total += item.Pret * item.Qty;
                lblTotal.Text = $"💰 TOTAL: {total:F0} lei";
            }

            void AttachCheckboxEvents()
            {
                foreach (Control ctrl in pnlItems.Controls)
                    if (ctrl is CheckBox chk)
                        chk.CheckedChanged += (_, _) => UpdateTotal();
            }

            RenderItems();
            AttachCheckboxEvents();
            UpdateTotal();

            int btnY = txtAddress.Bottom + 32;
            var btnPlace = CreateRoundButton("✅  Plasează comanda", C_Orange, Color.White, 260, 44);
            var btnCancel = CreateRoundButton("✖  Anulează", Color.FromArgb(45, 55, 85), Color.FromArgb(200, 215, 240), 120, 44);
            btnPlace.Location = new Point((dialogWidth - 260) / 2, btnY);
            btnCancel.Location = new Point(dialogWidth - 140, btnY);

            btnPlace.Click += (_, _) =>
            {
                string address = txtAddress.Text.Trim();
                if (string.IsNullOrWhiteSpace(address))
                {
                    ShowPopup("⚠️  Te rugăm să introduci adresa de livrare.", isToast: true);
                    return;
                }
                var selectedItems = workingCart
                    .Where(item => pnlItems.Controls.OfType<CheckBox>().Any(chk => chk.Tag == item && chk.Checked))
                    .Select(item => (item.Id, item.Nume, item.Pret, item.Qty))
                    .ToList();
                if (selectedItems.Count == 0)
                {
                    ShowPopup("Nu ai selectat niciun produs pentru comandă.", isToast: true);
                    return;
                }
                decimal totalSelected = selectedItems.Sum(i => i.Pret * i.Qty);
                dlg.Close();
                _cart = workingCart.Where(item => item.Qty > 0).Select(item => (item.Id, item.Nume, item.Pret, item.Qty)).ToList();
                PlaceOrderSelected(selectedItems, totalSelected, address);
            };
            btnCancel.Click += (_, _) => dlg.Close();

            dlg.Controls.AddRange(new Control[] { pnlItems, lblTotal, lblAddress, txtAddress, btnPlace, btnCancel });
            dlg.ClientSize = new Size(dialogWidth, btnY + 90);
            dlg.FormClosing += (_, _) =>
            {
                _cart = workingCart.Where(item => item.Qty > 0).Select(item => (item.Id, item.Nume, item.Pret, item.Qty)).ToList();
                SaveCartToFile();
                UpdateCartButton();
            };
            return dlg;
        }

        private void PlaceOrderSelected(System.Collections.Generic.List<(int Id, string Nume, decimal Pret, int Qty)> selectedItems, decimal total, string address)
        {
            try
            {
                int userId = DatabaseHelper.GetUserId(LoginForm.UtilizatorLogat);
                if (userId == 0) throw new Exception("Utilizator negăsit!");

                var itemsForDb = selectedItems.Select(i => (i.Id, i.Qty, i.Pret)).ToList();
                int orderId = DatabaseHelper.PlaseazaComandaCuDetaliu(userId, total, address, itemsForDb);

                foreach (var selected in selectedItems)
                {
                    int index = _cart.FindIndex(c => c.Id == selected.Id && c.Nume == selected.Nume && c.Pret == selected.Pret);
                    if (index >= 0) _cart.RemoveAt(index);
                }
                SaveCartToFile();
                UpdateCartButton();

                string details = string.Join("\n", selectedItems.Select(i => $"• {i.Nume}  x{i.Qty}  =  {i.Pret * i.Qty:F0} lei"));
                ShowPopup($"✅  Comanda #{orderId} plasată cu succes!\n\n{details}\n\n📍  Livrare la: {address}\n💰  Total: {total:F0} lei", isToast: false);
            }
            catch (Exception ex)
            {
                ShowPopup($"❌  Eroare la plasarea comenzii: {ex.Message}", isToast: false);
            }
        }

        // POPUP ENGINE
        private void ShowPopup(string message, bool isToast)
        {
            using var dlg = MakePopupForm(380, null);
            var lbl = PopupLabel(message, dlg.ClientSize.Width - 40);
            lbl.Location = new Point(20, 20);
            dlg.Controls.Add(lbl);
            if (isToast)
            {
                var t = new System.Windows.Forms.Timer { Interval = 1800 };
                t.Tick += (_, __) => { t.Stop(); dlg.Close(); };
                t.Start();
                dlg.ShowDialog(this);
            }
            else
            {
                int btnY = lbl.Bottom + 16;
                dlg.ClientSize = new Size(dlg.ClientSize.Width, btnY + 46);
                var btnOk = PopupBtn("OK", C_Orange, Color.White, 120);
                btnOk.Location = new Point((dlg.ClientSize.Width - 120) / 2, btnY);
                btnOk.Click += (_, __) => dlg.Close();
                dlg.Controls.Add(btnOk);
                dlg.ShowDialog(this);
            }
        }

        private bool ShowConfirm(string title, string message, string btnYesText, string btnNoText)
        {
            bool result = false;
            using var dlg = MakePopupForm(420, title);
            var lbl = PopupLabel(message, dlg.ClientSize.Width - 40);
            lbl.Location = new Point(20, string.IsNullOrEmpty(title) ? 20 : 54);
            dlg.Controls.Add(lbl);
            int btnY = lbl.Bottom + 16;
            dlg.ClientSize = new Size(dlg.ClientSize.Width, btnY + 50);
            var btnYes = PopupBtn(btnYesText, C_Orange, Color.White, 170);
            var btnNo = PopupBtn(btnNoText, Color.FromArgb(40, 136, 153, 187), Color.FromArgb(136, 153, 187), 110);
            btnYes.Location = new Point(20, btnY);
            btnNo.Location = new Point(dlg.ClientSize.Width - 130, btnY);
            btnYes.Click += (_, __) => { result = true; dlg.Close(); };
            btnNo.Click += (_, __) => { result = false; dlg.Close(); };
            dlg.Controls.Add(btnYes);
            dlg.Controls.Add(btnNo);
            dlg.ShowDialog(this);
            return result;
        }

        private string ShowInput(string title, string hint)
        {
            string result = "";
            using var dlg = MakePopupForm(400, title);
            var lblHint = PopupLabel(hint, dlg.ClientSize.Width - 40);
            lblHint.Location = new Point(20, 54);
            dlg.Controls.Add(lblHint);
            var txt = new TextBox
            {
                Location = new Point(20, lblHint.Bottom + 10),
                Size = new Size(dlg.ClientSize.Width - 40, 36),
                Font = new Font("Segoe UI", 11f),
                BackColor = Color.FromArgb(13, 27, 62),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Str. Exemplu, nr. 1..."
            };
            dlg.Controls.Add(txt);
            int btnY = txt.Bottom + 14;
            dlg.ClientSize = new Size(dlg.ClientSize.Width, btnY + 50);
            var btnOk = PopupBtn("✅  Confirmă", C_Orange, Color.White, 150);
            var btnCancel = PopupBtn("Anulează", Color.FromArgb(40, 136, 153, 187), Color.FromArgb(136, 153, 187), 110);
            btnOk.Location = new Point(20, btnY);
            btnCancel.Location = new Point(dlg.ClientSize.Width - 130, btnY);
            btnOk.Click += (_, __) => { result = txt.Text; dlg.Close(); };
            btnCancel.Click += (_, __) => dlg.Close();
            txt.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { result = txt.Text; dlg.Close(); } };
            dlg.Controls.Add(btnOk);
            dlg.Controls.Add(btnCancel);
            dlg.ShowDialog(this);
            return result;
        }

        private Form MakePopupForm(int width, string? title)
        {
            var dlg = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(22, 32, 64),
                Width = width,
                Height = 200,
                ShowInTaskbar = false,
                TopMost = true
            };
            dlg.Paint += (s, e) =>
            {
                var gp = new GraphicsPath();
                int r = 16;
                var b = dlg.ClientRectangle;
                gp.AddArc(b.Left, b.Top, r * 2, r * 2, 180, 90);
                gp.AddArc(b.Right - r * 2, b.Top, r * 2, r * 2, 270, 90);
                gp.AddArc(b.Right - r * 2, b.Bottom - r * 2, r * 2, r * 2, 0, 90);
                gp.AddArc(b.Left, b.Bottom - r * 2, r * 2, r * 2, 90, 90);
                gp.CloseFigure();
                dlg.Region = new Region(gp);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(C_Orange, 2);
                e.Graphics.DrawPath(pen, gp);
            };
            if (!string.IsNullOrEmpty(title))
            {
                dlg.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 14), AutoSize = true });
                dlg.Controls.Add(new Panel { Location = new Point(0, 44), Size = new Size(width, 2), BackColor = C_Orange });
            }
            bool drag = false; Point dragPt = Point.Empty;
            dlg.MouseDown += (s, e) => { drag = true; dragPt = e.Location; };
            dlg.MouseUp += (s, e) => drag = false;
            dlg.MouseMove += (s, e) => { if (drag) dlg.Location = new Point(dlg.Left + e.X - dragPt.X, dlg.Top + e.Y - dragPt.Y); };
            return dlg;
        }

        private static Label PopupLabel(string text, int maxWidth) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 10f),
            ForeColor = Color.FromArgb(200, 215, 240),
            MaximumSize = new Size(maxWidth, 0),
            AutoSize = true,
            Padding = new Padding(25, 0, 0, 0)
        };

        private static Button PopupBtn(string text, Color bg, Color fg, int width)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = fg,
                BackColor = bg,
                FlatStyle = FlatStyle.Flat,
                Width = width,
                Height = 36,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void ShowProductAddedPopup(string productName, string category, decimal price, decimal cartTotal)
        {
            using var dlg = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(22, 32, 64),
                Width = 440,
                Height = 280,
                ShowInTaskbar = false,
                TopMost = true
            };
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
            var lblCheck = new Label { Text = "✓", Font = new Font("Segoe UI", 42f, FontStyle.Bold), ForeColor = Color.FromArgb(80, 200, 80), AutoSize = true, Location = new Point(25, 30) };
            var lblMessage = new Label { Text = "Produs adăugat în coș!", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(120, 40) };
            var lblDetails = new Label { Text = $"{productName}\n{category} • {price:F0} lei", Font = new Font("Segoe UI", 10f), ForeColor = Color.FromArgb(200, 215, 240), AutoSize = true, Location = new Point(120, 70) };
            var lblTotal = new Label { Text = $"Total coș: {cartTotal:F0} lei", Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = C_Orange, AutoSize = true, Location = new Point(25, 140) };
            var line = new Panel { BackColor = C_Orange, Height = 2, Width = dlg.ClientSize.Width - 50, Location = new Point(25, 180) };
            var btnOk = new Button { Text = "OK", FlatStyle = FlatStyle.Flat, BackColor = C_Orange, ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Size = new Size(80, 32), Location = new Point(dlg.ClientSize.Width - 105, dlg.ClientSize.Height - 50), Cursor = Cursors.Hand };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (_, _) => dlg.Close();
            dlg.Controls.AddRange(new Control[] { lblCheck, lblMessage, lblDetails, lblTotal, line, btnOk });
            var timer = new System.Windows.Forms.Timer { Interval = 3500 };
            timer.Tick += (_, _) => { timer.Stop(); dlg.Close(); };
            timer.Start();
            dlg.ShowDialog(this);
        }
    }
}