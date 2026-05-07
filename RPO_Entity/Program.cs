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