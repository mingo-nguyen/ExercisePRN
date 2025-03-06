namespace FoodShopBlazor.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public List<Food> Menu { get; set; } = new List<Food>();
    }

}
