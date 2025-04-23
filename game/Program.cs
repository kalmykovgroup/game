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
            
            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    var wsManager = context.RequestServices.GetRequiredService<WebSocketManager>();
                    await wsManager.HandleConnection(context);
                }
                else
                {
                    await next();
                }
            });

            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
 
            app.Urls.Add("http://0.0.0.0:8005");

        }
    }
}

 