using MessagePack;

namespace Dream_Stream.Models.Messages
{
    [MessagePackObject]
    public class Message : BaseMessage
    {
        [Key(1)]
        public string[] Msg { get; set; }
    }
}