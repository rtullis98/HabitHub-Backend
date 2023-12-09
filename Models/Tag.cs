using HabitHub_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HabitHub_Backend.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public string ?Name { get; set; }

        public List<Habit> HabitList { get; set; }

    }
}
