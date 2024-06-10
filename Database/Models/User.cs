using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;

namespace Database.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [NotMapped]
        public string FullName => $"{LastName} {FirstName} {MiddleName}";

        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string? Description { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<Achievement> Achievements { get; set; }
        public ICollection<UserEvent> UserEvents { get; set; }
        public byte[]? ImageData { get; set; }
        public void SetP(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyP(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }
}