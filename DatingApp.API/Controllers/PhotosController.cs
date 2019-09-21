using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System;

namespace DatingApp.API.Controllers
{
	[Authorize]
	[Route("api/users/{userId}/photos")]
	[ApiController]
	public class PhotosController : ControllerBase
	{
		private readonly IDatingRepository _repo;
		private readonly IMapper _mapper;
		private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
		private Cloudinary _cloudinary;

		public PhotosController(
			IDatingRepository repo,
			IMapper mapper,
			IOptions<CloudinarySettings> cloudinaryConfig
			)
		{
			_cloudinaryConfig = cloudinaryConfig;
			_mapper = mapper;
			_repo = repo;

			Account acc = new Account(
				_cloudinaryConfig.Value.CloudName,
				_cloudinaryConfig.Value.ApiKey,
				_cloudinaryConfig.Value.ApiSecret
			);

			_cloudinary = new Cloudinary(acc);
		}

		private async Task<Photo> GetValidPhoto(int userId, int photoId)
		{
			// make this into a method as well
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return null;
			}

			var user = await _repo.GetUser(userId);

			if (!user.Photos.Any(p => p.Id == photoId))
			{
				return null;
			}

			var photo = await _repo.GetPhoto(photoId);
			return photo;
		}

		[HttpGet("{id}", Name = "GetPhoto")]
		public async Task<IActionResult> GetPhoto(int id)
		{
			var photo = await _repo.GetPhoto(id);
			var response = _mapper.Map<PhotoResponse>(photo);
			return Ok(photo);
		}

		[HttpPost]
		public async Task<IActionResult> AddUserPhoto(int userId, [FromForm]CreatePhoto newPhoto)
		{
			if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
			{
				return Unauthorized();
			}

			var user = await _repo.GetUser(userId);
			var file = newPhoto.File;
			var uploadResult = new ImageUploadResult();

			if (file.Length > 0)
			{
				// read contents of the file
				using (var stream = file.OpenReadStream())
				{
					var uploadParams = new ImageUploadParams()
					{
						File = new FileDescription(file.Name, stream),
						Transformation = new Transformation()
							.Width(500)
							.Height(500)
							.Crop("fill")
							.Gravity("face")
					};

					uploadResult = _cloudinary.Upload(uploadParams);
				}
			}

			newPhoto.Url = uploadResult.Uri.ToString();
			newPhoto.PublicId = uploadResult.PublicId;

			var photo = _mapper.Map<Photo>(newPhoto);

			// check if user has a main photo if not
			// set this photo to be main
			if (!user.Photos.Any(u => u.IsMain))
			{
				photo.IsMain = true;
			}

			user.Photos.Add(photo);

			if (await _repo.SaveAll())
			{
				var photoResponse = _mapper.Map<PhotoResponse>(photo);
				/*
					This post method returns CreatedAtRoute
					 Created at route takes 3 overloads first is the name of the route
					 In this case we want to return the GetPhoto route, the second is the Id and
					 the third is the response object
				*/
				return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoResponse);
			}
			return BadRequest("Could not add photo");
		}

		[HttpPost("{photoId}/setMain")]
		public async Task<IActionResult> SetMainPhoto(int userId, int photoId)
		{
			var photo = await GetValidPhoto(userId, photoId);

			if (photo == null)
			{
				return Unauthorized();
			}

			if (photo.IsMain)
			{
				return BadRequest("This is already the main photo");
			}

			var currentMainPhoto = await _repo.GetMainUserPhoto(userId);
			currentMainPhoto.IsMain = false;
			photo.IsMain = true;

			if (await _repo.SaveAll())
			{
				return NoContent();
			}
			return BadRequest("Could not set photo to main");
		}

		[HttpDelete("{photoId}")]
		public async Task<IActionResult> DeletePhoto(int userId, int photoId)
		{
			var photo = await GetValidPhoto(userId, photoId);

			if (photo == null)
			{
				return Unauthorized();
			}

			if (photo.IsMain)
			{
				return BadRequest("You cannot delete your main photo");
			}

			if (photo.PublicId != null)
			{
				var deleteParams = new DeletionParams(photo.PublicId);
				var result = _cloudinary.Destroy(deleteParams);

				if (result.Result == "ok")
				{
					_repo.Delete(photo);
				}
			}
			else
			{
				_repo.Delete(photo);
			}

			if (await _repo.SaveAll())
			{
				return Ok();
			}
			return BadRequest("Failed to delete photo");
		}
	}
}