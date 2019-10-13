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
		private readonly IDatingRepository _repo;
		private readonly IMapper _mapper;
		public MessagesController(IDatingRepository repo, IMapper mapper)
		{
			_repo = repo;
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

			var message = await _repo.GetMessage(userId);

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

			var messages = await _repo.GetMessagesForUser(messageParams);

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

			var messages = await _repo.GetMessageThread(userId, recipientId);
			var messageThread = _mapper.Map<IEnumerable<MessageViewModel>>(messages);
			return Ok(messageThread);
		}

		[HttpPost]
		public async Task<IActionResult> CreateMessage(int userId, MessageDataModel message)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return Unauthorized();
			}

			message.SenderId = userId;

			var recipient = await _repo.GetUser(message.RecipientId);

			if (recipient == null)
			{
				return BadRequest("Could not find user");
			}

			var newMessage = _mapper.Map<Message>(message);

			_repo.Add(newMessage);

			var messageForReturn = _mapper.Map<MessageDataModel>(message);

			if (await _repo.SaveAll())
			{
				return CreatedAtRoute("GetMessage", new { messageId = newMessage.Id }, messageForReturn);
			}

			throw new Exception("Creating the message failed on save");
		}
	}
}