using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using user.Models;

namespace user.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(UserDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<IQueryable<User>> GetAllUsers(int pageSize = 5, int page = 1)
        {
            return (await _context.Users
                .OrderBy(u => u.Email)
                .OrderBy(u => u.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync())
                .AsQueryable();
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
        public async Task<bool> IsEmailExisted(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<ServiceResponse<string>> Login(string email, string password)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (dbUser == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = $"{email} has not been found!",
                };
            }
            if (!VerifyPasswordHash(password, dbUser.PasswordHash, dbUser.PasswordSalt))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = $"email or password is wrong!",
                };
            }
            return new ServiceResponse<string>
            {
                Data = CreateToken(dbUser),
            };
        }
        public async Task<ServiceResponse<string>> Register(User toSaveUser, string password)
        {
            if (await IsEmailExisted(toSaveUser.Email))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = $"{toSaveUser.Email} already exists!",
                };
            };
            Utility.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            toSaveUser.Id = Utility.CreateRandomString(12);
            toSaveUser.PasswordHash = passwordHash;
            toSaveUser.PasswordSalt = passwordSalt;
            await _context.Users.AddAsync(toSaveUser);
            await _context.SaveChangesAsync();
            return new ServiceResponse<string>
            {
                Data = toSaveUser.Id,
            };
        }
        public async Task Delete(string id)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
            if (dbUser == null)
            {
                return;
            }
            try
            {
                _context.Users.Remove(dbUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception) { }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("MySettings:Token").Value)
            );

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
