using MediatR;

namespace IcMarkets.UseCases.FetchData;

public sealed record FetchDataCommand(string Coin, string Chain) : IRequest<FetchDataResponse>;
