using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.ViewModels
{
    public class UserListItemViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Student Profile")]
        public bool HasStudentProfile { get; set; }

        [Display(Name = "Instructor Profile")]
        public bool HasInstructorProfile { get; set; }
    }
}
