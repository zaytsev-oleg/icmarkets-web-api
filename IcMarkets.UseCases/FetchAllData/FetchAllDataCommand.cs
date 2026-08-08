using MediatR;

namespace IcMarkets.UseCases.FetchAllData;

public sealed record FetchAllDataCommand : IRequest<FetchAllDataResponse>;
