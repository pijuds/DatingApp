using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController:BaseApiController
{
    private readonly IUserRepositoty _repositoty;

    private readonly IMapper _mapper;

    public UsersController(IUserRepositoty repositoty,IMapper mapper)
    {

        _repositoty=repositoty;
        _mapper=mapper;

    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        
        // var user= await _repositoty.GetUserAsync();
        // var usersToReturn=_mapper.Map<IEnumerable<MemberDto>>(user);
        // return Ok(usersToReturn);
        var users= await _repositoty.GetMembersAsync();
        return Ok(users);
    }
    
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {

    //    var user= await  _repositoty.GetUserByNameAsync(username);

    //    var userToReturn=_mapper.Map<MemberDto>(user);

    //    return Ok(userToReturn);
       return await _repositoty.GetMemberAsync(username);
    }

}