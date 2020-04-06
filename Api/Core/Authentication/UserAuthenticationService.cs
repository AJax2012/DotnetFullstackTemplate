using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

using SourceName.Api.Model.Configuration;
using SourceName.Service.Users;


namespace SourceName.Api.Core.Authentication
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SecretsConfiguration _secretsConfiguration;
        private readonly IUserPasswordService _userPasswordService;
        private readonly IUserService _userService;

        public UserAuthenticationService(
            SecretsConfiguration secretsConfiguration,
            IUserPasswordService userPasswordService,
            IUserService userService)
        {
            _secretsConfiguration = secretsConfiguration;
            _userPasswordService = userPasswordService;
            _userService = userService;
        }

        public string Authenticate(string username, string password)
        {
            var user = _userService.GetForAuthentication(username);
            if (user == null || !user.IsActive)
            {
                return null;
            }

            if (!_userPasswordService.ValidateHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretsConfiguration.UserPasswordSecret);

            var roleClaims = new List<Claim>(user.Roles
                .Select(r => new Claim(ClaimTypes.Role, r.Role.Name)));

            var allClaims = new List<Claim>();
            allClaims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));
            allClaims.AddRange(roleClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(allClaims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}