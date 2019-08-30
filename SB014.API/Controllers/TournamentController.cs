using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SB014.API.Models;
using System;
using SB014.API.DAL;
using SB014.API.BAL;
using SB014.API.Domain;

namespace SB014.API.Controllers
{
    [Route("api/tournament")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        
        private readonly ITournamentRepository TournamentRepository;
        private readonly IMapper Mapper;
        private readonly IGameLogic GameLogic;
        public TournamentController(ITournamentRepository tournamentRepository, IMapper mapper, IGameLogic gameLogic)
        {
            this.TournamentRepository = tournamentRepository;
            Mapper = mapper;
            GameLogic = gameLogic;
        }         

        [HttpGet]
        public IActionResult GetTournaments()
        {            
            return Ok(Mapper.Map<List<Tournament>,List<TournamentModel>>(this.TournamentRepository.GetAll()));
        }


        [HttpGet]
        [Route("{tournamentid}/subscriber/{id}", Name="TournamentSubscriber")]
        public IActionResult GetTournamentSubscriber(Guid tournamentid, Guid id)
        {
            Tournament tournament = this.TournamentRepository.Get(tournamentid);
            if(tournament == null)
            {
                return NotFound();
            }

            Subscriber subscriber = this.TournamentRepository.GetSubscriber(tournamentid, id);
            if(subscriber == null)
            {
                return NotFound();  
            }
            return Ok(Mapper.Map<Subscriber,SubscriberModel>(subscriber));
        }

        [HttpPost]
        [Route("{id}/subscriber")]
        public IActionResult SubscribeToTournament(Guid id , [FromBody] SubscribeToTournamentModel subscribeToTournamentModel)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest();          
            }
            Tournament tournament = this.TournamentRepository.Get(id);
            if(tournament == null)
            {
                return NotFound();
            }

            Subscriber subscriber = Mapper.Map<SubscribeToTournamentModel,Subscriber>(subscribeToTournamentModel);
            subscriber.TournamentId = id;            
            Subscriber newSubscriber = this.TournamentRepository.AddSubscriber(subscriber);
            SubscriberModel tournamentSubscriberModel = Mapper.Map<Subscriber, SubscriberModel>(newSubscriber);

            // If no game exists create one
            bool doesGameExist = this.TournamentRepository.HasGame(id);
            if(doesGameExist == false)
            {
                var newGame  = this.GameLogic.BuildGame(id, tournament.CluesPerGame);
                var game = this.TournamentRepository.UpdateGame(newGame);
            }            
            
            return CreatedAtRoute("TournamentSubscriber", new {
               tournamentid = id,
               id = newSubscriber.Id 
            },tournamentSubscriberModel);
        }
        
        [HttpDelete]
        [Route("{id}/subscriber/{subscriberId}")]
        public IActionResult UnsubscribeFromTournament(Guid id, Guid subscriberId)
        {
            Tournament tournament = this.TournamentRepository.Get(id);
            if(tournament == null)
            {
                return NotFound();
            }
            Subscriber subscriber = this.TournamentRepository.GetSubscriber(id, subscriberId);
            if(subscriber == null)
            {
                return NotFound();  
            }

            this.TournamentRepository.RemoveSubscriber(id, subscriberId);

            return NoContent();
        }
    }
}