using System;
using System.Collections.Generic;
using System.Linq;
using Hookah_Advisor.Repository_Interfaces;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace Hookah_Advisor.TelegramBot
{
    public static class CallbackHandler
    {
        public static void BotOnCallbackQueryReceived(IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository,
            CallbackQueryEventArgs callbackQueryEventArgs, ITelegramBotClient botClient)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;
            var callbackData = callbackQuery.Data;
            var callbackType = callbackData.Split('_')[0];
            var idTobacco = Convert.ToInt32(callbackData.Split('_')[1]);
            var tobaccoSelected = tobaccoRepository.GetItemById(idTobacco);
            var user = userRepository.GetUserById(callbackQuery.From.Id);

            switch (callbackType)
            {
                case BotSettings.TypeSearchTobacco:
                    var tags = GetTagsFromTobacco(tobaccoSelected);
                    var messageWithTobacco =
                        $"{tobaccoSelected}\n{string.Join(" ", tags)}\n\n{tobaccoSelected.Description}";

                    SendTobacco(messageWithTobacco, tobaccoSelected,
                        user.SmokeLater.Contains(idTobacco) ? BotSettings.TypeUnSmoke : BotSettings.TypeSmokeLater,
                        callbackQuery, botClient);

                    break;
                case BotSettings.TypeSmokeLater:
                    user.SmokeLater.Add(idTobacco);

                    SendAnswerCallback(tobaccoSelected, BotSettings.AnswerSmokeLater, botClient, callbackQuery);

                    EditButtonText(BotSettings.KeyboardUnSmokeLater, BotSettings.TypeUnSmoke, botClient, callbackQuery,
                        idTobacco);
                    break;

                case BotSettings.TypeUnSmoke:
                    user.SmokedHistory.Add(idTobacco);
                    user.SmokeLater.Remove(idTobacco);

                    SendAnswerCallback(tobaccoSelected, BotSettings.AnswerUnSmokeLater, botClient,
                        callbackQuery);

                    EditButtonText(BotSettings.KeyboardSmokeLater,
                        BotSettings.TypeSmokeLater, botClient, callbackQuery, idTobacco);
                    break;
            }
        }

        private static IEnumerable<string> GetTagsFromTobacco(Tobacco tobaccoFromTap)
        {
            var tags = new HashSet<string>();
            foreach (var t in tobaccoFromTap.Categories.Select(t => $"#{t}"))
            {
                tags.Add(t);
            }

            foreach (var t in tags.Concat(tobaccoFromTap.Tastes.Select(t => $"#{t}")))
            {
                tags.Add(t);
            }

            return tags;
        }

        private static async void EditButtonText(string keyboard, string type, ITelegramBotClient botClient,
            CallbackQuery callbackQuery,
            int idTobacco
        )
        {
            await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId, callbackQuery.Message.Text,
                replyMarkup: new InlineKeyboardMarkup(
                    MessageSender.GetInlineKeyboard(keyboard, idTobacco,
                        type)));
        }

        private static async void SendAnswerCallback(Tobacco tobacco, string answerType,
            ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                answerType + $" {tobacco}"
            );
        }

        private static async void SendTobacco(string message, Tobacco tobacco, string buttonType,
            CallbackQuery callbackQuery, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, message,
                replyMarkup: new InlineKeyboardMarkup(
                    MessageSender.GetInlineKeyboard(BotSettings.KeyboardSmokeLater, tobacco.Id,
                        buttonType)));
            await botClient.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"{tobacco}");
        }
    }
}