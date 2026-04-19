using Razor.Templating.Core;

namespace Utils.DotNetCore.ExportPDF
{
    /// <summary>
    /// Internal implementation of <see cref="IHtmlContentRenderer"/> that uses Razor.Templating.Core
    /// to render Razor templates into HTML strings.
    /// </summary>
    public sealed class HtmlContentRenderer : IHtmlContentRenderer
    {
        private readonly IRazorTemplateEngine _engine;

        public HtmlContentRenderer(IRazorTemplateEngine engine)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        public async Task<string> RenderAsync<TModel>(string templateKey, TModel model) where TModel : class
        {
            return await _engine.RenderAsync(templateKey, model);
        }
    }
}