using System.ComponentModel.DataAnnotations;

namespace MvcPracownicy.Models
{
    public class Film
    {
        [Key]
        public int Id { get; set; }

        public string Tytul { get; set; }
        public int Rok { get; set; }

        // FK
        public int? StudioId { get; set; }
        public Studio? Studio { get; set; }

        public int? GatunekId { get; set; }
        public Gatunek? Gatunek { get; set; }

        public ICollection<Recenzja>? Recenzje { get; set; }
    }
}