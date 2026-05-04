using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
    }
}