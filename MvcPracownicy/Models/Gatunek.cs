using System.ComponentModel.DataAnnotations;

namespace MvcPracownicy.Models
{
    public class Gatunek
    {
        [Key]
        public int Id { get; set; }

        public string Nazwa { get; set; }

        public ICollection<Film>? Filmy { get; set; }
    }
}