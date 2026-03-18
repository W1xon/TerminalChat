using Microsoft.AspNetCore.SignalR;

namespace SignalRLearn;

public record Person(string FirstName, string Message);
public record Group(string Id, string KeyWord);
public class ChatHub : Hub
{

    private static readonly GroupRegistry _registry = new();
    public async Task Send(Person user, Group group)
    {
        await Clients.GroupExcept(group.Id, [Context.ConnectionId])
            .SendAsync("Receive", user);
    }

    public async Task Say(Person user, Group group)
    {
        if (!_registry.IsTrust(group))
        {
            await Clients.Caller.SendAsync("Greeting", "Комната недоступна");
            return;
        }

        await Clients.Caller.SendAsync("Greeting", $"Добро пожаловать в {group.Id}");

        await Groups.AddToGroupAsync(Context.ConnectionId, group.Id);

        string msg = $"{user.FirstName} присоединился к столу";

        await Clients.GroupExcept(group.Id, [Context.ConnectionId])
            .SendAsync("Greeting", msg);
    }

    public override async Task OnConnectedAsync()
    {
         Console.WriteLine($"{Context.ConnectionId}: присоединился");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"{Context.ConnectionId}: Отсоединился");
        await base.OnDisconnectedAsync(exception);
    }
}
