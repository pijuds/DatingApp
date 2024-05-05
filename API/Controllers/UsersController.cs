using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Extensions.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController:BaseApiController
{
    private readonly IUserRepositoty _repositoty;

    private readonly IMapper _mapper;

    private readonly IPhotoService _photoService;

    public UsersController(IUserRepositoty repositoty,IMapper mapper,IPhotoService photoService)
    {

        _repositoty=repositoty;
        _mapper=mapper;
        _photoService=photoService;

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

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
       //var username=User.FindFirst(ClaimTypes.NameIdentifier).Value;
       var username=User.GetUserName();
       var user= await _repositoty.GetUserByNameAsync(username);
       if (user==null) NotFound();
       _mapper.Map(memberUpdateDto,user);

       if(await _repositoty.SaveAllAsync()) return NoContent();
       return BadRequest("Failed to update user");

    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile File)
    {
        //var users=User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _repositoty.GetUserByNameAsync(User.GetUserName());

        var result = await _photoService.AddPhotoAsync(File);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        if(await _repositoty.SaveAllAsync()) 
        {
            return CreatedAtAction(nameof(GetUser),new{username=user.UserName},_mapper.Map<PhotoDto>(photo));
        }
        
        

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int PhotoId)
    {
        var user=await _repositoty.GetUserByNameAsync(User.GetUserName());
        if(user==null)
        {
            return NotFound();
        }
        var photo=user.Photos.FirstOrDefault(x=>x.Id==PhotoId);
        if(photo==null) return NotFound();

        if(photo.IsMain) return BadRequest("this is already your main photo");

        var currentMain=user.Photos.FirstOrDefault(x=>x.IsMain);

        if(currentMain != null)
        {
            currentMain.IsMain=false;
        }
        photo.IsMain=true;
        
        if(await _repositoty.SaveAllAsync()) return NoContent();

        return BadRequest("Problem setting in the main photo");

    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _repositoty.GetUserByNameAsync(User.GetUserName());

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("You cannot delete your main photo");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _repositoty.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting photo");
    }

}