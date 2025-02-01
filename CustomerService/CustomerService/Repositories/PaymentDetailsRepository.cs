using CustomerService.Data;
using CustomerService.Data.Entities;
using CustomerService.DTOs;
using CustomerService.Mappers;

namespace CustomerService.Repositories;

public interface IPaymentDetailsRepository
{
    bool DeletePaymentDetails(int customerId);
    PaymentDetailsDto GetPaymentDetails(int customerId);
    PaymentDetailsDto CreatePaymentDetails(PaymentDetail paymentDetail);
}

public class PaymentDetailsRepository : IPaymentDetailsRepository
{
    private readonly DbContext _dbContext;
    private readonly IPaymentDetailsMapper _paymentDetailsMapper;

    public PaymentDetailsRepository(DbContext dbContext, IPaymentDetailsMapper paymentDetailsMapper)
    {
        _dbContext = dbContext;
        _paymentDetailsMapper = paymentDetailsMapper;
    }

    public PaymentDetailsDto GetPaymentDetails(int customerId)
    {
        var paymentDetail = _dbContext.Set<PaymentDetail>().FirstOrDefault(x => x.CustomerId == customerId);
        return _paymentDetailsMapper.MapToDto(paymentDetail);
    }

    public PaymentDetailsDto CreatePaymentDetails(PaymentDetail paymentDetail)
    {
        var paymentDetailEntity = _dbContext.Set<PaymentDetail>().Add(paymentDetail);
        _dbContext.SaveChanges();
        return _paymentDetailsMapper.MapToDto(paymentDetailEntity.Entity);
    }
    
    public bool DeletePaymentDetails(int paymentId)
    {
        var paymentDetail = _dbContext.Set<PaymentDetail>().FirstOrDefault(x => x.PaymentId==paymentId);
        return paymentDetail == null;
    }
}