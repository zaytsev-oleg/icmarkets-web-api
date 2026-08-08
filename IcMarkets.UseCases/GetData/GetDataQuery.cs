using MediatR;

namespace IcMarkets.UseCases.GetData;

public sealed record GetDataQuery(string? Coin, string? Chain) : IRequest<GetDataResponse>
{
    public string? Name => string.IsNullOrEmpty(Coin) || string.IsNullOrEmpty(Chain) ? null : $"{Coin?.ToUpper()}.{Chain}";
}
