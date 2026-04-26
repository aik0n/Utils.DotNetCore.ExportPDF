using ExportPDF.WebSample.Services;
using Utils.DotNetCore.ExportPDF;

namespace ExportPDF.WebSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IDocumentCache, DocumentCache>();
            builder.Services.AddScoped<IDocumentSampleFactory, DocumentSampleFactory>();

            builder.Services
                .AddPdfGenerator()
                .WithDefaultContentRenderer()
                .WithPuppeteerExporter();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
