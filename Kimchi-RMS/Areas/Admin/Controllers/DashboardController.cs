using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Models.ViewModels;
using RMS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using RMS.Domain.Models;

namespace Kimchi_RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRole.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        static int prevMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, prevMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
 
        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetTotalBookingChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != BookingStatus.Pending);

            var countByCurrentMonth = totalBookings.Count(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now);
            var countByPrevMonth = totalBookings.Count(u => u.BookingDate >= previousMonthStartDate && u.BookingDate < currentMonthStartDate);

            return Json(GetBarVmData(totalBookings.Count(), countByCurrentMonth, countByPrevMonth));
        }
        public async Task<IActionResult> GetTotalOrdersChartData()
        {
            var totalOrders = _unitOfWork.Order.GetAll(u => u.Status != Status.Pending);
            var ordersCurrentMonthCount = totalOrders.Count(u => u.OrderDate >= currentMonthStartDate && u.OrderDate <= DateTime.Now);
            var ordersPrevMonthCount = totalOrders.Count(u => u.OrderDate < currentMonthStartDate && u.OrderDate >= previousMonthStartDate);


            return Json(GetBarVmData(totalOrders.Count(),ordersCurrentMonthCount,ordersPrevMonthCount));

        }
        public async Task<IActionResult> GetTotalRevenuChartData()
        {
            var totalTransactions =_unitOfWork.Transaction.GetAll();
            var totalRevenue = Convert.ToInt32(totalTransactions.Sum(u => u.Amount));

            var tranCurrentMonthCount = totalTransactions.Where(u => u.TransactionDate >= currentMonthStartDate && u.TransactionDate <= DateTime.Now).Sum(u => u.Amount);
            var tranPrevMonthCount = totalTransactions.Where(u => u.TransactionDate < currentMonthStartDate && u.TransactionDate >= previousMonthStartDate).Sum(u => u.Amount);

            return Json(GetBarVmData(totalRevenue, tranCurrentMonthCount, tranPrevMonthCount));
        }

        private static BarchartVm GetBarVmData(int totalCount, double currentMonthCount, double previousMonthCount)
        {
            BarchartVm barchartVm = new();

            int increaseDecreaseRatio = 100;
            if (previousMonthCount != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((currentMonthCount - previousMonthCount) / previousMonthCount * 100);
            }
            barchartVm.TotalCount = totalCount;
            barchartVm.CountInCurrentMonth = Convert.ToInt32(currentMonthCount);
            barchartVm.HasRatioIncreased = currentMonthCount > previousMonthCount;
            barchartVm.Series = new int[] { increaseDecreaseRatio };

            return barchartVm;

        }
        
        public IActionResult GetOrderLineChartData()
        {
            var orderData = _unitOfWork.Order.GetAll(u => u.OrderDate >= DateTime.Now.AddDays(-30))
                .GroupBy(u => u.OrderDate.Date)
                .Select(u => new
            {
                DateTime = u.Key,
                NewOrderCount = u.Count()
            });

            var newOrderData = orderData.Select(o=>o.NewOrderCount).ToArray();
            var categories = orderData.Select(u=>u.DateTime.ToString("MM/dd/yyyy")).ToArray();
            List<ChartData> chartDataList = new ()
            {
                new ChartData
                {
                    Name = "New Orders",
                    Data = newOrderData,
                }
            };

            LineChartVm lineChartVm = new()
            {
                Categories = categories,
                Series = chartDataList
            };
      
            return Json(lineChartVm);
        }

    }
}
