using game.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace game
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
           

            builder.Services.AddControllers(); 
            builder.Services.AddOpenApi();

            builder.Services.AddSingleton<WebSocketManager>();

            var app = builder.Build();
            
            app.Urls.Add("http://0.0.0.0:8005");
            
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Сервер запущен!");
            logger.LogInformation("Run");

            app.UseAuthorization();

            app.MapControllers();

            app.UseWebSockets();

            WebSocketManager wsManager = app.Services.GetRequiredService<WebSocketManager>();
            
            app.Map("/ws",  wsManager.HandleConnection);
            app.Run();
        }
    }
}

 