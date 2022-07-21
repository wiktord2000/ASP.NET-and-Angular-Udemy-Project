using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
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
        // Method returning all users (we could place List instead IEnumerable but the second option is better, )
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
            
            return await _context.Users.ToListAsync();
        }

        // This method get data about specific user using e.g. api/users/3
        [HttpGet("{id}")]
        // Method returning all users (we could place List instead IEnumerable but the second option is better, )
        public async Task<ActionResult<AppUser>> GetUser(int id){
            
            return await _context.Users.FindAsync(id);
        }
    }
}