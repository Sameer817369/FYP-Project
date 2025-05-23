﻿using RMS.Application.Service.Interface;
using RMS.Infrastructure.Data;

namespace RMS.Application.Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IMenuRepository Menu { get; set; }
        public IShoppingCartRepository ShoppingCart { get; set; }
        public IUserRepository User { get; set; }
        public IOrderRepository Order { get; set; }
        public IOrderDetailRepository OrderDetail { get; set; }
        public IBookingRepository Booking { get; set; }
        public IFeedBackRepository FeedBack { get; set; }
        public IEmoloyeeRepository Employee { get; set; }
        public ITransactionRepository Transaction { get; set; }
        public IInventoryRepository Inventory { get; set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Menu = new MenuRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            User = new UserRepository(_db);
            Order = new OrderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            Booking = new BookingRepository(_db);
            FeedBack = new FeedbackRepository(_db);
            Employee = new EmployeeRepository(_db);
            Transaction = new TransactionRepository(_db);
            Inventory = new InventoryRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
