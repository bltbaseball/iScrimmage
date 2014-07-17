using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class GuardianProfileModel
    {
        public Guardian Guardian { get; set; }
        public IList<PlayerProfileModel> Players { get; set; }
        public IList<TeamDTO> TeamsLookingForPlayers { get; set; }
    }

    public class CoachProfileModel
    {
        public Coach Coach { get; set; }
        public IList<TeamDTO> TeamsActive { get; set; }
        public IList<TeamDTO> TeamsInactive { get; set; }
        //public IList<GameDTO> Games { get; set; }
        public IList<GameDTO> PastGames { get; set; }
        public IList<GameDTO> UpcomingGames { get; set; }
        public IList<AvailableDatesDTO> AvailableDates { get; set; }
     }

    public class ContactModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Type { get; set; }
    }

    public class GameDTO
    {
        public Game Game { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public string GameTime { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string PlateUmpire { get; set; }
        public string FieldUmpire { get; set; }

        public static IList<GameDTO> MapGamesToModel(IList<Game> games)
        {
            var model = new List<GameDTO>();
            foreach (var game in games)
            {
                string gameType = "Normal";
                if (game.Bracket != null)
                {
                    if (game.BracketBracket != null)
                    {
                        gameType = "Bracket";
                    }
                    else
                    {
                        gameType = "Pool";
                    }
                    gameType += string.Format(" {0} - {1}", game.Bracket.Name, game.Bracket.Division.Name);
                }
                var bracketGenerator = game.GetBracketGenerator();
                model.Add(new GameDTO
                {
                    Game = game,
                    HomeTeamName = Bracket.TeamNameFromBracket(bracketGenerator, game.HomeTeam, game, true, null, null),
                    AwayTeamName = Bracket.TeamNameFromBracket(bracketGenerator, game.AwayTeam, game, false, null, null),
                    HomeTeam = Bracket.TeamFromBracket(bracketGenerator, game.HomeTeam, game, true),
                    AwayTeam = Bracket.TeamFromBracket(bracketGenerator, game.AwayTeam, game, false),
                    GameTime = game.GameDate.ToString(),
                    Location = (game.Location != null ? game.Location.Name : null),
                    Type = gameType,
                    Status = game.Status.ToString(),
                    PlateUmpire = Umpire.GetUmpireName(game.PlateUmpire),
                    FieldUmpire = Umpire.GetUmpireName(game.FieldUmpire)
                });
            }
            return model;
        }
    }

    public class TeamDTO
    {
        public Team Team { get; set; }
        public string Name { get; set; }
        public string League { get; set; }
        public string Division { get; set; }
        public string Class { get; set; }

        public static IList<TeamDTO> MapTeamsToModel(IList<Team> teams)
        {
            var model = new List<TeamDTO>();
            foreach (var item in teams)
            {
                model.Add(new TeamDTO()
                {
                    Team = item,
                    Name = string.Format("{0} {1} {2}", item.Name, item.Division.Name, item.Class.Name),
                    League = item.League.Name,
                    Division = item.Division.Name,
                    Class = item.Class.Name
                });
            }
            return model;
        }
    }

    public class AvailableDatesDTO
    {
        public AvailableDates AvailableDates { get; set; }
        public int Id { get; set; }
        public string Status { get; set; }
        public Team Team { get; set; }
        public DateTime Date { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public int? DistanceFromLocation { get; set; }
        public Game GameScheduled { get; set; }

        public static IList<AvailableDatesDTO> MapAvailableDates(IList<AvailableDates> dates)
        {
            var model = new List<AvailableDatesDTO>();
            foreach (var item in dates)
            {
                model.Add(MapAvailableDate(item));
            }
            return model;
        }

        public static AvailableDatesDTO MapAvailableDate(AvailableDates item)
        {
            var scheduled = AvailableDates.GetGameScheduled(item);
            return new AvailableDatesDTO
            {
                Id = item.Id,
                AvailableDates = item,
                Date = item.Date,
                GameScheduled = scheduled,
                LocationId = item.Location.Id,
                Location = item.Location.Name,
                Team = item.Team,
                Type = (item.IsHome && item.IsAway) ? "Home Or Away" : (item.IsHome ? "Home" : "Away"),
                DistanceFromLocation = item.DistanceFromLocation,
                Status = (scheduled == null ? "Available" : scheduled.Status.ToString())
            };
        }
    }   

    //public class PlayerProfileModel
    //{
    //    public Player Player { get; set; }
    //    public IList<TeamPlayer> TeamsActive { get; set; }
    //    public IList<TeamPlayer> TeamsInactive { get; set; }
    //    public IList<Game> Games { get; set; }
    //}

    public class PlayerProfileModel
    {
        public Player Player { get; set; }
        public IList<TeamPlayer> TeamsActive { get; set; }
        public IList<TeamPlayer> TeamsInactive { get; set; }
        public IList<Game> Games { get; set; }
        public IList<TeamDTO> TeamsLookingForPlayers {
            get { return TeamDTO.MapTeamsToModel(Team.GetActiveTeamsLookingForPlayers(this.Player)); }
        }
    }
}