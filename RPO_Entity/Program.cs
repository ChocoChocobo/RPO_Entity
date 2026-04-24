using Microsoft.EntityFrameworkCore;

// Класс, описывающий модель пользователя для базы данных
public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
}

// Класс, обеспечивающий взаимодействие с БД
public class ApplicationContext : DbContext // Класс, который определяет контекст данных
{
    public DbSet<User> Users => Set<User>(); // предоставляет БД набор объектов, которые должны в ней храниться
    public ApplicationContext()
    {
        Database.EnsureCreated(); // Функция, проверяющая наличие БД и создает ее, если та отсутствует
    }
    // Переопределение функции настройки БД, которая принимает в качестве параметра объект класса настроек
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Говорим объекту настроек установить подключение к определенной базе данных
        optionsBuilder.UseSqlite("Data Source=rpousers.db");
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        // С помощью конструкции using можно определить объект, который будет использоваться в этом блоке.
        using (ApplicationContext database = new ApplicationContext())
        {
            User david = new User { Name = "Давид", Age = 17 };
            User timur = new User { Name = "Тимур", Age = 18 };

            // Добавленией объектов в БД
            database.Users.Add(david);
            database.Users.Add(timur);
            database.SaveChanges(); // Сохранение изменений в БД

            // Получение объектов из БД и вывод их в консоль
            List<User>? users = database.Users.ToList();
            Console.WriteLine("Список пользователей РПО:");
            foreach (User user in users)
            {
                Console.WriteLine($"{user.Id}. {user.Name}, {user.Age}");
            }
        }        
    }
}

//      Практика
// 1. Создать класс Продукта, у которого есть поля идентификатора, названия, количества и цены. Указать свойства get и set
// 2. Создать класс ApplicationContext, который бы в себе хранил коллекцию объектов продуктов и имел остальные функции по аналогии с примером с занятия.
// 3. В Main продемонстрировать создание 5 объектов продуктов (включая пару объектов с кол-во 0), добавить их в базу данных, вывести и производить проверку на списание товара: Если кол-во товара 0, тогда добавлять его в коллекцию очереди на списание.