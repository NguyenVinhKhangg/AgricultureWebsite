using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgricultureStore.Application.DTOs.AuthDTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
        public string UserName {get ; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password {get; set;} = string.Empty;
    }
}