using Microsoft.AspNetCore.SignalR.Client;

namespace Client;

public class ChatService 
{
    private readonly HubConnection _connection;
    public event Action<Person> OnMessageReceived;
    public event Action<string> OnSystemMessage;

    public ChatService(string url) {
        _connection = new HubConnectionBuilder().WithUrl(url).WithAutomaticReconnect().Build();
        _connection.On<Person>("Receive", p => OnMessageReceived?.Invoke(p));
        _connection.On<string>("Greeting", g => OnSystemMessage?.Invoke(g));
    }

    public async Task Connect() => await _connection.StartAsync();
    public async Task Say(string user, string room, string key) => 
        await _connection.InvokeAsync("Say", new Person(user, ""), new Group(room, key));
    public async Task Send(string user, string room, string key, string msg) => 
        await _connection.InvokeAsync("Send", new Person(user, msg), new Group(room, key));
}