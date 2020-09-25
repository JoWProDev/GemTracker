﻿using System.Collections.Generic;

namespace GemTracker.Shared.Domain.Configs
{
    public class AgentApp
    {
        public string StoragePath { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
        public TelegramConfig TelegramConfig { get; set; }
        public TwitterConfig TwitterConfig { get; set; }
    }
}