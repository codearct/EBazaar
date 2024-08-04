namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
public record StoreBasketResult(string UserName);

public class StoreVasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreVasketCommandValidator()
    {
        RuleFor(x => x.Cart).NotNull().WithMessage("Cart can not be null");
        RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class StoreBasketHandler(IBasketRepository repository) 
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        ShoppingCart newCart = command.Cart;

        //TODO:update cache
        var cart = await repository.StoreBasket(newCart, cancellationToken);

        return new StoreBasketResult(cart.UserName);
    }
}
