using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class AdminForm
    {
        private void LoadDashboard()
        {
            DisposePnlContentChildren();
            pnlContent.Padding = new Padding(32, 24, 32, 32);
            BuildDashboard();
        }

        private void BuildDashboard()
        {
            var root = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            EnableDoubleBuffer(root);

            var stack = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 4, 0, 32)
            };

            stack.Controls.Add(BuildDashboardTitle());
            stack.Controls.Add(BuildDashboardStatsRow());
            stack.Controls.Add(BuildQuickActions());

            root.Controls.Add(stack);
            pnlContent.Controls.Add(root);

            void CenterStack(object? s, EventArgs e)
            {
                int maxW = Math.Max(1100, root.ClientSize.Width - 48);
                stack.MaximumSize = new Size(maxW, 0);
                stack.Width = Math.Min(maxW, Math.Max(stack.PreferredSize.Width, maxW));
                stack.Left = Math.Max(0, (root.ClientSize.Width - stack.Width) / 2);
                stack.Top = 8;
            }

            root.Resize += CenterStack;
            CenterStack(null, EventArgs.Empty);
        }

        private Control BuildDashboardTitle()
        {
            var wrap = new Panel
            {
                Width = 1100,
                Height = 88,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 12)
            };

            var lblMain = new Label
            {
                Text = "Dashboard Real-Time",
                Font = UiFont(30f, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = false,
                Size = new Size(1100, 48),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 8),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text = $"Delivo Admin  ·  {DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("ro-RO"))}",
                Font = UiFont(12f),
                ForeColor = ColorTextSecundar,
                AutoSize = false,
                Size = new Size(1100, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 58),
                BackColor = Color.Transparent
            };

            wrap.Controls.Add(lblMain);
            wrap.Controls.Add(lblSub);
            return wrap;
        }

        private Control BuildDashboardStatsRow()
        {
            int pCount = 0, cCount = 0, uCount = 0;
            decimal totalVenit = 0;
            try
            {
                pCount = DatabaseHelper.GetProduse().Count;
                var comenzi = DatabaseHelper.GetComenzi();
                cCount = comenzi.Count;
                totalVenit = comenzi.Sum(x => x.TotalPret);
                uCount = DatabaseHelper.GetUtilizatori().Count;
            }
            catch { }

            var table = new TableLayoutPanel
            {
                ColumnCount = 4,
                RowCount = 1,
                Width = 1100,
                Height = 188,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 16)
            };
            for (int i = 0; i < 4; i++)
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 188f));

            void AddCell(int col, string title, string value, string icon, Color accent)
            {
                var host = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8, 0, 8, 0), BackColor = Color.Transparent };
                host.Controls.Add(BuildPremiumStatCard(title, value, icon, accent));
                table.Controls.Add(host, col, 0);
            }

            AddCell(0, "Produse în catalog", pCount.ToString("N0"), "🍕", ColorPortocaliu);
            AddCell(1, "Comenzi totale", cCount.ToString("N0"), "📦", ColorAccentGreen);
            AddCell(2, "Venit total", totalVenit.ToString("N0") + " MDL", "💰", ColorAlb);
            AddCell(3, "Utilizatori", uCount.ToString("N0"), "👥", ColorAccentSky);

            return table;
        }

        private Panel BuildPremiumStatCard(string subtitle, string valueBig, string icon, Color accent)
        {
            var outer = new Panel
            {
                Dock = DockStyle.Fill,
                MinimumSize = new Size(240, 180),
                BackColor = Color.Transparent,
                Padding = new Padding(4, 4, 4, 8)
            };

            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorAlbastruCard,
                Padding = new Padding(24, 20, 24, 20)
            };
            WireRoundedCardPaint(card, 18);
            card.Resize += (_, _) => ApplyRoundedRegion(card, 18);

            var lblIcon = new Label
            {
                Text = icon,
                Font = UiFont(28f),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point(20, 12),
                BackColor = Color.Transparent
            };

            var lblVal = new Label
            {
                Text = valueBig,
                Font = UiFont(32f, FontStyle.Bold),
                ForeColor = accent,
                AutoSize = true,
                Location = new Point(20, 58),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text = subtitle,
                Font = UiFont(11f),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point(20, 128),
                MaximumSize = new Size(240, 0),
                BackColor = Color.Transparent
            };

            card.Controls.Add(lblIcon);
            card.Controls.Add(lblVal);
            card.Controls.Add(lblSub);

            Color normal = ColorAlbastruCard;
            Color hover = Color.FromArgb(255, 28, 40, 78);
            void SetHover(bool on) => card.BackColor = on ? hover : normal;

            foreach (Control c in new Control[] { outer, card, lblIcon, lblVal, lblSub })
            {
                c.MouseEnter += (_, _) => SetHover(true);
                c.MouseLeave += (_, _) => SetHover(false);
            }

            outer.Controls.Add(card);
            return outer;
        }

        private Control BuildQuickActions()
        {
            var block = new Panel
            {
                Width = 1100,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 8, 0, 0),
                Dock = DockStyle.Fill
            };

            var lbl = new Label
            {
                Text = "Acțiuni rapide administrator",
                Font = UiFont(20f, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = false,
                Size = new Size(1100, 40),
                TextAlign = ContentAlignment.MiddleCenter, // centrează textul în interiorul label-ului
                Margin = new Padding(0, 0, 0, 24)
            };

            // Grid pentru butoanele de acțiuni rapide.
            // Mărim lățimea coloanelor astfel încât titlurile să încapă pe un singur rând
            // și ajustăm dimensiunile interne ca să nu apară tăieri la margini.
            var grid = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
                Width = 940,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            grid.ColumnStyles.Clear();
            // coloane mai late pentru a evita înfășurarea textului pe două rânduri
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 460f));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 460f));
            // reduc înălțimea rândurilor (mai puțin în înălțime)
            for (int r = 0; r < 3; r++)
                grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 130f));
            // padding intern pentru spațiere egală
            grid.Padding = new Padding(12, 0, 12, 0);

            var actions = new[]
            {
                ("➕", "Adaugă produs", "Creează un produs nou în catalog", (Action)(() => { SetActiveNavButton(_navButtons[1]); ShowProductForm(ProductFormMode.Add); })),
                ("✏️", "Modifică produs", "Actualizează detaliile unui produs", (Action)(() => NavigateToSection(1, ProductListFocus.Edit))),
                ("🗑️", "Șterge produs", "Elimină produs din catalog", (Action)(() => NavigateToSection(1, ProductListFocus.Delete))),
                ("📦", "Vezi comenzi", "Gestionează comenzile clienților", (Action)(() => NavigateToSection(2))),
                ("👥", "Gestionare utilizatori", "Vizualizează și caută utilizatori", (Action)(() => NavigateToSection(3)))
            };

            for (int i = 0; i < actions.Length; i++)
            {
                var a = actions[i];
                int col = i % 2;
                int row = i / 2;
                // fiecare celulă are margină moderată pentru spațiere consistentă (nu exagerăm ca să nu provoace overflow)
                var cellHost = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 6, 10, 6), BackColor = Color.Transparent, Margin = new Padding(8) };
                cellHost.Controls.Add(BuildQuickActionCard(a.Item1, a.Item2, a.Item3, a.Item4));
                grid.Controls.Add(cellHost, col, row);
            }

            var inner = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Width = grid.Width,
                BackColor = Color.Transparent
            };
            // make heading the same width as the grid so it appears centered above the cards
            lbl.Size = new Size(grid.Width, lbl.Height);
            inner.Controls.Add(lbl);
            inner.Controls.Add(grid);

            // Centrare reală: un wrapper cu Dock=Fill care centrează inner
            var centerWrapper = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoSize = true
            };

            void CenterInner(object? s, EventArgs e)
            {
                inner.Left = Math.Max(0, (centerWrapper.ClientSize.Width - inner.Width) / 2);
            }

            centerWrapper.Resize += CenterInner;
            centerWrapper.Controls.Add(inner);
            block.Controls.Add(centerWrapper);

            // Forțează după render
            block.HandleCreated += CenterInner;

            return block;
        }

        private Panel BuildQuickActionCard(string icon, string title, string subtitle, Action onClick)
        {
            // Construiește un card de acțiune rapidă:
            // - pictograma este poziționată în stânga,
            // - titlul și subtitlul sunt stivuite vertical în dreapta,
            // - cardurile sunt mai late și mai puțin înalte pentru un aspect compact.
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorAlbastruCard,
                Cursor = Cursors.Hand,
                Padding = new Padding(16, 12, 16, 12),
                MinimumSize = new Size(400, 100) // asigurat <= lățimea coloanei (420)
            };
            WireRoundedCardPaint(card, 16);
            card.Resize += (_, _) => ApplyRoundedRegion(card, 16);

            // Pictograma, fixată în stânga.
            // pictograma puțin mai mică
            // pictograma puțin mai mică pentru a nu ocupa prea mult spațiu orizontal
            var lblIcon = new Label
            {
                Text = icon,
                Font = UiFont(22f),
                AutoSize = true,
                Location = new Point(18, 24),
                BackColor = Color.Transparent,
                ForeColor = ColorPortocaliu
            };

            // Containerul din dreapta care conține titlul și subtitlul.
            // mărim spațiul dintre pictogramă și text prin mutarea containerului la dreapta
            // plasează containerul de text mai aproape de pictogramă, dar suficient de lat
            // astfel încât să nu depășească lățimea coloanei
            var rightPanel = new Panel
            {
                BackColor = Color.Transparent,
                Location = new Point(100, 12),
                Size = new Size(320, 96),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = UiFont(15f, FontStyle.Bold),
                ForeColor = ColorAlb,
                AutoSize = true,
                Location = new Point(0, 6),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text = subtitle,
                Font = UiFont(11f),
                ForeColor = ColorTextSecundar,
                AutoSize = true,
                Location = new Point(0, 34),
                MaximumSize = new Size(320, 0), // limitează lățimea pentru a nu depăși cardul
                BackColor = Color.Transparent
            };

            rightPanel.Controls.Add(lblTitle);
            rightPanel.Controls.Add(lblSub);

            card.Controls.Add(lblIcon);
            card.Controls.Add(rightPanel);

            Color normal = ColorAlbastruCard;
            Color hover = Color.FromArgb(255, 32, 48, 88);

            // Leagă evenimentele pentru întregul card și pentru subcontroale.
            void Wire(Control c)
            {
                c.Cursor = Cursors.Hand;
                c.Click += (_, _) => onClick();
                c.MouseEnter += (_, _) => card.BackColor = hover;
                c.MouseLeave += (_, _) => card.BackColor = normal;
            }

            foreach (Control c in new Control[] { card, lblIcon, rightPanel, lblTitle, lblSub })
                Wire(c);

            return card;
        }
    }
}
