using System;
using System.Runtime.InteropServices;
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
            var telegramBot = container.Get<TelegramBot.TelegramBot>();
            telegramBot.Start();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            telegramBot.Stop();
        }

        private static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();
            container.Bind<IParser<User>>().To<UserParser>();
            container.Bind<IUserRepository>().To<UserRepository>();
            container.Bind<IParser<Option>>().To<RecommendParser>();
            container.Bind<IOptionRepository<Option>>().To<OptionRepositories>();
            container.Bind<IParser<Tobacco>>().To<TobaccoParser>();
            container.Bind<IItemRepository<Tobacco>>().To<TobaccoRepository>();
            container.Bind<ITelegramBotClient>().ToConstant(new TelegramBotClient(BotSettings.Token));
            container.Bind<TelegramBot.TelegramBot>().ToSelf();
            return container;
        }
    }
}