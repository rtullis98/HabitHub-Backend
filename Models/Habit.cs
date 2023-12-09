using HabitHub_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HabitHub_Backend.Models
{
    public class Habit
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int UserId { get; set; }

        public List<Tag> Tags { get; set; }

    }
}
