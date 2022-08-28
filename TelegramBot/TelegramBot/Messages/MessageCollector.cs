﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;
using TelegramBot.Data.Definitions;
using TelegramBot.Data.Models;
using TelegramBot.Data.ViewModels;
using TelegramBot.Interfaces;
using TelegramBot.Services;
using TelegramBot.Services.ApiServices;
using TelegramBot.Utils;

namespace TelegramBot.Messages
{
    public class MessageCollector
    {
        private readonly IBotService bot;
        private readonly int messageId;
        private readonly long chatId;

        public MessageCollector(long chatId, int messageId)
        {
            bot = new BotService(chatId);
            this.messageId = messageId;
            this.chatId = chatId;
        }

        #region Menus
        public async Task SendStartMenu()
        {
            await bot.SendMessage(Texts.StartMenu, await GenerateButtonsForStartMenu());
        }

        public async Task EditToStartMenu()
        {
            await bot.EditMessage(Texts.StartMenu, messageId, await GenerateButtonsForStartMenu());
        }

        public async Task EditToAdminPanel()
        {
            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetInlineButtons("Создать рассылку");

            buttonsGenerator.SetGoBackButton();

            await bot.EditMessage(Texts.AdminPanel, messageId, buttonsGenerator.GetButtons());
        }

        public async Task EditToAboutUsMenu()
        {
            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetInlineUrlButtons(("Наш сайт", AppSettings.FrontRoot));
            buttonsGenerator.SetGoBackButton();

            await bot.EditMessage(Texts.AboutUs, messageId, buttonsGenerator.GetButtons());
        }

        public async Task EditToLessonsMenu()
        {
            LessonsService lessonsService = new();
            var lessons = await lessonsService.Get();

            ButtonsGenerator buttonsGenerator = new();

            for (int i = 0; i < lessons.Count; i += 3)
            {
                if (lessons.Count < i + 3)
                {
                    if (lessons.Count - i == 2) buttonsGenerator.SetInlineButtons((lessons[i].Name, lessons[i].GetLessonCallback()),
                        (lessons[i + 1].Name, lessons[i + 1].GetLessonCallback()));
                    if (lessons.Count - i == 1) buttonsGenerator.SetInlineButtons((lessons[i].Name, lessons[i].GetLessonCallback()));
                }
                else
                {
                    buttonsGenerator.SetInlineButtons((lessons[i].Name, lessons[i].GetLessonCallback()),
                        (lessons[i + 1].Name, lessons[i + 1].GetLessonCallback()), (lessons[i + 2].Name, lessons[i + 2].GetLessonCallback()));
                }
            }

            buttonsGenerator.SetGoBackButton();

            await bot.EditMessage(Texts.DetailLessonInfo, messageId, buttonsGenerator.GetButtons());
        }

        public async Task EditToWeekNews(int newsShift = 0)
        {
            DateTime weekStartDate = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Monday)).AddDays(7 * newsShift);
            DateTime weekEndDate = weekStartDate.AddDays(6);

            var allNewsInSelectedWeek = await GetWeekNews(weekStartDate);
            ButtonsGenerator buttonsGenerator = new();


            if (weekEndDate < DateTime.Now && await CheckAnyNewsBefore(weekEndDate))
            {
                buttonsGenerator.SetInlineButtons(("⬅ Предыдущая", $"newsShift:{newsShift - 1}"), ("Следующая ➡", $"newsShift:{newsShift + 1}"));
            }
            else
            {
                if (weekEndDate < DateTime.Now) buttonsGenerator.SetInlineButtons(("Следующая ➡", $"newsShift:{newsShift + 1}"));
                else if (await CheckAnyNewsBefore(weekEndDate)) buttonsGenerator.SetInlineButtons(("⬅ Предыдущая", $"newsShift:{newsShift - 1}"));
            }
            buttonsGenerator.SetGoBackButton();

            await SendNews(allNewsInSelectedWeek, buttonsGenerator.GetButtons(), $"Новости, созданные в промежуток с {weekStartDate:dd-MM-yyyy} по {weekEndDate:dd-MM-yyyy}\n" +
                    Texts.NextWeek);
        }
        #endregion


        public async Task EditToLesson(int lessonId)
        {
            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetInlineButtons(("Новости по предмету", $"getNewsForLes{lessonId}I{messageId}"));
            buttonsGenerator.SetGoBackButton("Предметы");

            LessonsService lessonsService = new();
            var lesson = await lessonsService.Get(lessonId);

            await bot.EditMessage(lesson.GetLessonCard(), messageId, buttonsGenerator.GetButtons());
        }

        public async Task SendNewsForLesson(int lessonId, int previewMessageId)
        {
            await bot.DeleteMessage(messageId);
            await bot.DeleteMessage(previewMessageId);

            NewsService newsService = new();
            var news = await newsService.GetByLessonId(lessonId);

            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetGoBackButton($"lessonId:{lessonId}");

            await BotService.SendNews(news, new List<long> { chatId }, buttonsGenerator.GetButtons());
        }

        public async Task SendDetailedNews(int newsId, int previewMessageId)
        {
            await bot.DeleteMessage(messageId);
            await bot.DeleteMessage(previewMessageId);

            NewsService newsService = new();
            var news = await newsService.Get(newsId);
            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetGoBackButton("Новости");

            await BotService.SendNews(news, new List<long> { chatId }, buttonsGenerator.GetButtons());
        }

        public async Task UnknownMessage()
        {
            await bot.SendMessage(Texts.UnknownMessage);
        }

        #region Utils
        private async Task SendNews(IOrderedEnumerable<News> news, IReplyMarkup buttons = null, string caption = null)
        {
            string outputString = "";

            foreach (var oneNews in news)
            {
                outputString += oneNews.GetNewsCard();

                outputString += string.IsNullOrWhiteSpace(oneNews.Pictures) ? "\n\n" : $"Новость с картинками. Для просмотра картинок нажмите /news{oneNews.Id}I{messageId}\n\n";
            }

            await bot.EditMessage(outputString + caption, messageId, buttons);
        }
        private async Task<IOrderedEnumerable<News>> GetWeekNews(DateTime weekStartDate)
        {
            NewsService newsService = new();
            return (await newsService.Get(new StartEndTime
            {
                Start = weekStartDate,
                End = weekStartDate.AddDays(7)
            })).OrderBy(n => n.DateTimeOfCreate);
        }
        private async Task<bool> CheckAnyNewsBefore(DateTime date)
        {
            date = date.AddDays(-7);
            NewsService newsService = new();
            return await newsService.CheckNewsBefore(date);
        }
        private async Task<IReplyMarkup> GenerateButtonsForStartMenu()
        {
            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetInlineButtons(
                new List<string> { "Предметы" },
                new List<string> { "Новости" },
                new List<string> { "О нас" });

            var usersService = new UsersService();
            var currentUser = await usersService.GetByChatId(chatId);
            if (currentUser?.Role == Role.ADMIN) buttonsGenerator.SetInlineButtons("Панель администратора");
            return buttonsGenerator.GetButtons();
        }

        #endregion
    }
}