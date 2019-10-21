using System;
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
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepository _repo;
		private readonly IMapper _mapper;
		public UsersController(IUserRepository repo, IMapper mapper)
		{
			_mapper = mapper;
			_repo = repo;
		}

		[HttpGet]
		public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
		{
			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var user = await _repo.GetUser(currentUserId);

			// set user if to filter out current logged in user
			userParams.UserId = currentUserId;

			// return users of the opposite gender
			if (string.IsNullOrEmpty(userParams.Gender))
			{
				userParams.Gender = user.Gender == "male" ? "female" : "male";
			}

			var users = await _repo.GetUsers(userParams);
			var response = _mapper.Map<IEnumerable<UserList>>(users);
			Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
			return Ok(response);
		}

		[HttpGet("{id}", Name = "GetUser")]
		public async Task<IActionResult> GetUser(int id)
		{
			var user = await _repo.GetUser(id);
			var response = _mapper.Map<UserDetail>(user);
			return Ok(response);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(int id, UserEdit updatedUser)
		{
			// check to see if user is the current user that passed a auth token
			// if the path doesnt match the token we return unauth
			// this prevents a user updating another users data
			if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return Unauthorized();
			}

			var user = await _repo.GetUser(id);
			_mapper.Map(updatedUser, user);

			if (await _repo.SaveAll())
			{
				return NoContent();
			}

			throw new Exception($"Updating user {id} failed on save");
		}

		[HttpPost("{id}/like/{recipientId}")]
		public async Task<IActionResult> LikeUser(int id, int recipientId)
		{
			if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return Unauthorized();
			}

			var like = await _repo.GetLike(id, recipientId);

			// check if user has already liked the individual or not
			if (like != null)
			{
				return BadRequest("You already liked this user");
			}

			// check if liked user exist
			if (await _repo.GetUser(recipientId) == null)
			{
				return NotFound();
			}

			like = new Like
			{
				LikerId = id,
				LikeeId = recipientId
			};

			_repo.Add<Like>(like);

			if (await _repo.SaveAll())
			{
				return Ok();
			}
			return BadRequest("Failed to like user");
		}
	}
}