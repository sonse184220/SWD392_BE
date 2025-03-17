using Service.ChatModel;
using Repository.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Service.Services
{
    public class SqlServerDatabaseService : IDatabaseService
    {
        private readonly CityScoutContext _context;
        private readonly IConfiguration _configuration;

        public SqlServerDatabaseService(CityScoutContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<List<string>>> GetDataTable(string sqlQuery)
        {
            if (string.IsNullOrEmpty(sqlQuery))
            {
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sqlQuery));
            }

            var connectionString = _context.Database.GetDbConnection().ConnectionString ??
                                  _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not available.");
            }

            var rows = new List<List<string>>();
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sqlQuery, connection);
                using var reader = await command.ExecuteReaderAsync();

                bool headersAdded = false;
                while (await reader.ReadAsync())
                {
                    var cols = new List<string>();
                    var headerCols = new List<string>();
                    if (!headersAdded)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            headerCols.Add(reader.GetName(i).ToString());
                        }
                        headersAdded = true;
                        rows.Add(headerCols);
                    }

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        try
                        {
                            cols.Add(reader.GetValue(i).ToString());
                        }
                        catch
                        {
                            cols.Add("DataTypeConversionError");
                        }
                    }
                    rows.Add(cols);
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Failed to execute SQL query: {sqlQuery}. Error: {ex.Message}", ex);
            }

            return rows;
        }

        //public async Task<DatabaseSchema> GenerateSchema()
        //{
        //    var connectionString = _context.Database.GetDbConnection().ConnectionString ??
        //                          _configuration.GetConnectionString("DefaultConnection");
        //    if (string.IsNullOrEmpty(connectionString))
        //    {
        //        throw new InvalidOperationException("Connection string is not available.");
        //    }

        //    var dbSchema = new DatabaseSchema { SchemaRaw = new List<string>(), SchemaStructured = new List<TableSchema>() };
        //    List<KeyValuePair<string, string>> rows = new();

        //    string sqlQuery = @"
        //        SELECT 
        //            t.TABLE_NAME, 
        //            c.COLUMN_NAME 
        //        FROM 
        //            INFORMATION_SCHEMA.TABLES t
        //        JOIN 
        //            INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
        //        WHERE 
        //            t.TABLE_TYPE = 'BASE TABLE';";

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    using var command = new SqlCommand(sqlQuery, connection);
        //    using var reader = await command.ExecuteReaderAsync();

        //    while (await reader.ReadAsync())
        //    {
        //        rows.Add(new KeyValuePair<string, string>(reader.GetString(0), reader.GetString(1)));
        //    }

        //    var groups = rows.GroupBy(x => x.Key);
        //    foreach (var group in groups)
        //    {
        //        dbSchema.SchemaStructured.Add(new TableSchema { TableName = group.Key, Columns = group.Select(x => x.Value).ToList() });
        //    }

        //    var textLines = new List<string>();
        //    foreach (var table in dbSchema.SchemaStructured)
        //    {
        //        var schemaLine = $"- {table.TableName} (";
        //        foreach (var column in table.Columns)
        //        {
        //            schemaLine += column + ", ";
        //        }
        //        schemaLine += ")";
        //        schemaLine = schemaLine.Replace(", )", " )");
        //        textLines.Add(schemaLine);
        //    }

        //    // Add relationships for clarity
        //    textLines.Add("Relationships:");
        //    textLines.Add("- Destination.DistrictID references District.DistrictID");
        //    textLines.Add("- District.CityID references City.CityID");
        //    textLines.Add("- OpeningHours.DestinationID references Destination.DestinationID");
        //    textLines.Add("- Destination.CategoryID references Category.CategoryID");

        //    dbSchema.SchemaRaw = textLines;
        //    return dbSchema;
        //}


        public async Task<DatabaseSchema> GenerateSchema()
        {
            var connectionString = _context.Database.GetDbConnection().ConnectionString ??
                                  _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not available.");
            }

            var dbSchema = new DatabaseSchema
            {
                SchemaRaw = new List<string>(),
                SchemaStructured = new List<TableSchema>(),
                WardNames = new List<string>(),
                DistrictNames = new List<string>(),
                CityNames = new List<string>(),
                CategoryNames = new List<string>()  // Initialize category list
            };

            // Existing schema generation for tables and columns
            string tableQuery = @"
        SELECT t.TABLE_NAME, c.COLUMN_NAME 
        FROM INFORMATION_SCHEMA.TABLES t
        JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
        WHERE t.TABLE_TYPE = 'BASE TABLE';";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Fetch table schema
            using (var command = new SqlCommand(tableQuery, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                var rows = new List<KeyValuePair<string, string>>();
                while (await reader.ReadAsync())
                {
                    rows.Add(new KeyValuePair<string, string>(reader.GetString(0), reader.GetString(1)));
                }
                var groups = rows.GroupBy(x => x.Key);
                foreach (var group in groups)
                {
                    dbSchema.SchemaStructured.Add(new TableSchema { TableName = group.Key, Columns = group.Select(x => x.Value).ToList() });
                }
            }

            // Fetch city names
            using (var command = new SqlCommand("SELECT Name FROM City", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    dbSchema.CityNames.Add(reader.GetString(0));
                }
            }

            // Fetch district names
            using (var command = new SqlCommand("SELECT Name FROM District", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    dbSchema.DistrictNames.Add(reader.GetString(0));
                }
            }

            // Fetch ward names (assuming Ward is a column in Destination)
            using (var command = new SqlCommand("SELECT DISTINCT Ward FROM Destination WHERE Ward IS NOT NULL", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    dbSchema.WardNames.Add(reader.GetString(0));
                }
            }

            // Fetch category names
            using (var command = new SqlCommand("SELECT Name FROM Category", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    dbSchema.CategoryNames.Add(reader.GetString(0));
                }
            }

            // Generate SchemaRaw as before
            var textLines = new List<string>();
            foreach (var table in dbSchema.SchemaStructured)
            {
                var schemaLine = $"- {table.TableName} (";
                foreach (var column in table.Columns)
                {
                    schemaLine += column + ", ";
                }
                schemaLine += ")";
                schemaLine = schemaLine.Replace(", )", " )");
                textLines.Add(schemaLine);
            }

            textLines.Add("Relationships:");
            textLines.Add("- Destination.DistrictID references District.DistrictID");
            textLines.Add("- District.CityID references City.CityID");
            textLines.Add("- OpeningHours.DestinationID references Destination.DestinationID");
            textLines.Add("- Destination.CategoryID references Category.CategoryID");

            dbSchema.SchemaRaw = textLines;
            return dbSchema;
        }
    }

    public interface IDatabaseService
    {
        Task<List<List<string>>> GetDataTable(string sqlQuery);
        Task<DatabaseSchema> GenerateSchema();
    }
}