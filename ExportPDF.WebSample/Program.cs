using Utils.DotNetCore.ExportPDF;

namespace ExportPDF.WebSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddRazorTemplating();
            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IHtmlContentRenderer, HtmlContentRenderer>();
            builder.Services.AddTransient<IHtmlPdfExporter, PuppeteerSharpPdfExporter>();
            builder.Services.AddTransient<IPdfDocumentGenerator, PdfDocumentGenerator>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.MapInvoiceEndpoints();

            app.Run();
        }
    }
}