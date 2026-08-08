using IcMarkets.UseCases.FetchData;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IcMarkets.UseCases;
using IcMarkets.UseCases.FetchAllData;
using IcMarkets.UseCases.GetData;

namespace IcMarkets.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlockchainController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ILogger<BlockchainController> _logger;

    public BlockchainController(ISender mediator, ILogger<BlockchainController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Route("FetchData/{coin}/{chain}")]
    public async Task<IActionResult> FetchDataAsync([FromRoute] string coin, [FromRoute] string chain, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(new FetchDataCommand(coin, chain), ct);
            return Ok();
        }
        catch (Exception ex) when (ex.Message == Constants.NotSupportedCoinToChainPair)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex) when (ex.Message.Contains(Constants.EmptyResponse))
        {
            return NoContent();
        }
    }

    [HttpPost]
    [Route("FetchAllData")]
    public async Task<IActionResult> FetchAllDataAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new FetchAllDataCommand(), ct);
        return Ok();
    }

    [HttpGet]
    [Route("GetData")]
    public async Task<IActionResult> GetDataAsync([FromQuery] string? coin, [FromQuery] string? chain, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDataQuery(coin, chain), ct);
        return Ok(result);
    }
}
