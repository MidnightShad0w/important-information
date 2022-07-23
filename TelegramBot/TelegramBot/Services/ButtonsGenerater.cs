﻿using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Services
{
    public class ButtonsGenerater
    {
        private List<List<InlineKeyboardButton>> returnsButtons = new();

        public IReplyMarkup GetButtons()
        {
            return new InlineKeyboardMarkup(returnsButtons);
        }

        /// <summary>
        /// Возвращает кнопки с колбеком эквивалентным названию со знаком "@"
        /// </summary>
        /// <param name="markup">Разметка кнопок. первый лист - строчки, второй - столбцы</param>
        /// <returns></returns>
        public void SetInlineButtons(List<List<string>> markup)
        {
            AddButtons(markup, (lineMarkup) => lineMarkup.Select(text => InlineKeyboardButton.WithCallbackData(text, "@" + text)).ToList());
        }

        /// <summary>
        /// Возвращает кнопки с колбеком (@ ставится автоматически)
        /// </summary>
        /// <param name="markup">Разметка кнопок. Первый лист - строчки, второй - столбцы</param>
        /// <returns></returns>
        public void SetInlineButtons(List<List<(string name, string callback)>> markup)
        {
            AddButtons(markup, (lineMarkup) => lineMarkup.Select(b => InlineKeyboardButton.WithUrl(b.name, "@" + b.callback)).ToList());
        }

        public void SetInlineUrlButtons(List<List<(string name, string url)>> markup)
        {
            AddButtons(markup, (lineMarkup) => lineMarkup.Select(b => InlineKeyboardButton.WithUrl(b.name, b.url)).ToList());
        }

        private void AddButtons<T>(List<List<T>> markup, Func<List<T>, List<InlineKeyboardButton>> createLine)
        {
            foreach (var lineMarkup in markup)
            {
                returnsButtons.Add(createLine(lineMarkup));
            }
        }
    }
}
