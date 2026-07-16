using AutoMapper;
using BlogCMS.Api.DTOs;
using BlogCMS.Api.Models;

namespace BlogCMS.Api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<User, AuthorDto>();

        CreateMap<Category, CategoryDto>();
        CreateMap<Tag, TagDto>();

        CreateMap<Post, PostResponse>()
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.Tags.Select(pt => pt.Tag)))
            .ForMember(d => d.CommentCount, o => o.MapFrom(s => s.Comments.Count));

        CreateMap<Post, PostSummaryResponse>()
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.Tags.Select(pt => pt.Tag)))
            .ForMember(d => d.CommentCount, o => o.MapFrom(s => s.Comments.Count));

        CreateMap<Comment, CommentResponse>();
    }
}
