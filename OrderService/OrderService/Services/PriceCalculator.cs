namespace OrderService.Services;

public interface IPriceCalculator
{
    decimal CalculatePrice(int quantity, decimal productPrice);
}
public class PriceCalculator : IPriceCalculator
{
    public decimal CalculatePrice(int quantity, decimal productPrice)
    {
        return quantity * productPrice;
    }
}