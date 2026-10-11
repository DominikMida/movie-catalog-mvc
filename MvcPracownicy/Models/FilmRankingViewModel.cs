namespace MvcPracownicy.Models
{
    public class FilmRankingViewModel
    {
        public int Id { get; set; }
        public string Tytul { get; set; }
        public int Rok { get; set; }
        public string GatunekNazwa { get; set; }
        public string StudioNazwa { get; set; }
        public double SredniaOcena { get; set; }
        public int LiczbaRecenzji { get; set; }
    }
}
