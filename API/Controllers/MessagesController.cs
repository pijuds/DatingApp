using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Extensions.API;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MessagesController:BaseApiController
{
    private readonly IUserRepositoty _userrepositoty;

    private readonly IMapper _mapper;

     private readonly IMessageRepository _messageRepository;

  public MessagesController(IUserRepositoty userRepositoty, IMessageRepository messageRepository,IMapper mapper)
  {
    _userrepositoty=userRepositoty;
    _messageRepository=messageRepository;
    _mapper=mapper;
    
  }

  public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMesageDto)
  {
    var username=User.GetUserName();
    if(username==createMesageDto.RecipientUsername.ToLower())
    {
        return BadRequest("you can not send message yourself");
    }
     var sender= await _userrepositoty.GetUserByNameAsync(username);

     var receipent=await _userrepositoty.GetUserByNameAsync(createMesageDto.RecipientUsername);
     if(receipent ==null)
     {
       return NotFound();
     }
     var message= new Message()
     {
      Sender=sender,
      Recipient=receipent,
      SenderUsername=sender.UserName,
      RecipientUsername=receipent.UserName,
      Content=createMesageDto.Content
     };
     _messageRepository.AddMessage(message);

     if(await _messageRepository.SaveAllAsync()) return Ok (_mapper.Map<MessageDto>(message));
     return BadRequest("Failed to send Message");    
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
  {
      messageParams.Username=User.GetUserName();

      var messages=await _messageRepository.GetMessagesForUser(messageParams);

       Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage,
            messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
  }

[HttpGet("thread/{username}")]
public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
{
  var currentUserName=User.GetUserName();
  return Ok(await _messageRepository.GetMessageThread(currentUserName,username));

}

}