namespace RPO_Entity
{
    // Сущность сообщения включает:
    // - Время отправки
    // - Строка с содержимым
    // --- Время изменения
    // --- Статус прочитанного 
    // - id
    // - id пользователя
    // --- шифрование
    // - ключ переписки (для того, чтобы сообщение приходило в нужный чат)
    // - отправитель
    // - получатель
    public sealed class Message
    {
        public int Id { get; set; } // id
        public int SenderId { get; set; } // id отправителя
        public MessengerUser SenderUser { get; set; } = null; // сущность отправителя
        public int ReceiverId { get; set; } // id получателя
        public MessengerUser ReceiverUser { get; set; } = null; // сущность отправителя
        public string ConversationKey { get; set; } = String.Empty; // ключ переписки
        public string TextContent { get; set; } = String.Empty; // текст содержимого сообщения
        public DateTime TimeSent { get; set; } // время отправки (с помощью структуры, обозначающей время и дату)
    }
}
