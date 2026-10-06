using Microsoft.AspNetCore.Mvc;
using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;

namespace Snackis.Api.Controllers;

[ApiController]
[Route("api/discussions")]
public class DiscussionController(IPostService postService) : ControllerBase
{
    [HttpGet("{id:int}/posts")]
    [ProducesResponseType<IReadOnlyList<PostListDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PostListDto>>> GetPosts(int id)
    {
        try
        {
            return Ok(await postService.GetByDiscussionAsync(id));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
