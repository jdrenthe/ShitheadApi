using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShitheadApi.Utilities.Enums;

namespace ShitheadApi.Models.Entities
{
    public class Game
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int Deacks { get; set; }
    
        public TurnTime TurnTime { get; set; }

        public List<Player> Players { get; set; }

        public List<Card> Cards { get; set; }
    }
}
