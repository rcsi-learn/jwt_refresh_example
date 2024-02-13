using System.ComponentModel.DataAnnotations;

namespace Domain {
    public class User {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
