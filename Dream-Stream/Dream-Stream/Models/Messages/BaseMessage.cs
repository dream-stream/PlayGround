using MessagePack;
using Producer.Serialization;

namespace Dream_Stream.Models.Messages
{
    [Union(0, typeof(MessageHeader))]
    [Union(1, typeof(BatchedMessages))]
    [MessagePackObject]
    public abstract class BaseMessage
    {
        public BaseMessage() { }

        public virtual byte[] Serialize(ISerializer serializer)
        {
            return serializer.Serialize(this);
        }
    }
}
