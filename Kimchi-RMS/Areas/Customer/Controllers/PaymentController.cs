using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using payment_gateway_nepal;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;

namespace Kimchi_RMS.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> EsewaPayment()
        {
            try
            {
                if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                var esewaSandboxKey = _configuration["eSewa:SecretKey"];
                PaymentManager paymentManager = new PaymentManager(PaymentMethod.eSewa, PaymentVersion.v2, PaymentMode.Sandbox, esewaSandboxKey);
                // Get current URL (to be used for success and failure callbacks)
                string currentUrl = $"{Request.Scheme}://{Request.Host}";
                //getting order session data
                var order = GetSessionOrder();
                // Create the payment request object
                dynamic request = new
                {
                    Amount = order.TotalAmount,
                    TotalAmount = order.TotalAmount,
                    TransactionUuid = $"tx-{Guid.NewGuid().ToString("N").Substring(0, 8)}", // Unique transaction ID
                    ProductCode = "EPAYTEST", // Product code (for testing purpose)
                    SuccessUrl = $"{currentUrl}/Payment/Success", // Redirect on success
                    FailureUrl = $"{currentUrl}/Customer/Cart/Index", // Redirect on failure
                    SignedFieldNames = "total_amount,transaction_uuid,product_code" // Fields to be signed
                };

                // Initiate payment through the payment gateway
                var response = await paymentManager.InitiatePaymentAsync<ApiResponse>(request);

                // Check if response data is valid
                if (response != null && !string.IsNullOrEmpty(response.data))
                {
                    Transaction transaction = new()
                    {
                        Id = request.TransactionUuid,
                        Amount = request.TotalAmount
                    };
                    HttpContext.Session.SetString("Transaction", JsonConvert.SerializeObject(transaction));

                    // Redirect to eSewa payment gateway
                    return Redirect(response.data);  // Redirect to the URL provided by eSewa
                }
                else
                {
                    return NotFound("Payment initiation failed. Please try again.");  // Show error page
                }
            }
            catch (Exception ex)
            {
                return NotFound($"Error initiating payment:{ex.Message}");  // Show error page
            }
        }

        [Route("Payment/Success")]
        public async Task<IActionResult> VerifyEsewaPayment(string data)
        {
            try
            {
                var esewaSandboxKey = _configuration["eSewa:SecretKey"];
                PaymentManager paymentManager = new PaymentManager(PaymentMethod.eSewa, PaymentVersion.v2, PaymentMode.Sandbox, esewaSandboxKey);
                eSewaResponse response = await paymentManager.VerifyPaymentAsync<eSewaResponse>(data);
                if (response != null && !string.IsNullOrEmpty(response.status) && string.Equals(response.status, "complete", StringComparison.OrdinalIgnoreCase))
                {
                    // Handle successful payment
                    var order = GetSessionOrder();
                    var transaction = GetSessionTransaction();
                    if (order != null || transaction !=null)
                    {
                        _unitOfWork.Order.Add(order);
                        _unitOfWork.Save();
                        transaction.OrderId = order.Id;
                        _unitOfWork.Transaction.Add(transaction);
                        _unitOfWork.Save();
                        //Order details
                        HandelOrderDetails(order.Id, order.UserId);
                        // Clear session cart count and order
                        ClearSession();
                    }
                    else
                    {
                        TempData["error"] = "Payment verification failed. Please try again.";
                        return RedirectToAction("Index", "Cart");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Payment verification error. {ex.Message}");
            }
            return RedirectToAction("OrderConformation", "Cart");
        }

        //hepler methods
        //saves orderdetails
        private void HandelOrderDetails(int orderId, string userId)
        {
            var shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId);
            if (shoppingCartList == null || !shoppingCartList.Any())
            {
                throw new Exception("Shopping cart list is empty.");
            }

            foreach (var cart in shoppingCartList)
            {
                var menuList = _unitOfWork.Menu.GetAll(u => u.Id == cart.MenuId);
                if (cart.Menu == null)
                {
                    throw new Exception($"Menu item is null for cart item ID: {cart.Id}");
                }

                var orderDetail = new OrderDetail
                {
                    MenuId = cart.MenuId,
                    OrderId = orderId,
                    Price = cart.Menu.Price,
                    Quantity = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
            }

            _unitOfWork.Save();

            foreach (var cartItem in shoppingCartList)
            {
                _unitOfWork.ShoppingCart.Delete(cartItem);
            }

            _unitOfWork.Save();
        }


        private Order GetSessionOrder()
        {
            var serializedOrder = HttpContext.Session.GetString("PendingOrder");
            return !string.IsNullOrEmpty(serializedOrder) ? JsonConvert.DeserializeObject<Order>(serializedOrder) : null;
        }

        private Transaction GetSessionTransaction()
        {
            var serializedTransaction = HttpContext.Session.GetString("Transaction");
            return !string.IsNullOrEmpty(serializedTransaction) ? JsonConvert.DeserializeObject<Transaction>(serializedTransaction) : null;
        }
        // Clear session cart count and order
        private void ClearSession()
        {
            HttpContext.Session.Remove(ShoppingCart.SessionCart);
            HttpContext.Session.Remove("PendingOrder");
            HttpContext.Session.Remove("Transaction");
        }
    }
}






    
 

  
    
