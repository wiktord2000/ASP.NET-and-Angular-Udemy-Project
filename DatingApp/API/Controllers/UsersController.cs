using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{   
    // All stuff in this class needs authorization
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
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

        // This method get data about specific user using e.g. api/users/gloria
        [HttpGet("{username}", Name = "GetUser")]
        // Method returning all users (we could place List instead IEnumerable but the second option is better, )
        public async Task<ActionResult<MemberDto>> GetUser(string username){
            
            return await _userRepository.GetMemberAsync(username);

            // Old approach -----------------------------------------------------------------------------
            // var user =  await _userRepository.GetUserByUsernameAsync(username);
            // return _mapper.Map<MemberDto>(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){

            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            // -- Instead manualy mapping (updating particular props)
            // user.City = memberUpdateDto.City;
            // user.Country = memberUpdateDto.Country;
            // -- Map using automapper
            _mapper.Map(memberUpdateDto, user);

            // Flag changes
            _userRepository.Update(user);
            
            // Save changes in db
            if(await _userRepository.SaveAllAsync()) return NoContent();
            // If fail
            return BadRequest("Failed to update user");
        }


        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){

            // Gets the user's object from database
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            // Sending the given photo and waiting for response 
            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            // Preparing photo object
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            // Make main if user has no photos
            if(user.Photos.Count == 0) photo.IsMain = true;
            
            // Add photo to db and save changes
            user.Photos.Add(photo);
            
            if(await _userRepository.SaveAllAsync())
            {
                // _mapper.Map<PhotoDto>(photo) returns PhotoDto so only 3 props
                // GetUser - is the reference to the route which is needed to obtain the userDto 
                // 1. appraoch return CreatedAtRoute("GetUser", _mapper.Map<PhotoDto>(photo)); -> this didn't work because route require the username parameter !!!
                return CreatedAtRoute("GetUser", new {username = user.UserName} , _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId){

            // We sending this request with token assigned to logged user (username) so pass here another person will not be processed
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            // Not async because we points to obtained user
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo.IsMain) return BadRequest("This is already your main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain != null) currentMain.IsMain = false; 
            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId){

            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You can't delete your main photo");
            if(photo.PublicId != null){
                // result from cloudnery
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            // server update
            user.Photos.Remove(photo);
            if(await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Problem with delete photo");
        }

    }
}