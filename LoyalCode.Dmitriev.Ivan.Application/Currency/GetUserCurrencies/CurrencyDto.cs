namespace LoyalCode.Dmitriev.Ivan.Application.Currency.GetUserCurrencies
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Rate { get; set; }
    }
}