using ConnectingToPG;
using Npgsql;
using System.Data;

var connection = new ConnectToPg();


string sqlQuery = @"SELECT * FROM public.""Items"" ORDER BY ""ItemsId"" ASC ";


using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection._connection))
{
    using (NpgsqlDataReader reader = command.ExecuteReader())
    {
        while (reader.Read())
        {  
            for (int i = 0; i <= reader.FieldCount; i++)
            {
                // Retrieve data from the reader
                var column = reader.GetName(i);
                var value = reader.GetValue(i);
            }
 
        }
    }
}