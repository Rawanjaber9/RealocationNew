namespace WebApi.DTO
{
    public class CommentDTO
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
    }
}
