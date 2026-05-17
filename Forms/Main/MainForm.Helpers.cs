using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Delivo.Forms
{
    public partial class MainForm
    {
        private void InitFonts()
        {
            string fn = "Segoe UI";
            F_Logo = new Font("Showcard Gothic", 24, FontStyle.Bold);
            F_H1 = new Font(fn, 26, FontStyle.Bold);
            F_Bold = new Font(fn, 12, FontStyle.Bold);
            F_Normal = new Font(fn, 11, FontStyle.Regular);
            F_Small = new Font(fn, 10, FontStyle.Regular);
            F_Tiny = new Font(fn, 9, FontStyle.Regular);
        }

        private void Round(Control c, int r)
        {
            if (c.Width < 1 || c.Height < 1) return;
            var p = new GraphicsPath();
            p.AddArc(0, 0, r * 2, r * 2, 180, 90);
            p.AddArc(c.Width - r * 2, 0, r * 2, r * 2, 270, 90);
            p.AddArc(c.Width - r * 2, c.Height - r * 2, r * 2, r * 2, 0, 90);
            p.AddArc(0, c.Height - r * 2, r * 2, r * 2, 90, 90);
            p.CloseFigure();
            c.Region = new Region(p);
        }
    }
}