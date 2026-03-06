using Core_Diski_Demo.Models.ViewModels.Checkout;

namespace Core_Diski_Demo.Services;

public class PaymentGatewayService : IPaymentGatewayService
{
    public PaymentInitiationResult Initiate(CheckoutPaymentMethod method, decimal amount, string orderReference)
    {
        return method switch
        {
            CheckoutPaymentMethod.PayFast => new PaymentInitiationResult(method, orderReference, amount, "https://www.payfast.co.za", "Redirect to PayFast hosted checkout."),
            CheckoutPaymentMethod.Ozow => new PaymentInitiationResult(method, orderReference, amount, "https://ozow.com", "Proceed with Ozow instant EFT flow."),
            CheckoutPaymentMethod.Yoco => new PaymentInitiationResult(method, orderReference, amount, "https://www.yoco.com", "Use Yoco card checkout link integration."),
            CheckoutPaymentMethod.PeachPayments => new PaymentInitiationResult(method, orderReference, amount, "https://peachpayments.com", "Process payment via Peach Payments gateway."),
            _ => new PaymentInitiationResult(method, orderReference, amount, null, "Manual EFT selected. Show banking details and await proof of payment.")
        };
    }
}
