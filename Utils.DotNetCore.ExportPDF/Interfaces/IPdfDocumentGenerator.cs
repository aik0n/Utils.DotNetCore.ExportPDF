namespace Utils.DotNetCore.ExportPDF
{
    /// <summary>
    /// Orchestrates the full pipeline:
    /// Typed model -> Stage 1 -> HTML string -> Stage 2 -> PDF bytes
    /// Callers inject this service and never have to know which template
    /// engine or PDF engine is running underneath.
    /// </summary>
    public interface IPdfDocumentGenerator
    {
        /// <summary>
        /// Renders <paramref name="templateKey"/> with <paramref name="model"/>
        /// and returns the resulting PDF as a byte array ready to stream to the client.
        /// </summary>
        Task<byte[]> GenerateAsync<TModel>(string templateKey, TModel model, object options) where TModel : class;
    }
}