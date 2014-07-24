using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class LeaguesOverviewModel
    {
        public IList<LeagueOverviewModel> Leagues { get; set; }
        public IList<LeagueOverviewModel> Tournaments { get; set; }
        public IList<LeagueOverviewModel> Scrimmages { get; set; }
        public IList<LeagueOverviewModel> Other { get; set; }
        public IList<LeagueOverviewModel> All { get; set; }
    }

    public class TeamPlayerDTO
    {
        public TeamPlayer TeamPlayer { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public static IList<TeamPlayerDTO> MapTeamPlayers(IList<TeamPlayer> players)
        {
            var model = new List<TeamPlayerDTO>();
            foreach (var item in players)
            {
                model.Add(new TeamPlayerDTO
                {
                    TeamPlayer = item,
                    Name = string.Format("{0}, {1}", item.Player.LastName, item.Player.FirstName),
                    Status = item.Status.ToString()
                });
            }
            return model;
        }
    }

    public class TeamCoachDTO
    {
        public Team Team { get; set; }
        public Coach Coach { get; set; }

        public static IList<TeamCoachDTO> MapTeamCoaches(IList<Team> teams)
        {
            var model = new List<TeamCoachDTO>();
            foreach (var item in teams)
            {
                foreach(var coach in item.Coaches)
                {
                    model.Add(new TeamCoachDTO
                    {
                        Team = item,
                        Coach = coach
                    });
                }
            }
            return model;
        }
    }

    public class LeagueOverviewModel
    {
        public League League { get; set; }
        public string Name { get; set; }
        public string EndDate { get; set; }
        public IList<Game> Games { get; set; }
        public IList<Game> UpcomingGames() { return Games.Where<Game>(g => g.GameDate >= DateTime.Now).ToList(); }
        public IList<Game> UpcomingGames(Division division) { return Games.Where<Game>(g => g.GameDate >= DateTime.Now && g.Division == division).ToList(); }
        public IList<LeagueGameModel> UpcomingGamesModel()
        {
            return MapGamesToModel(Games.Where<Game>(g => g.GameDate >= DateTime.Now).ToList());
        }
        public IList<LeagueGameModel> UpcomingGamesModel (Division division)
        {
            return MapGamesToModel(Games.Where<Game>(g => g.GameDate >= DateTime.Now && g.Division == division).ToList());
        }
        public IList<Game> PastGames() { return Games.Where<Game>(g => g.GameDate < DateTime.Now).ToList(); }
        public IList<Game> PastGames(Division division) { return Games.Where<Game>(g => g.GameDate < DateTime.Now && g.Division == division).ToList(); }
        public IList<LeagueGameModel> PastGamesModel()
        {
            return MapGamesToModel(Games.Where<Game>(g => g.GameDate < DateTime.Now).ToList());
        }
        public IList<LeagueGameModel> PastGamesModel(Division division)
        {
            return MapGamesToModel(Games.Where<Game>(g => g.GameDate < DateTime.Now && g.Division == division).ToList());
        }
        public IList<BracketOverviewModel> Brackets { get; set; }
        public IList<TeamRankingModel> Rankings { get; set; }

        public static IList<LeagueGameModel> MapGamesToModel(IList<Game> games)
        {
            var model = new List<LeagueGameModel>();
            foreach (var game in games)
            {
                string gameType = "Normal";
                if(game.Bracket != null) 
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
                var fullBracket = game.GetBracketGenerator();// Bracket.GetFullBracket(game.Bracket);
                model.Add(new LeagueGameModel
                {
                    Game = game,
                    HomeTeamName = Bracket.TeamNameFromBracket(fullBracket, game.HomeTeam, game, true, null, null),
                    AwayTeamName = Bracket.TeamNameFromBracket(fullBracket, game.AwayTeam, game, false, null, null),
                    HomeTeam = Bracket.TeamFromBracket(fullBracket, game.HomeTeam, game, true),
                    AwayTeam = Bracket.TeamFromBracket(fullBracket, game.AwayTeam, game, false),
                    GameTime = game.GameDate.ToString(),
                    Location = (game.Location != null ? game.Location.Name : null),
                    Type = gameType,
                    Field = game.Field,
                    PlateUmpire = Umpire.GetUmpireName(game.PlateUmpire),
                    FieldUmpire = Umpire.GetUmpireName(game.FieldUmpire)
                });
            }
            return model;
        }
    }

    public class LeagueGameModel
    {
        public Game Game { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public string GameTime { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string PlateUmpire { get; set; }
        public string FieldUmpire { get; set; }
        public string Field { get; set; }
    }

    public class GameOverviewModel
    {
        public string UniqueAjaxName { get; set; }
        public string ReturnUrl { get; set; }
        public IList<LeagueGameModel> LeagueGames { get; set; }
    }
    public class BracketOverviewModel
    {
        public Bracket Bracket { get; set; }
        public IList<Game> PoolGames { get; set; }
        public string ReturnUrl { get; set; }
        public IList<BracketGameViewModel> PoolGamesModel
        {
            get
            {
                return MapGamesToGameModel(PoolGames);
            }
        }

        public IList<Game> BracketGames { get; set; }
        public IList<BracketGameViewModel> BracketGamesModel
        {
            get
            {
                return MapGamesToGameModel(BracketGames);
            }
        }
        public IList<TeamRankingModel> PoolRankings { get; set; }
        public IList<TeamRankingModel> BracketRankings { get; set; }

        private static IList<BracketGameViewModel> MapGamesToGameModel(IList<Game> games)
        {
            var model = new List<BracketGameViewModel>();
            foreach (var game in games)
            {
                //var fullBracket = Bracket.GetFullBracket(game.Bracket);
                //var bracketGame = fullBracket.Where(myg => myg.Bracket == game.BracketBracket &&
                //        myg.Position == game.BracketPosition).FirstOrDefault();
                var bracketGame = game.GetBracketGenerator();
                int? gameNumber = bracketGame == null ? null : bracketGame.GameNumber;
                
                model.Add(new BracketGameViewModel
                {
                    Game = game,
                    HomeTeamName = Bracket.TeamNameFromBracket(bracketGame, game.HomeTeam, game, true, null, null),
                    AwayTeamName = Bracket.TeamNameFromBracket(bracketGame, game.AwayTeam, game, false, null, null),
                    HomeTeam = Bracket.TeamFromBracket(bracketGame, game.HomeTeam, game, true),
                    AwayTeam = Bracket.TeamFromBracket(bracketGame, game.AwayTeam, game, false),
                    HomeTeamScore = game.HomeTeamScore,
                    AwayTeamScore = game.AwayTeamScore,
                    GameNumber = gameNumber
                });
            }
            return model;
        }
    }

    public class BracketGameViewModel
    {
        public Game Game { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public int? GameNumber { get; set; }
    }

    public class TeamRankingModel
    {
        public int Ranking { get; set; }
        public Team Team { get; set; }
        public string TeamName
        {
            get
            {
                return string.Format("{0} {1} {2}", Team.Name, Team.Division.Name, Team.Class.Name);
            }
        }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public int RunsAllowed { get; set; }
        public int RunsScored { get; set; }
        public int RunDifferential { get; set; }
        public int RunDifferentialMax8 { get; set; }
        public int Games { get; set; }
        public int MatchingRankLosses { get; set; }
        public int MatchingRankWins { get; set; }
        public IList<Game> TeamGames { get; set; } 
    }

    public class TeamOverviewModel
    {
        public League League { get; set; }
        public Team Team { get; set; }
        public IList<TeamPlayerDTO> Players { get; set; }
        public IList<GameDTO> Games { get; set; }
    }

    public class PlayerOverviewModel
    {
        public League League { get; set; }
        public IList<Player> Players { get; set; }
    }

    public class PitchOverviewModel
    {
        public League League { get; set; }
        public IList<TeamPlayerDTO> Players { get; set; }
    }

    public class CoachOverviewModel
    {
        public League League { get; set; }
        public IList<TeamCoachDTO> Coaches { get; set; }
    }

    public class ContactFormModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
    }

    public class LocationOverviewModel
    {
        [DisplayName("Location Name")]
        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Url { get; set; }

        public string Notes { get; set; }

        [DisplayName("Phone Number")]
        public string GroundsKeeperPhone { get; set; }
    }

    public class UmpireOverviewModel
    {
        public IList<Game> Games { get; set; }

        public Umpire Umpire { get; set; }
    }
}