﻿using HabitHub_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HabitHub_Backend.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Bio { get; set; }

        public string PhoneNumber { get; set; }

        public string ImageUrl { get; set; }

        public string Uid { get; set; }
        public List<Habit> HabitList { get; set; }
    }
}
