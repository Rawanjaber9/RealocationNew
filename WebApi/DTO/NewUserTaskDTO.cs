namespace WebApi.DTO
{
    public class NewUserTaskDTO
    {
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PriorityId { get; set; }
        public string PersonalNote { get; set; }
    }
}
