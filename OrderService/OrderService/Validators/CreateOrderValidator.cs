using OrderService.Dtos;
using FluentValidation;

namespace OrderService.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrder>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId is required.");
        RuleFor(x => x.OrderStatus).NotEmpty().WithMessage("OrderStatus is required.");
        RuleFor(x => x.TotalAmount).GreaterThan(0).WithMessage("TotalAmount must be greater than 0.");
        RuleFor(x => x.OrderDate).NotEmpty().WithMessage("OrderDate is required.");
        RuleFor(x => x.PaymentStatus).NotEmpty().WithMessage("PaymentStatus is required.");
        RuleFor(x => x.ShippingAddress).NotEmpty().WithMessage("ShippingAddress is required.");
    }
}