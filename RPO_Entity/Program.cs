using Microsoft.EntityFrameworkCore;

namespace RPO_Entity
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (RpousersContext database = new RpousersContext())
            {
                User david = new User { Name = "Давид", Age = 17 };
                User timur = new User { Name = "Тимур", Age = 18 };

                database.Users.Add(david);
                database.Users.Add(timur);
                database.SaveChanges();

                List<User>? users = database.Users.ToList();
                Console.WriteLine("Список пользователей РПО:");
                foreach (User user in users)
                {
                    Console.WriteLine($"{user.Id}. {user.Name}, {user.Age}");
                }
            }
        }
    }
}