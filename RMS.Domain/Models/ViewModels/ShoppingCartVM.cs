namespace RMS.Domain.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCardList { get; set; }
        public Order Order { get; set; }
        public OrderDetail OrderDetail { get; set; } 
    }
}
