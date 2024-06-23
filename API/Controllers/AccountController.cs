

using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    public class AccountController:BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        private readonly UserManager<AppUser> _userManager;

        private readonly IMapper _mapper;
        public AccountController(UserManager<AppUser> userManager,ITokenService tokenService,IMapper mapper)
        {
           _userManager=userManager;
            _tokenService=tokenService;
           _mapper=mapper;
        }  

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if(await UserExists(registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }
            
           var user = _mapper.Map<AppUser>(registerDto);


            
            
            user.UserName=registerDto.UserName.ToLower();
              
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);
             
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if(!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                UserName=user.UserName,
                token= _tokenService.CreateToken(user),
                KnownAs=user.KnownAs,
                PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
                Gender=user.Gender

            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto logindto)
        {    
          var user = await _userManager.Users
          .Include(p=>p.Photos)
          .SingleOrDefaultAsync(x => x.UserName == logindto.Username);
          if (user == null) 
          {
            return Unauthorized("Invalid username");
          }

           var result = await _userManager.CheckPasswordAsync(user, logindto.password);

           if (!result) return Unauthorized();


      
    // Clearing sensitive information before returning
    

    return new UserDto
    {
      UserName=user.UserName,
      token=_tokenService.CreateToken(user),
      PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
      KnownAs=user.KnownAs,
      Gender=user.Gender

    };
}

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x=>x.UserName==username.ToLower());

        }


    }
}