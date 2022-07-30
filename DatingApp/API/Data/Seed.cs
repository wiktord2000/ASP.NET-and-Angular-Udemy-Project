using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {   
        // The async declaration when nothing is returning (void function) - simple task instead of Task<Type>
        public static async Task SeedUsers(DataContext context){
            
            // If we have users in array - return
            if(await context.Users.AnyAsync()) return;

            // If not - add them from file
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            // Create random passwords 
            foreach (var user in users){
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Password"));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }

            // Save changes
            await context.SaveChangesAsync();
        }
    }
}