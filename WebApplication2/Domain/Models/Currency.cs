using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Domain.Models
{
    [Index(nameof(Currency_name), IsUnique = true)]
    public class Currency
    {
        public int Currency_id { get; set; }
        public required string Currency_name { get; set; }
        public ICollection<Rundown> Rundowns { get; set; } = [];
    }
}