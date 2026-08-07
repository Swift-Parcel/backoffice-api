using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Tags;

namespace SwiftParcel.Application.Tags.Queries.GetTags;

public class GetTagsQueryHandler(ITagRepository tagRepository) 
    : IRequestHandler<GetTagsQuery, Result<PagedResult<TagDto>>>
{
    public async Task<Result<PagedResult<TagDto>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var (tags, totalCount) = await tagRepository.GetPagedAsync(
            request.NameFilter, 
            request.PageNumber, 
            request.PageSize, 
            cancellationToken);

        var dtos = tags.Select(t => new TagDto(t.Id, t.Name)).ToList();

        var pagedResult = new PagedResult<TagDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return Result<PagedResult<TagDto>>.Success(pagedResult);
    }
}