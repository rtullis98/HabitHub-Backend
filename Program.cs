using HabitHub_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using HabitHub_Backend;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
app.MapGet("/users/{uid}", (HabitHubDbContext db, string uid) =>
{
    var user = db.Users.Where(x => x.Uid == uid).ToList();
    if (uid == null)
    {
        return Results.NotFound("Sorry, User not found!");
    }
    else
    {
        return Results.Ok(user);
    }
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


// View All Tags
app.MapGet("/tags", (HabitHubDbContext db) => {

    return db.Tags.ToList();

});

// Add a Tag to a Habit
app.MapPost("/habits/{HabId}/tags/new", (HabitHubDbContext db, int HabId, int tId) =>
{
    // Retrieve object reference of Habits in order to manipulate (Not a query result)
    var hab = db.Habits
    .Where(o => o.Id == HabId)
    .Include(o => o.TagList)
    .FirstOrDefault();

    var SelectedTag = db.Tags
    .Where(db => db.Id == tId)
    .FirstOrDefault();

    if (hab == null)
    {
        return Results.NotFound("Habit not found.");
    }
    hab.TagList.Add(SelectedTag);
    db.SaveChanges();
    return Results.Ok(hab);
});

// Delete Tags from a Habit
app.MapDelete("/habits/{HabId}/tags/{TagId}/remove", (HabitHubDbContext db, int HabId, int TagId) =>
{
    try
    {
        // Include should come first before selecting
        var SingleHabit = db.Habits
            .Include(Hab => Hab.TagList)
            .FirstOrDefault(x => x.Id == HabId);
        if (SingleHabit == null)
        {
            return Results.NotFound("Sorry for the inconvenience! This habit does not exist.");
        }
        // The reason why it didn't work before is because I didnt have a method after TagList
        var SelectedTagList = SingleHabit.TagList.FirstOrDefault(t => t.Id == TagId);
        SingleHabit.TagList.Remove(SelectedTagList);
        db.SaveChanges();
        return Results.Ok(SingleHabit.TagList);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Get Tags from a Habit
app.MapGet("/habits/{HabId}/tags", (HabitHubDbContext db, int HabId) =>
{
    try
    {
        var SingleHabit = db.Habits
            .Where(db => db.Id == HabId)
            .Include(Hab => Hab.TagList)
            .ToList();
        if (SingleHabit == null)
        {
            return Results.NotFound("Sorry for the inconvenience! This Habit does not exist.");
        }
        return Results.Ok(SingleHabit);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();
