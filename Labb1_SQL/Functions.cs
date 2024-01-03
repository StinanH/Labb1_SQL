// Stina Hedman 
// NET23

using System;
using System.Data.SqlClient;

namespace Labb1_SQL
{
    internal class Functions
    {
        //Function to add new student to database
        public static void AddNewStudent(string connectionString)
        {
            Console.Clear();
            Console.WriteLine("Lägg till ny Elev.\n");

            string newSFirstName;
            string newSLastName;
            string newSClassId;
            string newSBirthDateInput;
            DateTime sBirthDate;

            //Ask user to input information about new employee and store it.
            Console.WriteLine("Ange förnamn: ");
            newSFirstName = Console.ReadLine();

            Console.WriteLine("Ange efternamn: ");
            newSLastName = Console.ReadLine();

            //Check existing classes
            List<string> classIds = new List<string>();
            List<string> classPresentations = new List<string>();

            using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
            {
                connection.Open();
                SqlCommand getAverageGrade = new(("SELECT * FROM Classes"), connection);

                using (SqlDataReader reader = getAverageGrade.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        //add each existing class to list.
                        while (reader.Read())
                        {
                            int Id = reader.GetInt32(reader.GetOrdinal("ID"));
                            string Name = reader.GetString(reader.GetOrdinal("Class_Name"));
                            classIds.Add(Id.ToString());
                            classPresentations.Add($"ID : {Id}, Klass: {Name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Det finns inga klasser tillgängliga.");
                    }
                }
            }

            //ask for ClassID of class that the student enrolls to.
            Console.WriteLine("Ange klassID eleven ska gå i, tre siffror: ");
            newSClassId = Console.ReadLine();

            //while input classID isn't an existing class, print list of existing classes and ask again.
            while (!classIds.Contains(newSClassId))
            {
                Console.WriteLine("Denna klass existerar ej, vänligen ange en av följande klassers ID:");
                for (int i = 0; i <= classIds.Count - 1; i++)
                {
                    Console.WriteLine($"{classPresentations[i]}");
                }
                newSClassId = Console.ReadLine();
            }

            //ask for user input for birthday
            Console.WriteLine("Ange födelsedatum (åååå-mm-dd): ");
            newSBirthDateInput = Console.ReadLine();
            //while not a valid birthdate input, ask again.
            while (!DateTime.TryParse(newSBirthDateInput, out sBirthDate))
            {
                Console.WriteLine("Försök igen, ange födelsedatum (åååå-mm-dd): ");
                newSBirthDateInput = Console.ReadLine();
            }

            //add new student to database   
            using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"INSERT INTO dbo.Students VALUES ('{newSFirstName}', '{newSLastName}', {newSClassId}, CONVERT(date, '{sBirthDate.ToString("yyyy-MM-dd")}'))", connection);
                command.ExecuteNonQuery();

