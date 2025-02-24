using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Enums;
using Repository.Interfaces;
using Repository.Models;
using Repository.RequestModels;
using Repository.ViewModels;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;
        public AuthService(IAccountRepository accountRepository,IConfiguration configuration, IRoleRepository roleRepository)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _roleRepository = roleRepository;
        }
        public async Task<AccountViewModel> AuthenticateWithFirebaseAsync(FirebaseTokenRequest request)
        {
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.FirebaseToken);
                var email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"].ToString() : null;
                var name = decodedToken.Claims.ContainsKey("name") ? decodedToken.Claims["name"].ToString() : null;
                var user = await _accountRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    var userRole = await _roleRepository.GetRoleByNameAsync(UserRole.User.ToString());
                    if (userRole == null)
                    {
                        throw new Exception("User role not found in database");
                    }
                    var userId = Guid.NewGuid();
                    user = new Account()
                    {
                        Email = email,
                        UserId = userId.ToString(),
                        UserName = name,
                        RoleId = userRole.RoleId,
                    };
                    await _accountRepository.CreateAccountAsync(user);
                    user = await _accountRepository.GetByEmailAsync(email);
                }
                var role = user.Role.RoleName;
                var accessToken = GenerateAccessToken(user,role);
                var accountRespone = new AccountViewModel()
                {
                    accessToken = accessToken,
                };
                return accountRespone;
            }
            catch (FirebaseAuthException ex)
            {
                throw new UnauthorizedAccessException("Firebase authentication failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error authorization:" + ex.Message);
            }
        }
        private string GenerateAccessToken(Account account,string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryTime = int.Parse(_configuration["JwtSettings:Expire"]);

            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                     new Claim(ClaimTypes.NameIdentifier, account.UserId),
                    new  Claim(ClaimTypes.Email,account.Email),
                    new Claim(ClaimTypes.Name,account.UserName),
                    new Claim(ClaimTypes.Role, role)

                }),
                Expires = DateTime.UtcNow.AddDays(expiryTime),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature
                    )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            return accessToken;

        }
    }
}
