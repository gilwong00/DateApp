using System.Linq;
using AutoMapper;
using DatingApp.API.DTO;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			// creating mappings, First arg is the source, second is the destination its mapping to
			CreateMap<User, UserList>()
				/* 
					For each user in UserList, setting the photoUrl
				*/
				.ForMember(
					// which field we want to alter
					dest => dest.PhotoUrl,
					// which field we want to map from
					options => options.MapFrom(
						src => src.Photos.FirstOrDefault(
							photo => photo.IsMain).Url))

				.ForMember(
					dest => dest.Age,
					opts => opts.MapFrom(
						src => src.DateOfBirth.CalculateAge()));

			CreateMap<User, UserDetail>()
				.ForMember(
					dest => dest.PhotoUrl,
					options => options.MapFrom(
						src => src.Photos.FirstOrDefault(
							photo => photo.IsMain).Url))

				.ForMember(
					dest => dest.Age,
					opts => opts.MapFrom(
						src => src.DateOfBirth.CalculateAge()));

			CreateMap<Photo, PhotoDetail>();

			CreateMap<UserEdit, User>();

			CreateMap<Photo, PhotoDetail>();

			CreateMap<CreatePhoto, Photo>();

			CreateMap<Photo, PhotoResponse>();

			CreateMap<RegisterUser, User>();

			CreateMap<MessageDataModel, Message>().ReverseMap();

			CreateMap<Message, MessageViewModel>()
				.ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
				.ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
		}
	}
}