using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobHunter
{
    internal class Program
    {
        public static TelegramBotClient bot;

        static async Task Main(string[] args)
        {
            bot = new TelegramBotClient(Environment.GetEnvironmentVariable("API_TOKEN"));
            bot.StartReceiving(HandleNewMessage, async (bot, ex, ct) => Console.WriteLine(ex));

            var me = await bot.GetMeAsync();
            Console.WriteLine($"@{me.Username} is running...");

            while (true) Thread.Sleep(1000); // waste CPU cycles
        }

        private static async Task HandleNewMessage(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message is null) return;
            if (update.Message.Text is null) return;

            var msg = update.Message;
            Console.WriteLine($"Received message '{msg.Text}' in {msg.Chat}");
            // let's echo back received text in the chat
            await bot.SendTextMessageAsync(msg.Chat, $"{msg.From} said: {msg.Text}");
        }
    }
}
