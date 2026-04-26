using Microsoft.Extensions.Caching.Memory;

namespace ExportPDF.WebSample.Services
{
    public interface IDocumentCache : IMemoryCache
    {
        string CreateCacheKey();
    }
}
