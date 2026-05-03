using RazorLight;
using Utils.DotNetCore.ExportPDF;

namespace ExportPDF.ConsoleSample.Renderers
{
    public sealed class RazorLightHtmlContentRenderer : IHtmlContentRenderer
    {
        private readonly IRazorLightEngine _engine;

        public RazorLightHtmlContentRenderer(IRazorLightEngine engine)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        public async Task<string> RenderAsync<TModel>(string templateKey, TModel model) where TModel : class
        {
            return await _engine.CompileRenderAsync(templateKey, model);
        }
    }
}
