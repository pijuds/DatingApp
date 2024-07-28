using System.Numerics;
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
    

    private readonly IMapper _mapper;

    private readonly IuintofWork _uow;

  public MessagesController(IMapper mapper,IuintofWork uow)
  {
    
    _mapper=mapper;
    _uow=uow;
    
  }

  public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMesageDto)
  {
    var username=User.GetUserName();
    if(username==createMesageDto.RecipientUsername.ToLower())
    {
        return BadRequest("you can not send message yourself");
    }
     var sender= await _uow.UserRepository.GetUserByNameAsync(username);

     var receipent=await _uow.UserRepository.GetUserByNameAsync(createMesageDto.RecipientUsername);
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
     _uow.MessageRepository.AddMessage(message);

     if(await _uow.Complete()) return Ok (_mapper.Map<MessageDto>(message));
     return BadRequest("Failed to send Message");    
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
  {
      messageParams.Username=User.GetUserName();

      var messages=await _uow.MessageRepository.GetMessagesForUser(messageParams);

       Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage,
            messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
  }

[HttpGet("thread/{username}")]
public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
{
 var currentUserName=User.GetUserName();
  return Ok(await _uow.MessageRepository.GetMessageThread(currentUserName,username));

}

[HttpDelete("{id}")]

public async Task<ActionResult> DeleteMessage(int id)
{
  var username=User.GetUserName();

  var message=await _uow.MessageRepository.GetMessage(id);
  if(message.SenderUsername !=username && message.RecipientUsername !=username)
  return Unauthorized();

  if(message.SenderUsername == username) message.SenderDeleted=true;
  if(message.RecipientUsername==username) message.RecipientDeleted=true;

  if(message.SenderDeleted && message.RecipientDeleted)
  {
    _uow.MessageRepository.DeleteMessage(message);
  }

  if(await _uow.Complete()) return Ok();

  return BadRequest("Problem deleting message");
 
}

}