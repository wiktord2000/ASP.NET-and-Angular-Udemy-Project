using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }


        // // Sync version
        // [HttpGet]
        // // Method returning all users (we could place List instead IEnumerable but the second option is better, )
        // public ActionResult<IEnumerable<AppUser>> GetUsers(){
            
        //     return _context.Users.ToList();
        // }

        // Async version
        [HttpGet]
        [AllowAnonymous]
        // Method returning all users (we could place List instead IEnumerable but the second option is better, )
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
            
            return await _context.Users.ToListAsync();
        }

        [Authorize]
        // This method get data about specific user using e.g. api/users/3
        [HttpGet("{id}")]
        // Method returning all users (we could place List instead IEnumerable but the second option is better, )
        public async Task<ActionResult<AppUser>> GetUser(int id){
            
            return await _context.Users.FindAsync(id);
        }
    }
}