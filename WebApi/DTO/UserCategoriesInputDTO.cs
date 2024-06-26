namespace WebApi.DTO
{
    public class UserCategoriesInputDTO
    {
        public int UserId { get; set; }
        public List<int> SelectedCategories { get; set; }
    }
}
