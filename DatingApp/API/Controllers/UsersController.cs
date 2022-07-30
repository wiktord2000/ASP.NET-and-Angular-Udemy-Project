using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{   
    // All stuff in this class needs authorization
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
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
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers(){

            var users = await _userRepository.GetMembersAsync();
            // Note: We can't use -> return await _userRepository.GetMembersAsync(); -> it doesn't work
            return Ok(users);

            // Old approach ----------------------------------------------------------------------------                
            // var users = await _userRepository.GetUsersAsync();
            // var userToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            // return  Ok(userToReturn);
        }

        // This method get data about specific user using e.g. api/users/3
        [HttpGet("{username}")]
        // Method returning all users (we could place List instead IEnumerable but the second option is better, )
        public async Task<ActionResult<MemberDto>> GetUser(string username){
            
            return await _userRepository.GetMemberAsync(username);

            // Old approach -----------------------------------------------------------------------------
            // var user =  await _userRepository.GetUserByUsernameAsync(username);
            // return _mapper.Map<MemberDto>(user);
        }
    }
}