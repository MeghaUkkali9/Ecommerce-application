using PaymentService.Data;

namespace PaymentService.Extensions;

public static class Extension
{
    public static PaymentMethod MapPaymentMethod(this string paymentMethod)
    {
        return paymentMethod switch
        {
            "credit" => PaymentMethod.CreditCard,
            "debit" => PaymentMethod.DebitCard,
            "cash" => PaymentMethod.CashOnDelivery,
            _ => throw new NotSupportedException("Unsupported payment method.")
        };
    }
}