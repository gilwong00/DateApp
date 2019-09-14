using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IDatingRepository _repo;
		private readonly IMapper _mapper;
		public UsersController(IDatingRepository repo, IMapper mapper)
		{
			_mapper = mapper;
			_repo = repo;
		}

		[HttpGet]
		public async Task<IActionResult> GetUsers()
		{
			var users = await _repo.GetUsers();
			var response = _mapper.Map<IEnumerable<UserList>>(users);
			return Ok(response);
		}

		[HttpGet("{id}")]
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
	}
}