//using Microsoft.AspNetCore.Mvc;
//using BlogApplication.Services;
//using BlogApplication.Models;

//namespace BlogApplication.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class BlogPostsController : ControllerBase
//    {
//        private readonly MongoDBService _mongoDBService;

//        public BlogPostsController (MongoDBService blogPostService)
//        {
//            _blogPostService = blogPostService;
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(BlogPost newPost)
//        {
//            if (newPost == null)
//                return BadRequest("Invalid post data.");

//            await _blogPostService.CreateAsync(newPost);
//            return CreatedAtAction(nameof(Create), new { id = newPost.Id }, newPost);
//        }

//        [HttpGet]
//        public async Task<ActionResult<List<BlogPost>>> GetAll()
//        {
//            var posts = await _blogPostService.GetAllAsync();
//            return Ok(posts);
//        }
//    }
//}
