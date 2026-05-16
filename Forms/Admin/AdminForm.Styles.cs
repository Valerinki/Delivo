using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Delivo.Forms
{
    public partial class AdminForm
    {
        private Font UiFont(float emSize, FontStyle style = FontStyle.Regular)
        {
            try
            {
                return new Font("Poppins", emSize, style, GraphicsUnit.Point);
            }
            catch
            {
                try
                {
                    return new Font("Segoe UI Semibold", emSize, style, GraphicsUnit.Point);
                }
                catch
                {
                    return new Font("Segoe UI", emSize, style, GraphicsUnit.Point);
                }
            }
        }

        private static void EnableDoubleBuffer(Control c)
        {
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty
                | System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic,
                null,
                c,
                new object[] { true });
        }

        private static void ApplyRoundedRegion(Control ctrl, int radius)
        {
            if (ctrl.Width <= 0 || ctrl.Height <= 0)
                return;
            using var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(0, 0, d, d, 180, 90);
            path.AddArc(ctrl.Width - d, 0, d, d, 270, 90);
            path.AddArc(ctrl.Width - d, ctrl.Height - d, d, d, 0, 90);
            path.AddArc(0, ctrl.Height - d, d, d, 90, 90);
            path.CloseFigure();
            ctrl.Region?.Dispose();
            ctrl.Region = new Region(path);
        }

        private static GraphicsPath CreateRoundedRectPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return path;
            int d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void WireRoundedCardPaint(Panel card, int radius = 16)
        {
            card.Paint += (_, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var r = card.ClientRectangle;
                r.Width -= 1;
                r.Height -= 1;
                if (r.Width <= 1 || r.Height <= 1)
                    return;

                using var shadow = CreateRoundedRectPath(new Rectangle(r.X + 2, r.Y + 3, r.Width - 2, r.Height - 2), radius);
                using var shadowBrush = new SolidBrush(Color.FromArgb(38, 0, 0, 0));
                g.FillPath(shadowBrush, shadow);

                using var path = CreateRoundedRectPath(r, radius);
                using var fill = new SolidBrush(card.BackColor);
                g.FillPath(fill, path);
                using var border = new Pen(Color.FromArgb(46, 255, 255, 255), 1f);
                g.DrawPath(border, path);
            };
        }

        private void DockGridFullWidth(DataGridView dgv, Panel host)
        {
            dgv.Dock = DockStyle.Fill;
            dgv.Margin = Padding.Empty;
            dgv.ScrollBars = ScrollBars.Both;
            host.Padding = new Padding(0, 8, 0, 0);

            var gridShell = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorAlbastruCard,
                Padding = new Padding(1)
            };
            WireRoundedCardPaint(gridShell, 18);
            gridShell.Resize += (_, _) => ApplyRoundedRegion(gridShell, 18);
            gridShell.Controls.Add(dgv);
            host.Controls.Add(gridShell);
        }

        private Color GetOrderStatusColor(string status)
        {
            var s = status.Trim().ToLowerInvariant();
            if (s.Contains("livr") && (s.Contains("ată") || s.Contains("ata") || s.Contains("complet")))
                return ColorAccentGreen;
            if (s.Contains("anul") || s.Contains("cancel"))
                return ColorDanger;
            if (s.Contains("pregăt") || s.Contains("pregat") || s.Contains("prepar") || s.Contains("livrare"))
                return ColorAccentSky;
            return ColorPortocaliu;
        }
    }
}
