using Microsoft.EntityFrameworkCore;

namespace RPO_Entity
{
    internal class MessengerContext : DbContext
    {
        public DbSet<MessengerUser> Users => Set<MessengerUser>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<Message> Messages => Set<Message>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=messenger.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Задаем параметры сущности в базе данных
            modelBuilder.Entity<MessengerUser>(entity =>
            {
                entity.ToTable("MessengerUsers"); // название таблички
                entity.HasKey(x => x.Id); // Устанавливает основной ключ
                entity.Property(x => x.Name).IsRequired().HasMaxLength(16); // Устаналивает имени обязательное заполнение и размер в 16 символов
                entity.HasIndex(x => x.Name).IsUnique(); // Устанавливает уникальное значение
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("Session");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.SessionKey).IsRequired().HasMaxLength(32); // При необходимости можно и нужно изменить ключ сессии для шифрования!
                entity.Property(x => x.LastHeartbeatTime).IsRequired();
                entity.Property(x => x.IsActive).IsRequired();

                entity.HasOne(x => x.MessengerUser).WithMany(x => x.UserSessions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade); // Задаем отношения между сущностями пользователя и сессиями, устанавливая настройки удаления через каскад (при удалении сущности все связи будут автоматически удалены, что не вызовет ошибок в связях)
                entity.HasIndex(x => x.SessionKey).IsUnique();
                entity.HasIndex(x => new { x.LastHeartbeatTime, x.IsActive, x.UserId });
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Messages");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.TextContent).IsRequired().HasMaxLength(4096);
                entity.Property(x => x.ConversationKey).IsRequired().HasMaxLength(32);
                entity.Property(x => x.TimeSent).IsRequired();
                entity.HasOne(x => x.SenderUser).WithMany(x => x.SentMessages).HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict); // Устанавливаем отношения между отправителем и сообщениями, а удаление ограничиваем, чтобы не затронуло остальные сущности
                entity.HasOne(x => x.ReceiverUser).WithMany(x => x.ReceivedMessages).HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(x => new { x.ConversationKey, x.TimeSent});
                entity.HasIndex(x => new { x.ReceiverId, x.Id });
            });
        }
    }
}
