using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class OrdersForm : Form
    {
        private readonly Color ColorPortocaliu = Color.FromArgb(255, 107, 0);
        private readonly Color ColorAlbastruInchis = Color.FromArgb(13, 27, 62);
        private readonly Color ColorAlbastruCard = Color.FromArgb(22, 32, 64);
        private readonly Color ColorAlb = Color.White;
        private readonly Color ColorTextSecundar = Color.FromArgb(136, 153, 187);

        public OrdersForm()
        {
            BuildUI();
            LoadComenzi();
        }

        private Panel pnlList;

        private void BuildUI()
        {
            this.Text = "Delivo - Comenzile mele";
            this.Size = new Size(420, 700);
            this.BackColor = ColorAlbastruInchis;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = ColorAlbastruInchis };
            var lblLogo = new Label { Text = "DELIVO", Font = new Font("Showcard Gothic", 18, FontStyle.Bold), ForeColor = ColorPortocaliu, AutoSize = true, Location = new Point(16, 18) };
            var lblTitlu = new Label { Text = "Comenzile mele", Font = new Font("Poppins", 11, FontStyle.Bold), ForeColor = ColorAlb, AutoSize = true, Location = new Point(130, 20) };
            pnlHeader.Controls.Add(lblLogo);
            pnlHeader.Controls.Add(lblTitlu);

            pnlList = new Panel { Dock = DockStyle.Fill, BackColor = ColorAlbastruInchis, AutoScroll = true, Padding = new Padding(12, 10, 12, 10) };

            this.Controls.Add(pnlList);
            this.Controls.Add(pnlHeader);
        }

        private void LoadComenzi()
        {
            pnlList.Controls.Clear();
            try
            {
                var comenzi = DatabaseHelper.GetComenziUtilizator(LoginForm.UtilizatorLogat);
                int y = 10;
                foreach (var c in comenzi)
                {
                    var card = CreateOrderCard(c.Id, c.Status, c.TotalPret, c.DataComanda, c.AdresaLivrare);
                    card.Location = new Point(0, y);
                    pnlList.Controls.Add(card);
                    y += card.Height + 10;
                }
                if (comenzi.Count == 0)
                {
                    var lblEmpty = new Label { Text = "Nu ai comenzi inca.\nAdauga produse din meniu!", Font = new Font("Poppins", 11), ForeColor = ColorTextSecundar, AutoSize = true, Location = new Point(60, 100), TextAlign = ContentAlignment.MiddleCenter };
                    pnlList.Controls.Add(lblEmpty);
                }
            }
            catch (Exception ex)
            {
                var lbl = new Label { Text = "Eroare: " + ex.Message, Font = new Font("Poppins", 9), ForeColor = Color.Red, AutoSize = true, Location = new Point(10, 10) };
                pnlList.Controls.Add(lbl);
            }
        }

        private Panel CreateOrderCard(int id, string status, decimal total, DateTime data, string adresa)
        {
            var card = new Panel { Width = 370, Height = 90, BackColor = ColorAlbastruCard };
            RoundCorners(card, 12);

            Color statusColor = status switch
            {
                "In livrare" => Color.FromArgb(29, 158, 117),
                "Livrata" => ColorTextSecundar,
                _ => Color.FromArgb(239, 159, 39)
            };

            var lblId = new Label { Text = $"Comanda #{id}", Font = new Font("Poppins", 11, FontStyle.Bold), ForeColor = ColorAlb, AutoSize = true, Location = new Point(12, 10), BackColor = Color.Transparent };
            var lblStatus = new Label { Text = status, Font = new Font("Poppins", 9), ForeColor = statusColor, AutoSize = true, Location = new Point(240, 14), BackColor = Color.Transparent };
            var lblTotal = new Label { Text = $"{total} lei", Font = new Font("Poppins", 11, FontStyle.Bold), ForeColor = ColorPortocaliu, AutoSize = true, Location = new Point(12, 40), BackColor = Color.Transparent };
            var lblData = new Label { Text = data.ToString("dd.MM.yyyy  HH:mm"), Font = new Font("Poppins", 9), ForeColor = ColorTextSecundar, AutoSize = true, Location = new Point(12, 65), BackColor = Color.Transparent };
            var lblAdresa = new Label { Text = adresa, Font = new Font("Poppins", 8), ForeColor = ColorTextSecundar, AutoSize = true, Location = new Point(200, 65), BackColor = Color.Transparent };

            card.Controls.Add(lblId);
            card.Controls.Add(lblStatus);
            card.Controls.Add(lblTotal);
            card.Controls.Add(lblData);
            card.Controls.Add(lblAdresa);
            return card;
        }

        private void RoundCorners(Control ctrl, int radius)
        {
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