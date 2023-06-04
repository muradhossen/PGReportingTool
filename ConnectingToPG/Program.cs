using ConnectingToPG;
using ConnectingToPG.Models;
using Npgsql;
using System.Data;
using System.Reflection;

var connection = new ConnectToPg();
var dynamicClassFactory = new DynamicClassFactory();


string sqlQuery = @"SELECT * FROM public.""Items"" ORDER BY ""ItemsId"" ASC ";


List<Dictionary<string, object>> rows = new();

using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection._connection))
{
    using (NpgsqlDataReader reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            Dictionary<string, object> row = new();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                // Retrieve data from the reader
                string column = reader.GetName(i);
                var value = reader.GetValue(i);

                row.Add(column, value);
            }
            rows.Add(row);
        }
    }
}


var firstRow = rows[0];

var propertyParams = new List<PropertyParam>();

foreach (var key in firstRow.Keys)
{
    propertyParams.Add(new PropertyParam { Name = key, Type="string" });    
}

Type ReportDTO = dynamicClassFactory.CreateClass("ReportDTO", propertyParams);

// Create an instance of the dynamic type

Type listType = typeof(List<>).MakeGenericType(ReportDTO);

dynamic instance = Activator.CreateInstance(listType);


foreach (var prop in propertyParams)
{
    dynamic reportDtoInstance = Activator.CreateInstance(ReportDTO);

    PropertyInfo Property = ReportDTO.GetProperty(prop.Name);
    Property.SetValue(reportDtoInstance, "John Doe");

    instance.Add(reportDtoInstance);


}


Console.ReadKey();