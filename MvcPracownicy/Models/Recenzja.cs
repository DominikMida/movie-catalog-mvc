using System.ComponentModel.DataAnnotations;

namespace MvcPracownicy.Models
{
    public class Recenzja
    {
        [Key]
        public int Id { get; set; }

        public string Autor { get; set; }
        public int Ocena { get; set; }
        public string Komentarz { get; set; }

        // FK
        public int FilmId { get; set; }
        public Film Film { get; set; }
    }
}