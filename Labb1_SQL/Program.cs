// Stina Hedman 
// NET23

using System;
using System.Data.SqlClient;

namespace Labb1_SQL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool inMenu = true;
            string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = SchoolDB; Integrated Security = True";

                Console.WriteLine("Välkommen till skoldatabasen!");

            while (inMenu)
            {
                    Console.Clear();
                    Console.WriteLine("Ange ditt val:\n" +
                        "1. Hämta alla elever.\n" +
                        "2. Hämta alla elever i en viss klass.\n" +
                        "3. Lägg till ny personal.\n" +
                        "4. Hämta personal.\n" +
                        "5. Hämta betyg satta under de senaste 6 månaderna.\n" +
                        "6. Snittbetyg per kurs.\n" +
                        "7. Lägga till nya elever.");

                switch (Functions.InputCheckMenu(7, false))
                {

                    case 1:
                        Functions.ListAllStudents(connectionString);
                        break;


                    case 2:
                        Functions.ListAllStudentsInClass(connectionString);
                        break;


                    case 3:
                        Functions.AddNewEmployee(connectionString);
                        break;

                    case 4:
                        Functions.ListAllEmployees(connectionString);
                        break;

                    case 5:
                        Functions.ListGradesOfLastSixMonths(connectionString);
                        break;

                    case 6:
                        Functions.ShowAverageGrades(connectionString);
                        break;

                    case 7:
                        Functions.AddNewStudent(connectionString);
                        break;
                }
            }             
        }
    }
}
