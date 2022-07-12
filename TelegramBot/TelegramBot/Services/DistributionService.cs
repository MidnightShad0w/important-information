﻿using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Args;
using TelegramBot.Handlers;
using TelegramBot.Interfaces;

namespace TelegramBot.Services
{
    public static class DistributionService
    {
        public static Dictionary<long, IHandler> BusyUsersIdAndService { get; set; } = new();

        [Obsolete]
        public static async void DistributeMessages(object sender, MessageEventArgs e)
        {
            if (!BusyUsersIdAndService.Keys.Contains(e.Message.Chat.Id)) BaseHandler.OnMessage(sender, e);

            if (BusyUsersIdAndService.Keys.Contains(e.Message.Chat.Id))
            {
                var selectedServiceByChatId = BusyUsersIdAndService[e.Message.Chat.Id];
                await selectedServiceByChatId.ProcessMessage(e.Message.Text);
            }
        }

        [Obsolete]
        public static void DistributeCallbacks(object sender, CallbackQueryEventArgs e)
        {
            BusyUsersIdAndService.Remove(e.CallbackQuery.Message.Chat.Id);

            if (!BusyUsersIdAndService.Keys.Contains(e.CallbackQuery.Message.Chat.Id)) BaseHandler.OnCallback(sender, e);
        }
    }
}
