using eGym.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace eGym.Models
{
    public class OpinionModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public int GymId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? OpinionDate { get; set; }

    }
}


