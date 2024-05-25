

using System.Security.Cryptography;
using System.Text;
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
    
    public class AccountController:BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        private readonly IMapper _mapper;
        public AccountController(DataContext context,ITokenService tokenService,IMapper mapper)
        {
            _context=context;
            _tokenService=tokenService;
           _mapper=mapper;
        }  

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken");
            }
            if (!registerDto.DateOfBirth.HasValue)
            {
               return BadRequest("Date of Birth is required");
            }
           var user = _mapper.Map<AppUser>(registerDto);


            using var hmac=new HMACSHA512();
            
                user.UserName=registerDto.Username.ToLower();
                user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
                user.PasswordSalt=hmac.Key;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username=user.UserName,
                token=_tokenService.CreateToken(user),
                KnownAs=user.KnownAs,
                PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain)?.Url

            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto logindto)
        {    
          var user = await _context.Users
          .Include(p=>p.Photos)
          .SingleOrDefaultAsync(x => x.UserName == logindto.Username);
          if (user == null) 
          {
            return Unauthorized("Invalid username");
          }

        using (var hmac = new HMACSHA512(user.PasswordSalt))
        {
          var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid password");
            }
        }
    }



    // Clearing sensitive information before returning
    

    return new UserDto
    {
      Username=user.UserName,
      token=_tokenService.CreateToken(user),
      PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
      KnownAs=user.KnownAs


    };
}

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x=>x.UserName==username.ToLower());

        }


    }
}