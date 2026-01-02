using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgricultureStore.Application.DTOs.AuthDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<string> GenerateJwiTokenAsync(int userId, string username, string roleName);
    }
}