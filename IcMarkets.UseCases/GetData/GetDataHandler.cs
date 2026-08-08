using IcMarkets.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IcMarkets.UseCases.GetData;

public class GetDataHandler : IRequestHandler<GetDataQuery, GetDataResponse>
{
    private IcMarketsDbContext _dbContext;
    private ILogger<GetDataHandler> _logger;

    public GetDataHandler(IcMarketsDbContext context, ILogger<GetDataHandler> logger)
    {
        _dbContext = context;
        _logger = logger;
    }

    public async Task<GetDataResponse> Handle(GetDataQuery request, CancellationToken ct)
    {
        var query = _dbContext.Blockchains.AsQueryable();

        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(x => x.Name == request.Name);
        }

        query = query.OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Pk);

        var data = await query.ToArrayAsync(ct);

        return new GetDataResponse { Data = data };
    }
}
