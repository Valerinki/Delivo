using System;
using System.Drawing;
using System.Windows.Forms;

namespace Delivo.Forms
{
    public partial class MainForm
    {
        private void PnlNavbar_Paint(object? sender, PaintEventArgs e)
        {
            using var p = new Pen(Color.FromArgb(40, 255, 255, 255), 1);
            e.Graphics.DrawLine(p, 0, 0, pnlNavbar.Width, 0);
        }

        private void PnlHeader_Resize(object? sender, EventArgs e)
        {
            if (lblGreet == null) return;
            // repoziționează lblGreet (btnHam nu mai e câmp, îl căutăm în pnlHeader)
            foreach (Control c in pnlHeader.Controls)
            {
                if (c is Button btn && btn.Text == "≡")
                {
                    lblGreet.Location = new Point(pnlHeader.Width - lblGreet.PreferredWidth - 62, 24);
                    btn.Location = new Point(pnlHeader.Width - 54, 11);
                    break;
                }
            }
        }

        private void TxtSearch_GotFocus(object? sender, EventArgs e)
        {
            if (txtSearch.Text.TrimStart().StartsWith("Caută"))
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void TxtSearch_LostFocus(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "   Caută pizza, burgeri, sushi...";
                txtSearch.ForeColor = Color.FromArgb(130, 130, 130);
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e) => FilterSearch(txtSearch.Text);

        private void MainForm_Resize(object? sender, EventArgs e) => Relayout();
    }
}