namespace Utils.DotNetCore.ExportPDF
{
    public interface IHtmlContentRenderer
    {
        /// <summary>
        /// Renders the Razor template identified by <paramref name="templateKey"/>
        /// using <paramref name="model"/> and returns the resulting HTML string.
        /// </summary>
        Task<string> RenderAsync<TModel>(string templateKey, TModel model) where TModel : class;
    }
}