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

            var lblTitle = CreateSectionTitle("📦  Gestionare comenzi");
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 64;

            try
            {
                var comenzi = DatabaseHelper.GetComenzi();
                if (comenzi.Count == 0)
                {
                    root.Controls.Add(BuildOrdersEmptyState());
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

                    var body = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
                    DockGridFullWidth(dgv, body);
                    root.Controls.Add(body);
                }
            }
            catch (Exception ex)
            {
                root.Controls.Add(new Label
                {
                    Text = "Eroare la încărcarea comenzilor: " + ex.Message,
                    ForeColor = ColorDanger,
                    Font = UiFont(11f),
                    Dock = DockStyle.Top,
                    Height = 48,
                    Padding = new Padding(12, 12, 0, 0)
                });
            }

            root.Controls.Add(lblTitle);
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
                BackColor = Color.Transparent,
                Padding = new Padding(24, 48, 24, 24)
            };

            var inner = new Panel
            {
                Size = new Size(560, 300),
                BackColor = ColorAlbastruCard
            };
            WireRoundedCardPaint(inner, 20);
            inner.Resize += (_, _) => ApplyRoundedRegion(inner, 20);

            var icon = new Label
            {
                Text = "📭",
                Font = UiFont(48f),
                AutoSize = true,
                Location = new Point(252, 40),
                BackColor = Color.Transparent
            };

            var lbl = new Label
            {
                Text = "Nu există comenzi momentan",
                Font = UiFont(20f, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = false,
                Size = new Size(520, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 120),
                BackColor = Color.Transparent
            };

            var sub = new Label
            {
                Text = "Comenzile clienților vor apărea aici.\nPoți modifica statusul direct din tabel.",
                Font = UiFont(11f),
                ForeColor = ColorTextSecundar,
                AutoSize = false,
                Size = new Size(480, 60),
                TextAlign = ContentAlignment.TopCenter,
                Location = new Point(40, 168),
                BackColor = Color.Transparent
            };

            inner.Controls.Add(icon);
            inner.Controls.Add(lbl);
            inner.Controls.Add(sub);
            wrap.Controls.Add(inner);

            wrap.Layout += (_, _) =>
            {
                inner.Left = (wrap.ClientSize.Width - inner.Width) / 2;
                inner.Top = (wrap.ClientSize.Height - inner.Height) / 2;
            };

            return wrap;
        }
    }
}
