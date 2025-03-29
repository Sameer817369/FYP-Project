using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using payment_gateway_nepal;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;

namespace Kimchi_RMS.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentController(IConfiguration configuration,IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> EsewaPayment()
        {
            try
            {
                var esewaSandboxKey = _configuration["eSewa:SecretKey"];
                PaymentManager paymentManager = new PaymentManager(PaymentMethod.eSewa, PaymentVersion.v2, PaymentMode.Sandbox, esewaSandboxKey);
                // Get current URL (to be used for success and failure callbacks)
                string currentUrl = $"{Request.Scheme}://{Request.Host}";
                var serializedOrder = HttpContext.Session.GetString("PendingOrder");
                var order = JsonConvert.DeserializeObject<Order>(serializedOrder);
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
                    var serializedOrder = HttpContext.Session.GetString("PendingOrder");
                    if (!string.IsNullOrEmpty(serializedOrder))
                    {
                        var order = JsonConvert.DeserializeObject<Order>(serializedOrder);
                        if (order == null)
                        {
                            TempData["error"] = "Failed to deserialize order.";

                            return RedirectToAction("Index", "Cart");
                        }
                        _unitOfWork.Order.Add(order);
                        _unitOfWork.Save();

                        //Order details
                        var shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == order.UserId);
                        if (shoppingCartList == null || !shoppingCartList.Any())
                        {
                            throw new Exception("Shopping cart list is null");
                        }
                        foreach (var cart in shoppingCartList)
                        {
                            var menuList = _unitOfWork.Menu.GetAll(u => u.Id == cart.MenuId);
                            if (cart.Menu == null)
                            {
                                throw new Exception($"Menu item is null for cart item ID: {cart.Id}");
                            }
                            OrderDetail orderDetail = new()
                            {
                                MenuId = cart.MenuId,
                                OrderId = order.Id,
                                Price = cart.Menu.Price,
                                Quantity = cart.Count
                            };
                            _unitOfWork.OrderDetail.Add(orderDetail);
                        }
                        _unitOfWork.Save();
                        //cart clearing after order
                        foreach (var cartItem in shoppingCartList)
                        {
                            _unitOfWork.ShoppingCart.Delete(cartItem);
                        }
                        _unitOfWork.Save();
                        // Clear session cart count and order
                        HttpContext.Session.Remove(ShoppingCart.SessionCart);
                        HttpContext.Session.Remove("PendingOrder");
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
                TempData["error"] = $"An error occurred during payment verification: {ex.Message}";
                return StatusCode(500, "Payment verification error.");
            }
            return RedirectToAction("OrderConformation", "Cart");
        }
    }
}
