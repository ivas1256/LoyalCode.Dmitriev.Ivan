namespace LoyalCode.Dmitriev.Ivan.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public ICollection<UserFavoriteCurrency> Favorites { get; set; } = [];
    }
}
