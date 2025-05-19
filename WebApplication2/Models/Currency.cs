using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models
{
    [Index(nameof(Currency_name), IsUnique = true)]
    public class Currency
    {
        public int Currency_id { get; set; }
        public string Currency_name { get; set; }

        // Навигационное свойство
        public ICollection<Rundown> Rundowns { get; set; }
    }
}