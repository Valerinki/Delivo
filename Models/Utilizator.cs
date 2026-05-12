namespace Delivo.Models
{
    public class Utilizator
    {
        public int Id { get; set; }
        public string NumeUtilizator { get; set; } = "";
        public string Parola { get; set; } = "";
        public string Rol { get; set; } = "client";
        public string NumeComplet { get; set; } = "";
        public string Telefon { get; set; } = "";
        public DateTime DataCreare { get; set; }
    }
}