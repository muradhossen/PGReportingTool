using Npgsql;
using System.Data;

namespace ConnectingToPG
{
    internal class ConnectToPg
    {

        string connectionString = "Server=localhost;Port=5432;Database=CustomerOrder;User Id=postgres;Password=12345;";


      public  NpgsqlConnection _connection;
        public ConnectToPg()
        {
            _connection = new NpgsqlConnection(connectionString);
            OpenConnection();
        }

        public void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection = new NpgsqlConnection(connectionString);
            }

            _connection.Open();
            Console.WriteLine("Connection successful!");
        }

        public void CloseConnection()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }


    }
}
