﻿using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Models;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GemTracker.Shared.Domain
{
    public class T
    {
        private readonly ITelegramService _telegramService;
        private readonly ITwitterService _twitterService;
        public T(
            ITelegramService telegramService,
            ITwitterService twitterService)
        {
            _telegramService = telegramService;
            _twitterService = twitterService;
        }
        public async Task<Notified> Notify(IEnumerable<Gem> gems)
        {
            var result = new Notified();
            try
            {
                if (gems.AnyAndNotNull())
                {
                    foreach (var gem in gems)
                    {
                        var msgTg = M.MessageForTelegram(gem);
                        var sentTg = await _telegramService.SendMessageAsync(msgTg.Item2, msgTg.Item1);

                        if (!sentTg.Success)
                        {
                            result.Message += $"Telegram Error: {sentTg.Message}";
                        }

                        var msgTw = M.MessageForTwitter(gem);
                        var sentTw = await _twitterService.SendMessageAsync(msgTw);

                        if (!sentTw.Success)
                        {
                            result.Message += $"Twitter Error: {sentTw.Message}";
                        }

                        Thread.Sleep(1000); // to not fall in api limits
                    }
                }
                else
                    result.Message = "Nothing to send";
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }
    }
}