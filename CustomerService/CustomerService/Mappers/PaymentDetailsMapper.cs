using CustomerService.Data.Entities;
using CustomerService.DTOs;

namespace CustomerService.Mappers;

public interface IPaymentDetailsMapper
{
    PaymentDetailsDto MapToDto(PaymentDetail paymentDetail);
    PaymentDetail MapToPaymentDetail(PaymentDetailsDto paymentDetail);
}

public class PaymentDetailsMapper : IPaymentDetailsMapper
{
    public PaymentDetailsDto MapToDto(PaymentDetail paymentDetail)
    {
        if(paymentDetail is null) return null;
        return new PaymentDetailsDto()
        {
            PaymentId = paymentDetail.PaymentId,
            CustomerId = paymentDetail.CustomerId,
            CardNumber = paymentDetail.CardNumber,
            CVV = paymentDetail.CVV,
            CardHolderName = paymentDetail.CardHolderName,
            ExpiryDate = paymentDetail.ExpiryDate,
        };
    }

    public PaymentDetail MapToPaymentDetail(PaymentDetailsDto paymentDetail)
    {
        if(paymentDetail is null) return null;
        return new PaymentDetail()
        {
            PaymentId = paymentDetail.PaymentId,
            CustomerId = paymentDetail.CustomerId,
            CardNumber = paymentDetail.CardNumber,
            CVV = paymentDetail.CVV,
            CardHolderName = paymentDetail.CardHolderName,
            ExpiryDate = paymentDetail.ExpiryDate,
        };
    }
}