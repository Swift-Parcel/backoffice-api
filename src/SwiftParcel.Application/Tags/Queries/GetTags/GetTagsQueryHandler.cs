using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Tags;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Tags.Queries.GetTags;

public class GetTagsQueryHandler(ITagRepository tagRepository) 
    : IRequestHandler<GetTagsQuery, Result<PagedList<TagDto>>>
{
    public async Task<Result<PagedList<TagDto>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var (tags, totalCount) = await tagRepository.GetPagedAsync(
            request.NameFilter, 
            request.PageNumber, 
            request.PageSize, 
            cancellationToken);

        var dtos = tags.Select(t => new TagDto(t.Id, t.Name)).ToList();

        var pagedResult = new PagedList<TagDto>
        (
            items : dtos,
            count : totalCount,
            pageNumber : request.PageNumber,
            pageSize : request.PageSize
        );

        return Result<PagedList<TagDto>>.Success(pagedResult);
    }
}