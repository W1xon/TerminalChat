using Client;
public record Person(string FirstName, string Message);
public record Group(string Id, string KeyWord);

class Program
{
    private static string userName;
    private static string roomID;
    private static string keyWord;
    private static bool isAuthorized;
    private static ChatService _chat;

    static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Authorization();

        _chat = new ChatService("http://localhost:5032/chat");
        _chat.OnMessageReceived += person => PrintMessage(person.FirstName, person.Message, ConsoleColor.Cyan);
        _chat.OnSystemMessage += msg => 
        {
            PrintMessage("SYSTEM", msg, ConsoleColor.Yellow);
            if (msg != "Комната недоступна") isAuthorized = true;
        };

        try 
        {
            await _chat.Connect();
            await _chat.Say(userName, roomID, keyWord);
            
            // Костыль, но ладно
            await Task.Delay(1000); 

            if (isAuthorized)
            {
                Console.Clear();
                Console.WriteLine($"--- Подключено к [{roomID}] как {userName} ---");
                await StartChatLoop();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка доступа или комната закрыта.");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private static async Task StartChatLoop()
    {
        while (true)
        {
            var message = Console.ReadLine();
            if (string.IsNullOrEmpty(message)) continue;

            Console.SetCursorPosition(0, Console.CursorTop - 1);
            PrintMessage("Вы", message, ConsoleColor.Green);

            await _chat.Send(userName, roomID, keyWord, message);
        }
    }

    private static void Authorization()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("=== SIGNALR TERMINAL CHAT ===");
        Console.ResetColor();

        Console.Write("Имя: ");
        userName = Console.ReadLine() ?? "Guest";
        Console.Write("Комната: ");
        roomID = Console.ReadLine() ?? "Public";
        Console.Write("Ключ: ");
        keyWord = Console.ReadLine() ?? "";
    }

    private static void PrintMessage(string author, string text, ConsoleColor color)
    {
        string time = DateTime.Now.ToString("HH:mm");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"[{time}] ");
        Console.ForegroundColor = color;
        Console.Write($"{author}: ");
        Console.ResetColor();
        Console.WriteLine(text);
    }
}