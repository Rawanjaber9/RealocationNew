namespace WebApi.DTO
{
    public class CombinedInputDTO
    {
        public int UserId { get; set; }
        public string DestinationCountry { get; set; }
        public DateTime MoveDate { get; set; }
        public bool HasChildren { get; set; }
        public List<int> SelectedCategories { get; set; }
    }

}
