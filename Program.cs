using HabitHub_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using HabitHub_Backend;
using System.Security.Cryptography;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
//ADD CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000",
                                "http://localhost:7285")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<HabitHubDbContext>(builder.Configuration["HabitHubDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

// Register New User
app.MapPost("/register", (HabitHubDbContext db, User payload) =>
{
    User NewUser = new User()
    {
        Name = payload.Name,
        Bio = payload.Bio,
        Email = payload.Email,
        PhoneNumber = payload.PhoneNumber,
        ImageUrl = payload.ImageUrl,
        Uid = payload.Uid,
    };
    db.Users.Add(NewUser);
    db.SaveChanges();
    return Results.Ok(NewUser.Name);
});

// Edit User
app.MapPut("/users/update/{userId}", (HabitHubDbContext db, int userId, User NewUser) =>
{
    User SelectedUser = db.Users.FirstOrDefault(x => x.Id == userId);
    if (SelectedUser == null)
    {
        return Results.NotFound("This User is not found in the database. Please Try again!");
    }

    SelectedUser.Name = NewUser.Name;
    SelectedUser.Bio = NewUser.Bio;
    SelectedUser.Email = NewUser.Email;
    SelectedUser.PhoneNumber = NewUser.PhoneNumber;
    SelectedUser.ImageUrl = NewUser.ImageUrl;
    db.SaveChanges();
    return Results.Created("/users/update/{uid}", SelectedUser);

});

// Check User
app.MapMethods("/api/checkuser/{uid}", new[] { "GET", "OPTIONS" }, (HabitHubDbContext db, string uid) =>
{
    var userExists = db.Users.Where(x => x.Uid == uid).FirstOrDefault();
    if (userExists == null)
    {
        return Results.StatusCode(204);
    }
    return Results.Ok(userExists);
});


// Get User by Id
app.MapGet("/users/return/{iden}", (HabitHubDbContext db, int iden) =>
{
    return db.Users.FirstOrDefault(x => x.Id == iden);
});

// View All Users
app.MapGet("/users", (HabitHubDbContext db) => {

    return db.Users.ToList();

});

// Get User's Habits
app.MapGet("/user/{userId}/habits", (HabitHubDbContext db, int userId) => {

    return db.Habits.Where(x => x.UserId == userId);
});

// Get Single Habit By User
app.MapGet("/api/singlehabitbyuser/{id}", async (HabitHubDbContext db, int id) =>
{
    try
    {
        var habit = await db.Habits
            .Include(h => h.Tags)  // Include Tags navigation property
            .FirstOrDefaultAsync(h => h.Id == id);

        if (habit == null)
        {
            return Results.NotFound("Habit not found");
        }

        return Results.Ok(habit);
    }
    catch (Exception ex)
    {
        // Log the exception for debugging purposes
        Console.WriteLine($"Error: {ex.Message}");

        return Results.Problem("Internal Server Error", statusCode: 500);
    }
});












// View All Habits
app.MapGet("/habits", (HabitHubDbContext db) => {

    return db.Habits.ToList();

});

// View a Habit
app.MapGet("/habit/{HabId}", (HabitHubDbContext db, int HabId) => {

    return db.Habits.FirstOrDefault(o => o.Id == HabId);

});

// Create a Habit
app.MapPost("/habits/new", (HabitHubDbContext db, Habit payload) => {

    db.Habits.Add(payload);
    db.SaveChanges();
    return Results.Created("/habits/new", payload);

});

// Update An Habit
app.MapPut("/habits/{HabId}/update", (HabitHubDbContext db, int HabId, Habit payload) => {

    Habit SelectedHabit = db.Habits.FirstOrDefault(o => o.Id == HabId);

    if (SelectedHabit == null)
    {
        return Results.NotFound("Habit was not found!");
    }

    SelectedHabit.Title = payload.Title;
    SelectedHabit.Description = payload.Description;
    SelectedHabit.ImageUrl = payload.ImageUrl;
    SelectedHabit.UserId = payload.UserId;

    db.SaveChanges();
    return Results.Ok("The existing Habit has been updated.");

});

// Delete a Habit
app.MapDelete("/habits/{HabId}/remove", (HabitHubDbContext db, int HabId) => {

    Habit SelectedHabit = db.Habits.FirstOrDefault(o => o.Id == HabId);

    db.Habits.Remove(SelectedHabit);
    db.SaveChanges();
    return Results.Ok("Habit has been removed.");

});


// Get All Tags
app.MapGet("/api/tags", (HabitHubDbContext db) =>
{

    return db.Tags.ToList();

});

//Create a Tag
app.MapPost("/api/tag", (HabitHubDbContext db, Tag tag) =>
{
    db.Tags.Add(tag);
    db.SaveChanges();
    return Results.Created($"/api/tag", tag);
});

//Add a Tag to a Habit
app.MapPost("/api/habit/taghabit/{HabId}/{tagId}", (HabitHubDbContext db, int HabId, int tagId) =>
{
    var habit = db.Habits.SingleOrDefault(s => s.Id == HabId);
    var tag = db.Tags.SingleOrDefault(g => g.Id == tagId);

    if (habit.Tags == null)
    {
        habit.Tags = new List<Tag>();
    }
    habit.Tags.Add(tag);
    db.SaveChanges();
    return habit;

});

//// Delete Tags from a Habit
//app.MapDelete("/habits/{HabId}/tags/{TagId}/remove", (HabitHubDbContext db, int HabId, int TagId) =>
//{
//    try
//    {
//        // Include should come first before selecting
//        var SingleHabit = db.Habits
//            .Include(Hab => Hab.TagList)
//            .FirstOrDefault(x => x.Id == HabId);
//        if (SingleHabit == null)
//        {
//            return Results.NotFound("Sorry for the inconvenience! This habit does not exist.");
//        }
//        // The reason why it didn't work before is because I didnt have a method after TagList
//        var SelectedTagList = SingleHabit.TagList.FirstOrDefault(t => t.Id == TagId);
//        SingleHabit.TagList.Remove(SelectedTagList);
//        db.SaveChanges();
//        return Results.Ok(SingleHabit.TagList);
//    }
//    catch (Exception ex)
//    {
//        return Results.Problem(ex.Message);
//    }
//});

//// Get Tags from a Habit
//app.MapGet("/habits/{HabId}/tags", (HabitHubDbContext db, int HabId) =>
//{
//    try
//    {
//        var SingleHabit = db.Habits
//            .Where(db => db.Id == HabId)
//            .Include(Hab => Hab.TagList)
//            .ToList();
//        if (SingleHabit == null)
//        {
//            return Results.NotFound("Sorry for the inconvenience! This Habit does not exist.");
//        }
//        return Results.Ok(SingleHabit);
//    }
//    catch (Exception ex)
//    {
//        return Results.Problem(ex.Message);
//    }
//});

app.Run();
