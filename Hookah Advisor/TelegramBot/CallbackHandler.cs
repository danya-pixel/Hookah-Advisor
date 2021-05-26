using System;
using System.Collections.Generic;
using System.Linq;
using Hookah_Advisor.Repositories;
using Hookah_Advisor.Repository_Interfaces;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Hookah_Advisor.TelegramBot
{
    public class CallbackHandler
    {
        public static async void BotOnCallbackQueryReceived(IUserRepository userRepository,
            TobaccoRepository tobaccoRepository,
            CallbackQueryEventArgs callbackQueryEventArgs, ITelegramBotClient _botClient)
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
                    foreach (var t in tobaccoFromTap.categories.Select(t => $"#{t}"))
                    {
                        tags.Add(t);
                    }

                    foreach (var t in tags.Concat(tobaccoFromTap.tastes.Select(t => $"#{t}")))
                    {
                        tags.Add(t);
                    }

                    var result =
                        $"{tobaccoFromTap}\n{string.Join(" ", tags)}\n\n{tobaccoFromTap.description}";
                    if (user.SmokeLater.Contains(idTobacco))
                    {
                        await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, result,
                            replyMarkup: new InlineKeyboardMarkup(
                                TelegramMessageSender.GetInlineKeyboard("Я покурил", idTobacco, "unShmokeLater")));
                        await _botClient.AnswerCallbackQueryAsync(
                            callbackQuery.Id,
                            $"{tobaccoFromTap}"
                        );
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, result,
                            replyMarkup: new InlineKeyboardMarkup(
                                TelegramMessageSender.GetInlineKeyboard("Покурить позже", idTobacco, "shmokeLater")));
                        await _botClient.AnswerCallbackQueryAsync(
                            callbackQuery.Id,
                            $"{tobaccoFromTap}"
                        );
                    }

                    break;
                case "shmokeLater":
                    user.SmokeLater.Add(idTobacco);
                    await _botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id,
                        $"Покумарим {tobaccoFromTap}"
                    );
                    await _botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, callbackQuery.Message.Text,
                        replyMarkup: new InlineKeyboardMarkup(
                            TelegramMessageSender.GetInlineKeyboard("Я покурил", idTobacco, "unShmokeLater")));
                    break;
                case "unShmokeLater":
                    user.SmokeLater.Remove(idTobacco);
                    await _botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id,
                        $"Покалюмбасили {tobaccoFromTap}"
                    );
                    await _botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, callbackQuery.Message.Text,
                        replyMarkup: new InlineKeyboardMarkup(
                            TelegramMessageSender.GetInlineKeyboard("Покурить позже", idTobacco, "shmokeLater")));
                    break;
            }
        }
    }
}