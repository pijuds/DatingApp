using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository:IUserRepositoty
{
     private readonly DataContext _context;

     private readonly IMapper _mapper;

    
    public UserRepository(DataContext context,IMapper mapper)
    {
      _context = context;
      _mapper=mapper;
      
    }

    public async Task<MemberDto> GetMemberAsync(string username,bool isCurrentUser)
    {
        // return _context.Users.Where(x=>x.UserName==username)
        // .Select(user=>new MemberDto{
        //     Id=user.Id,
        //     UserName=user.UserName,
        //     KnownAs=user.KnownAs
        // }).SingleOrDefaultAsync();
       var query = _context.Users
 .Where(x => x.UserName == username)
 .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
 .AsQueryable();
 if (isCurrentUser) query = query.IgnoreQueryFilters();

 return await query.FirstOrDefaultAsync();
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query=_context.Users.AsQueryable();
        query=query.Where(u=>u.UserName !=userParams.CurrentUsername);
        query=query.Where(u=>u.Gender==userParams.Gender);
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActive)
        };
       // var qry=_context.Users
              //  .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
               // .AsNoTracking();

        return await PagedList<MemberDto>.CreateAsync(
        query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
        userParams.PageNumber,userParams.PageSize);
       
    }

    //public Task<IEnumerable<AppUser>> GetUserAsync()
    //{
    // throw new NotImplementedException();
    //}

    public async Task<IEnumerable<AppUser>> GetUserAsync()
    {
      //return  throw new Exception("An error occurred");
      return await _context.Users.Include(p=>p.Photos).ToListAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByNameAsync(string username)
    {
         return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
    }

 public async Task<AppUser> GetUserByPhotoId(int photoId)
 {
   return await _context.Users
   .Include(p => p.Photos)
   .IgnoreQueryFilters()
   .Where(p => p.Photos.Any(p => p.Id == photoId))
   .FirstOrDefaultAsync();
 }
    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<string> GetUserGender(string username)
    {
        return await _context.Users.Where(x=>x.UserName==username).Select(x=>x.Gender).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

   // public async Task<bool> SaveAllAsync()
   // {
        //return await _context.SaveChangesAsync()>0;
    //}

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

}