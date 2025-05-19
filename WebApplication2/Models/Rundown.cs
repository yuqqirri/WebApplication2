namespace WebApplication2.Models
{
    public class Rundown
    {
        public int Rundown_id { get; set; }
        public DateTime Rundown_date { get; set; }
        public decimal Rundown_value { get; set; }
        public int Currency_id { get; set; }

        // Навигационное свойство
        public Currency Currency { get; set; }
    }
}