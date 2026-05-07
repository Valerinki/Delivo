using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Delivo.Models;

namespace Delivo.Data
{
    public class DatabaseHelper
    {
        private static readonly string ConnectionString =
            "Server=127.0.0.1;Port=3306;Database=delivodb;Uid=root;Pwd=root;";

        public static MySqlConnection GetConnection()
            => new MySqlConnection(ConnectionString);

        public static string? VerificaAutentificare(string username, string parola)
        {
            using var conn = GetConnection(); conn.Open();
            var cmd = new MySqlCommand("SELECT Rol FROM Utilizatori WHERE NumeUtilizator=@u AND Parola=@p", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", parola);
            return cmd.ExecuteScalar()?.ToString();
        }

        public static bool InregistreazaUtilizator(string username, string parola, string numeComplet, string telefon = "")
        {
            using var conn = GetConnection(); conn.Open();
            var cmdV = new MySqlCommand("SELECT COUNT(*) FROM Utilizatori WHERE NumeUtilizator=@u", conn);
            cmdV.Parameters.AddWithValue("@u", username);
            if (Convert.ToInt32(cmdV.ExecuteScalar()) > 0) return false;

            var cmd = new MySqlCommand(
                "INSERT INTO Utilizatori (NumeUtilizator,Parola,Rol,NumeComplet,Telefon) VALUES (@u,@p,'client',@n,@t)", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", parola);
            cmd.Parameters.AddWithValue("@n", numeComplet);
            cmd.Parameters.AddWithValue("@t", telefon);
            cmd.ExecuteNonQuery();
            return true;
        }

        public static List<(int Id, string Nume, string Descriere, decimal Pret, string Categorie)> GetProduse()
        {
            var lista = new List<(int, string, string, decimal, string)>();
            using var conn = GetConnection(); conn.Open();
            var cmd = new MySqlCommand(
                "SELECT p.Id,p.Nume,p.Descriere,p.Pret,c.Nume FROM Produse p JOIN Categorii c ON p.CategorieId=c.Id WHERE p.Disponibil=1 ORDER BY c.Nume,p.Nume", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add((r.GetInt32(0), r.GetString(1), r.IsDBNull(2) ? "" : r.GetString(2), r.GetDecimal(3), r.GetString(4)));
            return lista;
        }

        public static int PlaseazaComanda(int utilizatorId, decimal total, string adresa)
        {
            using var conn = GetConnection(); conn.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO Comenzi (UtilizatorId,TotalPret,AdresaLivrare) VALUES (@u,@t,@a)", conn);
            cmd.Parameters.AddWithValue("@u", utilizatorId);
            cmd.Parameters.AddWithValue("@t", total);
            cmd.Parameters.AddWithValue("@a", adresa);
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public static int GetUserId(string username)
        {
            using var conn = GetConnection(); conn.Open();
            var cmd = new MySqlCommand("SELECT Id FROM Utilizatori WHERE NumeUtilizator=@u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            var r = cmd.ExecuteScalar();
            return r != null ? Convert.ToInt32(r) : 0;
        }

        public static List<Comanda> GetComenzi()
        {
            var lista = new List<Comanda>();
            using var conn = GetConnection(); conn.Open();
            var cmd = new MySqlCommand(
                "SELECT c.Id,u.NumeUtilizator,c.DataComanda,c.Status,c.TotalPret,c.AdresaLivrare FROM Comenzi c JOIN Utilizatori u ON c.UtilizatorId=u.Id ORDER BY c.DataComanda DESC", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Comanda
                {
                    Id = r.GetInt32(0),
                    NumeUtilizator = r.GetString(1),
                    DataComanda = r.GetDateTime(2),
                    Status = r.GetString(3),
                    TotalPret = r.GetDecimal(4),
                    AdresaLivrare = r.IsDBNull(5) ? "" : r.GetString(5)
                });
            return lista;
        }

        public static List<Comanda> GetComenziUtilizator(string username)
        {
            var lista = new List<Comanda>();
            using var conn = GetConnection(); conn.Open();
            var cmd = new MySqlCommand(
                "SELECT c.Id,c.DataComanda,c.Status,c.TotalPret,c.AdresaLivrare FROM Comenzi c JOIN Utilizatori u ON c.UtilizatorId=u.Id WHERE u.NumeUtilizator=@u ORDER BY c.DataComanda DESC", conn);
            cmd.Parameters.AddWithValue("@u", username);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Comanda
                {
                    Id = r.GetInt32(0),
                    NumeUtilizator = username,
                    DataComanda = r.GetDateTime(1),
                    Status = r.GetString(2),
                    TotalPret = r.GetDecimal(3),
                    AdresaLivrare = r.IsDBNull(4) ? "" : r.GetString(4)
                });
            return lista;
        }

        public static List<Utilizator> GetUtilizatori()
        {
            var lista = new List<Utilizator>();
            using var conn = GetConnection(); conn.Open();
            var cmd = new MySqlCommand("SELECT Id,NumeUtilizator,NumeComplet,Rol,IFNULL(Telefon,'') FROM Utilizatori", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Utilizator
                {
                    Id = r.GetInt32(0),
                    NumeUtilizator = r.GetString(1),
                    NumeComplet = r.IsDBNull(2) ? "" : r.GetString(2),
                    Rol = r.GetString(3),
                    Telefon = r.GetString(4)
                });
            return lista;
        }
    }
}