using Dream_Stream.Models.Messages;

namespace Dream_Stream.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T obj) where T : BaseTransferMessage;
        T Deserialize<T>(byte[] message);
    }
}
