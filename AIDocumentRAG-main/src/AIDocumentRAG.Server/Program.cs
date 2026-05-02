namespace AIDocumentRAG.Server
{
    using AIDocumentRAG.Server.Core.ChatInference;
using AIDocumentRAG.Server.Core.FileManagement;
using AIDocumentRAG.Server.Core.NoteGeneration;
using AIDocumentRAG.Server.Models.Configuration;
using AIDocumentRAG.Server.Services.ChatInference;
using AIDocumentRAG.Server.Services.FileManagement;
using AIDocumentRAG.Server.Services.NoteGeneration;

    using Microsoft.SemanticKernel;

    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Add CORS services
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("https://localhost:58585", "https://localhost:4200", "https://localhost:51222", "https://localhost:55441", "https://localhost:63576", "https://localhost:63577")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Configure File Management Options
            builder.Services.Configure<FileManagementOptions>(
                builder.Configuration.GetSection(FileManagementOptions.SectionName));

            // Register AI Services
            string? openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (!string.IsNullOrEmpty(openAiApiKey))
            {
                // Register AI Services using KernelBuilder pattern
                var kernelBuilder = Kernel.CreateBuilder();
                kernelBuilder.AddOpenAIChatCompletion("gpt-4o-mini", openAiApiKey, serviceId: "openai");
                Console.WriteLine("OpenAI service registered successfully.");

                Kernel kernel = kernelBuilder.Build();
                builder.Services.AddSingleton(kernel);
                builder.Services.AddScoped<IAIChatService, AIChatService>();
            }
            else
            {
                // Check if Ollama is available
                bool ollamaAvailable = false;
                try
                {
                    using var client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(2);
                    var response = client.GetAsync("http://localhost:11434").Result;
                    ollamaAvailable = response.IsSuccessStatusCode;
                }
                catch
                {
                    ollamaAvailable = false;
                }

                if (ollamaAvailable)
                {
                    Console.WriteLine("Ollama detected. Using Ollama for AI services.");
                    var kernelBuilder = Kernel.CreateBuilder();
                    kernelBuilder.AddOllamaChatCompletion("phi4", new Uri("http://localhost:11434"), serviceId: "ollama");
                    Kernel kernel = kernelBuilder.Build();
                    builder.Services.AddSingleton(kernel);
                    builder.Services.AddScoped<IAIChatService, AIChatService>();
                }
                else
                {
                    Console.WriteLine("No AI provider available (OpenAI or Ollama). Using Mock AI service for testing.");
                    builder.Services.AddScoped<IAIChatService, MockAIChatService>();
                }
            }

            // Register Document Summary Service as Scoped to match AIChatService lifetime
            builder.Services.AddScoped<IDocumentSummaryService, DocumentSummaryService>();

            // Register Note Generation Services
            builder.Services.AddScoped<INoteRepository, NoteRepository>();
            builder.Services.AddScoped<INoteGenerationService, NoteGenerationService>();

            // Register File Management Services (updated order for DI)
            builder.Services.AddScoped<IFileProcessor, FileProcessor>();
            builder.Services.AddScoped<IDirectoryCopier, DirectoryCopier>();
            builder.Services.AddScoped<FileManagementService>();

            WebApplication app = builder.Build();

            app.UseDefaultFiles();
            app.MapStaticAssets();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            // Enable CORS middleware
            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}