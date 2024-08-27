namespace WebApi.DTO
{
    public class UpdateTaskDTO
    {
        public string? TaskName { get; set; }
        public string? TaskDescription { get; set; }
        public int? PriorityId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? PersonalNote { get; set; }
    }


}
