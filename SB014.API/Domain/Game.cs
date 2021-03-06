using System;
using System.Collections.Generic;

namespace SB014.API.Domain
{
    public class Game
    {
        public Game()
        {
            Clues = new List<Clue>();
        }
        public Guid Id { get; set; }
        public Guid TournamentId { get; set; }

        public DateTime Created { get; set; }
        
        public List<Clue> Clues { get; set; }
    }
}