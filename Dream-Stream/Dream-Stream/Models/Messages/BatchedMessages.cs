﻿using System.Collections.Generic;
using MessagePack;

namespace Dream_Stream.Models.Messages
{
    [MessagePackObject]
    public class BatchedMessages : BaseMessage
    {
        [Key(1)]
        public List<Message> Messages { get; set; }
    }
}
