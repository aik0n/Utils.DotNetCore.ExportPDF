using Microsoft.Extensions.Caching.Memory;

namespace ExportPDF.WebSample.Services
{
    public class DocumentCache : IDocumentCache
    {
        private readonly IMemoryCache _inner;

        public DocumentCache(IMemoryCache inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public bool TryGetValue(object key, out object? value)
        {
            return _inner.TryGetValue(key, out value);
        }

        public ICacheEntry CreateEntry(object key)
        {
            return _inner.CreateEntry(key);
        }

        public void Remove(object key)
        {
            _inner.Remove(key);
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public string CreateCacheKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
