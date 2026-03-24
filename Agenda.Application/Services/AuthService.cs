using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Agenda.Application.Dtos;
using Agenda.Application.Interfaces;
using Agenda.Core.Entities.Core.CustomEntities.ResponseApi.Details;
using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.Interfaces;
using Agenda.Core.QueryFilters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Agenda.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<ResponseGetObject> Login(LoginQueryFilter queryFilter)
    {
        try
        {
            var matches = await _unitOfWork.UsersRepository.Find(
                u => u.Email == queryFilter.Email && u.Password == queryFilter.Password);

            var user = matches.FirstOrDefault();

            if (user == null)
            {
                return new ResponseGetObject
                {
                    Data     = null,
                    Messages = new[] { new Message { Type = "error", Description = "Email o contraseña inválidos." } },
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            var (token, expiresAt) = GenerateToken(user.Id, user.Name, user.Email);

            return new ResponseGetObject
            {
                Data = new LoginResponse
                {
                    Token     = token,
                    UserId    = user.Id,
                    Name      = user.Name,
                    Email     = user.Email,
                    ExpiresAt = expiresAt
                },
                Messages   = new[] { new Message { Type = "success", Description = "Inicio de sesión exitoso." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponseGetObject
            {
                Data     = null,
                Messages = new[] { new Message { Type = "error", Description = "Error al iniciar sesión." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    private (string Token, DateTime ExpiresAt) GenerateToken(int userId, string name, string email)
    {
        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt   = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpiresInHours"] ?? "8"));

        var claims = new[]
        {
            new Claim("userId",           userId.ToString()),
            new Claim(ClaimTypes.Name,    name),
            new Claim(ClaimTypes.Email,   email)
        };

        var token = new JwtSecurityToken(
            issuer:            _configuration["Jwt:Issuer"],
            audience:          _configuration["Jwt:Audience"],
            claims:            claims,
            expires:           expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}