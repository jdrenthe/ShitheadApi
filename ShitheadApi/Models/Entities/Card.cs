using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShitheadApi.Utilities.Enums;

namespace ShitheadApi.Models.Entities
{
    public class Card
    {
        [Key]
        public Guid Id { get; set; }

        public int Value { get; set; }

        public string Symbol { get; set; }

        public State State { get; set; }

        public Seven Seven { get; set; }

        public bool Excluded { get; set; }

        public int Order { get; set; }

        [ForeignKey("Player")]
        public Guid? PlayerId { get; set; }

        public Player Player { get; set; }

        [ForeignKey("Game")]
        public Guid GameId { get; set; }

        public Game Game { get; set; }
    }
}
