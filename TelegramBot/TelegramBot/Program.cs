﻿using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using TelegramBot.Messages;
using TelegramBot.Services;

namespace TelegramBot
{
    internal class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            try
            {
                var client = new TelegramBotClient(AppSettings.Token);
                SingletonService.TelegramClient = client;

                client.StartReceiving();
                client.OnMessage += OnMessageHandler;
                client.OnMessage += LogService.MessageLoging;
                client.OnCallbackQuery += OnCallbackQweryHandlerAsync;
                client.OnCallbackQuery += LogService.CallbackLoging;
                Console.ReadLine();
                client.StopReceiving();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        [Obsolete]
        private static async void OnCallbackQweryHandlerAsync(object sender, CallbackQueryEventArgs e)
        {
            MessageCollector message = new(e.CallbackQuery.Message.Chat.Id);

            Func<Task> response = e.CallbackQuery.Data switch
            {
                _ => message.UnknownMessage()
            };

            await response();
        }

        [Obsolete]
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            MessageCollector message = new(e.Message.Chat.Id);

            Func<Task> response = e.Message.Text switch
            {
                "/start" => message.StartMenu(),
                "Привет" => message.SendText("Привет"),
                _ => message.UnknownMessage()
            };

            await response();
        }
    }
}
