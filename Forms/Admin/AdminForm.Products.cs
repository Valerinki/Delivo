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
            lblTitle.Height = 56;

            var row = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 64,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(0, 10, 0, 0),
                BackColor = Color.Transparent
            };

            var lblMsg = new Label
            {
                AutoSize = true,
                ForeColor = ColorDanger,
                Font = UiFont(10f, FontStyle.Bold),
                Margin = new Padding(16, 16, 0, 0),
                Visible = false,
                MaximumSize = new Size(600, 0)
            };

            var txtSearch = CreateStyledTextBox();
            txtSearch.PlaceholderText = "Caută după nume…";
            txtSearch.Width = 340;
            txtSearch.Margin = new Padding(0, 4, 14, 0);

            var cmbCat = CreateStyledCombo();
            cmbCat.Width = 250;
            cmbCat.Margin = new Padding(0, 4, 14, 0);
            cmbCat.Items.Add("Toate categoriile");
            foreach (var c in DatabaseHelper.GetCategorii())
                cmbCat.Items.Add(c.Nume);
            cmbCat.SelectedIndex = 0;

            var btnRefresh = CreateSecondaryGhostButton("↻  Refresh", 150);
            btnRefresh.Margin = new Padding(6, 4, 6, 0);
            btnRefresh.Click += (_, _) => LoadProduse(ProductListFocus.None);

            var btnAdd = CreatePrimaryOrangeButton("➕  Adaugă", 170);
            btnAdd.Margin = new Padding(6, 4, 6, 0);
            btnAdd.Click += (_, _) =>
            {
                SetActiveNavButton(_navButtons[1]);
                ShowProductForm(ProductFormMode.Add);
            };

            var btnEdit = CreateSecondaryGhostButton("✏️  Modifică", 170);
            btnEdit.Margin = new Padding(6, 4, 6, 0);
            btnEdit.Click += (_, _) =>
            {
                if (!TryGetSelectedProductId(dgv, out var id, out _))
                {
                    lblMsg.Text = "Selectează un produs din tabel pentru modificare.";
                    lblMsg.Visible = true;
                    return;
                }
                lblMsg.Visible = false;
                ShowProductForm(ProductFormMode.Edit, id);
            };

            var btnDel = CreateSecondaryGhostButton("🗑️  Șterge", 160);
            btnDel.Margin = new Padding(6, 4, 6, 0);
            btnDel.Click += (_, _) =>
            {
                if (!TryGetSelectedProductId(dgv, out var id, out var nume))
                {
                    lblMsg.Text = "Selectează un produs din tabel pentru ștergere.";
                    lblMsg.Visible = true;
                    return;
                }
                lblMsg.Visible = false;
                ShowDeleteProductPanel(bodyPanel, dgv, id, nume);
            };

            void ApplyFilters()
            {
                string q = txtSearch.Text.Trim().ToLowerInvariant();
                string? cat = cmbCat.SelectedIndex <= 0 ? null : cmbCat.SelectedItem?.ToString();
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;
                    var nume = row.Cells["Nume"].Value?.ToString() ?? "";
                    var categorie = row.Cells["Categorie"].Value?.ToString() ?? "";
                    bool okName = string.IsNullOrEmpty(q) || nume.ToLowerInvariant().Contains(q);
                    bool okCat = cat == null || categorie.Equals(cat, StringComparison.OrdinalIgnoreCase);
                    row.Visible = okName && okCat;
                }
            }

            txtSearch.TextChanged += (_, _) => ApplyFilters();
            cmbCat.SelectedIndexChanged += (_, _) => ApplyFilters();

            row.Controls.Add(txtSearch);
            row.Controls.Add(cmbCat);
            row.Controls.Add(btnRefresh);
            row.Controls.Add(btnAdd);
            row.Controls.Add(btnEdit);
            row.Controls.Add(btnDel);
            row.Controls.Add(lblMsg);

            bar.Controls.Add(row);
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
            pnlContent.Padding = new Padding(24, 18, 24, 24);

            var shell = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            EnableDoubleBuffer(shell);

            var card = new Panel
            {
                Width = 720,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = ColorAlbastruCard,
                Padding = new Padding(46, 40, 46, 40)
            };
            WireRoundedCardPaint(card, 22);
            card.Resize += (_, _) => ApplyRoundedRegion(card, 22);

            var lblErr = new Label
            {
                ForeColor = ColorDanger,
                Font = UiFont(10f, FontStyle.Bold),
                AutoSize = true,
                Visible = false,
                MaximumSize = new Size(620, 0)
            };

            var lblHead = new Label
            {
                Text = mode == ProductFormMode.Add ? "➕  Produs nou" : "✏️  Modifică produs",
                Font = UiFont(24f, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };

            var txtNume = CreateStyledTextBox();
            txtNume.Height = 48;
            var nudPret = new NumericUpDown
            {
                DecimalPlaces = 2,
                Maximum = 999999,
                Minimum = 0,
                Increment = 0.5M,
                Height = 48,
                BackColor = ColorInputBg,
                ForeColor = ColorAlb,
                Font = UiFont(11f),
                BorderStyle = BorderStyle.FixedSingle,
                ThousandsSeparator = true
            };

            var cmbCat = CreateStyledCombo();
            foreach (var c in DatabaseHelper.GetCategorii())
                cmbCat.Items.Add(new CatItem(c.Id, c.Nume));
            if (cmbCat.Items.Count > 0) cmbCat.SelectedIndex = 0;

            var txtDesc = CreateStyledTextBox(multiline: true);

            var pic = new PictureBox
            {
                Size = new Size(260, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = ColorInputBg,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnImg = CreateSecondaryGhostButton("Alege imagine (previzualizare)", 320);
            var lblImgNote = new Label
            {
                Text = "Imagine opțională — previzualizare locală (nu se salvează în baza de date).",
                ForeColor = ColorTextSecundar,
                Font = UiFont(9f),
                AutoSize = true,
                MaximumSize = new Size(480, 0)
            };

            btnImg.Click += (_, _) =>
            {
                using var ofd = new OpenFileDialog { Filter = "Imagini|*.png;*.jpg;*.jpeg;*.gif;*.bmp|Toate|*.*" };
                if (ofd.ShowDialog() != DialogResult.OK) return;
                try
                {
                    pic.Image?.Dispose();
                    pic.Image = Image.FromFile(ofd.FileName);
                    lblErr.Visible = false;
                }
                catch (Exception ex)
                {
                    lblErr.Text = "Imaginea nu a putut fi încărcată: " + ex.Message;
                    lblErr.Visible = true;
                }
            };

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

            var tlp = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 2,
                BackColor = Color.Transparent
            };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180f));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            int row = 0;
            void AddField(string caption, Control field)
            {
                var cap = new Label
                {
                    Text = caption,
                    Dock = DockStyle.Fill,
                    Font = UiFont(11f, FontStyle.Bold),
                    ForeColor = ColorTextSecundar,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(0, 14, 16, 0)
                };
                field.Margin = new Padding(0, 10, 0, 0);
                field.Dock = DockStyle.Fill;
                tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlp.Controls.Add(cap, 0, row);
                tlp.Controls.Add(field, 1, row);
                row++;
            }

            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlp.Controls.Add(lblHead, 0, row);
            tlp.SetColumnSpan(lblHead, 2);
            row++;

            AddField("Nume produs", txtNume);
            AddField("Preț (MDL)", nudPret);
            AddField("Categorie", cmbCat);
            AddField("Descriere", txtDesc);

            var imgStack = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            imgStack.Controls.Add(pic);
            imgStack.Controls.Add(btnImg);
            imgStack.Controls.Add(lblImgNote);
            AddField("Imagine", imgStack);

            tlp.Controls.Add(lblErr, 0, row);
            tlp.SetColumnSpan(lblErr, 2);
            row++;

            var flpBtns = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 28, 0, 0),
                BackColor = Color.Transparent
            };
            var btnSave = CreatePrimaryOrangeButton("Salvează", 180);
            btnSave.Height = 52;
            var btnCancel = CreateSecondaryGhostButton("Anulează", 160);
            btnCancel.Height = 52;
            flpBtns.Controls.Add(btnSave);
            flpBtns.Controls.Add(new Panel { Width = 16, Height = 10 });
            flpBtns.Controls.Add(btnCancel);

            tlp.Controls.Add(flpBtns, 0, row);
            tlp.SetColumnSpan(flpBtns, 2);

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

            card.Controls.Add(tlp);
            shell.Controls.Add(card);
            shell.Layout += (_, _) =>
            {
                card.Left = Math.Max(16, (shell.ClientSize.Width - card.Width) / 2);
                card.Top = Math.Max(16, (shell.ClientSize.Height - card.Height) / 2);
            };

            pnlContent.Controls.Add(shell);
        }
    }
}
