using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace Hookah_Advisor.TelegramBot
{
    public static class MessageSender
    {
        public static async void SendAnswerCallback(Tobacco tobacco, string answerType,
            ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                answerType + $" {tobacco}"
            );
        }

        public static async void SendTobacco(string message, Tobacco tobacco, string buttonType,
            CallbackQuery callbackQuery, ITelegramBotClient botClient)
        {
            switch (buttonType)
            {
                case BotSettings.TypeSmokeLater:
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, message,
                        replyMarkup: new InlineKeyboardMarkup(
                            GetInlineKeyboard(BotSettings.KeyboardSmokeLater, tobacco.Id,
                                buttonType)));
                    await botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id,
                        $"{tobacco}");
                    break;
                case BotSettings.TypeUnSmoke:
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, message,
                        replyMarkup: new InlineKeyboardMarkup(
                            GetInlineKeyboard(BotSettings.KeyboardUnSmokeLater, tobacco.Id,
                                buttonType)));
                    await botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id,
                        $"{tobacco}");
                    break;
            }
        }

        public static async void SendText(string text, ITelegramBotClient botClient, Message message)
        {
            if (text == BotSettings.StartMessage)
            {
                var chat = message.Chat;
                var userFirstName = message.From.FirstName;
                text = BotSettings.HelloMessage + userFirstName + BotSettings.StartMessage;
            }

            await botClient.SendTextMessageAsync(
                message.Chat,
                text,
                replyMarkup: GetButtons());
        }

        public static async void SendTextWithInlineKeyboard(string text, string requestType,
            ITelegramBotClient botClient,
            Message message, IEnumerable<Tobacco> tobaccos, User user)
        {
            switch (text)
            {
                case BotSettings.SmokedHistoryMessage:
                    await botClient.SendTextMessageAsync(
                        message.Chat,
                        text,
                        replyMarkup: new InlineKeyboardMarkup(GetInlineKeyboard(
                            tobaccos.Select(t => t.ToString()),
                            user.SmokedHistory, requestType)));
                    break;
                case BotSettings.SmokeLaterMessage:
                    await botClient.SendTextMessageAsync(
                        message.Chat,
                        text,
                        replyMarkup: new InlineKeyboardMarkup(GetInlineKeyboard(
                            tobaccos.Select(t => t.ToString()),
                            user.SmokeLater, requestType)));
                    break;
            }
        }


        public static async void PrintTobaccosToKeyboard(Message message, ITelegramBotClient botClient,
            List<Tobacco> tobaccos)
        {
            var array = tobaccos.Select(t => t.ToString());
            var idTobaccos = tobaccos.Select(t => t.Id);

            var keyboardMarkup =
                new InlineKeyboardMarkup(GetInlineKeyboard(array, idTobaccos, BotSettings.TypeSearchTobacco));
            await botClient.SendTextMessageAsync(
                message.From.Id,
                BotSettings.SearchListMessage,
                replyMarkup: keyboardMarkup
            );
        }

        public static async void PrintOptionToKeyboard(Message message, ITelegramBotClient botClient,
            Option option, string func)
        {
            switch (func)
            {
                case "keyboard":
                {
                    var array = option.AnswersTastes.Select(t => t.ToString());

                    var optNum = option.QuestionNumber;
                    var keyboardMarkup =
                        new InlineKeyboardMarkup(GetInlineKeyboard(array, optNum, BotSettings.TypeOption));

                    await botClient.SendTextMessageAsync(
                        message.From.Id,
                        "Смотри что есть",
                        replyMarkup: keyboardMarkup
                    );
                    break;
                }

                case "question":
                {
                    var array = option.FirstAnswers.Select(t => t.ToString());
                    var question = option.Question;
                    var keyboardMarkup =
                        new InlineKeyboardMarkup(GetInlineKeyboard(array, option.QuestionNumber,
                            BotSettings.TypeOptionYesNo));
                    await botClient.SendTextMessageAsync(
                        message.From.Id,
                        question,
                        replyMarkup: keyboardMarkup
                    );
                    break;
                }
            }
        }

        private static IEnumerable<IEnumerable<InlineKeyboardButton>> GetInlineKeyboard<T>(
            IEnumerable<string> stringArray,
            IEnumerable<T> idTobaccos, string type)
        {
            var keyboardInline = stringArray
                .Zip(idTobaccos, (str, id) => new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = str,
                        CallbackData = $"{type}_{id}"
                    }
                });

            return keyboardInline;
        }

        private static IEnumerable<IEnumerable<InlineKeyboardButton>> GetInlineKeyboard(
            IEnumerable<string> stringArray,
            int idOption, string type)
        {
            var keyboardInline = stringArray
                .Select((str) => new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = str,
                        CallbackData = $"{type}_{idOption}_{str}"
                    }
                });

            return keyboardInline;
        }


        private static IEnumerable<IEnumerable<InlineKeyboardButton>> GetInlineKeyboard<T>(string str, T id,
            string type)
        {
            return new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = str,
                        CallbackData = $"{type}_{id}"
                    }
                }
            };
        }

        public static async void EditButtonText(string keyboard, string type, ITelegramBotClient botClient,
            CallbackQuery callbackQuery,
            int idTobacco
        )
        {
            await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId, callbackQuery.Message.Text,
                replyMarkup: new InlineKeyboardMarkup(
                    GetInlineKeyboard(keyboard, idTobacco,
                        type)));
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new()
                    {
                        new KeyboardButton {Text = BotSettings.ButtonSearch},
                        new KeyboardButton {Text = BotSettings.ButtonRecommendation}
                    },
                    new()
                    {
                        new KeyboardButton {Text = BotSettings.ButtonSmokeLater},
                        new KeyboardButton {Text = BotSettings.ButtonHistory}
                    }
                },
                ResizeKeyboard = true
            };
        }
    }
}