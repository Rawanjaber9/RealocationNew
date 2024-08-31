using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPostController : ControllerBase
    {
        RealocationAppContext db = new RealocationAppContext();


        [HttpPost("add-post/{userId}")]
        public async Task<IActionResult> AddPost(int userId, [FromBody] PostDTO postDto)
        {
            // שליפת שם המשתמש מטבלת המשתמשים
            var username = await db.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();

            if (username == null)
            {
                return NotFound("User not found");
            }

            var post = new Post
            {
                UserId = userId,
                Username = username,
                Content = postDto.Content,
                CreatedAt = DateTime.Now,
                UpdatedAt = null // משאיר את UpdatedAt ריק בשלב זה
            };

            db.Posts.Add(post);
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Post added successfully!",
                Username = username,
                Content = post.Content
            });
        }










        // הוספת תגובה לפוסט
        [HttpPost("add-comment/{postId}/{userId}")]
        public async Task<IActionResult> AddComment(int postId, int userId, [FromBody] CommentDTO commentDto)
        {
            // שליפת שם המשתמש מטבלת המשתמשים
            var username = await db.Users
             .Where(u => u.UserId == userId)
            .Select(u => u.Username)
            .FirstOrDefaultAsync();

            if (username == null)
            {
                return NotFound("User not found");
            }
            //שמירת התגובה
            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Username = username,
                Content = commentDto.Content,
                CreatedAt = DateTime.Now
            };

            db.Comments.Add(comment);
            await db.SaveChangesAsync();


            //החזרת שם המשתמש ותוכן התגובה
            return Ok(new
            {
                message = "Comment added successfully!",
                Username = username,
                Content = comment.Content
            });
        }






        //קונטרולר שמביא את תוכן של פוסט מסוים יחד עם שם המשתמש שכתב את הפוסט
        //ואת התגובות שהופיעו על אותו פוסט יחד שם שמות המשתמשים
        [HttpGet("get-post-details/{postId}")]
        public async Task<IActionResult> GetPostDetails(int postId)
        {
            // שליפת הפרטים של הפוסט
            var post = await db.Posts
                .Where(p => p.PostId == postId)
                .Select(p => new
                {
                    p.PostId,
                    p.Username,
                    p.Content,
                    p.CreatedAt,
                    Comments = db.Comments
                        .Where(c => c.PostId == p.PostId)
                        .Select(c => new
                        {
                            c.CommentId,
                            c.Username,
                            c.Content,
                            c.CreatedAt
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound("Post not found");
            }

            return Ok(post);
        }








        //מביא את כל הפוסטין יחד עם התגגובות שנכתבו עליהם
        //לפי מספר משתמש

        [HttpGet("user-posts/{userId}")]
        public async Task<IActionResult> GetUserPostsWithComments(int userId)
        {
            // שליפת כל הפוסטים שהמשתמש העלה
            var userPosts = await db.Posts
                .Where(p => p.UserId == userId)
                .Select(p => new
                {
                    p.Username,
                    p.Content,
                    p.CreatedAt,
                    Comments = db.Comments
                                .Where(c => c.PostId == p.PostId)
                                .Select(c => new
                                {
                                    c.Username,
                                    c.Content,
                                    c.CreatedAt
                                })
                                .ToList()
                })
                .ToListAsync();

            if (userPosts == null || !userPosts.Any())
            {
                return NotFound("No posts found for this user.");
            }

            return Ok(userPosts);
        }




        //מביא את כל הפוסטין יחד עם התגגובות שנכתבו עליהם
        //לפי שם מדינה
        [HttpGet("posts-by-destination/{destinationCountry}")]
        public async Task<IActionResult> GetPostsByDestination(string destinationCountry)
        {
            // שלב 1: מציאת כל המשתמשים שבחרו באותה מדינת יעד
            var usersInDestination = await db.RelocationDetails
                .Where(rd => rd.DestinationCountry == destinationCountry)
                .Select(rd => rd.UserId)
                .ToListAsync();

            if (!usersInDestination.Any())
            {
                return NotFound("No users found for the specified destination country.");
            }

            // שלב 2: שליפת כל הפוסטים שהעלו המשתמשים האלה
            var posts = await db.Posts
                .Where(p => usersInDestination.Contains(p.UserId))
                .Select(p => new
                {
                    p.PostId,
                    p.Username,
                    p.Content,
                    p.CreatedAt,
                    p.UpdatedAt,
                    Comments = db.Comments
                        .Where(c => c.PostId == p.PostId)
                        .Select(c => new
                        {
                            c.CommentId,
                            c.Username,
                            c.Content,
                            c.CreatedAt
                        })
                        .ToList()
                })
                .ToListAsync();

            if (!posts.Any())
            {
                return NotFound("No posts found for the specified destination country.");
            }

            return Ok(posts);
        }





    }
}
