using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){

            // Check that the username exists in db
            if( await UserExist(registerDto.Username)) return BadRequest("Username is taken");
            // using - If we finished with particular class it will be disposed correctly
            // With new instance of the HMACSHA512 class is provided new PasswordSald key
            using var hmac = new HMACSHA512();
            var user = new AppUser{
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            // It's only set up tracking of the change! Not update the database. 
            _context.Users.Add(user);
            // Save changes
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){

            var user = await _context.Users
                            .Include(p => p.Photos)
                            .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            if(user == null) return Unauthorized("Invalid username");
            
            // If we will not insert into constructor old key (which is storing in db) we will get new one 
            using var hmac = new HMACSHA512(user.PasswordSalt);

            // So if the computed password will be valid we will get the same PasswordHash -> Let's check this
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // Compare
            for (int i = 0; i < computedHash.Length; i++)   
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url
            };
        }


        private async Task<bool> UserExist(string username){

            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}