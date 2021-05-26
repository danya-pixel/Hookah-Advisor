using System;
using Hookah_Advisor.Parsers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Hookah_Advisor.Repositories;
using Hookah_Advisor.TelegramBot;
using Telegram.Bot.Types.Enums;


namespace Hookah_Advisor
{
    class Program
    {
        private static ITelegramBotClient _botClient;
        private static readonly UserRepository UserRepository = new(new UserParser());
        private static readonly TobaccoRepository TobaccoRepository = new(new TobaccoParser());

        static void Main()
        {
            _botClient = new TelegramBotClient(BotSettings.Token);

            var me = _botClient.GetMeAsync().Result;
            Console.WriteLine(
                $"Bot is working with id: {me.Id} and name {me.FirstName}."
            );

            _botClient.OnMessage += BotOnMessage;
            _botClient.OnCallbackQuery += BotOnCallbackQueryReceived;
            _botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            _botClient.StopReceiving();
            UserRepository.Save();
        }

        private static void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type != MessageType.Text)
            {
                TelegramMessageSender.SendWhenNotTextMessage(message, _botClient);
                return;
            }

            TelegramMessage.MessageReceived(message, UserRepository,
                TobaccoRepository, _botClient);
        }

        private static void BotOnCallbackQueryReceived(object sender,
            CallbackQueryEventArgs callbackQueryEventArgs)
        {
            CallbackHandler.BotOnCallbackQueryReceived(UserRepository, TobaccoRepository, callbackQueryEventArgs,
                _botClient);
        }
    }
}