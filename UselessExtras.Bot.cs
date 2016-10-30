using System;

using System.Diagnostics;

using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

namespace UselessExtras
{
    public class UselessExtrasBot
    {
        TelegramBotClient BotClient;
        User BotUser;
        public string ReadTokenFromUserProfile(string configFileName = ".telegram/uselessextrasbot")
        {
            var tokenFilePath = $"{Environment.ExpandEnvironmentVariables($"%HOME%/{configFileName}")}";
            return System.IO.File.ReadAllText(tokenFilePath).Trim();
        }
        public void Run()
        {
            var token = ReadTokenFromUserProfile();
            BotClient = new TelegramBotClient(token);

            BotClient.OnInlineQuery += BotOnInlineQueryReceived;
            BotClient.OnMessage += BotOnMessageReceived;
            //Bot.OnMessageEdited += BotOnMessageReceived;
            //Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            //Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            BotClient.OnReceiveError += (sndr, err) => { Debug.Fail($"{err}"); };

            BotUser = BotClient.GetMeAsync().Result;

            BotClient.PollingTimeout = new TimeSpan(0, 0, 20);
            BotClient.StartReceiving();
            Console.WriteLine($"Your bot {BotUser.Username}({BotUser.Id}) is running");
            Console.Write($"Press Enter to stop it and exit...");
            Console.ReadLine();
            BotClient.StopReceiving();
        }
        // an easy way of initiating private chat
        void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            InlineQueryHandlerForPrivateChat(sender, inlineQueryEventArgs);
        }
        async void InlineQueryHandlerForPrivateChat(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            await BotClient.AnswerInlineQueryAsync(inlineQueryEventArgs.InlineQuery.Id,
                new InlineQueryResult[] { },
                switchPmText: $"Go chat with {BotUser?.FirstName ?? "the bot"}"
            );
        }
        async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message != null)
                switch (message.Type)
                {
                    case MessageType.TextMessage:
                        await BotClient.SendTextMessageAsync(message.Chat.Id, $"Why did you say '{message.Text}'?");
                        break;
                }
        }
    }
}