namespace WebApi.DTO
{
    public class RecommendTasksInputDTO
    {
        public int UserId { get; set; }
        public List<int> RecommendedTaskIds { get; set; }
    }
}
