namespace Nbtc.Serialization.Message
{
    public class MessageResult
    {
        private MessageResult()
        {
        }

        public MessageStatut Statut { get; private set; }
        public Network.Message Message { get; private set; }
        public string Error { get; private set; }

        public static MessageResult Failed(string error)
        {
            return new MessageResult
            {
                Statut = MessageStatut.Failed,
                Message = null,
                Error = error,
            };
        }

        public static MessageResult Succeed(Network.Message message)
        {
            return new MessageResult
            {
                Statut = MessageStatut.Succeed,
                Message = message
            };
        }

        public static MessageResult Missing()
        {
            return new MessageResult
            {
                Statut = MessageStatut.Missing
            };
        }
    }
}