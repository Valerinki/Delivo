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

        public static bool InregistreazaUtilizator(string numeComplet, string username, string parola, string telefon = "", string email = "")
        {
            using var conn = GetConnection(); conn.Open();
            var cmdV = new MySqlCommand("SELECT COUNT(*) FROM Utilizatori WHERE NumeUtilizator=@u", conn);
            cmdV.Parameters.AddWithValue("@u", username);
            if (Convert.ToInt32(cmdV.ExecuteScalar()) > 0) return false;

            var cmd = new MySqlCommand(
                "INSERT INTO Utilizatori (NumeUtilizator,Parola,Rol,NumeComplet,Telefon,Email) VALUES (@u,@p,'client',@n,@t,@e)", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", parola);
            cmd.Parameters.AddWithValue("@n", numeComplet);
            cmd.Parameters.AddWithValue("@t", telefon);
            cmd.Parameters.AddWithValue("@e", email);
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

        // Return list of product ids marked as popular (ordered)
        public static List<int> GetProdusePopulare()
        {
            var lista = new List<int>();
            using var conn = GetConnection(); conn.Open();
            // Return distinct ProdusId ordered by highest Ordine. This avoids duplicates if table has repeated rows.
            var cmd = new MySqlCommand("SELECT ProdusId FROM ProdusePopulare GROUP BY ProdusId ORDER BY MAX(Ordine) DESC, ProdusId", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(r.GetInt32(0));
            return lista;
        }
        // Adaugă această metodă în clasa DatabaseHelper (de exemplu, după metoda PlaseazaComanda)

        /// <summary>
        /// Plasează o comandă cu detalii (produse individuale). Returnează ID-ul comenzii.
        /// </summary>
        public static int PlaseazaComandaCuDetaliu(int utilizatorId, decimal total, string adresa, List<(int produsId, int cantitate, decimal pretUnitar)> items)
        {
            using var conn = GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                // 1. Inserare în tabela Comenzi (folosind aceeași structură ca metoda existentă)
                string sqlComanda = "INSERT INTO Comenzi (UtilizatorId, TotalPret, AdresaLivrare) VALUES (@uid, @total, @adr)";
                int comandaId;
                using (var cmd = new MySqlCommand(sqlComanda, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@uid", utilizatorId);
                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@adr", adresa);
                    cmd.ExecuteNonQuery();
                    comandaId = (int)cmd.LastInsertedId;
                }

                // 2. Inserare detalii în tabela detaliicomenzi
                string sqlDetaliu = "INSERT INTO detaliicomenzi (ComandaId, ProdusId, Cantitate, PretUnitar) VALUES (@cid, @pid, @qty, @pret)";
                foreach (var (produsId, cantitate, pretUnitar) in items)
                {
                    using var cmd = new MySqlCommand(sqlDetaliu, conn, tran);
                    cmd.Parameters.AddWithValue("@cid", comandaId);
                    cmd.Parameters.AddWithValue("@pid", produsId);
                    cmd.Parameters.AddWithValue("@qty", cantitate);
                    cmd.Parameters.AddWithValue("@pret", pretUnitar);
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return comandaId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
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
        public static bool IsUsernameTaken(string username)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Utilizatori WHERE NumeUtilizator=@u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
        public static bool IsEmailTaken(string email)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Utilizatori WHERE Email=@e", conn);
            cmd.Parameters.AddWithValue("@e", email);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static bool IsPhoneTaken(string telefon)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Utilizatori WHERE Telefon=@t", conn);
            cmd.Parameters.AddWithValue("@t", telefon);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static List<(int Id, string Nume)> GetCategorii()
        {
            var lista = new List<(int, string)>();
            using var conn = GetConnection();
            conn.Open();
            var cmd = new MySqlCommand("SELECT Id,Nume FROM Categorii ORDER BY Nume", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add((r.GetInt32(0), r.GetString(1)));
            return lista;
        }

        /// <summary>Toate produsele (inclusiv indisponibile) pentru panoul admin.</summary>
        public static List<(int Id, string Nume, string Descriere, decimal Pret, string Categorie, int CategorieId, bool Disponibil)>
            GetProduseAdmin()
        {
            var lista = new List<(int, string, string, decimal, string, int, bool)>();
            using var conn = GetConnection();
            conn.Open();
            var cmd = new MySqlCommand(
                "SELECT p.Id,p.Nume,p.Descriere,p.Pret,c.Nume,c.Id,p.Disponibil FROM Produse p JOIN Categorii c ON p.CategorieId=c.Id ORDER BY c.Nume,p.Nume",
                conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                bool disp = true;
                if (!r.IsDBNull(6))
                {
                    var v = r.GetValue(6);
                    disp = v switch
                    {
                        bool b => b,
                        byte b => b != 0,
                        int i => i != 0,
                        long l => l != 0,
                        _ => Convert.ToInt32(v) != 0
                    };
                }

                lista.Add((
                    r.GetInt32(0),
                    r.GetString(1),
                    r.IsDBNull(2) ? "" : r.GetString(2),
                    r.GetDecimal(3),
                    r.GetString(4),
                    r.GetInt32(5),
                    disp));
            }
            return lista;
        }

        public static bool AdaugaProdus(string nume, string descriere, decimal pret, int categorieId)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO Produse (Nume,Descriere,Pret,CategorieId,Disponibil) VALUES (@n,@d,@p,@c,1)",
                conn);
            cmd.Parameters.AddWithValue("@n", nume);
            cmd.Parameters.AddWithValue("@d", descriere ?? "");
            cmd.Parameters.AddWithValue("@p", pret);
            cmd.Parameters.AddWithValue("@c", categorieId);
            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool ActualizeazaProdus(int id, string nume, string descriere, decimal pret, int categorieId)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new MySqlCommand(
                "UPDATE Produse SET Nume=@n,Descriere=@d,Pret=@p,CategorieId=@c WHERE Id=@id",
                conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@n", nume);
            cmd.Parameters.AddWithValue("@d", descriere ?? "");
            cmd.Parameters.AddWithValue("@p", pret);
            cmd.Parameters.AddWithValue("@c", categorieId);
            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool StergeProdusSoft(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new MySqlCommand("UPDATE Produse SET Disponibil=0 WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool ActualizeazaStatusComanda(int comandaId, string status)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new MySqlCommand("UPDATE Comenzi SET Status=@s WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@s", status);
            cmd.Parameters.AddWithValue("@id", comandaId);
            return cmd.ExecuteNonQuery() > 0;
        }
        /// <summary>Returnează toate comenzile unui utilizator (header + detalii produse).</summary>
        public static List<UserOrder> GetComenziUtilizatorCuDetalii(string username)
        {
            var userId = GetUserId(username);
            if (userId == 0) return new List<UserOrder>();

            var comenzi = new List<UserOrder>();
            using var conn = GetConnection();
            conn.Open();

            // 1. Header comenzi
            var cmdHeader = new MySqlCommand(
                "SELECT Id, DataComanda, Status, TotalPret, AdresaLivrare FROM Comenzi WHERE UtilizatorId = @uid ORDER BY DataComanda DESC", conn);
            cmdHeader.Parameters.AddWithValue("@uid", userId);
            using var rdr = cmdHeader.ExecuteReader();
            var ords = new List<(int id, DateTime data, string status, decimal total, string adresa)>();
            while (rdr.Read())
            {
                ords.Add((
                    rdr.GetInt32(0),
                    rdr.GetDateTime(1),
                    rdr.GetString(2),
                    rdr.GetDecimal(3),
                    rdr.IsDBNull(4) ? "" : rdr.GetString(4)
                ));
            }
            rdr.Close();

            // 2. Pentru fiecare comandă, preia detaliile
            foreach (var ord in ords)
            {
                var details = new List<(string numeProdus, int cantitate, decimal pretUnitar)>();
                var cmdDet = new MySqlCommand(
                    @"SELECT p.Nume, d.Cantitate, d.PretUnitar 
              FROM detaliicomenzi d
              JOIN Produse p ON d.ProdusId = p.Id
              WHERE d.ComandaId = @cid", conn);
                cmdDet.Parameters.AddWithValue("@cid", ord.id);
                using var rdrDet = cmdDet.ExecuteReader();
                while (rdrDet.Read())
                {
                    details.Add((
                        rdrDet.GetString(0),
                        rdrDet.GetInt32(1),
                        rdrDet.GetDecimal(2)
                    ));
                }
                comenzi.Add(new UserOrder
                {
                    Id = ord.id,
                    Data = ord.data,
                    Status = ord.status,
                    Total = ord.total,
                    Adresa = ord.adresa,
                    Produse = details
                });
            }
            return comenzi;
        }
        public static bool ActualizeazaParola(string username, string parolaVeche, string parolaNoua)
        {
            using var conn = GetConnection();
            conn.Open();
            // Verifică parola veche
            var cmdCheck = new MySqlCommand("SELECT COUNT(*) FROM Utilizatori WHERE NumeUtilizator=@u AND Parola=@p", conn);
            cmdCheck.Parameters.AddWithValue("@u", username);
            cmdCheck.Parameters.AddWithValue("@p", parolaVeche);
            if (Convert.ToInt32(cmdCheck.ExecuteScalar()) == 0)
                return false;

            var cmdUpdate = new MySqlCommand("UPDATE Utilizatori SET Parola=@pNoua WHERE NumeUtilizator=@u", conn);
            cmdUpdate.Parameters.AddWithValue("@pNoua", parolaNoua);
            cmdUpdate.Parameters.AddWithValue("@u", username);
            return cmdUpdate.ExecuteNonQuery() > 0;
        }
        public class UserOrder
        {
            public int Id { get; set; }
            public DateTime Data { get; set; }
            public string Status { get; set; } = "";
            public decimal Total { get; set; }
            public string Adresa { get; set; } = "";
            public List<(string NumeProdus, int Cantitate, decimal PretUnitar)> Produse { get; set; } = new();
        }
    }
}