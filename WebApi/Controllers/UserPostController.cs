﻿using Data.Models;
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


    }
}