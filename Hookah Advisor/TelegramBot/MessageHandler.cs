using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Hookah_Advisor.Repository_Interfaces;


namespace Hookah_Advisor.TelegramBot
{
    public static class MessageHandler
    {
        public static void MessageReceived(Message message, IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository, ITelegramBotClient botClient,
            IOptionRepository<Option> optionRepository
        )
        {
            var userFirstName = message.From.FirstName;
            var userId = message.From.Id;

            if (IsInvalidMessage(userId, userRepository, message))
            {
                MessageSender.SendText(BotSettings.InvalidUserMessage, botClient, message);
                return;
            }
            

            switch (message.Text)
            {
                case BotSettings.StartCommand:
                {
                    Commands.Start(botClient, message, userRepository, userFirstName);
                    break;
                }

                case BotSettings.HelpCommand:
                    Commands.Help(botClient, message, userRepository);
                    break;

                case BotSettings.RandomCommand:
                    Commands.Random(botClient, message, tobaccoRepository);
                    break;

                case BotSettings.ClearHistoryCommand:
                    Commands.ClearHistory(botClient, message, userRepository);
                    break;

                case BotSettings.ButtonSearch:
                    Commands.Search(botClient, message, userRepository);
                    break;

                case BotSettings.ButtonRecommendation:


                    Commands.Recommendation(botClient, message, userRepository, optionRepository, tobaccoRepository);

                    break;

                case BotSettings.ButtonSmokeLater:
                    Commands.SmokeLater(botClient, message, userRepository, tobaccoRepository);
                    break;

                case BotSettings.ButtonHistory:
                    Commands.History(botClient, message, userRepository, tobaccoRepository);
                    break;

                default:
                    Commands.TextReceived(botClient, message, userRepository, tobaccoRepository, optionRepository,
                        "default");
                    break;
            }
        }

        private static bool IsInvalidMessage(int userId, IUserRepository userRepository, Message message)
        {
            return !userRepository.IsUserRegistered(userId) & message.Text != BotSettings.StartCommand;
        }
    }
}