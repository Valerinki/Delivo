using System;
using System.Windows.Forms;
using Delivo.Data;
using Microsoft.VisualBasic;

namespace Delivo.Forms
{
    public partial class MainForm
    {
        private void AddCart(int id, string nume, decimal pret)
        {
            int i = _cart.FindIndex(x => x.Id == id);
            if (i >= 0)
            {
                var t = _cart[i];
                _cart[i] = (t.Id, t.Nume, t.Pret, t.Qty + 1);
            }
            else _cart.Add((id, nume, pret, 1));

            decimal tot = 0;
            foreach (var x in _cart) tot += x.Pret * x.Qty;
            _navBtns[2].Text = "🛒\nCoș(" + _cart.Count + ")";
            _navBtns[2].ForeColor = C_Orange;
            MessageBox.Show("✓ " + nume + " adăugat!\nTotal: " + tot.ToString("F0") + " lei", "Coș", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowCart()
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Coșul este gol!", "Coș", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string msg = "🛒 Coș:\n\n";
            decimal tot = 0;
            foreach (var x in _cart)
            {
                msg += $"• {x.Nume} x{x.Qty} = {x.Pret * x.Qty:F0} lei\n";
                tot += x.Pret * x.Qty;
            }
            if (MessageBox.Show(msg + $"\n💰 TOTAL: {tot:F0} lei\n\nPlasezi comanda?", "Coș",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                PlaceOrder(tot);
        }

        private void PlaceOrder(decimal tot)
        {
            string adr = Interaction.InputBox("Adresa livrare:", "Comandă", "");
            if (string.IsNullOrWhiteSpace(adr)) return;
            try
            {
                int uid = DatabaseHelper.GetUserId(LoginForm.UtilizatorLogat);
                int oid = DatabaseHelper.PlaseazaComanda(uid, tot, adr);
                _cart.Clear();
                _navBtns[2].Text = "📋\nComenzi";
                _navBtns[2].ForeColor = C_Muted;
                MessageBox.Show($"✅ Comanda #{oid} plasată!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message); }
        }
    }
}