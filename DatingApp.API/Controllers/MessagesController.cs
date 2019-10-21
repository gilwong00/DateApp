using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
	[ServiceFilter(typeof(LogUserActivity))] // anytime a method in this controller gets call, we run this filter
	[Authorize]
	[Route("api/users/{userId}/[controller]")]
	[ApiController]
	public class MessagesController : ControllerBase
	{
		private readonly IUserRepository _userRepo;
		private readonly IMessageRepository _messageRepo;
		private readonly IMapper _mapper;
		public MessagesController(IMessageRepository messageRepo, IUserRepository userRepo, IMapper mapper)
		{
			_userRepo = userRepo;
			_messageRepo = messageRepo;
			_mapper = mapper;
		}

		[HttpGet("{messageId}", Name = "GetMessage")]
		public async Task<IActionResult> GetMessage(int userId, int messageId) // we get userId from the root of the method that calls this
		{
			// We get User from ControllerBase
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return Unauthorized();
			}

			var message = await _messageRepo.GetMessage(userId);

			if (message == null)
			{
				return NotFound();
			}

			return Ok(message);
		}

		[HttpGet]
		public async Task<IActionResult> GetUserMessages(int userId, [FromQuery]MessageParams messageParams)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return Unauthorized();
			}

			messageParams.UserId = userId;

			var messages = await _messageRepo.GetMessagesForUser(messageParams);

			var response = _mapper.Map<IEnumerable<MessageViewModel>>(messages);

			Response.AddPagination(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

			return Ok(response);
		}

		[HttpGet("thread/{recipientId}")]
		public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return Unauthorized();
			}

			var messages = await _messageRepo.GetMessageThread(userId, recipientId);
			var messageThread = _mapper.Map<IEnumerable<MessageViewModel>>(messages);
			return Ok(messageThread);
		}

		[HttpPost]
		public async Task<IActionResult> CreateMessage(int userId, MessageDataModel message)
		{
			var sender = await _userRepo.GetUser(userId);

			if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return Unauthorized();
			}

			message.SenderId = userId;

			var recipient = await _userRepo.GetUser(message.RecipientId);

			if (recipient == null)
			{
				return BadRequest("Could not find user");
			}

			var newMessage = _mapper.Map<Message>(message);

			_messageRepo.Add(newMessage);

			if (await _messageRepo.SaveAll())
			{
				var response = _mapper.Map<MessageViewModel>(newMessage);
				return CreatedAtRoute("GetMessage", new { messageId = newMessage.Id }, response);
			}

			throw new Exception("Creating the message failed on save");
		}

		[HttpPost("{messageId}")]
		public async Task<IActionResult> DeleteMessage(int messageId, int userId)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return Unauthorized();
			}

			var message = await _messageRepo.GetMessage(messageId);

			// Only delete if both parties delete the message
			if (message.SenderId == userId)
			{
				message.SenderDeleted = true;
			}

			if (message.RecipientId == userId)
			{
				message.RecipientDeleted = true;
			}

			if (message.SenderDeleted && message.RecipientDeleted)
			{
				_messageRepo.Delete(message);
			}

			if (await _messageRepo.SaveAll())
			{
				return NoContent();
			}

			throw new Exception("Error deleting message");
		}

		[HttpPost("{messageId}/read")]
		public async Task<IActionResult> MarkMassageIsRed(int userId, int messageId)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return Unauthorized();
			}

			var message = await _messageRepo.GetMessage(messageId);

			if (message.RecipientId != userId)
			{
				return Unauthorized();
			}

			message.isRead = true;
			message.DateRead = DateTime.Now;

			await _messageRepo.SaveAll();
			return NoContent();
		}
	}
}