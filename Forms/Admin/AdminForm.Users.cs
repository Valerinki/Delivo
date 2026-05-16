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
                Height = 120,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 12)
            };

            // ── Titlu ─────────────────────────────────────────────────
            var lblTitle = CreateSectionTitle("👥  Utilizatori Delivo");
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

            // Helper centrare verticală
            void AddCentered(Control c, int rightMargin = 10)
            {
                c.Margin = new Padding(0, (56 - c.Height) / 2, rightMargin, 0);
                flow.Controls.Add(c);
            }

            // ── Search ────────────────────────────────────────────────
            var txtSearch = CreateStyledTextBox();
            txtSearch.PlaceholderText = "🔍  Caută utilizator…";
            txtSearch.Width = 380;
            txtSearch.Height = 36;

            // ── Separator ─────────────────────────────────────────────
            var sepV = new Panel
            {
                Width = 1,
                Height = 32,
                BackColor = Color.FromArgb(40, 255, 255, 255)
            };

            // ── Refresh ───────────────────────────────────────────────
            var btnRefresh = CreateSecondaryGhostButton("↻  Refresh", 130);
            btnRefresh.Height = 36;
            btnRefresh.Click += (_, _) => LoadUtilizatori();

            AddCentered(txtSearch, 16);
            sepV.Margin = new Padding(0, (56 - 32) / 2, 16, 0);
            flow.Controls.Add(sepV);
            AddCentered(btnRefresh, 0);

            toolbarPanel.Controls.Add(flow);

            topBlock.Controls.Add(toolbarPanel);
            topBlock.Controls.Add(lblTitle);

            // ── Tabel ─────────────────────────────────────────────────
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
                e.CellStyle.BackColor = admin ? Color.FromArgb(200, 200, 70, 50) : Color.FromArgb(200, 45, 100, 180);
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
