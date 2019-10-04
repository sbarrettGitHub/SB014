using System;
using System.Collections.Generic;
using SB014.API.DAL;
using SB014.API.Domain;
using SB014.API.Helpers;

namespace SB014.API.BAL
{
    public class GameLogic : IGameLogic
    {
        
        private readonly IWordRepository WordRepository; 
        private readonly IDateTimeHelper DateTimeHelper; 
        public GameLogic(IWordRepository wordRepository, IDateTimeHelper dateTimeHelper)
        {
            WordRepository = wordRepository;
            DateTimeHelper = dateTimeHelper;
        }

        public Game BuildGame(Guid tournamentId, int cluesPerGame)
        {
            IEnumerable<string> unscrambledWords = this.WordRepository.GetWords(cluesPerGame);
            List<Clue> anagrams = new List<Clue>();
            
            // Scramble words into anangrams
            foreach (string word in unscrambledWords)
            {
                anagrams.Add(new Clue
                { 
                    ClueId = Guid.NewGuid(),
                    GameClue = this.Scramble(word),
                    Answer = word
                });
            }
            
            return new Game{
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                Clues = anagrams,
                Created = this.DateTimeHelper.CurrentDateTime
            };
        }

        private string Scramble(string word)
        {
            char[] chars = new char[word.Length]; 
            Random rand = new Random(10000); 
            int index = 0; 
            while (word.Length > 0) 
            { 
                // Get a random number between 0 and the length of the word. 
                // Take the character from the random position 
                int next = rand.Next(0, word.Length - 1); 
                
                //and add to our char array. 
                chars[index] = word[next];                
                
                // Remove the character from the word. 
                word = word.Substring(0, next) + word.Substring(next + 1); 
                ++index; 
            } 
            return new String(chars); 
        }
    }

}