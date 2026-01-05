using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgricultureStore.Application.DTOs.AuthDTOs
{
    public class LoginResponseDto
    {
        public string Token {get; set;} = string.Empty;
        public DateTime Expiration {get; set;}
        public int UserId {get; set;}
        public string UserName {get; set; } = string.Empty;
        public string RoleName { get ; set ;} = string.Empty;
    }
}