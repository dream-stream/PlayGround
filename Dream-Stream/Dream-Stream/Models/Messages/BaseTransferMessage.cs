using Dream_Stream.Serialization;
using MessagePack;

namespace Dream_Stream.Models.Messages
{
    [Union(0, typeof(MessageHeader))]
    [Union(1, typeof(BatchedMessages))]
    [MessagePackObject]
    public abstract class BaseTransferMessage
    {
        public BaseTransferMessage() { }

        public virtual byte[] Serialize(ISerializer serializer)
        {
            return serializer.Serialize(this);
        }
    }
}
