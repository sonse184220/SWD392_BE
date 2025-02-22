using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Interfaces;
using Repository.Models;
using Repository.RequestModels;
using Repository.ViewModels;
using Service.Interfaces;
using System;
using System.Collections.Generic;
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
        public AuthService(IAccountRepository accountRepository,IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration; 
        }
        public async Task<AccountViewModel> AuthenticateWithFirebaseAsync(FirebaseTokenRequest request)
        {
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.FirebaseToken);
                var uid = decodedToken.Uid;
                var email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"].ToString() : null;
                var name = decodedToken.Claims.ContainsKey("name") ? decodedToken.Claims["name"].ToString() : null;
                var user = await _accountRepository.GetByUidAsync(uid);
                if (user == null)
                {
                    user = new Account()
                    {
                        Email = email,
                        UserId = uid,
                        UserName = name,
                    };
                    await _accountRepository.CreateAccountAsync(user);
                }
                var accessToken = GenerateAccessToken(user);
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
        private string GenerateAccessToken(Account account)
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
                    new Claim(ClaimTypes.Name,account.UserName)

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
