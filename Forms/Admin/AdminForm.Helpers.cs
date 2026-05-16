using System;
using System.Drawing;
using System.Windows.Forms;

namespace Delivo.Forms
{
    public partial class AdminForm
    {
        private void DisposePnlContentChildren()
        {
            while (pnlContent.Controls.Count > 0)
            {
                var c = pnlContent.Controls[0];
                pnlContent.Controls.RemoveAt(0);
                c.Dispose();
            }
        }

        private DataGridView CreatePremiumDataGridView(bool readOnly = true)
        {
            var dgv = new DataGridView
            {
                BackgroundColor = ColorAlbastruCard,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
                AllowUserToAddRows = false,
                ReadOnly = readOnly,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(34, 255, 255, 255),
                RowTemplate = { Height = 62 },
                DefaultCellStyle =
                {
                    BackColor = ColorAlbastruCard,
                    ForeColor = ColorAlb,
                    SelectionBackColor = Color.FromArgb(140, 255, 107, 0),
                    SelectionForeColor = ColorAlb,
                    Font = UiFont(11.5f),
                    Padding = new Padding(18, 10, 18, 10),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                ColumnHeadersDefaultCellStyle =
                {
                    BackColor = ColorPortocaliu,
                    ForeColor = ColorAlb,
                    Font = UiFont(12f, FontStyle.Bold),
                    Padding = new Padding(18, 14, 18, 14),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                ColumnHeadersHeight = 62,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                AdvancedCellBorderStyle =
                {
                    Left = DataGridViewAdvancedCellBorderStyle.None,
                    Right = DataGridViewAdvancedCellBorderStyle.None,
                    Top = DataGridViewAdvancedCellBorderStyle.None,
                    Bottom = DataGridViewAdvancedCellBorderStyle.Single
                },
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText
            };

            EnableDoubleBuffer(dgv);

            dgv.RowPrePaint += (_, e) =>
            {
                if (e.RowIndex < 0)
                    return;
                var row = dgv.Rows[e.RowIndex];
                if (row.Selected)
                    return;
                row.DefaultCellStyle.BackColor = (e.RowIndex % 2 == 0)
                    ? ColorAlbastruCard
                    : Color.FromArgb(255, 18, 29, 58);
            };

            dgv.CellMouseEnter += (_, e) =>
            {
                if (e.RowIndex < 0)
                    return;
                var row = dgv.Rows[e.RowIndex];
                if (!row.Selected)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 31, 45, 84);
            };

            dgv.CellMouseLeave += (_, e) =>
            {
                if (e.RowIndex < 0)
                    return;
                var row = dgv.Rows[e.RowIndex];
                if (!row.Selected)
                    row.DefaultCellStyle.BackColor = (e.RowIndex % 2 == 0)
                        ? ColorAlbastruCard
                        : Color.FromArgb(255, 18, 29, 58);
            };

            return dgv;
        }

        private TextBox CreateStyledTextBox(bool multiline = false)
        {
            var t = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = ColorInputBg,
                ForeColor = ColorAlb,
                Font = UiFont(11f),
                Height = multiline ? 118 : 46,
                Margin = new Padding(0)
            };
            if (multiline)
            {
                t.Multiline = true;
                t.ScrollBars = ScrollBars.Vertical;
                t.Height = 118;
            }

            return t;
        }

        private ComboBox CreateStyledCombo(bool dropDownList = true)
        {
            return new ComboBox
            {
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorInputBg,
                ForeColor = ColorAlb,
                Font = UiFont(11f),
                DropDownStyle = dropDownList ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown,
                IntegralHeight = false,
                ItemHeight = 34,
                Height = 46
            };
        }

        private Button CreatePrimaryOrangeButton(string text, int width = 180)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(width, 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorPortocaliu,
                ForeColor = ColorAlb,
                Font = UiFont(11.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ColorPortocaliuSoft;
            ApplyRoundedRegion(b, 14);
            b.Resize += (_, _) => ApplyRoundedRegion(b, 14);
            return b;
        }

        private Button CreateSecondaryGhostButton(string text, int width = 160)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(width, 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 20, 32, 62),
                ForeColor = ColorAlb,
                Font = UiFont(11f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(90, 255, 107, 0);
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 32, 44, 82);
            ApplyRoundedRegion(b, 14);
            b.Resize += (_, _) => ApplyRoundedRegion(b, 14);
            return b;
        }

        private Label CreateSectionTitle(string text, ContentAlignment align = ContentAlignment.MiddleLeft)
        {
            return new Label
            {
                Text = text,
                Font = UiFont(24f, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = false,
                Height = 56,
                TextAlign = align,
                BackColor = Color.Transparent
            };
        }
    }
}
