namespace Delivo.Models
{
    public class Produs
    {
        public int Id { get; set; }
        public string Nume { get; set; } = "";
        public string Descriere { get; set; } = "";
        public decimal Pret { get; set; }
        public int CategorieId { get; set; }
        public string Categorie { get; set; } = "";
        public bool Disponibil { get; set; } = true;
    }
}