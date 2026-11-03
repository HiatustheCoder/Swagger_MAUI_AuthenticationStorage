
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

const string connectionString = "Data Source=items.db";

// Create SQLite database automatically
using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = """
        CREATE TABLE IF NOT EXISTS Items (
            ItemID TEXT PRIMARY KEY,
            ItemName TEXT NOT NULL,
            ItemDescription TEXT NOT NULL
        );
        """;

    command.ExecuteNonQuery();
}

// BASIC AUTHENTICATION
app.MapPost("/api/auth/login", (HttpContext context) =>
{
    string? authorization =
        context.Request.Headers.Authorization.ToString();

    if (string.IsNullOrWhiteSpace(authorization) ||
        !authorization.StartsWith("Basic ",
            StringComparison.OrdinalIgnoreCase))
    {
        return Results.Unauthorized();
    }

    try
    {
        string encoded = authorization.Substring(6).Trim();

        string decoded = Encoding.UTF8.GetString(
            Convert.FromBase64String(encoded));

        string[] credentials = decoded.Split(':', 2);

        if (credentials.Length == 2 &&
            credentials[0] == "Wolff01" &&
            credentials[1] == "Password1")
        {
            return Results.Ok(new
            {
                success = true,
                message = "Login successful"
            });
        }
    }
    catch (FormatException)
    {
        return Results.Unauthorized();
    }

    return Results.Unauthorized();
});

// GET ALL ITEMS
app.MapGet("/api/items", () =>
{
    var items = new List<Item>();

    using var connection =
        new SqliteConnection(connectionString);

    connection.Open();

    var command = connection.CreateCommand();

    command.CommandText =
        "SELECT ItemID, ItemName, ItemDescription FROM Items";

    using var reader = command.ExecuteReader();

    while (reader.Read())
    {
        items.Add(new Item
        {
            ItemID = reader.GetString(0),
            ItemName = reader.GetString(1),
            ItemDescription = reader.GetString(2)
        });
    }

    return Results.Ok(items);
});

// SAVE ITEM
app.MapPost("/api/items", ([FromBody] Item item) =>
{
    if (string.IsNullOrWhiteSpace(item.ItemID) ||
        string.IsNullOrWhiteSpace(item.ItemName) ||
        string.IsNullOrWhiteSpace(item.ItemDescription))
    {
        return Results.BadRequest(
            "All item fields are required.");
    }

    using var connection =
        new SqliteConnection(connectionString);

    connection.Open();

    var command = connection.CreateCommand();

    command.CommandText = """
        INSERT INTO Items (ItemID, ItemName, ItemDescription)
        VALUES ($id, $name, $description)
        ON CONFLICT(ItemID) DO UPDATE SET
        ItemName = excluded.ItemName,
        ItemDescription = excluded.ItemDescription;
        """;

    command.Parameters.AddWithValue("$id", item.ItemID);
    command.Parameters.AddWithValue("$name", item.ItemName);
    command.Parameters.AddWithValue(
        "$description", item.ItemDescription);

    command.ExecuteNonQuery();

    return Results.Ok(new
    {
        success = true,
        message = "Item saved successfully"
    });
});

app.Run();

public class Item
{
    public string ItemID { get; set; } = "";
    public string ItemName { get; set; } = "";
    public string ItemDescription { get; set; } = "";
}
