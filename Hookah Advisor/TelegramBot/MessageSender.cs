using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace Hookah_Advisor.TelegramBot
{
    public static class MessageSender
    {
        public static async void SendWhenNotTextMessage(Message message, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(
                message.Chat,
                BotSettings.InvalidMessage);
        }

        public static async void SendStartMessage(Message message, ITelegramBotClient botClient)
        {
            var chat = message.Chat;
            var userFirstName = message.From.FirstName;

            await botClient.SendTextMessageAsync(
                chat,
                BotSettings.HelloMessage + userFirstName + BotSettings.StartMessage, replyMarkup: GetButtons());
        }

        public static async void SendHelpMessage(Message message, ITelegramBotClient botClient)
        {
            var chat = message.Chat;

            await botClient.SendTextMessageAsync(
                chat,
                BotSettings.HelpMessage);
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
                        new KeyboardButton {Text = BotSettings.ButtonRecommendations}
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

        public static async void PrintTobaccoToKeyboard(Message message, ITelegramBotClient botClient,
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

        public static IEnumerable<IEnumerable<InlineKeyboardButton>> GetInlineKeyboard<T>(
            IEnumerable<string> stringArray,
            IEnumerable<T> idTobaccos, string type)
        {
            var keyboardInline = stringArray
                .Zip(idTobaccos, (str, idTobacco) => new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = str,
                        CallbackData = $"{type}_{idTobacco}"
                    }
                });

            return keyboardInline;
        }

        public static IEnumerable<IEnumerable<InlineKeyboardButton>> GetInlineKeyboard<T>(string str, T idTobacco,
            string type)
        {
            return new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = str,
                        CallbackData = $"{type}_{idTobacco}"
                    }
                }
            };
        }
    }
}