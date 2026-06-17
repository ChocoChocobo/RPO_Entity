using Microsoft.EntityFrameworkCore;

// 1. Сущность сообщения
// 2. Сущность пользователя
// 3. Сущность сессии
// 4. Контейнер сообщения - объект передачи данных
// 5. Контекст

// ---Разбиение текста на два сообщения, если превышает лимит

namespace RPO_Entity
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await MessengerCore.Initialize();

            Console.WriteLine("Введите ваше имя: ");
            string username = Console.ReadLine(); // Сделать обработку исключений
            MessengerUser mUser;
            try
            {
                 mUser = await MessengerCore.GetCreateUserAsync(username);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }

            string sessionKey = await MessengerCore.CreateSessionAsync(mUser.Id);

            using var cts = new CancellationTokenSource();
            Task heartbeatTask = RunHeartbeatAsync(sessionKey, cts.Token);
            Task notificationTask = RunNotificationAsync(mUser.Id, cts.Token);

            try
            {
                Console.Clear();
                Console.WriteLine($"Вы вошли как: {mUser.Name}.");

                while (true)
                {
                    // Получаем активных пользователей и выводим
                    List<MessengerUser> activeUsers = await MessengerCore.GetActiveUsersAsync(mUser.Id);
                    Console.WriteLine("Активные пользователи:");
                    if (activeUsers.Count == 0)
                    {
                        Console.WriteLine("Активных пользователей нет.");
                    }
                    else
                    {
                        for (int i = 0; i < activeUsers.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {activeUsers[i].Name}");
                        }
                    }

                    // Выбор пользователя для чата
                    Console.Write("Выберите пользователя для чата: ");
                    string userChoice = Console.ReadLine() ?? string.Empty;
                    if (int.TryParse(userChoice, out int choice))
                    {
                        MessengerUser otherUser = activeUsers[choice - 1];
                        await OpenChatAsync(mUser, otherUser);
                        Console.Clear();
                        continue;
                    }

                    Console.WriteLine("Неверный выбор ты олух!");
                }
            }
            finally
            {
                cts.Cancel();
                try
                {
                    await Task.WhenAll(heartbeatTask, notificationTask);
                }
                catch (Exception) { }

                await MessengerCore.CloseSessionAsync(sessionKey);
            }
        }
        // Функция, позволяющая бесконечно параллельно обновлять признак жизни сессии пользователя
        private static async Task RunHeartbeatAsync(string sessionKey, CancellationToken cancellationToken)
        {
            // Поскольку для логики проверки на признак жизни необходимо учитывать интервал, то создаем таймер
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await MessengerCore.RefreshSessionAsync(sessionKey);
            }
        }

        private static async Task RunNotificationAsync(int userId, CancellationToken cancellationToken)
        {
            int lastSeenId = await MessengerCore.GetLastIncomingMessageAsync(userId);
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                List<MessageDto> incomingMessages = await MessengerCore.GetIncomingMessagesSinceAsync(userId, lastSeenId);
                if (incomingMessages.Count == 0) continue;

                foreach (var message in incomingMessages)
                {
                    Console.WriteLine($"[{message.TimeSent}] Новое сообщение от {message.SenderName}: {message.TextContent}");
                }

                lastSeenId = incomingMessages[^1].Id; // Инициализация индекса последним индексом из списка сообщений
            }
        }

        private static async Task OpenChatAsync(MessengerUser mUser, MessengerUser mOtherUser)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"'!back' чтобы вернуться назад. \n");
                List<MessageDto> messages = await MessengerCore.GetConversationAsync(mUser.Id, mOtherUser.Id);

                // Вывод старой переписки
                if (messages.Count == 0) Console.WriteLine("Сообщений пока нет. \n");
                foreach (var message in messages)
                {
                    string author = message.SenderId == mUser.Id ? "Ты" : mOtherUser.Name;
                    Console.WriteLine($"[{message.TimeSent}] {author}: {message.TextContent}");
                }
                // Обработка ввода
                Console.Write("> ");
                string? userInput = Console.ReadLine();
                if (userInput == null) return;
                if (userInput == "!back") return;

                try
                {
                    await MessengerCore.SendMessageAsync(mUser.Id, mOtherUser.Id, userInput);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"У вас ошибка головного мозга: {exception.Message}");
                }
            }
        }
    }
}