namespace RPO_Entity
{
    // Класс, представляющий объект передачи данных сообщения. Нужен для того, чтобы не отдавать напрямую сущности Entity. Нужен для того, чтобы создавать классы, содержащие только те данные, которые нужны для отображения или передачи.
    public sealed class MessageDto 
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; } = String.Empty;
        public string ReceiverName { get; set; } = String.Empty;
        public string TextContent { get; set; } = String.Empty;
        public DateTime TimeSent { get; set; }
    }
}
