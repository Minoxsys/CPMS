using System;
using System.Data.Entity;
using System.Linq;
using DBInitializer.Migrations;

namespace DBInitializer
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<UnitOfWork, Configuration>());

                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Patients.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the database: {0} -> {1}", ex.Message, ex.StackTrace);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("The Database has been successfully initialized!");
            Console.ReadLine();
        }
    }
}
