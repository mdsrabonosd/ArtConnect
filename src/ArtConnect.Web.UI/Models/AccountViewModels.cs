using System.ComponentModel.DataAnnotations;

namespace ArtConnect.Web.UI.Models
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required, StringLength(100), Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, Phone, StringLength(20), Display(Name = "Phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Use at least 8 characters with a capital letter, a small letter and a number.")]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>Customer or Artist. Admin is not allowed.</summary>
        [Required]
        public string Role { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Bio { get; set; }

        [Range(0, 80), Display(Name = "Years of experience")]
        public int ExperienceYears { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == "Artist" && string.IsNullOrWhiteSpace(Bio))
            {
                yield return new ValidationResult("Please write a short bio.", new[] { nameof(Bio) });
            }
        }
    }

    public class ProfileViewModel
    {
        [Required, StringLength(100), Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        [Required, Phone, StringLength(20), Display(Name = "Phone number")]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password), Display(Name = "Current password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password), Display(Name = "New password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Use at least 8 characters with a capital letter, a small letter and a number.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(NewPassword)), Display(Name = "Confirm new password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
