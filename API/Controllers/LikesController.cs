using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Extensions.API;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController : BaseApiController
{
    private readonly IUserRepositoty _userRepositoty;

    private readonly ILikesRepository _likesRepository;

    public LikesController(IUserRepositoty userRepositoty,ILikesRepository likesRepository)
    {
        _userRepositoty=userRepositoty;
        _likesRepository=likesRepository;

    }

     [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = int.Parse(User.GetUserId());
        var likedUser = await _userRepositoty.GetUserByNameAsync(username);
        var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

        if (userLike != null) return BadRequest("You already like this user");

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _userRepositoty.SaveAllAsync()) return Ok();

        return BadRequest("Failed to like user");
    }

     [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikeParams  likeParams)
    {
        likeParams.userId=int.Parse(User.GetUserId());
        var users= await _likesRepository.GetUserLikes(likeParams);
         Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
            users.TotalCount, users.TotalPages));

        return Ok(users);

    
        return Ok(users);
    }

}