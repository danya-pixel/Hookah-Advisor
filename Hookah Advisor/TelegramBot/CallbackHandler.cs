using System;
using System.Collections.Generic;
using System.Linq;
using Hookah_Advisor.Repository_Interfaces;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;


namespace Hookah_Advisor.TelegramBot
{
    public static class CallbackHandler
    {
        public static async void BotOnCallbackQueryReceived(IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository,
            CallbackQueryEventArgs callbackQueryEventArgs, ITelegramBotClient botClient)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;
            var callbackData = callbackQuery.Data;
            var type = callbackData.Split('_')[0];
            var idTobacco = Convert.ToInt32(callbackData.Split('_')[1]);
            var tobaccoFromTap = tobaccoRepository.GetItemById(idTobacco);
            var user = userRepository.GetUserById(callbackQuery.From.Id);

            switch (type)
            {
                case "tobaccoFromRequest":
                    var tags = new HashSet<string>();
                    foreach (var t in tobaccoFromTap.Categories.Select(t => $"#{t}"))
                    {
                        tags.Add(t);
                    }

                    foreach (var t in tags.Concat(tobaccoFromTap.Tastes.Select(t => $"#{t}")))
                    {
                        tags.Add(t);
                    }

                    var result =
                        $"{tobaccoFromTap}\n{string.Join(" ", tags)}\n\n{tobaccoFromTap.Description}";
                    if (user.SmokeLater.Contains(idTobacco))
                    {
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, result,
                            replyMarkup: new InlineKeyboardMarkup(
                                TelegramMessageSender.GetInlineKeyboard("Я покурил", idTobacco, "unShmokeLater")));
                        await botClient.AnswerCallbackQueryAsync(
                            callbackQuery.Id,
                            $"{tobaccoFromTap}"
                        );
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, result,
                            replyMarkup: new InlineKeyboardMarkup(
                                TelegramMessageSender.GetInlineKeyboard("Покурить позже", idTobacco, "shmokeLater")));
                        await botClient.AnswerCallbackQueryAsync(
                            callbackQuery.Id,
                            $"{tobaccoFromTap}"
                        );
                    }

                    break;
                case "shmokeLater":
                    user.SmokeLater.Add(idTobacco);
                    await botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id,
                        $"Покумарим {tobaccoFromTap}"
                    );
                    await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, callbackQuery.Message.Text,
                        replyMarkup: new InlineKeyboardMarkup(
                            TelegramMessageSender.GetInlineKeyboard("Я покурил", idTobacco, "unShmokeLater")));
                    break;
                case "unShmokeLater":
                    user.SmokedHistory.Add(idTobacco);
                    user.SmokeLater.Remove(idTobacco);
                    await botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id,
                        $"Покалюмбасили {tobaccoFromTap}"
                    );
                    await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, callbackQuery.Message.Text,
                        replyMarkup: new InlineKeyboardMarkup(
                            TelegramMessageSender.GetInlineKeyboard("Покурить позже", idTobacco, "shmokeLater")));
                    break;
            }
        }
    }
}