﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBot.Data;
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

        [Obsolete]
        public MessageCollector(long chatId, int messageId)
        {
            bot = new BotService(chatId);
            this.messageId = messageId;
        }

        public async Task SendStartMenu()
        {
            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetInlineButtons(new List<List<string>>()
            {
                new List<string>{ "Предметы" },
                new List<string>{ "Новости" },
                new List<string>{ "О нас" },
            });

            await bot.SendMessage("Доброе пожаловать в чат Важной информации.\nЧто бы вы хотели узнать?", buttonsGenerator.GetButtons());
        }

        public async Task EditToStartMenu()
        {
            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetInlineButtons(new List<List<string>>()
            {
                new List<string>{ "Предметы" },
                new List<string>{ "Новости" },
                new List<string>{ "О нас" },
            });

            await bot.EditMessage("Доброе пожаловать в чат Важной информации.\nЧто бы вы хотели узнать?", messageId, buttonsGenerator.GetButtons());
        }

        public async Task EditToAboutUsMenu()
        {
            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetInlineUrlButtons(new List<(string, string)> { ("Наш сайт", AppSettings.FrontRoot) });
            buttonsGenerator.SetInlineButtons(new List<(string, string)> { ("↪ Назад", "/start") });

            await bot.EditMessage(MessagesTexts.AboutUs, messageId, buttonsGenerator.GetButtons());
        }

        public async Task SendWeekNews(int newsShift = 0)
        {
            DateTime currentWeekStartDate = DateTime.Now.AddDays(-(DateTime.Now.DayOfWeek - DayOfWeek.Monday));
            NewsService newsService = new();
            var allNewsInSelectedWeek = await newsService.Get(new StartEndTime { Start = currentWeekStartDate.AddDays(newsShift), End = currentWeekStartDate.AddDays(7 + newsShift) });

            foreach (var news in allNewsInSelectedWeek)
            {
                await bot.SendMessage($"date time: {news.DateTimeOfCreate}\n" +
                    $"text: {news.Message}\n" +
                    $"pictures: {news.Pictures}");
            }

            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetInlineButtons(new List<(string, string)> { ("⬅ Предыдущая", $"newsShift:{newsShift - 1}"), ("Следующая ➡", $"newsShift:{newsShift + 1}") });
            buttonsGenerator.SetInlineButtons(new List<(string, string)> { ("↪ Назад", "/start") });
            await bot.SendMessage($"Новости, созданные в промежуток С {currentWeekStartDate.AddDays(newsShift):dd-MM-yyyy} ДО {currentWeekStartDate.AddDays(7 + newsShift):dd-MM-yyyy}\n" +
                $"Для перехода к другой неделе нажмите на кнопку", buttonsGenerator.GetButtons());
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
                    if (lessons.Count - i == 2) buttonsGenerator.SetInlineButtons(new List<(string, string)> { (lessons[i].Name, lessons[i].GetLessonCallback()),
                        (lessons[i + 1].Name, lessons[i + 1].GetLessonCallback()) });
                    if (lessons.Count - i == 1) buttonsGenerator.SetInlineButtons(new List<(string, string)> { (lessons[i].Name, lessons[i].GetLessonCallback()) });
                }
                else
                {
                    buttonsGenerator.SetInlineButtons(new List<(string, string)> { (lessons[i].Name, lessons[i].GetLessonCallback()),
                        (lessons[i + 1].Name, lessons[i + 1].GetLessonCallback()), (lessons[i + 2].Name, lessons[i + 2].GetLessonCallback()) });
                }
            }

            buttonsGenerator.SetInlineButtons(new List<(string, string)> { ("↪ Назад", "/start") });

            await bot.EditMessage("Для просмотра детальной информации по предмету, нажмите на кнопку", messageId, buttonsGenerator.GetButtons());
        }

        public async Task EditToLesson(int lessonId)
        {
            ButtonsGenerator buttonsGenerator = new();
            buttonsGenerator.SetInlineButtons(new List<(string, string)> { ("↪ Назад", "Предметы") });

            LessonsService lessonsService = new();
            var lesson = await lessonsService.Get(lessonId);

            await bot.EditMessage($"id: {lesson.Id}\n" +
                $"name: {lesson.Name}\n" +
                $"teacher: {lesson.Teacher}\n" +
                $"information: {lesson.Information}", messageId, buttonsGenerator.GetButtons());
        }

        public async Task EditToText(string text)
        {
            await bot.EditMessage(text, messageId);
        }

        public async Task UnknownMessage()
        {
            await bot.SendMessage("Пока я не понимаю данное сообщение, но скоро научусь");
        }
    }
}