using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class AdminForm : Form
    {
        private readonly Color ColorPortocaliu = Color.FromArgb(255, 107, 0);
        private readonly Color ColorAlbastruInchis = Color.FromArgb(13, 27, 62);
        private readonly Color ColorAlbastruCard = Color.FromArgb(22, 32, 64);
        private readonly Color ColorAlb = Color.White;
        private readonly Color ColorTextSecundar = Color.FromArgb(136, 153, 187);

        private Panel pnlContent;

        public AdminForm()
        {
            BuildUI();
            LoadDashboard();
        }

        private void BuildUI()
        {
            this.Text = "Delivo - Panou Admin";
            this.Size = new Size(800, 600);
            this.BackColor = ColorAlbastruInchis;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // === HEADER ===
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorPortocaliu
            };

            var lblTitle = new Label
            {
                Text = "DELIVO  —  Panou Administrator",
                Font = new Font("Poppins", 13, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(20, 18)
            };

            var lblUser = new Label
            {
                Text = "Admin: " + LoginForm.UtilizatorLogat,
                Font = new Font("Poppins", 9),
                ForeColor = Color.FromArgb(220, 255, 255, 255),
                AutoSize = true,
                Location = new Point(620, 22)
            };

            var btnLogout = new Button
            {
                Text = "Iesi",
                Font = new Font("Poppins", 9),
                ForeColor = ColorAlb,
                BackColor = Color.FromArgb(180, 0, 0),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(60, 30),
                Location = new Point(710, 15),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) => { new LoginForm().Show(); this.Close(); };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblUser);
            pnlHeader.Controls.Add(btnLogout);

            // === SIDEBAR ===
            var pnlSidebar = new Panel
            {
                Width = 180,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(10, 20, 50)
            };

            string[] menuItems = { "Dashboard", "Produse", "Comenzi", "Utilizatori" };
            string[] menuEmoji = { "📊", "🍕", "📋", "👥" };
            for (int i = 0; i < menuItems.Length; i++)
            {
                int idx = i;
                var btn = new Button
                {
                    Text = menuEmoji[i] + "  " + menuItems[i],
                    Font = new Font("Poppins", 10),
                    ForeColor = ColorTextSecundar,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(180, 48),
                    Location = new Point(0, 80 + i * 52),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(16, 0, 0, 0),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => {
                    switch (idx)
                    {
                        case 0: LoadDashboard(); break;
                        case 1: LoadProduse(); break;
                        case 2: LoadComenzi(); break;
                        case 3: LoadUtilizatori(); break;
                    }
                };
                pnlSidebar.Controls.Add(btn);
            }

            // === CONTINUT PRINCIPAL ===
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorAlbastruInchis,
                AutoScroll = true,
                Padding = new Padding(20)
            };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlHeader);
        }

        // ===== DASHBOARD =====
        private void LoadDashboard()
        {
            pnlContent.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = "Dashboard",
                Font = new Font("Poppins", 16, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            pnlContent.Controls.Add(lblTitlu);

            // Carduri statistici
            string[] titluri = { "Comenzi azi", "In livrare", "Produse", "Clienti" };
            string[] valori = { "24", "8", "5", "2" };
            Color[] culori = { ColorPortocaliu, Color.FromArgb(29, 158, 117),
                               Color.FromArgb(55, 138, 221), Color.FromArgb(132, 79, 11) };

            for (int i = 0; i < 4; i++)
            {
                var card = CreateStatCard(titluri[i], valori[i], culori[i]);
                card.Location = new Point(20 + i * 140, 60);
                pnlContent.Controls.Add(card);
            }

            // Tabel comenzi recente
            var lblRecente = new Label
            {
                Text = "Comenzi recente",
                Font = new Font("Poppins", 12, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(20, 180)
            };
            pnlContent.Controls.Add(lblRecente);

            var dgv = CreateDataGrid();
            dgv.Location = new Point(20, 210);
            dgv.Size = new Size(560, 250);

            try
            {
                var comenzi = DatabaseHelper.GetComenzi();
                dgv.Columns.Add("Id", "#");
                dgv.Columns.Add("Utilizator", "Client");
                dgv.Columns.Add("Total", "Total");
                dgv.Columns.Add("Status", "Status");
                dgv.Columns.Add("Data", "Data");
                foreach (var c in comenzi)
                    dgv.Rows.Add(c.Id, c.NumeUtilizator, c.TotalPret + " lei", c.Status, c.DataComanda.ToString("dd.MM.yyyy"));
            }
            catch { dgv.Rows.Add("--", "Eroare BD", "--", "--", "--"); }

            pnlContent.Controls.Add(dgv);
        }

        // ===== PRODUSE =====
        private void LoadProduse()
        {
            pnlContent.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = "Gestionare Produse",
                Font = new Font("Poppins", 16, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var btnAdauga = new Button
            {
                Text = "+ Adauga produs",
                Font = new Font("Poppins", 10),
                ForeColor = ColorAlb,
                BackColor = ColorPortocaliu,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(160, 36),
                Location = new Point(400, 16),
                Cursor = Cursors.Hand
            };
            btnAdauga.FlatAppearance.BorderSize = 0;
            btnAdauga.Click += (s, e) => AdaugaProdus();

            var dgv = CreateDataGrid();
            dgv.Location = new Point(20, 70);
            dgv.Size = new Size(560, 380);

            try
            {
                var produse = DatabaseHelper.GetProduse();
                dgv.Columns.Add("Id", "ID");
                dgv.Columns.Add("Nume", "Produs");
                dgv.Columns.Add("Categorie", "Categorie");
                dgv.Columns.Add("Pret", "Pret");
                foreach (var p in produse)
                    dgv.Rows.Add(p.Id, p.Nume, p.Categorie, p.Pret + " lei");
            }
            catch { dgv.Rows.Add("--", "Eroare BD", "--", "--"); }

            pnlContent.Controls.Add(lblTitlu);
            pnlContent.Controls.Add(btnAdauga);
            pnlContent.Controls.Add(dgv);
        }

        private void AdaugaProdus()
        {
            string nume = Microsoft.VisualBasic.Interaction.InputBox("Numele produsului:", "Adauga produs", "");
            if (string.IsNullOrWhiteSpace(nume)) return;
            string pretStr = Microsoft.VisualBasic.Interaction.InputBox("Pretul (lei):", "Adauga produs", "0");
            if (!decimal.TryParse(pretStr, out decimal pret)) return;
            MessageBox.Show($"Produs '{nume}' cu pretul {pret} lei adaugat!\n(Implementare completa - etapa urmatoare)",
                "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadProduse();
        }

        // ===== COMENZI =====
        private void LoadComenzi()
        {
            pnlContent.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = "Comenzi",
                Font = new Font("Poppins", 16, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var dgv = CreateDataGrid();
            dgv.Location = new Point(20, 70);
            dgv.Size = new Size(560, 380);

            try
            {
                var comenzi = DatabaseHelper.GetComenzi();
                dgv.Columns.Add("Id", "#");
                dgv.Columns.Add("Client", "Client");
                dgv.Columns.Add("Total", "Total");
                dgv.Columns.Add("Adresa", "Adresa");
                dgv.Columns.Add("Status", "Status");
                dgv.Columns.Add("Data", "Data");
                foreach (var c in comenzi)
                    dgv.Rows.Add(c.Id, c.NumeUtilizator, c.TotalPret + " lei",
                        c.AdresaLivrare, c.Status, c.DataComanda.ToString("dd.MM.yyyy HH:mm"));
            }
            catch { dgv.Rows.Add("--", "Eroare BD", "--", "--", "--", "--"); }

            pnlContent.Controls.Add(lblTitlu);
            pnlContent.Controls.Add(dgv);
        }

        // ===== UTILIZATORI =====
        private void LoadUtilizatori()
        {
            pnlContent.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = "Utilizatori",
                Font = new Font("Poppins", 16, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var dgv = CreateDataGrid();
            dgv.Location = new Point(20, 70);
            dgv.Size = new Size(560, 380);

            try
            {
                var utilizatori = DatabaseHelper.GetUtilizatori();
                dgv.Columns.Add("Id", "ID");
                dgv.Columns.Add("Username", "Username");
                dgv.Columns.Add("Nume", "Nume complet");
                dgv.Columns.Add("Rol", "Rol");
                dgv.Columns.Add("Telefon", "Telefon");
                foreach (var u in utilizatori)
                    dgv.Rows.Add(u.Id, u.NumeUtilizator, u.NumeComplet, u.Rol, u.Telefon);
            }
            catch { dgv.Rows.Add("--", "Eroare BD", "--", "--", "--"); }

            pnlContent.Controls.Add(lblTitlu);
            pnlContent.Controls.Add(dgv);
        }

        // ===== HELPER =====
        private Panel CreateStatCard(string titlu, string valoare, Color culoare)
        {
            var card = new Panel
            {
                Size = new Size(120, 80),
                BackColor = ColorAlbastruCard
            };
            RoundCorners(card, 10);

            var lblVal = new Label
            {
                Text = valoare,
                Font = new Font("Poppins", 22, FontStyle.Bold),
                ForeColor = culoare,
                AutoSize = true,
                Location = new Point(12, 10),
                BackColor = Color.Transparent
            };
            var lblTit = new Label
            {
                Text = titlu,
                Font = new Font("Poppins", 8),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point(12, 50),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblVal);
            card.Controls.Add(lblTit);
            return card;
        }

        private DataGridView CreateDataGrid()
        {
            var dgv = new DataGridView
            {
                BackgroundColor = ColorAlbastruCard,
                ForeColor = ColorAlb,
                GridColor = Color.FromArgb(30, 255, 255, 255),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Poppins", 9),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorPortocaliu;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ColorAlb;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Poppins", 9, FontStyle.Bold);
            dgv.DefaultCellStyle.BackColor = ColorAlbastruCard;
            dgv.DefaultCellStyle.ForeColor = ColorAlb;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 107, 0, 80);
            dgv.DefaultCellStyle.SelectionForeColor = ColorAlb;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(18, 28, 56);
            return dgv;
        }

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
}