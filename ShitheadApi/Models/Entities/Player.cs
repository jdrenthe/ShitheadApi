using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShitheadApi.Utilities.Enums;

namespace ShitheadApi.Models.Entities
{
    public class Player
    {
        [Key]
        public Guid Id { get; set; }

        public bool Joint { get; set; }

        public bool SwitchedCards { get; set; }

        public int Position { get; set; }

        public Turn Turn { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public User User { get; set; }

        [ForeignKey("Game")]
        public Guid GameId { get; set; }

        public Game Game { get; set; }
    }
}
