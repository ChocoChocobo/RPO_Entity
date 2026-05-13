using Microsoft.EntityFrameworkCore;

namespace RPO_Entity
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (RpousersContext database = new RpousersContext())
            {
                database.PrintInfo();
                List<User> users = new List<User>();
                
                if (database.TryDeleteUsers(new int[] {3, 4} ))
                {
                    Console.WriteLine(" Успешное удаление пользователей с такими id!");
                }
                else
                {
                    Console.WriteLine("Неудача!");
                }

                // ПРОВЕРКА ЛОГИКИ НА СОВЕРШЕННОЛЕТНЕГО ПОЛЬЗОВАТЕЛЯ
                /*if (database.TryGetAdultUsers(out users))
                {
                    database.PrintUserList(users);
                    Console.Write("Введите id пользователя для редактирования: ");
                    database.UpdateUser(Convert.ToInt32(Console.ReadLine()));
                }*/

                // ПРОВЕРКА ЛОГИКИ НА ПОЛУЧЕНИЕ ВСЕХ ПОЛЬЗОВАТЕЛЕЙ С ИМЕНЕМ
                //Console.Write("Введите имя, которое нужно искать при поиске в БД: ");
                /*if (database.TryGetUsersByName(Console.ReadLine(), out users))
                {
                    database.PrintUserList(users);
                    Console.Write("Введите id пользователя для редактирования: ");
                    database.UpdateUser(Convert.ToInt32(Console.ReadLine()));
                }
                else Console.WriteLine("Неудача!");*/
            }
        }
    }
}

//      Практика по миграции
// 1. В текущей программе необходимо добавить два новых свойства классу User: IsTopScore (является ли пользователь преуспевающим) и ChildrenAmount (количество у пользователя). Определить свойства get и set. 
// 2. Добавить в БД 3 новых пользователя через функции от контекста, чтобы убедиться, что миграция не сломает наши данные.
// 3. Создать миграцию, применив следующую команду в коносли диспечтера пакетов: 
//      dotnet ef migrations add НАЗВАНИЕ_МИГРАЦИИ --project НАЗВАНИЕ_ПРОЕКТА
// 4. Применить миграцию с помощью команды:
//      dotnet ef database update --project НАЗВАНИЕ_ПРОЕКТА