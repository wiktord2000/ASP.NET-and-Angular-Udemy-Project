using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public IMapper _mapper { get; set; }
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }


        // -------------------------------- new approach of automapper
        public async Task<MemberDto> GetMemberAsync(string username)
        {
            // Note: ProjectTo comes from Automapper
            // Note2: _mapper.ConfigurationProvider (add options provided in AutoMapperProfiles e.g. calcualte and assign age)
            // Note3: We don't have to use Includs() method, the Automapper have it built in 

            return await _context.Users
                .Where(x => x.UserName == username)
                // .Select(user => new MemberDto
                // {
                //     Id = user.Id,
                //     Username = user.UserName
                // })
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                            .ToListAsync();
        }

        // --------------------------------------------------------------


        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include( p => p.Photos)
                .SingleOrDefaultAsync((e) => e.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include( p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            // SaveChanges returns number of saved changes 
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            // Flag modify
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
