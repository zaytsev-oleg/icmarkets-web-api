using IcMarkets.Domain;

namespace IcMarkets.UseCases.GetData;

public sealed class GetDataResponse
{
    public Blockchain[] Data { get; set; } = [];
    
    public int Count => Data.Length;
}
