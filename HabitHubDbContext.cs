using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HabitHub_Backend.Models;

namespace HabitHub_Backend
{
    public class HabitHubDbContext : DbContext
    {
        public DbSet<Habit> Habits { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        public HabitHubDbContext(DbContextOptions<HabitHubDbContext> context) : base(context)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasData(new User[]
            {
            new User {Id = 1, Name="Riley Tullis", Bio="The greatest person you'll ever meet.", PhoneNumber="123-456-7890", ImageUrl="https://th.bing.com/th/id/R.f08431063da214d8c07452cca215447f?rik=7gKQvCXgiLVQXw&pid=ImgRaw&r=0", Email="riley@email.com", Uid="" },
            new User {Id = 2, Name="Jovanni Feliz", Bio="Eh, he's ok.", PhoneNumber="098-765-4321", ImageUrl="https://th.bing.com/th/id/R.e733fb390ae9a3c28ca2389bd2466be7?rik=tGXnQ7Yf6T1kBQ&pid=ImgRaw&r=0", Email="jovanni@email.com", Uid="",}

            });

            modelBuilder.Entity<Habit>().HasData(new Habit[]
           {
            new Habit {Id = 1, Title="Work out more", Description="I would like to work out more.", ImageUrl="https://th.bing.com/th/id/R.1c9c3d67b55baf0ed910b39c5f833d06?rik=QmmXTGBc19MRUQ&pid=ImgRaw&r=0", UserId=1},
            new Habit {Id = 2, Title="Sleep better", Description="I would like to get 8 hours of sleep a night.", ImageUrl="https://static0.reviewthisimages.com/wordpress/wp-content/uploads/2019/10/sleeping.jpg", UserId=2},

           });

            modelBuilder.Entity<Tag>().HasData(new Tag[]
           {
            new Tag {Id = 1, Name="Easy"},
            new Tag {Id = 2, Name="Slightly difficult"},
            new Tag {Id = 3, Name="Challenging"},
            new Tag {Id = 4, Name="Extremely Challenging"},

           });
        }
    }
};
