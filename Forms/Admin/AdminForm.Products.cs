using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class AdminForm
    {
        private void LoadProduse(ProductListFocus productFocus = ProductListFocus.None)
        {
            DisposePnlContentChildren();
            pnlContent.Padding = new Padding(24, 18, 24, 24);

            var root = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            EnableDoubleBuffer(root);

            var topBlock = new Panel
            {
                Dock = DockStyle.Top,
                Height = productFocus != ProductListFocus.None ? 188 : 132,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 12)
            };

            if (productFocus != ProductListFocus.None)
            {
                var hint = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 50,
                    BackColor = Color.FromArgb(48, 255, 170, 60),
                    Margin = new Padding(0, 0, 0, 10)
                };
                ApplyRoundedRegion(hint, 10);
                hint.Controls.Add(new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = ColorAlb,
                    Font = UiFont(10.5f, FontStyle.Bold),
                    BackColor = Color.Transparent,
                    Text = productFocus == ProductListFocus.Edit
                        ? "Selectează un rând din tabel, apoi apasă „Modifică produs” din bara de unelte."
                        : "Selectează un rând din tabel, apoi apasă „Șterge produs” din bara de unelte."
                });
                topBlock.Controls.Add(hint);
            }

            var dgv = CreatePremiumDataGridView(readOnly: true);
            dgv.Name = "dgvProduse";

            var body = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            DockGridFullWidth(dgv, body);

            BuildProductsToolbar(topBlock, body, dgv);

            root.Controls.Add(body);
            root.Controls.Add(topBlock);
            pnlContent.Controls.Add(root);

            try
            {
                BuildProductsTable(dgv);
            }
            catch (Exception ex)
            {
                body.Controls.Add(new Label
                {
                    Text = "Nu s-au putut încărca produsele: " + ex.Message,
                    ForeColor = ColorDanger,
                    Font = UiFont(11f),
                    Dock = DockStyle.Top,
                    Height = 40,
                    Padding = new Padding(12, 8, 0, 0)
                });
            }
        }

        private void BuildProductsToolbar(Panel topBlock, Panel bodyPanel, DataGridView dgv)
        {
            var bar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            var lblTitle = CreateSectionTitle("🍕  Catalog produse");
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 52;

            // ── Bara de filtre + acțiuni ──────────────────────────────
            var toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = ColorAlbastruCard,
                Padding = new Padding(16, 0, 16, 0),
                Margin = new Padding(0, 0, 0, 12)
            };
            WireRoundedCardPaint(toolbarPanel, 14);
            toolbarPanel.Resize += (_, _) => ApplyRoundedRegion(toolbarPanel, 14);

            var lblMsg = new Label
            {
                AutoSize = true,
                ForeColor = ColorDanger,
                Font = UiFont(10f, FontStyle.Bold),
                Visible = false,
                MaximumSize = new Size(400, 0)
            };

            // ── Search box ────────────────────────────────────────────
            var txtSearch = CreateStyledTextBox();
            txtSearch.PlaceholderText = "🔍  Caută după nume…";
            txtSearch.Width = 260;
            txtSearch.Height = 36;

            // ── Combo categorii ───────────────────────────────────────
            var cmbCat = CreateStyledCombo();
            cmbCat.Width = 200;
            cmbCat.Height = 36;
            cmbCat.Items.Add("Toate categoriile");
            foreach (var c in DatabaseHelper.GetCategorii())
                cmbCat.Items.Add(c.Nume);
            cmbCat.SelectedIndex = 0;

            // ── Separator vertical ────────────────────────────────────
            var sepV = new Panel
            {
                Width = 1,
                BackColor = Color.FromArgb(40, 255, 255, 255)
            };

            // ── Butoane acțiuni ───────────────────────────────────────
            var btnRefresh = CreateSecondaryGhostButton("↻  Refresh", 130);
            btnRefresh.Height = 36;
            btnRefresh.Click += (_, _) => LoadProduse(ProductListFocus.None);

            var btnAdd = CreatePrimaryOrangeButton("➕  Adaugă", 150);
            btnAdd.Height = 36;
            btnAdd.Click += (_, _) =>
            {
                SetActiveNavButton(_navButtons[1]);
                ShowProductForm(ProductFormMode.Add);
            };

            var btnEdit = CreateSecondaryGhostButton("✏️  Modifică", 150);
            btnEdit.Height = 36;
            btnEdit.Click += (_, _) =>
            {
                if (!TryGetSelectedProductId(dgv, out var id, out _))
                {
                    lblMsg.Text = "Selectează un produs pentru modificare.";
                    lblMsg.Visible = true;
                    return;
                }
                lblMsg.Visible = false;
                ShowProductForm(ProductFormMode.Edit, id);
            };

            var btnDel = CreateSecondaryGhostButton("🗑️  Șterge", 140);
            btnDel.Height = 36;
            btnDel.Click += (_, _) =>
            {
                if (!TryGetSelectedProductId(dgv, out var id, out var nume))
                {
                    lblMsg.Text = "Selectează un produs pentru ștergere.";
                    lblMsg.Visible = true;
                    return;
                }
                lblMsg.Visible = false;
                ShowDeleteProductPanel(bodyPanel, dgv, id, nume);
            };

            // ── Layout intern toolbar: tot centrat vertical ───────────
            toolbarPanel.Paint += (_, e) => { }; // trigger double buffer

            // Folosim un FlowLayoutPanel cu aliniere verticală centrată
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            // Helper: centrează vertical orice control în flow de 56px
            void AddCentered(Control c, int rightMargin = 10)
            {
                c.Margin = new Padding(0, (56 - c.Height) / 2, rightMargin, 0);
                flow.Controls.Add(c);
            }

            AddCentered(txtSearch, 10);
            AddCentered(cmbCat, 16);

            // Separator vertical manual
            sepV.Height = 32;
            sepV.Margin = new Padding(0, (56 - 32) / 2, 16, 0);
            flow.Controls.Add(sepV);

            AddCentered(btnRefresh, 8);
            AddCentered(btnAdd, 8);
            AddCentered(btnEdit, 8);
            AddCentered(btnDel, 0);

            toolbarPanel.Controls.Add(flow);

            // ── Mesaj eroare sub toolbar ──────────────────────────────
            var msgPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 28,
                BackColor = Color.Transparent
            };
            lblMsg.Location = new Point(4, 4);
            msgPanel.Controls.Add(lblMsg);

            void ApplyFilters()
            {
                string q = txtSearch.Text.Trim().ToLowerInvariant();
                string? cat = cmbCat.SelectedIndex <= 0 ? null : cmbCat.SelectedItem?.ToString();
                foreach (DataGridViewRow r in dgv.Rows)
                {
                    if (r.IsNewRow) continue;
                    var nume = r.Cells["Nume"].Value?.ToString() ?? "";
                    var categorie = r.Cells["Categorie"].Value?.ToString() ?? "";
                    bool okName = string.IsNullOrEmpty(q) || nume.ToLowerInvariant().Contains(q);
                    bool okCat = cat == null || categorie.Equals(cat, StringComparison.OrdinalIgnoreCase);
                    r.Visible = okName && okCat;
                }
            }

            txtSearch.TextChanged += (_, _) => ApplyFilters();
            cmbCat.SelectedIndexChanged += (_, _) => ApplyFilters();

            // ── Asamblare ─────────────────────────────────────────────
            // Ordinea în DockStyle.Top: ultimul adăugat = primul sus
            bar.Controls.Add(msgPanel);
            bar.Controls.Add(toolbarPanel);
            bar.Controls.Add(lblTitle);
            topBlock.Controls.Add(bar);
        }

        private static bool TryGetSelectedProductId(DataGridView dgv, out int id, out string nume)
        {
            id = 0;
            nume = "";
            if (dgv.CurrentRow == null || dgv.CurrentRow.IsNewRow)
                return false;
            var idObj = dgv.CurrentRow.Cells["Id"].Value;
            if (idObj == null || !int.TryParse(idObj.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
                return false;
            nume = dgv.CurrentRow.Cells["Nume"].Value?.ToString() ?? "";
            return true;
        }

        private void BuildProductsTable(DataGridView dgv)
        {
            dgv.Columns.Clear();
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", FillWeight = 26, MinimumWidth = 70 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nume", HeaderText = "Nume produs", FillWeight = 150, MinimumWidth = 220 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Pret", HeaderText = "Preț", FillWeight = 54, MinimumWidth = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Categorie", HeaderText = "Categorie", FillWeight = 82, MinimumWidth = 150 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Activ", HeaderText = "Activ", FillWeight = 42, MinimumWidth = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descriere", HeaderText = "Descriere", Visible = false });

            foreach (var p in DatabaseHelper.GetProduseAdmin())
            {
                dgv.Rows.Add(
                    p.Id, p.Nume,
                    p.Pret.ToString("N2", CultureInfo.InvariantCulture) + " MDL",
                    p.Categorie, p.Disponibil ? "Da" : "Nu", p.Descriere);
            }
        }

        private void ShowDeleteProductPanel(Panel bodyPanel, DataGridView dgv, int id, string nume)
        {
            var veil = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(230, 8, 14, 36)
            };
            EnableDoubleBuffer(veil);

            var card = new Panel
            {
                Size = new Size(500, 240),
                BackColor = ColorAlbastruCard,
                Padding = new Padding(32, 28, 32, 28)
            };
            WireRoundedCardPaint(card, 20);
            card.Resize += (_, _) => ApplyRoundedRegion(card, 20);

            var lbl = new Label
            {
                Text = "Sigur doriți să eliminați produsul din catalog?\n\n" + nume,
                Font = UiFont(12f, FontStyle.Bold),
                ForeColor = ColorAlb,
                Location = new Point(28, 24),
                Size = new Size(440, 90),
                BackColor = Color.Transparent
            };

            var btnDa = CreatePrimaryOrangeButton("Elimină", 160);
            btnDa.Location = new Point(28, 150);
            var btnNu = CreateSecondaryGhostButton("Anulează", 160);
            btnNu.Location = new Point(208, 150);

            void RemoveVeil()
            {
                if (veil.Parent != null)
                {
                    bodyPanel.Controls.Remove(veil);
                    veil.Dispose();
                }
            }

            btnNu.Click += (_, _) => RemoveVeil();
            btnDa.Click += (_, _) =>
            {
                try { DatabaseHelper.StergeProdusSoft(id); }
                finally { RemoveVeil(); LoadProduse(ProductListFocus.None); }
            };

            card.Controls.Add(lbl);
            card.Controls.Add(btnDa);
            card.Controls.Add(btnNu);
            veil.Controls.Add(card);
            veil.Layout += (_, _) =>
            {
                card.Left = (veil.Width - card.Width) / 2;
                card.Top = (veil.Height - card.Height) / 2;
            };

            bodyPanel.Controls.Add(veil);
            veil.BringToFront();
        }

        private sealed class CatItem
        {
            public int Id { get; }
            public string Nume { get; }
            public CatItem(int id, string nume) { Id = id; Nume = nume; }
            public override string ToString() => Nume;
        }

        private void ShowProductForm(ProductFormMode mode, int? productId = null)
        {
            DisposePnlContentChildren();
            pnlContent.Padding = new Padding(0); // ✅ fără padding exterior

            // ── Shell full-fill, fără AutoScroll ──────────────────────
            var shell = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            EnableDoubleBuffer(shell);

            // ── Card central ──────────────────────────────────────────
            var card = new Panel
            {
                Width = 680,
                Height = 620,                          // ✅ înălțime fixă — fără scroll
                BackColor = ColorAlbastruCard,
                Padding = new Padding(48, 36, 48, 36)
            };
            WireRoundedCardPaint(card, 22);
            card.Resize += (_, _) => ApplyRoundedRegion(card, 22);

            // ── Header card ───────────────────────────────────────────
            var lblHead = new Label
            {
                Text = mode == ProductFormMode.Add ? "➕  Produs nou" : "✏️  Modifică produs",
                Font = UiFont(22f, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = false,
                Size = new Size(584, 48),
                Location = new Point(48, 28),
                TextAlign = ContentAlignment.MiddleCenter,  // ✅ centrat
                BackColor = Color.Transparent
            };

            var sep = new Panel
            {
                Size = new Size(584, 1),
                Location = new Point(48, 84),
                BackColor = Color.FromArgb(40, 255, 255, 255)
            };

            // ── Mesaj eroare ──────────────────────────────────────────
            var lblErr = new Label
            {
                ForeColor = ColorDanger,
                Font = UiFont(10f, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(584, 24),
                Location = new Point(48, 92),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false,
                BackColor = Color.Transparent
            };

            // ── Grid 2 coloane: label | input ─────────────────────────
            var tlp = new TableLayoutPanel
            {
                Size = new Size(584, 380),
                Location = new Point(48, 120),
                ColumnCount = 2,
                RowCount = 4,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140f));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            for (int i = 0; i < 4; i++)
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 72f));

            // ── Câmpuri ───────────────────────────────────────────────
            var txtNume = CreateStyledTextBox();
            txtNume.Height = 44;

            var nudPret = new NumericUpDown
            {
                DecimalPlaces = 2,
                Maximum = 999999,
                Minimum = 0,
                Increment = 0.5M,
                Height = 44,
                BackColor = ColorInputBg,
                ForeColor = ColorAlb,
                Font = UiFont(11f),
                BorderStyle = BorderStyle.FixedSingle,
                ThousandsSeparator = true
            };

            var cmbCat = CreateStyledCombo();
            cmbCat.Height = 44;
            foreach (var c in DatabaseHelper.GetCategorii())
                cmbCat.Items.Add(new CatItem(c.Id, c.Nume));
            if (cmbCat.Items.Count > 0) cmbCat.SelectedIndex = 0;

            var txtDesc = CreateStyledTextBox(multiline: true);
            txtDesc.Height = 80;

            // Helper: adaugă label + control în grid
            void AddRow(int row, string caption, Control ctrl)
            {
                var lbl = new Label
                {
                    Text = caption,
                    Font = UiFont(10.5f, FontStyle.Bold),
                    ForeColor = ColorTextSecundar,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0, 0, 16, 0)
                };
                ctrl.Dock = DockStyle.Fill;
                ctrl.Margin = new Padding(0, 12, 0, 12);
                tlp.Controls.Add(lbl, 0, row);
                tlp.Controls.Add(ctrl, 1, row);
            }

            AddRow(0, "Nume produs", txtNume);
            AddRow(1, "Preț (MDL)", nudPret);
            AddRow(2, "Categorie", cmbCat);
            AddRow(3, "Descriere", txtDesc);

            // ── Precompletare la Edit ─────────────────────────────────
            if (mode == ProductFormMode.Edit && productId is int pid)
            {
                var p = DatabaseHelper.GetProduseAdmin().FirstOrDefault(x => x.Id == pid);
                if (p.Id == 0)
                {
                    lblErr.Text = "Produsul nu a fost găsit.";
                    lblErr.Visible = true;
                }
                else
                {
                    txtNume.Text = p.Nume;
                    nudPret.Value = Math.Clamp(p.Pret, nudPret.Minimum, nudPret.Maximum);
                    txtDesc.Text = p.Descriere;
                    for (int i = 0; i < cmbCat.Items.Count; i++)
                    {
                        if (cmbCat.Items[i] is CatItem ci && ci.Id == p.CategorieId)
                        { cmbCat.SelectedIndex = i; break; }
                    }
                }
            }

            // ── Butoane centrate ──────────────────────────────────────
            var btnSave = CreatePrimaryOrangeButton("  Salvează", 180);
            btnSave.Height = 48;
            btnSave.Location = new Point(48 + 584 / 2 - 188, 516);   // centrat

            var btnCancel = CreateSecondaryGhostButton("  Anulează", 160);
            btnCancel.Height = 48;
            btnCancel.Location = new Point(48 + 584 / 2 + 8, 516);

            // ── Logică butoane ────────────────────────────────────────
            btnCancel.Click += (_, _) => LoadProduse(ProductListFocus.None);

            btnSave.Click += (_, _) =>
            {
                lblErr.Visible = false;
                if (string.IsNullOrWhiteSpace(txtNume.Text))
                {
                    lblErr.Text = "Introduceți numele produsului.";
                    lblErr.Visible = true;
                    return;
                }
                if (cmbCat.SelectedItem is not CatItem cat)
                {
                    lblErr.Text = "Selectați o categorie.";
                    lblErr.Visible = true;
                    return;
                }
                try
                {
                    if (mode == ProductFormMode.Add)
                    {
                        if (!DatabaseHelper.AdaugaProdus(txtNume.Text.Trim(), txtDesc.Text.Trim(), nudPret.Value, cat.Id))
                            throw new InvalidOperationException("Inserarea a eșuat.");
                    }
                    else if (productId is int eid)
                    {
                        if (!DatabaseHelper.ActualizeazaProdus(eid, txtNume.Text.Trim(), txtDesc.Text.Trim(), nudPret.Value, cat.Id))
                            throw new InvalidOperationException("Actualizarea a eșuat.");
                    }
                }
                catch (Exception ex)
                {
                    lblErr.Text = ex.Message;
                    lblErr.Visible = true;
                    return;
                }
                LoadProduse(ProductListFocus.None);
            };

            // ── Asamblare card ────────────────────────────────────────
            card.Controls.Add(lblHead);
            card.Controls.Add(sep);
            card.Controls.Add(lblErr);
            card.Controls.Add(tlp);
            card.Controls.Add(btnSave);
            card.Controls.Add(btnCancel);

            // ── Centrare card în shell ────────────────────────────────
            void CenterCard(object? s, EventArgs e)
            {
                card.Left = Math.Max(24, (shell.ClientSize.Width - card.Width) / 2);
                card.Top = Math.Max(24, (shell.ClientSize.Height - card.Height) / 2);
            }
            shell.Resize += CenterCard;
            shell.HandleCreated += CenterCard;
            shell.Controls.Add(card);
            pnlContent.Controls.Add(shell);
            CenterCard(null, EventArgs.Empty);
        }
    }
}
