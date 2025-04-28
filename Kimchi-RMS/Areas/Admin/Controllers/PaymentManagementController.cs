using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;
using System.Transactions;

namespace Kimchi_RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRole.Role_Admin)]
    public class PaymentManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentManagementController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            try
            {
                var transaction = _unitOfWork.Transaction.GetAll().OrderByDescending(u=>u.TransactionDate).ToList();
                foreach (var item in transaction)
                {
                    var orderList = _unitOfWork.Order.GetById(u => u.Id == item.OrderId);
                    var useList = _unitOfWork.User.GetAll(u => u.Id == orderList.UserId);
                }
                return View(transaction);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
           
        }
        [HttpPost]
        public IActionResult Delete(string transactionId)
        {
            try
            {
                var transactionToDelete = _unitOfWork.Transaction.GetById(u => u.Id == transactionId);
                if (transactionToDelete != null)
                {
                    _unitOfWork.Transaction.Delete(transactionToDelete);
                    _unitOfWork.Save();
                    TempData["success"] = "Transaction Successfully Deleted";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "Transaction not removed";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult FilterByDate(DateTime? startDate, DateTime?endDate)
        {
            var filteredTransactions = startDate.HasValue && endDate.HasValue
                ? _unitOfWork.Transaction.GetAll(u=>u.TransactionDate >= startDate.Value &&  u.TransactionDate <= endDate.Value) 
                : _unitOfWork.Transaction.GetAll();

            foreach(var transaction in filteredTransactions)
            {
                var orderList = _unitOfWork.Order.GetById(u => u.Id == transaction.OrderId);
                var useList = _unitOfWork.User.GetAll(u => u.Id == orderList.UserId);
            }
            if (!filteredTransactions.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }
            return PartialView("_TransactionTablePartial", filteredTransactions);
        }
    }
}
