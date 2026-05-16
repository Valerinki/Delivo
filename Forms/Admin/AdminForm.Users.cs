using System;
using System.Drawing;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class AdminForm
    {
        private void LoadUtilizatori()
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
                Height = 126,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 12)
            };

            var lblTitle = CreateSectionTitle("👥  Utilizatori Delivo");
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 58;

            var row = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 64,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0)
            };

            var txtSearch = CreateStyledTextBox();
            txtSearch.PlaceholderText = "Caută utilizator…";
            txtSearch.Width = 420;
            txtSearch.Margin = new Padding(0, 4, 14, 0);

            var btnRefresh = CreateSecondaryGhostButton("↻  Refresh", 150);
            btnRefresh.Margin = new Padding(6, 4, 0, 0);
            btnRefresh.Click += (_, _) => LoadUtilizatori();

            row.Controls.Add(txtSearch);
            row.Controls.Add(btnRefresh);

            topBlock.Controls.Add(row);
            topBlock.Controls.Add(lblTitle);

            var dgv = CreatePremiumDataGridView(readOnly: true);
            dgv.Name = "dgvUsers";

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", FillWeight = 24, MinimumWidth = 70 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "User", HeaderText = "Nume utilizator", FillWeight = 90, MinimumWidth = 170 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nume", HeaderText = "Nume complet", FillWeight = 105, MinimumWidth = 220 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Rol", HeaderText = "Rol", FillWeight = 44, MinimumWidth = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tel", HeaderText = "Telefon", FillWeight = 62, MinimumWidth = 150 });

            dgv.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (dgv.Columns[e.ColumnIndex].Name != "Rol") return;
                var rol = e.Value?.ToString() ?? "";
                bool admin = rol.Equals("admin", StringComparison.OrdinalIgnoreCase);
                e.CellStyle.BackColor = admin
                    ? Color.FromArgb(200, 200, 70, 50)
                    : Color.FromArgb(200, 45, 100, 180);
                e.CellStyle.ForeColor = ColorAlb;
                e.CellStyle.Font = UiFont(10.5f, FontStyle.Bold);
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                e.CellStyle.SelectionForeColor = ColorAlb;
            };

            void ReloadRows()
            {
                dgv.Rows.Clear();
                var users = DatabaseHelper.GetUtilizatori();
                string q = txtSearch.Text.Trim().ToLowerInvariant();
                foreach (var u in users)
                {
                    if (!string.IsNullOrEmpty(q))
                    {
                        var blob = (u.NumeUtilizator + " " + u.NumeComplet + " " + u.Rol + " " + u.Telefon).ToLowerInvariant();
                        if (!blob.Contains(q)) continue;
                    }
                    dgv.Rows.Add(u.Id, u.NumeUtilizator, u.NumeComplet, u.Rol, u.Telefon);
                }
            }

            txtSearch.TextChanged += (_, _) => ReloadRows();

            var body = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            DockGridFullWidth(dgv, body);

            root.Controls.Add(body);
            root.Controls.Add(topBlock);

            try { ReloadRows(); }
            catch (Exception ex)
            {
                body.Controls.Add(new Label
                {
                    Text = "Eroare la încărcarea utilizatorilor: " + ex.Message,
                    ForeColor = ColorDanger,
                    Font = UiFont(11f),
                    Dock = DockStyle.Top,
                    Height = 40
                });
            }

            pnlContent.Controls.Add(root);
        }
    }
}
