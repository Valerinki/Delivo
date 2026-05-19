using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Delivo.Data;
using System.IO;
using System.Text.Json;

namespace Delivo.Forms
{
    public partial class ProfileForm : Form
    {
        private readonly Color ColorPortocaliu = Color.FromArgb(255, 107, 0);
        private readonly Color ColorAlbastruInchis = Color.FromArgb(13, 27, 62);
        private readonly Color ColorAlbastruCard = Color.FromArgb(22, 32, 64);
        private readonly Color ColorAlb = Color.White;
        private readonly Color ColorTextSecundar = Color.FromArgb(136, 153, 187);
        private readonly Color ColorDanger = Color.FromArgb(226, 75, 74);

        public ProfileForm()
        {
            BuildUI();
            LoadStatistics();
        }

        private void BuildUI()
        {
            // Setări fereastră principală – stil popup modern, dimensiuni ajustate
            this.Text = "";
            this.Size = new Size(540, 760); // înălțime mărită pentru a încăpea totul
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorAlbastruInchis;
            this.ShowInTaskbar = false;
            this.TopMost = true;

            // Bordură portocalie și colțuri rotunde
            this.Paint += (s, e) =>
            {
                var gp = new GraphicsPath();
                int r = 20;
                var rect = this.ClientRectangle;
                gp.AddArc(rect.Left, rect.Top, r * 2, r * 2, 180, 90);
                gp.AddArc(rect.Right - r * 2, rect.Top, r * 2, r * 2, 270, 90);
                gp.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                gp.AddArc(rect.Left, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                gp.CloseFigure();
                this.Region = new Region(gp);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(ColorPortocaliu, 2);
                e.Graphics.DrawPath(pen, gp);
            };

            // Buton închidere (X) în dreapta sus
            var btnClose = new Button
            {
                Text = "✕",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = ColorTextSecundar,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(36, 36),
                Location = new Point(this.ClientSize.Width - 48, 12),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            // Titlu fereastră
            var lblTitle = new Label
            {
                Text = "👤  Profilul meu",
                Font = new Font("Poppins", 14, FontStyle.Bold),
                ForeColor = ColorAlb,
                Location = new Point(24, 18),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            var separator = new Panel
            {
                BackColor = ColorPortocaliu,
                Height = 2,
                Width = this.ClientSize.Width - 48,
                Location = new Point(24, 52)
            };
            this.Controls.Add(separator);

            // Avatar (centrat)
            var pnlAvatar = new Panel { Size = new Size(80, 80), BackColor = ColorPortocaliu };
            RoundCorners(pnlAvatar, 40);
            pnlAvatar.Location = new Point((this.ClientSize.Width - 80) / 2, 70);
            string initiale = LoginForm.UtilizatorLogat.Length > 0
                ? LoginForm.UtilizatorLogat.Substring(0, 1).ToUpper() : "U";
            var lblInitiale = new Label
            {
                Text = initiale,
                Font = new Font("Poppins", 32, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = false,
                Size = new Size(80, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlAvatar.Controls.Add(lblInitiale);
            this.Controls.Add(pnlAvatar);

            // Nume utilizator (centrat)
            var lblNume = new Label
            {
                Text = LoginForm.UtilizatorLogat,
                Font = new Font("Poppins", 16, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 150) / 2, 160)
            };
            this.Controls.Add(lblNume);

            var lblRol = new Label
            {
                Text = LoginForm.RolLogat,
                Font = new Font("Poppins", 11),
                ForeColor = ColorPortocaliu,
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 80) / 2, 190)
            };
            this.Controls.Add(lblRol);

            // Cardurile statistice se vor adăuga separat în LoadStatistics, dar le poziționăm la y=240
            // Butoanele de acțiune le vom pune mai jos, începând de la y=340 (după carduri)
            // Pentru a nu se suprapune, butoanele vor fi create după ce cardurile sunt adăugate.
            // De aceea, amânăm crearea butoanelor până după LoadStatistics.
        }

        private void LoadStatistics()
        {
            int totalOrders = 0, activeOrders = 0;
            decimal totalSpent = 0;
            try
            {
                var comenzi = DatabaseHelper.GetComenziUtilizator(LoginForm.UtilizatorLogat);
                totalOrders = comenzi.Count;
                activeOrders = comenzi.FindAll(c => c.Status != "Livrată").Count;
                foreach (var c in comenzi) totalSpent += c.TotalPret;
            }
            catch { }

            int cardWidth = (this.ClientSize.Width - 80) / 3;
            int startX = 24;
            int statsY = 230; // poziția cardurilor (mai sus)
            AddStatCard("Comenzi", totalOrders.ToString(), startX, statsY, cardWidth);
            AddStatCard("Active", activeOrders.ToString(), startX + cardWidth + 12, statsY, cardWidth);
            AddStatCard("Lei chelt.", totalSpent.ToString("F0"), startX + (cardWidth + 12) * 2, statsY, cardWidth);

            // Acum adăugăm butoanele de acțiune, mai jos, pentru a nu acoperi cardurile
            int btnWidth = this.ClientSize.Width - 80;
            int btnHeight = 48;
            int btnX = 40;
            int startY = 340; // spațiu suficient între carduri și butoane
            int spacing = 16;

            var btnAddresses = CreateActionButton("📍  Adrese Salvate", btnX, startY, btnWidth, btnHeight);
            btnAddresses.Click += (s, e) => OpenAddressPopup();

            var btnChangePassword = CreateActionButton("🔒  Schimbă Parola", btnX, startY + btnHeight + spacing, btnWidth, btnHeight);
            btnChangePassword.Click += (s, e) => OpenChangePasswordPopup();

            var btnOrders = CreateActionButton("📦  Ultimele comenzi", btnX, startY + (btnHeight + spacing) * 2, btnWidth, btnHeight);
            btnOrders.Click += (s, e) => OpenOrdersPopup();

            this.Controls.Add(btnAddresses);
            this.Controls.Add(btnChangePassword);
            this.Controls.Add(btnOrders);

            // Buton Deconectare (jos)
            var btnLogout = new Button
            {
                Text = "Deconectare",
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorDanger,
                BackColor = Color.FromArgb(30, 226, 75, 74),
                Font = new Font("Poppins", 11, FontStyle.Bold),
                Size = new Size(this.ClientSize.Width - 48, 44),
                Location = new Point(24, this.ClientSize.Height - 70),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnLogout.FlatAppearance.BorderSize = 1;
            btnLogout.FlatAppearance.BorderColor = ColorDanger;
            RoundCorners(btnLogout, 22);
            btnLogout.Click += (s, e) =>
            {
                var result = MessageBox.Show("Sigur vrei să te deconectezi?", "Deconectare",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    new LoginForm().Show();
                    Application.OpenForms[0]?.Close();
                    this.Close();
                }
            };
            this.Controls.Add(btnLogout);

            // Eveniment redimensionare pentru a ajusta pozițiile relative
            this.Resize += (s, e) =>
            {
                // Recalculează pozițiile butoanelor și altor elemente
                var separator = this.Controls.OfType<Panel>().FirstOrDefault(p => p.BackColor == ColorPortocaliu);
                var btnClose = this.Controls.OfType<Button>().FirstOrDefault(b => b.Text == "✕");
                var lblTitle = this.Controls.OfType<Label>().FirstOrDefault(l => l.Text == "👤  Profilul meu");
                var pnlAvatar = this.Controls.OfType<Panel>().FirstOrDefault(p => p.Size == new Size(80, 80));
                var lblNume = this.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Size == 16 && l.ForeColor == ColorAlb);
                var lblRol = this.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Size == 11 && l.ForeColor == ColorPortocaliu);

                if (separator != null) separator.Width = this.ClientSize.Width - 48;
                if (btnClose != null) btnClose.Location = new Point(this.ClientSize.Width - 48, 12);
                if (lblTitle != null) lblTitle.Location = new Point(24, 18);
                if (pnlAvatar != null) pnlAvatar.Location = new Point((this.ClientSize.Width - 80) / 2, 70);
                if (lblNume != null) lblNume.Location = new Point((this.ClientSize.Width - lblNume.Width) / 2, 160);
                if (lblRol != null) lblRol.Location = new Point((this.ClientSize.Width - lblRol.Width) / 2, 190);

                // Recalculează cardurile (ele sunt adăugate direct, fără referință, dar le putem găsi după BackColor)
                var cards = this.Controls.OfType<Panel>().Where(p => p.BackColor == ColorAlbastruCard && p.Size.Height == 70).ToList();
                if (cards.Count == 3)
                {
                    int cardWidth = (this.ClientSize.Width - 80) / 3;
                    int startX = 24;
                    int statsY = 230;
                    cards[0].Location = new Point(startX, statsY);
                    cards[1].Location = new Point(startX + cardWidth + 12, statsY);
                    cards[2].Location = new Point(startX + (cardWidth + 12) * 2, statsY);
                    foreach (var c in cards) c.Width = cardWidth;
                }

                // Actualizează pozițiile butoanelor de acțiune
                var actionBtns = this.Controls.OfType<Button>().Where(b => b.Text.Contains("›")).ToList();
                if (actionBtns.Count == 3)
                {
                    int btnWidth = this.ClientSize.Width - 80;
                    int btnX = 40;
                    int startY = 340;
                    int btnHeight = 48;
                    int spacing = 16;
                    actionBtns[0].Location = new Point(btnX, startY);
                    actionBtns[0].Width = btnWidth;
                    actionBtns[1].Location = new Point(btnX, startY + btnHeight + spacing);
                    actionBtns[1].Width = btnWidth;
                    actionBtns[2].Location = new Point(btnX, startY + (btnHeight + spacing) * 2);
                    actionBtns[2].Width = btnWidth;
                }

                var btnLogoutCtrl = this.Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Deconectare");
                if (btnLogoutCtrl != null)
                {
                    btnLogoutCtrl.Location = new Point(24, this.ClientSize.Height - 70);
                    btnLogoutCtrl.Width = this.ClientSize.Width - 48;
                }
            };
        }

        private Button CreateActionButton(string text, int x, int y, int width, int height)
        {
            var btn = new Button
            {
                Text = text + "  ›",
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAlbastruCard,
                ForeColor = ColorAlb,
                Font = new Font("Poppins", 11, FontStyle.Bold),
                Size = new Size(width, height),
                Location = new Point(x, y),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            RoundCorners(btn, 24);
            return btn;
        }

        private void AddStatCard(string label, string valoare, int x, int y, int width)
        {
            var card = new Panel { Size = new Size(width, 70), Location = new Point(x, y), BackColor = ColorAlbastruCard };
            RoundCorners(card, 12);
            var lblVal = new Label
            {
                Text = valoare,
                Font = new Font("Poppins", 20, FontStyle.Bold),
                ForeColor = ColorPortocaliu,
                AutoSize = true,
                Location = new Point((width - 60) / 2, 10),
                BackColor = Color.Transparent
            };
            var lblLbl = new Label
            {
                Text = label,
                Font = new Font("Poppins", 10),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point((width - 70) / 2, 45),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblVal);
            card.Controls.Add(lblLbl);
            this.Controls.Add(card);
        }

        // --- Sub-popup-uri cu aceeași dimensiune (540x760) ---
        private void OpenAddressPopup()
        {
            using var popup = new SubPopupForm("📍  Adrese salvate", 540, 760);
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = ColorAlbastruInchis };

            // Container principal (mutat mai jos pentru a evita suprapunerea cu titlul)
            var container = new Panel
            {
                Size = new Size(panel.Width - 48, panel.Height - 100),
                Location = new Point(24, 90),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            panel.Controls.Add(container);

            // Text explicativ (centrat)
            var lblInfo = new Label
            {
                Text = "📌  Lista adreselor tale salvate",
                Font = new Font("Poppins", 11, FontStyle.Bold),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point((container.Width - 220) / 2, 5),
                TextAlign = ContentAlignment.MiddleCenter
            };
            container.Controls.Add(lblInfo);

            // Lista de adrese (centrată, 80% lățime)
            int listWidth = (int)(container.Width * 0.8);
            int listX = (container.Width - listWidth) / 2;
            var lstAddresses = new ListBox
            {
                Width = listWidth,
                Height = 180,
                Location = new Point(listX, 45),
                BackColor = ColorAlbastruCard,
                ForeColor = ColorAlb,
                BorderStyle = BorderStyle.None,
                Font = new Font("Poppins", 10),
                IntegralHeight = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            container.Controls.Add(lstAddresses);

            // --- Câmp text pentru adresă (design modern) ---
            var txtAddress = new TextBox
            {
                Width = listWidth,
                Height = 42,
                Location = new Point(listX, lstAddresses.Bottom + 18),
                BackColor = ColorAlbastruCard,
                ForeColor = ColorAlb,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Poppins", 10),
                PlaceholderText = "✏️  Scrie aici o adresă nouă sau editează una existentă...",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            // Adăugăm un efect vizual subtil
            txtAddress.GotFocus += (s, e) => txtAddress.BackColor = Color.FromArgb(35, 45, 75);
            txtAddress.LostFocus += (s, e) => txtAddress.BackColor = ColorAlbastruCard;
            container.Controls.Add(txtAddress);

            // Buton Salvează (centrat)
            int btnSaveWidth = 180;
            int btnSaveHeight = 42;
            var btnSave = new Button
            {
                Text = "💾  Salvează adresa",
                Size = new Size(btnSaveWidth, btnSaveHeight),
                Location = new Point((container.Width - btnSaveWidth) / 2, txtAddress.Bottom + 18),
                BackColor = ColorPortocaliu,
                ForeColor = ColorAlb,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Poppins", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            RoundCorners(btnSave, 21);
            container.Controls.Add(btnSave);

            // Butoane pentru editare/ștergere (centrate)
            int btnWidth = 150;
            int btnHeight = 38;
            int spacing = 20;
            int totalWidth = btnWidth * 2 + spacing;
            int startX = (container.Width - totalWidth) / 2;
            int btnY = btnSave.Bottom + 24;

            var btnEditSelected = new Button
            {
                Text = "✏️  Editează selectată",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point(startX, btnY),
                BackColor = Color.FromArgb(45, 55, 85),
                ForeColor = ColorAlb,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Poppins", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            btnEditSelected.FlatAppearance.BorderSize = 0;
            RoundCorners(btnEditSelected, 19);
            btnEditSelected.Click += (s, e) =>
            {
                if (lstAddresses.SelectedItem == null || !lstAddresses.Enabled)
                {
                    MessageBox.Show("Selectați o adresă din listă.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                txtAddress.Text = lstAddresses.SelectedItem.ToString();
                txtAddress.Focus();
            };

            var btnDeleteSelected = new Button
            {
                Text = "🗑️  Șterge selectată",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point(startX + btnWidth + spacing, btnY),
                BackColor = ColorDanger,
                ForeColor = ColorAlb,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Poppins", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            btnDeleteSelected.FlatAppearance.BorderSize = 0;
            RoundCorners(btnDeleteSelected, 19);
            btnDeleteSelected.Click += (s, e) =>
            {
                if (lstAddresses.SelectedItem == null || !lstAddresses.Enabled)
                {
                    MessageBox.Show("Selectați o adresă.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (MessageBox.Show("Ștergeți adresa selectată?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    lstAddresses.Items.Remove(lstAddresses.SelectedItem);
                    if (lstAddresses.Items.Count == 0)
                    {
                        lstAddresses.Items.Add("📭  Nu ai adrese salvate momentan");
                        lstAddresses.Enabled = false;
                        lstAddresses.ForeColor = ColorTextSecundar;
                    }
                    SaveAddresses(lstAddresses);
                    txtAddress.Clear();
                }
            };

            container.Controls.Add(btnEditSelected);
            container.Controls.Add(btnDeleteSelected);

            // Încărcare adrese + mesaj inițial
            string addrFile = Path.Combine(Application.UserAppDataPath, "user_addresses.json");
            bool hasAddresses = false;
            if (File.Exists(addrFile))
            {
                try
                {
                    var json = File.ReadAllText(addrFile);
                    var list = JsonSerializer.Deserialize<List<string>>(json);
                    if (list != null && list.Count > 0)
                    {
                        hasAddresses = true;
                        foreach (var a in list) lstAddresses.Items.Add(a);
                        lstAddresses.Enabled = true;
                    }
                }
                catch { }
            }
            if (!hasAddresses)
            {
                lstAddresses.Items.Add("📭  Nu ai adrese salvate momentan");
                lstAddresses.Enabled = false;
                lstAddresses.ForeColor = ColorTextSecundar;
            }

            // Eveniment Salvează (adaugă sau înlocuiește)
            btnSave.Click += (s, e) =>
            {
                string newAddr = txtAddress.Text.Trim();
                if (string.IsNullOrWhiteSpace(newAddr))
                {
                    MessageBox.Show("Introduceți o adresă validă.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (lstAddresses.SelectedItem != null && lstAddresses.Enabled && lstAddresses.SelectedIndex >= 0)
                {
                    // Editare: înlocuiește elementul selectat
                    int idx = lstAddresses.SelectedIndex;
                    lstAddresses.Items[idx] = newAddr;
                }
                else
                {
                    // Adăugare nouă
                    if (lstAddresses.Items.Count == 1 && lstAddresses.Items[0].ToString().Contains("Nu ai adrese"))
                    {
                        lstAddresses.Items.Clear();
                        lstAddresses.Enabled = true;
                        lstAddresses.ForeColor = ColorAlb;
                    }
                    lstAddresses.Items.Add(newAddr);
                }
                SaveAddresses(lstAddresses);
                txtAddress.Clear();
                if (!lstAddresses.Enabled) lstAddresses.Enabled = true;
            };

            // Ajustare la redimensionare
            container.Resize += (_, _) =>
            {
                lblInfo.Location = new Point((container.Width - lblInfo.Width) / 2, 5);
                int newListWidth = (int)(container.Width * 0.8);
                int newListX = (container.Width - newListWidth) / 2;
                lstAddresses.Width = newListWidth;
                lstAddresses.Location = new Point(newListX, 45);
                txtAddress.Width = newListWidth;
                txtAddress.Location = new Point(newListX, lstAddresses.Bottom + 18);
                btnSave.Location = new Point((container.Width - btnSaveWidth) / 2, txtAddress.Bottom + 18);
                int newStartX = (container.Width - totalWidth) / 2;
                int newBtnY = btnSave.Bottom + 24;
                btnEditSelected.Location = new Point(newStartX, newBtnY);
                btnDeleteSelected.Location = new Point(newStartX + btnWidth + spacing, newBtnY);
            };

            popup.Controls.Add(panel);
            popup.ShowDialog(this);
        }

        private void SaveAddresses(ListBox lst)
        {
            try
            {
                var list = lst.Items.Cast<string>().ToList();
                var json = JsonSerializer.Serialize(list);
                string addrFile = Path.Combine(Application.UserAppDataPath, "user_addresses.json");
                File.WriteAllText(addrFile, json);
            }
            catch { }
        }

        private void OpenChangePasswordPopup()
        {
            using var popup = new SubPopupForm("🔒  Schimbă parola", 540, 760);
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = ColorAlbastruInchis };

            // Container principal (mutat mai jos pentru a lăsa mai mult spațiu sus)
            var container = new Panel
            {
                Size = new Size(panel.Width - 48, panel.Height - 100),
                Location = new Point(24, 160), // coborât pentru a nu se suprapune cu titlul
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            panel.Controls.Add(container);

            // Text explicativ pe două rânduri
            var lblInfoLine1 = new Label
            {
                Text = "Completează câmpurile",
                Font = new Font("Poppins", 11, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point((container.Width - 180) / 2, 5),
                TextAlign = ContentAlignment.MiddleCenter
            };
            container.Controls.Add(lblInfoLine1);

            var lblInfoLine2 = new Label
            {
                Text = "pentru a-ți schimba parola",
                Font = new Font("Poppins", 10),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point((container.Width - 180) / 2, lblInfoLine1.Bottom + 2),
                TextAlign = ContentAlignment.MiddleCenter
            };
            container.Controls.Add(lblInfoLine2);

            int fieldWidth = (int)(container.Width * 0.7);
            int fieldX = (container.Width - fieldWidth) / 2;
            int startY = lblInfoLine2.Bottom + 25; // spațiu după text

            // Câmp parolă veche
            var lblOld = new Label
            {
                Text = "Parola veche",
                Font = new Font("Poppins", 10, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(fieldX, startY)
            };
            container.Controls.Add(lblOld);

            var txtOld = new TextBox
            {
                Width = fieldWidth,
                Height = 42,
                Location = new Point(fieldX, lblOld.Bottom + 5),
                BackColor = ColorAlbastruCard,
                ForeColor = ColorAlb,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Poppins", 10),
                PasswordChar = '●',
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            txtOld.GotFocus += (s, e) => txtOld.BackColor = Color.FromArgb(35, 45, 75);
            txtOld.LostFocus += (s, e) => txtOld.BackColor = ColorAlbastruCard;
            container.Controls.Add(txtOld);

            // Câmp parolă nouă
            var lblNew = new Label
            {
                Text = "Parola nouă",
                Font = new Font("Poppins", 10, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(fieldX, txtOld.Bottom + 20)
            };
            container.Controls.Add(lblNew);

            var txtNew = new TextBox
            {
                Width = fieldWidth,
                Height = 42,
                Location = new Point(fieldX, lblNew.Bottom + 5),
                BackColor = ColorAlbastruCard,
                ForeColor = ColorAlb,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Poppins", 10),
                PasswordChar = '●',
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            txtNew.GotFocus += (s, e) => txtNew.BackColor = Color.FromArgb(35, 45, 75);
            txtNew.LostFocus += (s, e) => txtNew.BackColor = ColorAlbastruCard;
            container.Controls.Add(txtNew);

            // Panou cu reguli de validare (sub câmpul parolă nouă)
            var pnlRules = new FlowLayoutPanel
            {
                Location = new Point(fieldX, txtNew.Bottom + 8),
                Size = new Size(fieldWidth, 28),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            string[] ruleNames = { "min 8", "A-Z", "0-9", "!?#" };
            Label[] ruleLabels = new Label[4];
            for (int i = 0; i < 4; i++)
            {
                ruleLabels[i] = new Label
                {
                    Text = ruleNames[i],
                    Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                    ForeColor = ColorTextSecundar,
                    BackColor = Color.FromArgb(25, ColorPortocaliu),
                    Size = new Size(65, 22),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(0, 0, 6, 0)
                };
                RoundCorners(ruleLabels[i], 11);
                pnlRules.Controls.Add(ruleLabels[i]);
            }
            container.Controls.Add(pnlRules);

            // Câmp confirmare parolă
            var lblConfirm = new Label
            {
                Text = "Confirmă parola nouă",
                Font = new Font("Poppins", 10, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(fieldX, pnlRules.Bottom + 15)
            };
            container.Controls.Add(lblConfirm);

            var txtConfirm = new TextBox
            {
                Width = fieldWidth,
                Height = 42,
                Location = new Point(fieldX, lblConfirm.Bottom + 5),
                BackColor = ColorAlbastruCard,
                ForeColor = ColorAlb,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Poppins", 10),
                PasswordChar = '●',
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            txtConfirm.GotFocus += (s, e) => txtConfirm.BackColor = Color.FromArgb(35, 45, 75);
            txtConfirm.LostFocus += (s, e) => txtConfirm.BackColor = ColorAlbastruCard;
            container.Controls.Add(txtConfirm);

            // Label pentru mesajul de confirmare a parolei
            var lblMatchMsg = new Label
            {
                AutoSize = true,
                Location = new Point(fieldX, txtConfirm.Bottom + 5),
                Font = new Font("Poppins", 8.5f),
                ForeColor = Color.FromArgb(80, 200, 80)
            };
            container.Controls.Add(lblMatchMsg);

            // Funcție de verificare a regulilor parolei
            void UpdatePasswordRules(string pwd)
            {
                bool[] checks = new bool[4];
                checks[0] = pwd.Length >= 8;
                checks[1] = pwd.Any(char.IsUpper);
                checks[2] = pwd.Any(char.IsDigit);
                checks[3] = pwd.Any(ch => "!@#$%^&*()_+[]{};:?/\\|<>".Contains(ch));

                for (int i = 0; i < 4; i++)
                {
                    ruleLabels[i].ForeColor = checks[i] ? Color.FromArgb(80, 200, 80) : ColorTextSecundar;
                    ruleLabels[i].BackColor = checks[i] ? Color.FromArgb(25, 80, 200, 80) : Color.FromArgb(25, ColorPortocaliu);
                }
            }

            // Verificare confirmare live
            void CheckPasswordMatch()
            {
                if (txtNew.Text == txtConfirm.Text && txtNew.Text.Length > 0)
                {
                    lblMatchMsg.ForeColor = Color.FromArgb(80, 200, 80);
                    lblMatchMsg.Text = "✓  Parolele coincid";
                }
                else if (txtConfirm.Text.Length > 0)
                {
                    lblMatchMsg.ForeColor = ColorDanger;
                    lblMatchMsg.Text = "✗  Parolele nu coincid";
                }
                else
                {
                    lblMatchMsg.Text = "";
                }
            }

            txtNew.TextChanged += (s, e) =>
            {
                UpdatePasswordRules(txtNew.Text);
                CheckPasswordMatch();
            };
            txtConfirm.TextChanged += (s, e) => CheckPasswordMatch();

            // Label pentru mesaj general de eroare
            var lblErrorMsg = new Label
            {
                AutoSize = true,
                Font = new Font("Poppins", 8.5f),
                ForeColor = ColorDanger,
                Location = new Point((container.Width - 200) / 2, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            container.Controls.Add(lblErrorMsg);

            // Buton Schimbă parola
            int btnWidth = 220;
            int btnHeight = 44;
            var btnChange = new Button
            {
                Text = "🔑  Schimbă parola",
                Size = new Size(btnWidth, btnHeight),
                BackColor = ColorPortocaliu,
                ForeColor = ColorAlb,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Poppins", 10, FontStyle.Bold)
            };
            btnChange.FlatAppearance.BorderSize = 0;
            RoundCorners(btnChange, 22);
            container.Controls.Add(btnChange);

            // Buton Anulează
            int btnCancelWidth = 140;
            var btnCancel = new Button
            {
                Text = "Anulează",
                Size = new Size(btnCancelWidth, 40),
                BackColor = Color.FromArgb(45, 55, 85),
                ForeColor = ColorAlb,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Poppins", 9, FontStyle.Bold)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            RoundCorners(btnCancel, 20);
            btnCancel.Click += (s, e) => popup.Close();
            container.Controls.Add(btnCancel);

            // Mesaj de succes
            var lblSuccessMsg = new Label
            {
                AutoSize = true,
                Font = new Font("Poppins", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 200, 80),
                Location = new Point((container.Width - 150) / 2, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            container.Controls.Add(lblSuccessMsg);

            // Eveniment buton schimbare
            btnChange.Click += async (s, e) =>
            {
                string oldPwd = txtOld.Text.Trim();
                string newPwd = txtNew.Text.Trim();
                string confirm = txtConfirm.Text.Trim();

                lblErrorMsg.Visible = false;
                lblSuccessMsg.Visible = false;

                if (string.IsNullOrEmpty(oldPwd) || string.IsNullOrEmpty(newPwd))
                {
                    lblErrorMsg.Text = "❌  Toate câmpurile sunt obligatorii.";
                    lblErrorMsg.Location = new Point((container.Width - lblErrorMsg.Width) / 2, btnCancel.Bottom + 10);
                    lblErrorMsg.Visible = true;
                    return;
                }
                if (newPwd != confirm)
                {
                    lblErrorMsg.Text = "❌  Parolele noi nu coincid.";
                    lblErrorMsg.Location = new Point((container.Width - lblErrorMsg.Width) / 2, btnCancel.Bottom + 10);
                    lblErrorMsg.Visible = true;
                    return;
                }
                if (!(newPwd.Length >= 8 && newPwd.Any(char.IsUpper) && newPwd.Any(char.IsDigit) && newPwd.Any(ch => "!@#$%^&*()_+[]{};:?/\\|<>".Contains(ch))))
                {
                    lblErrorMsg.Text = "❌  Parola nu respectă regulile de securitate.";
                    lblErrorMsg.Location = new Point((container.Width - lblErrorMsg.Width) / 2, btnCancel.Bottom + 10);
                    lblErrorMsg.Visible = true;
                    return;
                }

                try
                {
                    if (DatabaseHelper.ActualizeazaParola(LoginForm.UtilizatorLogat, oldPwd, newPwd))
                    {
                        lblSuccessMsg.Text = "✅  Parola a fost schimbată cu succes!";
                        lblSuccessMsg.Location = new Point((container.Width - lblSuccessMsg.Width) / 2, btnCancel.Bottom + 10);
                        lblSuccessMsg.Visible = true;
                        btnChange.Enabled = false;
                        btnCancel.Enabled = false;
                        await Task.Delay(1200);
                        popup.Close();
                    }
                    else
                    {
                        lblErrorMsg.Text = "❌  Parola veche este incorectă.";
                        lblErrorMsg.Location = new Point((container.Width - lblErrorMsg.Width) / 2, btnCancel.Bottom + 10);
                        lblErrorMsg.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    lblErrorMsg.Text = $"❌  Eroare: {ex.Message}";
                    lblErrorMsg.Location = new Point((container.Width - lblErrorMsg.Width) / 2, btnCancel.Bottom + 10);
                    lblErrorMsg.Visible = true;
                }
            };

            // Ajustare layout la redimensionare
            container.Resize += (_, _) =>
            {
                int newFieldWidth = (int)(container.Width * 0.7);
                int newFieldX = (container.Width - newFieldWidth) / 2;

                // Recalculare poziții pentru toate elementele
                lblInfoLine1.Location = new Point((container.Width - lblInfoLine1.Width) / 2, 5);
                lblInfoLine2.Location = new Point((container.Width - lblInfoLine2.Width) / 2, lblInfoLine1.Bottom + 2);

                int newStartY = lblInfoLine2.Bottom + 25;

                lblOld.Location = new Point(newFieldX, newStartY);
                txtOld.Width = newFieldWidth;
                txtOld.Location = new Point(newFieldX, lblOld.Bottom + 5);

                lblNew.Location = new Point(newFieldX, txtOld.Bottom + 20);
                txtNew.Width = newFieldWidth;
                txtNew.Location = new Point(newFieldX, lblNew.Bottom + 5);

                pnlRules.Width = newFieldWidth;
                pnlRules.Location = new Point(newFieldX, txtNew.Bottom + 8);

                lblConfirm.Location = new Point(newFieldX, pnlRules.Bottom + 15);
                txtConfirm.Width = newFieldWidth;
                txtConfirm.Location = new Point(newFieldX, lblConfirm.Bottom + 5);

                lblMatchMsg.Location = new Point(newFieldX, txtConfirm.Bottom + 5);

                btnChange.Location = new Point((container.Width - btnWidth) / 2, txtConfirm.Bottom + 50);
                btnCancel.Location = new Point((container.Width - btnCancelWidth) / 2, btnChange.Bottom + 15);

                if (lblErrorMsg.Visible)
                    lblErrorMsg.Location = new Point((container.Width - lblErrorMsg.Width) / 2, btnCancel.Bottom + 10);
                if (lblSuccessMsg.Visible)
                    lblSuccessMsg.Location = new Point((container.Width - lblSuccessMsg.Width) / 2, btnCancel.Bottom + 10);
            };

            popup.Controls.Add(panel);
            popup.ShowDialog(this);
        }

        private void OpenOrdersPopup()
        {
            using var popup = new SubPopupForm("📦  Ultimele comenzi", 540, 760);
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = ColorAlbastruInchis };

            // Container principal (poziționat similar cu celelalte popup-uri)
            var container = new Panel
            {
                Size = new Size(panel.Width - 48, panel.Height - 100),
                Location = new Point(24, 110),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            panel.Controls.Add(container);

            // Încărcare comenzi
            var comenzi = DatabaseHelper.GetComenziUtilizator(LoginForm.UtilizatorLogat);
            var latest = comenzi.OrderByDescending(c => c.DataComanda).Take(10).ToList();
            int totalOrders = comenzi.Count;

            // Text explicativ pe două rânduri + număr total comenzi
            var lblInfoLine1 = new Label
            {
                Text = $"Istoricul comenzilor tale ({totalOrders})",
                Font = new Font("Poppins", 11, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point((container.Width - 200) / 2, 5),
                TextAlign = ContentAlignment.MiddleCenter
            };
            container.Controls.Add(lblInfoLine1);

            var lblInfoLine2 = new Label
            {
                Text = "ultimele 10 comenzi plasate",
                Font = new Font("Poppins", 10),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point((container.Width - 180) / 2, lblInfoLine1.Bottom + 2),
                TextAlign = ContentAlignment.MiddleCenter
            };
            container.Controls.Add(lblInfoLine2);

            // Panel scrollabil pentru lista de comenzi
            var pnlOrdersScroll = new Panel
            {
                Location = new Point(0, lblInfoLine2.Bottom + 20),
                BackColor = Color.Transparent,
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            container.Controls.Add(pnlOrdersScroll);

            if (latest.Count == 0)
            {
                var lblEmpty = new Label
                {
                    Text = "📭  Nu ai plasat nicio comandă încă.",
                    Font = new Font("Poppins", 11),
                    ForeColor = ColorTextSecundar,
                    AutoSize = true,
                    Location = new Point((pnlOrdersScroll.Width - 220) / 2, 30),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                pnlOrdersScroll.Controls.Add(lblEmpty);
                pnlOrdersScroll.Resize += (_, _) =>
                {
                    lblEmpty.Location = new Point((pnlOrdersScroll.Width - lblEmpty.Width) / 2, 30);
                };
            }
            else
            {
                int y = 10;
                foreach (var cmd in latest)
                {
                    // Card pentru fiecare comandă
                    var card = new Panel
                    {
                        Width = pnlOrdersScroll.Width - 30,
                        Height = 95,
                        BackColor = ColorAlbastruCard,
                        Margin = new Padding(0, 0, 0, 12),
                        Padding = new Padding(12)
                    };
                    RoundCorners(card, 14);
                    card.Location = new Point(10, y);
                    card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                    // ID comandă
                    var lblId = new Label
                    {
                        Text = $"Comanda #{cmd.Id}",
                        Font = new Font("Poppins", 11, FontStyle.Bold),
                        ForeColor = ColorPortocaliu,
                        AutoSize = true,
                        Location = new Point(12, 10)
                    };
                    card.Controls.Add(lblId);

                    // Data
                    var lblData = new Label
                    {
                        Text = cmd.DataComanda.ToString("dd MMM yyyy, HH:mm"),
                        Font = new Font("Poppins", 9),
                        ForeColor = ColorTextSecundar,
                        AutoSize = true,
                        Location = new Point(12, 36)
                    };
                    card.Controls.Add(lblData);

                    // Total
                    var lblTotal = new Label
                    {
                        Text = $"{cmd.TotalPret:F0} lei",
                        Font = new Font("Poppins", 12, FontStyle.Bold),
                        ForeColor = ColorAlb,
                        AutoSize = true,
                        Location = new Point(card.Width - 100, 20),
                        TextAlign = ContentAlignment.MiddleRight,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    card.Controls.Add(lblTotal);

                    // Status
                    var lblStatus = new Label
                    {
                        Text = cmd.Status,
                        Font = new Font("Poppins", 9, FontStyle.Bold),
                        ForeColor = cmd.Status == "Livrată" ? Color.FromArgb(80, 200, 80) : ColorPortocaliu,
                        AutoSize = true,
                        Location = new Point(card.Width - 100, 50),
                        TextAlign = ContentAlignment.MiddleRight,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    card.Controls.Add(lblStatus);

                    pnlOrdersScroll.Controls.Add(card);
                    y += card.Height + 8;
                }

                // Ajustare la redimensionare pentru carduri
                pnlOrdersScroll.Resize += (_, _) =>
                {
                    foreach (Control ctrl in pnlOrdersScroll.Controls)
                    {
                        if (ctrl is Panel card)
                        {
                            card.Width = pnlOrdersScroll.Width - 30;
                            foreach (Control inner in card.Controls)
                            {
                                if (inner is Label lbl && (lbl.Text.EndsWith("lei") || lbl.Text == "Livrată" || lbl.Text.Contains("În")))
                                {
                                    inner.Location = new Point(card.Width - 100, inner.Location.Y);
                                }
                            }
                        }
                    }
                };
            }

            // Buton închidere (poziționat mai sus, la 50px de margine)
            int btnWidth = 160;
            int btnHeight = 38;
            var btnClose = new Button
            {
                Text = "Închide",
                Size = new Size(btnWidth, btnHeight),
                BackColor = ColorPortocaliu,
                ForeColor = ColorAlb,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Poppins", 10, FontStyle.Bold)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            RoundCorners(btnClose, 21);
            btnClose.Click += (s, e) => popup.Close();
            container.Controls.Add(btnClose);

            // Ajustare layout general la redimensionare
            container.Resize += (_, _) =>
            {
                // Repoziționare texte
                lblInfoLine1.Location = new Point((container.Width - lblInfoLine1.Width) / 2, 5);
                lblInfoLine2.Location = new Point((container.Width - lblInfoLine2.Width) / 2, lblInfoLine1.Bottom + 2);
                // Setează dimensiunea și poziția panelului scrollabil
                pnlOrdersScroll.Location = new Point(0, lblInfoLine2.Bottom + 20);
                pnlOrdersScroll.Size = new Size(container.Width, container.Height - (lblInfoLine2.Bottom + 20) - 60);
                // Butonul de închidere – poziționat la 50px de marginea de jos a containerului
                btnClose.Location = new Point((container.Width - btnWidth) / 2, container.Height - 65);
            };

            // Forțează un prim calcul al dimensiunilor
            container.PerformLayout();
            popup.Controls.Add(panel);
            popup.ShowDialog(this);
        }

        private string ShowInputDialog(string title, string prompt, Form parent, string defaultValue = "")
        {
            using var dlg = new Form
            {
                Text = title,
                Width = 420,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = ColorAlbastruInchis
            };
            var lbl = new Label { Text = prompt, Location = new Point(16, 20), AutoSize = true, ForeColor = ColorAlb, Font = new Font("Poppins", 10) };
            var txt = new TextBox { Text = defaultValue, Location = new Point(16, 48), Width = 370, BackColor = ColorAlbastruCard, ForeColor = ColorAlb, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Poppins", 10) };
            var btnOk = new Button { Text = "OK", Location = new Point(210, 88), Width = 80, Height = 32, BackColor = ColorPortocaliu, ForeColor = ColorAlb, FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0;
            var btnCancel = new Button { Text = "Anulează", Location = new Point(300, 88), Width = 90, Height = 32, BackColor = Color.FromArgb(45, 55, 85), ForeColor = ColorAlb, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnOk.Click += (s, e) => { dlg.DialogResult = DialogResult.OK; dlg.Close(); };
            btnCancel.Click += (s, e) => { dlg.DialogResult = DialogResult.Cancel; dlg.Close(); };
            dlg.Controls.Add(lbl);
            dlg.Controls.Add(txt);
            dlg.Controls.Add(btnOk);
            dlg.Controls.Add(btnCancel);
            return dlg.ShowDialog(parent) == DialogResult.OK ? txt.Text : "";
        }

        // Helper round corners
        private void RoundCorners(Control ctrl, int radius)
        {
            if (ctrl.Width < 1 || ctrl.Height < 1) return;
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(ctrl.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(ctrl.Width - radius * 2, ctrl.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, ctrl.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            ctrl.Region = new Region(path);
        }
    }

    // Clasă auxiliară pentru sub-popup-uri (aceleași dimensiuni ca principalul)
    public class SubPopupForm : Form
    {
        private readonly Color ColorPortocaliu = Color.FromArgb(255, 107, 0);
        private readonly Color ColorAlbastruInchis = Color.FromArgb(13, 27, 62);

        public SubPopupForm(string title, int width, int height)
        {
            this.Text = "";
            this.Size = new Size(width, height);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorAlbastruInchis;
            this.ShowInTaskbar = false;
            this.TopMost = true;

            // Bordură portocalie și colțuri rotunde
            this.Paint += (s, e) =>
            {
                var gp = new GraphicsPath();
                int r = 20;
                var rect = this.ClientRectangle;
                gp.AddArc(rect.Left, rect.Top, r * 2, r * 2, 180, 90);
                gp.AddArc(rect.Right - r * 2, rect.Top, r * 2, r * 2, 270, 90);
                gp.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                gp.AddArc(rect.Left, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                gp.CloseFigure();
                this.Region = new Region(gp);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(ColorPortocaliu, 2);
                e.Graphics.DrawPath(pen, gp);
            };

            // Titlu
            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Poppins", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(24, 18),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            var separator = new Panel
            {
                BackColor = ColorPortocaliu,
                Height = 2,
                Width = this.ClientSize.Width - 48,
                Location = new Point(24, 52)
            };
            this.Controls.Add(separator);

            // Buton închidere
            var btnClose = new Button
            {
                Text = "✕",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(136, 153, 187),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(36, 36),
                Location = new Point(this.ClientSize.Width - 48, 12),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            this.Resize += (_, _) =>
            {
                separator.Width = this.ClientSize.Width - 48;
                btnClose.Location = new Point(this.ClientSize.Width - 48, 12);
                lblTitle.Location = new Point(24, 18);
            };
        }
    }
}