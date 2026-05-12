namespace Delivo.Models
{
    public class Comanda
    {
        public int Id { get; set; }
        public int UtilizatorId { get; set; }
        public string NumeUtilizator { get; set; } = "";
        public DateTime DataComanda { get; set; }
        public string Status { get; set; } = "In asteptare";
        public decimal TotalPret { get; set; }
        public string AdresaLivrare { get; set; } = "";
        public List<DetaliiComanda> Detalii { get; set; } = new();
    }

    public class DetaliiComanda
    {
        public int Id { get; set; }
        public int ComandaId { get; set; }
        public int ProdusId { get; set; }
        public string NumeProdus { get; set; } = "";
        public int Cantitate { get; set; }
        public decimal PretUnitar { get; set; }
        public decimal Subtotal => Cantitate * PretUnitar;
    }
}