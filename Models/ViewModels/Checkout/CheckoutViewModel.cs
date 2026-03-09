using System.ComponentModel.DataAnnotations;

namespace Core_Diski_Demo.Models.ViewModels.Checkout;

public class CheckoutViewModel
{
    [Required, StringLength(200)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    public CheckoutPaymentMethod PaymentMethod { get; set; }

    public decimal TotalAmount { get; set; }
}

public enum CheckoutPaymentMethod
{
    PayFast,
    Ozow,
    Yoco,
    PeachPayments,
    ManualEft
}

public record PaymentInitiationResult(
    CheckoutPaymentMethod Method,
    string OrderReference,
    decimal Amount,
    string? GatewayUrl,
    string Message);

public class CheckoutConfirmationViewModel
{
    public string OrderReference { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public CheckoutPaymentMethod PaymentMethod { get; set; }
    public string? GatewayUrl { get; set; }
    public string Message { get; set; } = string.Empty;
}
