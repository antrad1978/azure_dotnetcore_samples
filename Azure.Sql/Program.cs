using System;
using System.Data.SqlClient;

namespace Azure.Sql
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ConnectionString =
                "Server=tcp:ngtserver1.database.windows.net,1433;Initial Catalog=ngtdb1;Persist Security Info=False;User ID=ngt;Password=Data123456789;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();
                    String sql = "SELECT Restaurant.name FROM Person, likes, Restaurant WHERE MATCH(Person - (likes)->Restaurant) AND Person.name = 'John'; ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0}", reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }
}