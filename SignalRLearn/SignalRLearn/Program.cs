using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace SignalRLearn;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSignalR();
        var app = builder.Build();
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapHub<ChatHub>("/chat");

        app.Run();
    }
}