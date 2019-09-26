using MessagePack;

namespace Dream_Stream.Models.Messages
{
    [Union(0, typeof(MessageHeader))]
    [Union(1, typeof(Message))]
    [MessagePackObject]
    public abstract class BaseMessage
    {
        public BaseMessage() { }
    }
}
