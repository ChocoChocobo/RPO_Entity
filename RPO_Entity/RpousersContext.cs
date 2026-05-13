using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Xml.Linq;

namespace RPO_Entity
{
    public partial class RpousersContext : DbContext
    {
        public RpousersContext()
        {
            //Database.EnsureCreated();
        }

        public RpousersContext(DbContextOptions<RpousersContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        // Объект потока вывода
        StreamWriter logWriter = new StreamWriter("log.log", false);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=rpousers.db");
            // Логирование осуществляется с помощью функции LogTo, которая принимает поток вывода. Отсюда следует, что можно вывести информацию в консоль либо в файл.
            //optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.LogTo(logWriter.WriteLine, Microsoft.Extensions.Logging.LogLevel.Error);
            // Каждое сообщение в логе закреплено за определенным идентификатором:
            // CoreEventId - событие для инфраструктуры Entity;
            // RelationalEventId - событие характерное для реляционной базы данных
            //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.ConnectionCreated });
            //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });

            // Существует дополнительно класс DbLoggerCategory, который позволяет удобно фильтровать логгирование по разным категориям:
            /*optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Connection.Name });
            optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Name });
            optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name });
            optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Transaction.Name });*/
            //optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Query.Name });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public void PrintInfo()
        {
            List<User>? users = Users.ToList();
            Console.WriteLine("Список пользователей РПО:");
            foreach (User user in users)
            {
                Console.WriteLine($"{user.Id}. {user.Name}, {user.Age}");
            }
        }

        public void PrintUserList(List<User> users)
        {
            Console.WriteLine("Конкретный список пользователей: ");
            foreach (User user in users)
            {
                Console.WriteLine($"{user.Id}. {user.Name}, {user.Age}");
            }
        }

        //  ---CRUD---
        // Create
        public void AddUser(User user)
        {
            Users.Add(user);
            SaveChanges();
        }

        public void AddUser(User[] users)
        {
            Users.AddRange(users);
            SaveChanges();
        }

        // Read
        public List<User> GetUser()
        {
            return Users.ToList();
        }
        
        public bool TryGetUsersByName(string name, out List<User> outUsers)
        {
            // Where принимает лямбда-функцию, которая работает с объектом класса и проверяет что-либо в соответствии с условием
            var users = Users.Where(user => user.Name != null && user.Name.Contains(name)).ToList(); // в объект users заносим все подходящие значения из БД в соответствии с условием.
            if (users.Count == 0) // Если значений в зависимости от условия нет, то просто инициализируем список без значений и возвращаем false
            {
                Console.WriteLine("Пользователей с таким именем нет в БД!");
                outUsers = new List<User>();
                return false;
            }
            else
            {
                Console.WriteLine("Пользователи с таким именем есть в БД!");
                outUsers = users;
                return true;
            }
        }

        // Возвращение всех совершеннолетних пользователей из выборки
        public bool TryGetAdultUsers(out List<User> outUsers)
        {
            // Нахождение пользоваталей совершеннолетних
            var users = Users.Where(user => user.Age >= 18).ToList();
            if (users.Any())
            {
                Console.WriteLine("Совершеннолетних пользователей в БД нет!");
                outUsers = new List<User>();
                return false;
            }
            else
            {
                Console.WriteLine("Совершеннолетние пользователи в БД есть!");
                outUsers = users;
                return true;
            }
        }

        // Update 
        public void UpdateUser(int id)
        {
            User? user = Users.Find(id);
            if (user != null)
            {
                Console.Write("Введите новое имя для пользователя: ");
                user.Name = Console.ReadLine();
                Console.Write("Введите новый возраст для пользователя: ");
                user.Age = Convert.ToInt32(Console.ReadLine());
                
                SaveChanges();
            }
            else Console.WriteLine($"Подходящего пользователя с id: {id} не найдено");
        }

        // Delete
        public void DeleteUser(int id)
        {
            User? user = Users.Find(id);
            if (user != null)
            {
                Users.Remove(user);
                SaveChanges();
            }
            else Console.WriteLine($"Значения с id: {id} не было найдено");
        }

        public bool TryDeleteUsers(int[] ids)
        { 
            // Делаем выборку пользоваталей с совпадающими с передаваемым массивом id
            List<User> usersToDelete = Users.Where(user => ids.Contains(user.Id)).ToList();

            if (usersToDelete.Any()) // Проверяем есть ли какие-либо элементы в коллекции
            {
                Users.RemoveRange(usersToDelete);
                SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}