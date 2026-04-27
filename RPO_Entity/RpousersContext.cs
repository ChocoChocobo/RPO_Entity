using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Timers;

namespace RPO_Entity
{
    public partial class RpousersContext : DbContext
    {
        public RpousersContext()
        {
            Database.EnsureCreated();
        }

        public RpousersContext(DbContextOptions<RpousersContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source=rpousers.db");

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

        //          Практика
        //  Реализовать следующий функционал:
        //  Остальные переопределяемые функции, возвращающие список пользователей из базы даннъх в соответсвии с условиями: по имени, по возрасту и по оценке

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

        public void DeleteUser(int[] id)
        {
            List<User> users = GetUser();

            for (int i = 0; i < Users.Count(); i++)
            {
                for (int j = 0; j < id.Length; j++)
                {
                    if (users[i].Id == id[j])
                    {
                        Users.Remove(users[i]);
                        SaveChanges();
                    }
                    else Console.WriteLine($"Значения с id: {id[j]} не было найдено");
                }
            }
        }
    }
}