using System;
using Hookah_Advisor.Parsers;
using Telegram.Bot;
using Hookah_Advisor.Repositories;
using Hookah_Advisor.Repository_Interfaces;
using Ninject;


namespace Hookah_Advisor
{
    class Program
    {
        static void Main()
        {
            var container = ConfigureContainer();
            var tgBot = container.Get<TelegramBot.TelegramBot>();
            tgBot.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            tgBot.Stop();
        }

        private static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();
            container.Bind<IParser<User>>().To<UserParser>();
            container.Bind<IUserRepository>().To<UserRepository>();
            container.Bind<IParser<Tobacco>>().To<TobaccoParser>();
            container.Bind<IItemRepository<Tobacco>>().To<TobaccoRepository>();
            container.Bind<ITelegramBotClient>().ToConstant(new TelegramBotClient(BotSettings.Token));
            container.Bind<TelegramBot.TelegramBot>().ToSelf();
            return container;
        }
    }
}