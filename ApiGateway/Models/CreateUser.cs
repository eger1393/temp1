using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    public class CreateUser
    {
        [Required]
        [MaxLength(64)]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Name can only contain characters and spaces")]
        public string Name { get; set; }
    }
}