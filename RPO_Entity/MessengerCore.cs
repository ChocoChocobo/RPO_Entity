using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace RPO_Entity
{
    public static class MessengerCore // ДЗ: Статичный класс зачем он нужен
    {
        // Интервал по времени, который засекается для определения активности пользователя
        private static TimeSpan Timeout = TimeSpan.FromSeconds(0);

        // Функция, ответственная за инициализацию БД
        public static async Task Initialize()
        {
            using var db = new MessengerContext();
            await db.Database.EnsureCreatedAsync();
        }

        // Получение или создание пользователя
        public static async Task<MessengerUser> GetCreateUserAsync(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Имя пользователя не может быть пустым!");
            name = name.Trim();
            using var db = new MessengerContext();
            // Ищем пользователя в базе данных
            var user = await db.Users.FirstOrDefaultAsync(x => x.Name == name);
            if (user != null) return user; // если пользователь найден, то возвращаем его

            user = new MessengerUser(); // Создаем нового пользователя, если не найден
            user.Name = name;
            db.Users.Add(user);
            await db.SaveChangesAsync(); // Обязательно сохраняем изменения асинхронно, поскольку работаем с асинхронностью!
            return user;
        }

        // Создать сессию
        public static async Task<string> CreateSessionAsync(int userId)
        {
            using var db = new MessengerContext();
            // Генерация идентификатора с установленным форматом в виде N, который означает 32 цифры без разделения
            string sessionKey = Guid.NewGuid().ToString("N");
            // Добавление новой сессии с выставлением времени последнего признака жизни
            db.Sessions.Add(new Session
            {
                UserId = userId,
                SessionKey = sessionKey,
                LastHeartbeatTime = DateTime.Now,
                IsActive = true,
            });

            await db.SaveChangesAsync();
            return sessionKey;
        }

        // Обновить сессию
        public static async Task RefreshSessionAsync(string sessionKey)
        {
            using var db = new MessengerContext();
            // Находим нужную сессию
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.SessionKey == sessionKey);
            if (session == null || !session.IsActive) return;
            // Обновляем время признака жизни сессии, что она жива
            session.LastHeartbeatTime = DateTime.Now;
            await db.SaveChangesAsync();
        }

        // Закрыть сессию
        public static async Task CloseSessionAsync(string sessionKey)
        {
            using var db = new MessengerContext();
            var session = await db.Sessions.FirstOrDefaultAsync(x => x.SessionKey == sessionKey);
            if (session == null) return;
            // В отличие от открытия сессии мы теперь не проверяем на неактивную сессию, а при нахождении выставляем сессию неактивной
            session.IsActive = false;
            session.LastHeartbeatTime = DateTime.Now;
            await db.SaveChangesAsync();
        }
        // Активен ли пользователь !!!!!!!!!!
        public static Task<bool> IsUserActiveAsync(int userId)
        {
            DateTime timeThreshold = DateTime.Now.Subtract(Timeout);

            using var db = new MessengerContext();
            // Проверяем на наличие пользователя, у которого есть активная и живая сессия 
            return db.Users.AsNoTracking().AnyAsync(x => x.Id == userId && x.UserSessions.Any(x => x.IsActive && x.LastHeartbeatTime > timeThreshold));
        }

        // Получение активного пользователя
        public static Task<List<MessengerUser>> GetActiveUsersAsync(int currentUserId)
        {
            DateTime timeThreshold = DateTime.Now.Subtract(Timeout);
            using var db = new MessengerContext();
            return db.Users.AsNoTracking()
                .Where(x =>
                            x.Id != currentUserId && 
                            x.UserSessions.Any(x => x.IsActive && x.LastHeartbeatTime >= timeThreshold))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Отправить сообщение 
        public static async Task SendMessageAsync(int senderId, int receiverId, string textContent)
        {
            textContent = textContent.Trim('\n');
            if (senderId == receiverId)
                throw new ArgumentException("Ты пишешб самому себе!");
            if (string.IsNullOrWhiteSpace(textContent))
                throw new ArgumentException("Сообщение не может быть пустым!");
            bool IsSenderActive = await IsUserActiveAsync(senderId);
            bool IsReceiverActive = await IsUserActiveAsync(receiverId);
            if (!IsSenderActive)
                throw new ArgumentException("Ты не в сети, дубина!");
            if (!IsReceiverActive)
                throw new ArgumentException("Твой болтун не онлайн, лол");
            using var db = new MessengerContext();
            db.Messages.Add(new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                TextContent = textContent,
                ConversationKey = $"{senderId}:{receiverId}",
                TimeSent = DateTime.Now,
            });
            await db.SaveChangesAsync();
        }

        // Получитб диалог
        public static Task<List<MessageDto>> GetConversationAsync(int userAId, int userBId)
        {
            using var db = new MessengerContext();
            string key = $"{userAId}:{userBId}";

            return db.Messages.AsNoTracking()
                                .Where(m => m.ConversationKey == key)
                                .OrderBy(m => m.TimeSent)
                                .ThenBy(m => m.Id)
                                .Select(m => new MessageDto
                                {
                                    Id = m.Id,
                                    SenderId = m.SenderId,
                                    ReceiverId = m.ReceiverId,
                                    SenderName = m.SenderUser.Name,
                                    ReceiverName = m.ReceiverUser.Name,
                                    TextContent = m.TextContent,
                                    TimeSent = m.TimeSent,
                                }).ToListAsync();
        }

        // Получитб последнее входящее сообщение
        public static Task<int> GetLastIncomingMessageAsync(int userId)
        {
            using var db = new MessengerContext();

            return db.Messages.AsNoTracking()
                                .Where(m => m.ReceiverId == userId)
                                .Select(m => (int?)m.SenderId)
                                .MaxAsync().ContinueWith(t => t.Result ?? 0); // Формируем количество новых сообщений и возвращаем их результат. MaxAsync нужен для того, чтобы мы могли вернуть задачу Task
        }

        // Получитб последнее входящее соообщение от 
        public static Task<List<MessageDto>> GetIncomingMessagesSinceAsync(int userId, int sinceMessageId)
        {
            using var db = new MessengerContext();

            return db.Messages.AsNoTracking()
                                .Where(m => m.ReceiverId == userId && m.Id > sinceMessageId)
                                .OrderBy(m => m.Id)
                                .Select(m => new MessageDto
                                {
                                    Id = m.Id,
                                    SenderId = m.SenderId,
                                    ReceiverId = m.ReceiverId,
                                    SenderName = m.SenderUser.Name,
                                    ReceiverName = m.ReceiverUser.Name,
                                    TextContent = m.TextContent,
                                    TimeSent = m.TimeSent
                                }).ToListAsync();
        }
    }
}
