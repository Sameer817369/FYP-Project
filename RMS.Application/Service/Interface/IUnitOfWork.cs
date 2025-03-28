namespace RMS.Application.Service.Interface
{
    public interface IUnitOfWork
    {
        IMenuRepository Menu { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IUserRepository User { get; }
        IOrderRepository Order { get; }
        IOrderDetailRepository OrderDetail { get; }
        IBookingRepository Booking { get; }
        void Save();
    }
}
