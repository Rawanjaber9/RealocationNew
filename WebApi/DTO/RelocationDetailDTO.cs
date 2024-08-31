namespace WebApi.DTO
{
    public class RelocationDetailDTO
    {
        public int? UserId { get; set; }
        public string? DestinationCountry { get; set; } // לא חובה להזין ערך
        public DateTime? MoveDate { get; set; }  // Nullable
        public bool? HasChildren { get; set; }  // Nullable
        public List<int>? SelectedCategories { get; set; }  // לא חובה להזין ערך
    }



}
