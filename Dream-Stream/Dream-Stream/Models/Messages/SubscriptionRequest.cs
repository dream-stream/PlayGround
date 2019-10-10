using MessagePack;

namespace Dream_Stream.Models.Messages
{
    [MessagePackObject]
    public class SubscriptionRequest : IMessage
    {
        [Key(1)]
        public string Topic { get; set; }
    }
}
