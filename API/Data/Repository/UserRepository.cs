using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        readonly IMapper _mapper;
        readonly DataContext _context;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                                        .Include(user => user.Photos)
                                        .SingleOrDefaultAsync(user => user.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                                        .Include(user => user.Photos
                                            .Select(photo => photo.IsApproved))
                                        .ToListAsync();
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            query = query.Where(user => user.UserName != userParams.CurrentUsername);
            query = query.Where(user => user.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(user => user.Created),
                _ => query.OrderByDescending(user => user.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(
                                                                                    _mapper.ConfigurationProvider)
                                                                                    .AsNoTracking(),
                                                                                    userParams.PageNumber,
                                                                                    userParams.PageSize);

        }

        public async Task<MemberDto> GetMemberAsync(string username, bool isCurrentUser)
        {
            var query = _context.Users
                                      .Where(x => x.UserName == username)
                                      .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                                      .AsQueryable();

            if (isCurrentUser) query = query.IgnoreQueryFilters();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users
                                        .Where(user => user.UserName == username)
                                        .Select(user => user.Gender)
                                        .FirstOrDefaultAsync();
        }


        public async Task<AppUser> GetUserByPhotoId(int photoId)
        {
            return await _context.Users
                                        .Include(p => p.Photos)
                                        .IgnoreQueryFilters()
                                        .Where(p => p.Photos.Any(p => p.Id == photoId))
                                        .FirstOrDefaultAsync();
        }

    }
}