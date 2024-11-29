using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
    .WithOpenApi();
app.MapGet("/helloworld", (int? limit, string? name, int? page) =>
{
    var petOwners = new List<object>();

    // Set default values if parameters are not provided
    limit = limit ?? 10;  // Default to 10 if not provided
    page = page ?? 1;     // Default to page 1 if not provided

    var offset = (page.Value - 1) * limit.Value;  // Calculate the offset for pagination

    // Build SQL query with filtering and pagination
    var query = "SELECT * FROM PetOwners WHERE 1 = 1";  // Start with a basic query

    // Add filtering by name if provided
    if (!string.IsNullOrEmpty(name))
    {
        query += " AND Customer_firstname LIKE @Name";
    }

    // Add pagination
    query += " ORDER BY Customer_ID OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

    // Use 'using' to manage the SQL connection and command
    using (var sqlConnection = new Microsoft.Data.SqlClient.SqlConnection("Server=localhost;Database=VetweinaryClinic;Trusted_Connection=True;TrustServerCertificate=True;"))
    {
        var cmd = new SqlCommand
        {
            CommandText = query,
            CommandType = CommandType.Text,
            Connection = sqlConnection
        };

        // Add parameters to avoid SQL injection
        cmd.Parameters.AddWithValue("@Offset", offset);
        cmd.Parameters.AddWithValue("@Limit", limit);

        // Add name filter if provided
        if (!string.IsNullOrEmpty(name))
        {
            cmd.Parameters.AddWithValue("@Name", "%" + name + "%");  // Use wildcard for LIKE search
        }

        sqlConnection.Open();

        // Use 'using' for the reader to ensure it is disposed properly
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                petOwners.Add(new
                {
                    Id = reader["Customer_ID"],
                    Name = reader["Customer_firstname"],
                    Phone = reader["Phone_number"],
                });
            }
        }
    }

    return Results.Ok(petOwners);  // Return the list as JSON
});


app.MapPost("/addPetOwner", async (PetOwner newOwner) =>
{
    // Define the SQL connection string
var connectionString = "Server=localhost;Database=VetweinaryClinic;Trusted_Connection=True;TrustServerCertificate=True;";

    // Create a SQL connection using 'using' to ensure it's properly disposed
using (var sqlConnection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
{
    var cmd = new SqlCommand
    {
        CommandText = "INSERT INTO PetOwners ( Phone_number, Customer_firstname, Customer_lastname) VALUES (@Phone_number, @Customer_firstname, @Customer_lastname)",
        CommandType = CommandType.Text,
        Connection = sqlConnection
    };

        // Add parameters to avoid SQL injection

    cmd.Parameters.AddWithValue("@Phone_number", newOwner.Phone_number);
    cmd.Parameters.AddWithValue("@Customer_firstname", newOwner.Customer_firstname);
    cmd.Parameters.AddWithValue("@Customer_lastname", newOwner.Customer_lastname);

        // Open the connection and execute the query
    sqlConnection.Open();
    await cmd.ExecuteNonQueryAsync(); // Using ExecuteNonQuery for insert operations

    return Results.Ok(new { message = "Pet owner added successfully!" });
}
});

app.Run();
internal class PetOwner
{

    public string Phone_number { get; set; }
    public string Customer_firstname { get; set; }
    public string Customer_lastname { get; set; }
}
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
