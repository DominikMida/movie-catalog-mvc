namespace MvcPracownicy.Models
{
    public class UzytkownikViewModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public bool IsAdmin { get; set; }
        public string Token { get; set; }
    }
}