                Console.WriteLine("Tryck på valfri tangent för att återgå till menyn.");
                Console.ReadKey();
            }
        }

        //Function to list all students
        public static void ListAllStudents(string connectionString)
        {
            Console.Clear();
            Console.WriteLine("Vill du ha eleverna sorterade efter\n1.Förnamn\n2.Efternamn");
            Console.WriteLine();

            bool sortByFirstName = true;
            string sortFirst = "";
            string sortSecond = "";

            //let user choose if the students should be ordered by first or last name.
            switch (InputCheckMenu(2, true))
            {

                case 1:
                    Console.Clear();
                    Console.WriteLine("Eleverna sorteras utifrån förnamn");
                    sortFirst = "First_Name";
                    sortSecond = "Last_Name";
                    break;

                case 2:
                    Console.Clear();
                    Console.WriteLine("Eleverna sorteras utifrån efternamn");
                    sortFirst = "Last_Name";
                    sortSecond = "First_Name";
                    sortByFirstName = false;
                    break;

                case 0:
                    break;
            }

            //let user choose descending or ascending order
            Console.WriteLine("Vilken ordning vill du sortera i? \n1. Fallande \n2. Stigande");
            Console.WriteLine();
            string order = "";

            switch (InputCheckMenu(2, true))
            {
                case 1:
                    Console.Clear();
                    order = "DESC";
                    break;

                case 2:
                    Console.Clear();
                    order = "ASC";
                    break;

                case 0:
                    break;
            }

            using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
            {
                connection.Open();
                SqlCommand command = new($"SELECT {sortFirst}, {sortSecond} FROM Students ORDER BY {sortFirst} {order};", connection);


                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.HasRows)
                    {
                        //print first and last name in desierd order
                        while (reader.Read())
                        {
                            string firstName = reader.GetString(reader.GetOrdinal("First_Name"));
                            string lastName = reader.GetString(reader.GetOrdinal("Last_Name"));

                            if (sortByFirstName)
                            {
                                Console.WriteLine(firstName + " " + lastName);
                            }
                            else
                            {
                                Console.WriteLine(lastName + " " + firstName);
                            }
                        }


                        Console.WriteLine("Tryck på valfri tangent för att återgå till menyn.");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("Ingen info att skriva ut");
                    }
                }
                connection.Close();
            }

        }

        //Function to list all students belonging to certain class.
        public static void ListAllStudentsInClass(string connectionString)
        {
            Console.Clear();
            //list to store current classnames to pick from
            List<string> className = new List<string>();

            //command for reader to fetch current classnames and store in list
            using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
            {
                connection.Open();
                SqlCommand getClassNames = new(("SELECT Class_Name FROM Classes"), connection);

                using (SqlDataReader readClassNames = getClassNames.ExecuteReader())
                {
                    if (readClassNames.HasRows)
                    {
                        while (readClassNames.Read())
                        {
                            string name = readClassNames.GetString(readClassNames.GetOrdinal("Class_Name"));
                            className.Add(name);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ingen info att skriva ut");
                    }
                }
            }
            Console.WriteLine("Ange vilken klass du vill hämta alla elever från: ");

            for (int i = 0; i < className.Count; i++)
            {
                Console.WriteLine($"{i + 1} : {className[i]}");
            }
            Console.WriteLine();

            int index = InputCheckMenu(className.Count, true);
            if (index == 0)
            {
                return;
            }

            else
            {
                index -= 1;

                using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
                {
                    connection.Open();
                    SqlCommand command = new($"SELECT s.ID, s.First_Name, s.Last_Name, s.Birthdate, c.Class_Name, c.ID FROM Students s LEFT JOIN Classes c ON (s.Class_ID = c.ID) WHERE c.Class_Name = '{className[index]}'", connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Console.Clear();
                            Console.WriteLine($"Elever i klass {className[index]} : ");

                            //print student
                            while (reader.Read())
                            {
                                string sFirstName = reader.GetString(reader.GetOrdinal("First_Name"));
                                string sLastName = reader.GetString(reader.GetOrdinal("Last_Name"));
                                DateTime sDateOfBirth = reader.GetDateTime(reader.GetOrdinal("Birthdate"));
                                int sStudentID = reader.GetInt32(reader.GetOrdinal("ID"));
                                string sClassName = reader.GetString(reader.GetOrdinal("Class_Name"));
                                Console.WriteLine($"{sFirstName} {sLastName}, {sStudentID}, {sDateOfBirth.ToString("yyyy-MM-dd")}, Klass: {sClassName}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ingen info att skriva ut");
                        }
                    }
                }
            }
            Console.WriteLine("Tryck på valfri tangent för att återgå till menyn.");
            Console.ReadKey();
        }


        //Function to add new employee to database
        public static void AddNewEmployee(string connectionString)
        {
            Console.WriteLine("Lägga till ny personal.");
            
            string pFirstName;
            string pLastName;
            string pOccupation;
            string birthDateInput;
            DateTime birthDate;

            //Ask user to input information about new employee and store it.
            Console.WriteLine("Ange förnamn: ");
            pFirstName = Console.ReadLine();
            Console.WriteLine("Ange efternamn: ");
            pLastName = Console.ReadLine();
            Console.WriteLine("Ange titel: ");
            pOccupation = Console.ReadLine();
            Console.WriteLine("Ange födelsedatum: ");
            birthDateInput = Console.ReadLine();

            while (!DateTime.TryParse(birthDateInput, out birthDate))
            {
                Console.WriteLine("Försök igen, ange födelsedatum (åååå-mm-dd): ");
                birthDateInput = Console.ReadLine();
            }

            //skapa ny personal   
            using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"INSERT INTO dbo.Employees VALUES ('{pLastName}', '{pFirstName}', '{pOccupation}', CONVERT(date, '{birthDate.ToString("yyyy-MM-dd")}'))", connection);
                command.ExecuteNonQuery();

                Console.WriteLine("Tryck på valfri tangent för att återgå till menyn.");
                Console.ReadKey();
            }
        }

        //Function to list all employees
        public static void ListAllEmployees(string connectionString)
        {

            //print options
            Console.WriteLine("Välj:\n1. Lista alla anställda.\n2. Lista alla anställda inom specifikt yrke.");

            switch (InputCheckMenu(2, true))
            {
                //List all employees
                case 1:
                    using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
                    {
                        connection.Open();
                        SqlCommand command = new($"SELECT * FROM Employees;", connection);
                        {

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                
                                if (reader.HasRows)
                                {
                                    //print employee
                                    while (reader.Read())
                                    {
                                        string peFirstName = reader.GetString(reader.GetOrdinal("First_Name"));
                                        string peLastName = reader.GetString(reader.GetOrdinal("Last_Name"));
                                        string pebirthdate = reader.GetDateTime(reader.GetOrdinal("Birthdate")).ToString("yyyy-MM-dd");
                                        string peOccupation = reader.GetString(reader.GetOrdinal("Occupation"));
                                        Console.WriteLine($"{peFirstName} {peLastName} {pebirthdate} {peOccupation}");
                                    }
                                }

                                else
                                {
                                    Console.WriteLine("Ingen info att skriva ut");
                                }

                                Console.WriteLine("Tryck på valfri tangent för att återgå till menyn.");
                                Console.ReadKey();

                            }

                        }
                    }
                break;

                //List employees within certain occupation
                case 2:

                    //list to store current occupations to pick from
                    List<string> occupations = new List<string>();

                    using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
                    {
                        connection.Open();

                        SqlCommand command = new(("SELECT Occupation FROM Employees"), connection);
                        SqlDataReader readOccupations = command.ExecuteReader();

                        if (readOccupations.HasRows)
                        {
                            while (readOccupations.Read())
                            {
                                string occ = readOccupations.GetString(readOccupations.GetOrdinal("Occupation"));
                                if (!occupations.Contains(occ))
                                {
                                    occupations.Add(occ);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Det finns inga anställda inom några yrken att skriva ut");
                        }

                        readOccupations.Close();
                    }
                    Console.Clear();
                    Console.WriteLine("Ange vilket yrke du vill se samtliga anställda inom : ");

                    //prints out the different occupations currently employed
                    for (int i = 0; i < occupations.Count; i++)
                    {
                        Console.WriteLine($"{i + 1} : {occupations[i]}");
                    }
                    Console.WriteLine();

                    int occIndex = InputCheckMenu(occupations.Count, true);

                    //if 0 return to main menu
                    if (occIndex == 0)
                    {
                        return;
                    }

                    //else use chosen index to find all employees within that occupation in list.
                    else
                    {
                        occIndex -= 1;

                        using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
                        {
                            connection.Open();
                            SqlCommand command = new($"SELECT First_Name, Last_Name, Birthdate, Occupation FROM Employees WHERE Occupation = '{occupations[occIndex]}';", connection);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    Console.Clear();
                                    Console.WriteLine($"Anställda {occupations[occIndex]}:");

                                    //Prints all employed with chosen occupation.
                                    while (reader.Read())
                                    {
                                        string perFirstName = reader.GetString(reader.GetOrdinal("First_Name"));
                                        string perLastName = reader.GetString(reader.GetOrdinal("Last_Name"));
                                        DateTime perDateOfBirth = reader.GetDateTime(reader.GetOrdinal("Birthdate"));
                                        string perOccupation = reader.GetString(reader.GetOrdinal("Occupation"));
                                        Console.WriteLine($"{perFirstName} {perLastName} {perDateOfBirth.ToString("yyyy-MM-dd")} {perOccupation}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Ingen info att skriva ut");
                                }
                            }

                        }
                    }
                    Console.WriteLine("Tryck på valfri tangent för att återgå till menyn.");
                    Console.ReadKey();
                    break;
            }
        }


        //Function to show grades set within last 6 months.
        public static void ListGradesOfLastSixMonths(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
            {
                connection.Open();
                SqlCommand command = new($"SELECT g.Student_ID, s.First_Name, s.Last_Name, g.GRADE, g.Date_Set, g.Course_ID, c.Course_Name FROM Grades g JOIN Courses C on (g.Course_ID = c.ID) JOIN Students s on (Student_ID = s.ID);", connection);
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        Console.WriteLine("Betyg satta senaste 6 månaderna: ");
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                DateTime gradeSetDate = reader.GetDateTime(reader.GetOrdinal("Date_Set"));

                                //if grade is set within last 6 months, print it
                                if (DateTime.Now.AddMonths(-6) <= gradeSetDate)
                                {
                                    string gradeFirstName = reader.GetString(reader.GetOrdinal("First_Name"));
                                    string gradeLastName = reader.GetString(reader.GetOrdinal("Last_Name"));
                                    int gradeSet = reader.GetInt32(reader.GetOrdinal("GRADE"));
                                    string courseName = reader.GetString(reader.GetOrdinal("Course_Name"));

                                    Console.WriteLine($"{gradeFirstName} {gradeLastName}, {courseName}, {gradeSet}, {gradeSetDate.ToString("yyyy-MM-dd")}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Inga betyg satta under de senaste 6 månaderna.");
                        }

                        Console.WriteLine("Tryck på valfri tangent för att återgå till menyn.");
                        Console.ReadKey();
                    }
                }
            }
        }

        //Function to hsow average grade in each course.
        public static void ShowAverageGrades(string connectionString)
        {
            Console.WriteLine("Snittbetyg i samtliga kurser:\n");

            //Command for reader to fetch current classnames and store in list
            using (SqlConnection connection = new SqlConnection(@$"{connectionString}"))
            {
                connection.Open();
                SqlCommand getAverageGrade = new(("SELECT CAST(AVG(1. * g.GRADE)as numeric(10,2)) as Avg_Grade, c.Course_Name FROM GRADES g LEFT JOIN Courses c ON (g.Course_ID = c.ID) GROUP BY c.Course_Name"), connection);

                using (SqlDataReader reader = getAverageGrade.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        //print average grade from each Course
                        while (reader.Read())
                        {
                            Decimal avgGrade = reader.GetDecimal(reader.GetOrdinal("Avg_Grade"));
                            string courseName = reader.GetString(reader.GetOrdinal("Course_Name"));

                            Console.WriteLine($"{courseName} : {avgGrade}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ingen info att skriva ut");
                    }
                }
            }

            Console.WriteLine("\nTryck på valfri knapp för att återgå till menyn..");
            Console.ReadKey();
            Console.Clear();
        }


        //Function used to check that input is valid for menu options.
        public static int InputCheckMenu(int highestOption, bool returnToMenuOption)
        {
            bool checkInput = true;
            string inputStr = Console.ReadLine();
            int inputInt;

            //while checking valid input.
            while (checkInput)
            {
                while (!Int32.TryParse(inputStr, out inputInt))
                {
                    if (returnToMenuOption)
                    {
                        Console.WriteLine($"Ange en siffra mellan 0-{highestOption}");
                        Console.Write("Ange 0 för att återgå till huvudmenyn.");
                    }

                    else
                    {
                        Console.WriteLine($"Ange en siffra mellan 1-{highestOption}");
                    }
                    inputStr = Console.ReadLine();
                }

                if (inputInt >= 1 || inputInt <= highestOption)
                {
                    checkInput = false;
                    Console.Clear();
                    return inputInt;
                }
                else if (returnToMenuOption)
                {
                    if (inputInt == 0)
                    {
                        checkInput = false;
                        Console.Clear();
                        return inputInt;
                    }
                }
                else
                {
                    Console.WriteLine($"Ange en siffra mellan 0-{highestOption.ToString()}");
                }
            }

            Console.Clear();
            return inputInt = 0;
        }
    }
}
