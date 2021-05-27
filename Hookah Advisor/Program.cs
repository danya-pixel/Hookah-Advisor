using System;
using Hookah_Advisor.Parsers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Hookah_Advisor.Repositories;
using Hookah_Advisor.TelegramBot;
using Ninject;
using Telegram.Bot.Types.Enums;


namespace Hookah_Advisor
{
    class Program
    {
        private static ITelegramBotClient _botClient;
        private static TobaccoRepository _tobaccoRepository;
        private static UserRepository _userRepository;

        static void Main()
        {
            _botClient = new TelegramBotClient(BotSettings.Token);
            var container = ConfigureContainer();
            _userRepository = container.Get<UserRepository>();
            _tobaccoRepository = container.Get<TobaccoRepository>();

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
            _userRepository.Save();
        }

        private static void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type != MessageType.Text)
            {
                TelegramMessageSender.SendWhenNotTextMessage(message, _botClient);
                return;
            }

            TelegramMessage.MessageReceived(message, _userRepository,
                _tobaccoRepository, _botClient);
        }

        private static void BotOnCallbackQueryReceived(object sender,
            CallbackQueryEventArgs callbackQueryEventArgs)
        {
            CallbackHandler.BotOnCallbackQueryReceived(_userRepository, _tobaccoRepository, callbackQueryEventArgs,
                _botClient);
        }

        private static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();
            container.Bind<IParser<User>>().To<UserParser>();
            container.Bind<UserRepository>().ToSelf();
            container.Bind<IParser<Tobacco>>().To<TobaccoParser>();
            container.Bind<TobaccoRepository>().ToSelf();
            return container;
        }
    }
}