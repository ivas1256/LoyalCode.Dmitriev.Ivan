namespace LoyalCode.Dmitriev.Ivan.Domain
{
    public class UserFavoriteCurrency
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
