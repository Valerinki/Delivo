using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Delivo.Data;
using Delivo.Models;

namespace Delivo.Forms
{
    public partial class AdminForm
    {
        private void LoadComenzi()
        {
            DisposePnlContentChildren();
            pnlContent.Padding = new Padding(24, 18, 24, 24);

            var root = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            EnableDoubleBuffer(root);

            // ── Top block: titlu + toolbar ────────────────────────────
            var topBlock = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 12)
            };

            var lblTitle = CreateSectionTitle("📦  Gestionare comenzi");
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 52;

            // ── Toolbar card ──────────────────────────────────────────
            var toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = ColorAlbastruCard,
                Padding = new Padding(16, 0, 16, 0)
            };
            WireRoundedCardPaint(toolbarPanel, 14);
            toolbarPanel.Resize += (_, _) => ApplyRoundedRegion(toolbarPanel, 14);

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            void AddCentered(Control c, int rightMargin = 10)
            {
                c.Margin = new Padding(0, (56 - c.Height) / 2, rightMargin, 0);
                flow.Controls.Add(c);
            }

            var btnRefresh = CreateSecondaryGhostButton("↻  Refresh", 130);
            btnRefresh.Height = 36;
            btnRefresh.Click += (_, _) => LoadComenzi();

            // Informație utilă în toolbar
            var lblInfo = new Label
            {
                Text = "💡  Poți modifica statusul comenzii direct din coloana Status din tabel",
                Font = UiFont(10f),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            lblInfo.Margin = new Padding(16, (56 - 20) / 2, 0, 0);

            AddCentered(btnRefresh, 16);
            flow.Controls.Add(lblInfo);
            toolbarPanel.Controls.Add(flow);

            topBlock.Controls.Add(toolbarPanel);
            topBlock.Controls.Add(lblTitle);

            // ── Body ──────────────────────────────────────────────────
            var body = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            try
            {
                var comenzi = DatabaseHelper.GetComenzi();

                if (comenzi.Count == 0)
                {
                    body.Controls.Add(BuildOrdersEmptyState());
                }
                else
                {
                    var dgv = CreatePremiumDataGridView(readOnly: false);
                    dgv.Name = "dgvComenzi";
                    dgv.ReadOnly = false;
                    dgv.EditMode = DataGridViewEditMode.EditOnEnter;

                    dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", FillWeight = 25, MinimumWidth = 70, ReadOnly = true });
                    dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Client", HeaderText = "Client", FillWeight = 72, MinimumWidth = 150, ReadOnly = true });
                    dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Data", HeaderText = "Data comandă", FillWeight = 70, MinimumWidth = 150, ReadOnly = true });
                    dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", FillWeight = 46, MinimumWidth = 120, ReadOnly = true });
                    dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Adresa", HeaderText = "Adresă livrare", FillWeight = 110, MinimumWidth = 220, ReadOnly = true });

                    var colStatus = new DataGridViewComboBoxColumn
                    {
                        Name = "Status",
                        HeaderText = "Status",
                        FillWeight = 66,
                        MinimumWidth = 150,
                        FlatStyle = FlatStyle.Flat,
                        DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                    };
                    colStatus.Items.AddRange(StatusComenziAdmin);
                    foreach (var c in comenzi)
                    {
                        var n = NormalizeStatusForAdmin(c.Status);
                        if (!colStatus.Items.Contains(n))
                            colStatus.Items.Add(n);
                    }
                    dgv.Columns.Add(colStatus);

                    foreach (var c in comenzi)
                    {
                        int i = dgv.Rows.Add(
                            c.Id, c.NumeUtilizator,
                            c.DataComanda.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                            c.TotalPret.ToString("N2", CultureInfo.InvariantCulture) + " MDL",
                            c.AdresaLivrare,
                            NormalizeStatusForAdmin(c.Status));
                        dgv.Rows[i].Tag = c;
                    }

                    foreach (DataGridViewColumn col in dgv.Columns)
                        if (col.Name != "Status") col.ReadOnly = true;

                    dgv.CellFormatting += (_, e) =>
                    {
                        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                        if (dgv.Columns[e.ColumnIndex].Name != "Status") return;
                        var status = e.Value?.ToString() ?? "";
                        var c = GetOrderStatusColor(status);
                        e.CellStyle.BackColor = Color.FromArgb(210, c);
                        e.CellStyle.ForeColor = ColorAlb;
                        e.CellStyle.Font = UiFont(10.5f, FontStyle.Bold);
                        e.CellStyle.SelectionBackColor = Color.FromArgb(200, c);
                        e.CellStyle.SelectionForeColor = ColorAlb;
                    };

                    dgv.CellEndEdit += (_, e) =>
                    {
                        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                        if (dgv.Columns[e.ColumnIndex].Name != "Status") return;
                        if (dgv.Rows[e.RowIndex].Tag is not Comanda co) return;
                        var newStatus = dgv.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "";
                        try
                        {
                            if (DatabaseHelper.ActualizeazaStatusComanda(co.Id, newStatus))
                                co.Status = newStatus;
                        }
                        catch
                        {
                            dgv.Rows[e.RowIndex].Cells["Status"].Value = NormalizeStatusForAdmin(co.Status);
                        }
                    };

                    DockGridFullWidth(dgv, body);
                }
            }
            catch (Exception ex)
            {
                body.Controls.Add(new Label
                {
                    Text = "Eroare la încărcarea comenzilor: " + ex.Message,
                    ForeColor = ColorDanger,
                    Font = UiFont(11f),
                    Dock = DockStyle.Top,
                    Height = 48,
                    Padding = new Padding(12, 12, 0, 0)
                });
            }

            root.Controls.Add(body);
            root.Controls.Add(topBlock);
            pnlContent.Controls.Add(root);
        }

        private static string NormalizeStatusForAdmin(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return StatusComenziAdmin[0];
            var s = status.Trim();
            foreach (var a in StatusComenziAdmin)
                if (string.Equals(a, s, StringComparison.OrdinalIgnoreCase))
                    return a;

            return s switch
            {
                "In asteptare" or "În asteptare" => "În așteptare",
                "In preparare" or "În preparare" => "În preparare",
                "In livrare" or "În livrare" => "În livrare",
                "Livrata" or "Livrată" => "Livrată",
                _ => s
            };
        }

        private Panel BuildOrdersEmptyState()
        {
            var wrap = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            EnableDoubleBuffer(wrap);

            var card = new Panel
            {
                Size = new Size(520, 260),
                BackColor = ColorAlbastruCard,
                Padding = new Padding(40, 36, 40, 36)
            };
            WireRoundedCardPaint(card, 20);
            card.Resize += (_, _) => ApplyRoundedRegion(card, 20);

            var lblIcon = new Label
            {
                Text = "📭",
                Font = UiFont(44f),
                AutoSize = false,
                Size = new Size(440, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 28),
                BackColor = Color.Transparent
            };

            // Linie separator sub icon
            var sep = new Panel
            {
                Size = new Size(440, 1),
                Location = new Point(40, 104),
                BackColor = Color.FromArgb(40, 255, 255, 255)
            };

            var lblMain = new Label
            {
                Text = "Nu există comenzi momentan",
                Font = UiFont(18f, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = false,
                Size = new Size(440, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 116),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text = "Comenzile plasate de clienți vor apărea aici.\nStatusul poate fi modificat direct din tabel.",
                Font = UiFont(10.5f),
                ForeColor = ColorTextSecundar,
                AutoSize = false,
                Size = new Size(440, 52),
                TextAlign = ContentAlignment.TopCenter,
                Location = new Point(40, 164),
                BackColor = Color.Transparent
            };

            card.Controls.Add(lblIcon);
            card.Controls.Add(sep);
            card.Controls.Add(lblMain);
            card.Controls.Add(lblSub);

            wrap.Controls.Add(card);
            wrap.Layout += (_, _) =>
            {
                card.Left = Math.Max(0, (wrap.ClientSize.Width - card.Width) / 2);
                card.Top = Math.Max(0, (wrap.ClientSize.Height - card.Height) / 2);
            };

            return wrap;
        }
    }
}