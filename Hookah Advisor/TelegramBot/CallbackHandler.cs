using System;
using Hookah_Advisor.Repository_Interfaces;
using Telegram.Bot;
using Telegram.Bot.Args;


namespace Hookah_Advisor.TelegramBot
{
    public static class CallbackHandler
    {
        public static void BotOnCallbackQueryReceived(IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository,
            CallbackQueryEventArgs callbackQueryEventArgs, ITelegramBotClient botClient)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;
            var callbackType = callbackQuery.Data.Split('_')[0];
            var idTobacco = Convert.ToInt32(callbackQuery.Data.Split('_')[1]);
            var tobaccoSelected = tobaccoRepository.GetItemById(idTobacco);
            var user = userRepository.GetUserById(callbackQuery.From.Id);

            switch (callbackType)
            {
                case BotSettings.TypeSearchTobacco:
                    var tags = tobaccoSelected.GetTagsFromTobacco();
                    var messageWithTobacco =
                        $"{tobaccoSelected}\n{string.Join(" ", tags)}\n\n{tobaccoSelected.Description}";

                    MessageSender.SendTobacco(messageWithTobacco, tobaccoSelected,
                        user.SmokeLater.Contains(idTobacco) ? BotSettings.TypeUnSmoke : BotSettings.TypeSmokeLater,
                        callbackQuery, botClient);

                    break;
                case BotSettings.TypeSmokeLater:
                    user.SmokeLater.Add(idTobacco);

                    MessageSender.SendAnswerCallback(tobaccoSelected, BotSettings.AnswerSmokeLater, botClient,
                        callbackQuery);

                    MessageSender.EditButtonText(BotSettings.KeyboardUnSmokeLater, BotSettings.TypeUnSmoke, botClient,
                        callbackQuery,
                        idTobacco);
                    break;

                case BotSettings.TypeUnSmoke:
                    user.SmokedHistory.Add(idTobacco);
                    user.SmokeLater.Remove(idTobacco);

                    MessageSender.SendAnswerCallback(tobaccoSelected, BotSettings.AnswerUnSmokeLater, botClient,
                        callbackQuery);

                    MessageSender.EditButtonText(BotSettings.KeyboardSmokeLater,
                        BotSettings.TypeSmokeLater, botClient, callbackQuery, idTobacco);
                    break;
            }
        }
    }
}