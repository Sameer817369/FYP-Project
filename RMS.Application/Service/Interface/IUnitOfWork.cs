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
        IFeedBackRepository FeedBack { get; }
        IEmoloyeeRepository Employee { get; }
        ITransactionRepository Transaction { get; }
        IInventoryRepository Inventory { get; }
        void Save();
    }
}
