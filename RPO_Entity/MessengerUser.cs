namespace RPO_Entity
{
    // Что может обозначать пользователя в мессенджере?
    // - id
    // --- сменный id
    // - имя
    // --- возраст
    // -- количество лайков
    // -- количество дизлайков
    // - отправленные сообщения
    // - полученные сообщения
    // - сессии
    // -- статус
    public sealed class MessengerUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public ICollection<Message> SentMessages { get; set; } = new List<Message>(); // Коллекция отправленных пользователем сообщений
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>(); // Коллекция полученных пользователем сообщений
        public ICollection<Session> UserSessions { get; set; } = new List<Session>(); // Количество сессий пользователя
    }
}
