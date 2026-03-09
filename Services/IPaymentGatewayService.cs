using Core_Diski_Demo.Models.ViewModels.Checkout;

namespace Core_Diski_Demo.Services;

public interface IPaymentGatewayService
{
    PaymentInitiationResult Initiate(CheckoutPaymentMethod method, decimal amount, string orderReference);
}
