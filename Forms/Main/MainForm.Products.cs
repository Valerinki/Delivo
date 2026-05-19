using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Delivo.Data;

namespace Delivo.Forms
{
    public partial class MainForm
    {
        private void LoadProds()
        {
            try
            {
                var toateProdusele = DatabaseHelper.GetProduse();

                var popularIds = DatabaseHelper.GetProdusePopulare() ?? new List<int>();
                // make unique and limit to 10
                popularIds = popularIds.Distinct().Take(10).ToList();

                _all = new List<(int, string, string, decimal, string)>();

                Debug.WriteLine("[LoadProds] Popular IDs: " + string.Join(",", popularIds));

                foreach (var id in popularIds)
                {
                    var produs = toateProdusele.FirstOrDefault(p => p.Id == id);
                    Debug.WriteLine($"[LoadProds] id={id} matched={(produs.Id!=0?produs.Nume:"<not found>")}");
                    if (produs.Id != 0)
                        _all.Add(produs);
                }
            }
            catch
            {
                _all = new List<(int, string, string, decimal, string)>();
            }

            RefreshCards();
        }

        private void RefreshCards()
        {
            if (flpProducts == null) return;
            flpProducts.Controls.Clear();
            int availableWidth = flpProducts.Width - 120;
            int cardWidth = availableWidth < 900 ? (availableWidth / 2) - 20 : 420;

            // Determină lista de produse în funcție de categoria selectată
            List<(int Id, string Nume, string Descriere, decimal Pret, string Categorie)> listToShow;
            if (string.IsNullOrEmpty(_selectedCategory))
                listToShow = _all;
            else
                listToShow = _all.FindAll(p => p.Categorie.Equals(_selectedCategory, StringComparison.OrdinalIgnoreCase));

            // show at most 10 products to keep the view compact
            foreach (var p in listToShow)
            {
                var card = MakeCard(p.Id, p.Nume, p.Descriere, p.Pret, p.Categorie, cardWidth);
                card.Margin = new Padding(18, 10, 18, 22);
                flpProducts.Controls.Add(card);
            }
        }

        private void FilterCat(string cat)
        {
            // dacă utilizatorul apasă încă o dată aceeași categorie
            // revine la produsele populare
            if (_selectedCategory == cat)
            {
                _selectedCategory = "";
                RefreshCards();
                return;
            }

            _selectedCategory = cat;

            var list = _all.FindAll(p =>
                p.Categorie.Equals(cat, StringComparison.OrdinalIgnoreCase) ||
                p.Categorie.Equals(cat == "Băuturi" ? "Bauturi" : cat,
                StringComparison.OrdinalIgnoreCase));

            ShowList(list);
        }
        private void FilterSearch(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.TrimStart().StartsWith("Caută"))
            {
                RefreshCards();
                return;
            }
            ShowList(_all.FindAll(p => p.Nume.ToLower().Contains(q.ToLower()) || p.Categorie.ToLower().Contains(q.ToLower())));
        }

        private void ShowList(List<(int Id, string Nume, string Descriere, decimal Pret, string Categorie)> list)
        {
            flpProducts.Controls.Clear();
            int cw = Math.Max(200, (flpProducts.Width - 24) / 2);
            // limit to 10 items when showing a custom list
            foreach (var p in list)
                flpProducts.Controls.Add(MakeCard(p.Id, p.Nume, p.Descriere, p.Pret, p.Categorie, cw));
        }

        private Panel MakeCard(int id, string nume, string desc, decimal pret, string cat, int w)
        {
            Color acc = CatColor.ContainsKey(cat) ? CatColor[cat] : C_Orange;
            string em = CatEmoji.ContainsKey(cat) ? CatEmoji[cat] : "🍽";

            var card = new Panel
            {
                Width = w,
                Height = 270,
                BackColor = C_Card,
                Margin = new Padding(18, 10, 18, 24),
                Cursor = Cursors.Hand
            };
            Round(card, 16);

            var pnlImg = new Panel
            {
                Width = w,
                Height = 145,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(30, acc)
            };
            pnlImg.Paint += (s, pe) => {
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var lgb = new LinearGradientBrush(new Point(0, 0), new Point(w, 120),
                    Color.FromArgb(70, acc), Color.FromArgb(15, Color.Black));
                pe.Graphics.FillRectangle(lgb, 0, 0, w, 120);
            };
            var lblEm = new Label
            {
                Text = em,
                Font = new Font("Segoe UI Emoji", 64),
                AutoSize = false,
                Size = new Size(w, 120),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            var lblBadge = new Label
            {
                Text = " " + cat + " ",
                Font = new Font(F_Tiny.FontFamily, 9, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(170, acc),
                AutoSize = true,
                Location = new Point(10, 10),
                Padding = new Padding(5, 2, 5, 2)
            };
            Round(lblBadge, 8);
            pnlImg.Controls.Add(lblEm);
            pnlImg.Controls.Add(lblBadge);

            var lblNume = new Label
            {
                Text = nume,
                Font = new Font(F_Normal.FontFamily, 11, FontStyle.Bold),
                ForeColor = C_Text,
                AutoSize = false,
                Width = w - 20,
                Height = 44,
                Location = new Point(16, 155),
                BackColor = Color.Transparent
            };
            var lblDesc = new Label
            {
                Text = desc,
                Font = new Font(F_Tiny.FontFamily, 9),
                ForeColor = C_Muted,
                AutoSize = false,
                Width = w - 20,
                Height = 18,
                Location = new Point(16, 198),
                BackColor = Color.Transparent
            };
            var lblPret = new Label
            {
                Text = pret.ToString("F0") + " lei",
                Font = new Font(F_Bold.FontFamily, 13, FontStyle.Bold),
                ForeColor = C_Orange,
                AutoSize = true,
                Location = new Point(16, 225),
                BackColor = Color.Transparent
            };
            var btnAdd = new Button
            {
                Text = "+",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = C_Orange,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 40),
                Location = new Point(w - 62, 218),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            Round(btnAdd, 10);
            btnAdd.Click += (s, e) => AddCart(id, nume, pret);

            card.Controls.Add(pnlImg);
            card.Controls.Add(lblNume);
            card.Controls.Add(lblDesc);
            card.Controls.Add(lblPret);
            card.Controls.Add(btnAdd);
            card.MouseEnter += (s, e) => card.BackColor = _darkMode ? Color.FromArgb(28, 40, 76) : Color.FromArgb(225, 230, 250);
            card.MouseLeave += (s, e) => card.BackColor = C_Card;
            return card;
        }
    }
}