namespace RPO_Entity
{
    // Сущность сессии пользователя включает:
    // - id
    // - id отправителя
    // - сущность получателя
    // - время живности сессии (heartbeat)
    // - ключ сессии
    // - активна ли сессия
    public sealed class Session
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public MessengerUser MessengerUser { get; set; } = null;
        public string SessionKey { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime LastHeartbeatTime { get; set; }
    }
}
