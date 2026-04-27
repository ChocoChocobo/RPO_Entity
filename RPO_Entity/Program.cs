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
                database.UpdateUser(1);
                database.PrintInfo();
                database.DeleteUser(1);
                database.PrintInfo();
            }
        }
    }
}