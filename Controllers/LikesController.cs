//using Sportiva.Services;

//namespace Sportiva.Controllers
//{
//    [Route("api/likes")]
//    [ApiController]
//    [Authorize]
//    public class LikesController(ILikeService _linkService) : ControllerBase
//    {
//        [HttpPost("{postId}")]
//        public async Task<IActionResult> AddLike(string postId, CancellationToken ct)
//        {
//            var response = await _linkService.AddLikeAsync(User.GetUserId()!, postId, ct);
//            return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
//        }
//        [HttpDelete("{postId}")]
//        public async Task<IActionResult> RemoveLike(string postId, CancellationToken ct)
//        {
//            var response = await _linkService.RemoveLikeAsync(User.GetUserId()!, postId, ct);
//            return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
//        }
//        [HttpGet("{postId}")]
//        public async Task<IActionResult> GetLikes(string postId, CancellationToken ct)
//        {
//            return Ok(await _linkService.GetLikesAsync(postId, ct));
//        }
//    }
//}
