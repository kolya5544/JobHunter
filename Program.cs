using JobHunter.Database;
using JobHunter.JSON;
using System.Text;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobHunter
{
    internal class Program
    {
        public static TelegramBotClient bot;
        public static string TG_TOKEN = Environment.GetEnvironmentVariable("API_TOKEN");
        public static UserState us = new UserState();
        public static API api = new API();

        static async Task Main(string[] args)
        {
            bot = new TelegramBotClient(TG_TOKEN);
            bot.StartReceiving(TelegramBot.HandleNewMessage, async (bot, ex, ct) => Console.WriteLine(ex), new Telegram.Bot.Polling.ReceiverOptions() { ThrowPendingUpdates = true });

            var me = await bot.GetMeAsync();
            Console.WriteLine($"@{me.Username} is running...");

            while (true) Thread.Sleep(1000); // waste CPU cycles
        }

        
    }
}
