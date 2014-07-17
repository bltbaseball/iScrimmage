using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    /// <summary>
    /// A match between two opposing teams.
    /// </summary>
    public class GameNewModel
    {
        [Required]
        [DisplayName("Home Team")]
        public int HomeTeamId { get; set; }

        [Required]
        [DisplayName("Away Team")]
        public int AwayTeamId { get; set; }

        [Required]
        [DisplayName("Game Date")]
        public string GameDate { get; set; }

        [Required]
        [DisplayName("Game Time")]
        public string GameTime { get; set; }

        [Required]
        [DisplayName("Location")]
        public int LocationId { get; set; }

        [Required]
        [DisplayName("Innings")]
        public int? Innings { get; set; }

        public int CreatedById { get; set; }
        
        public DateTime CreatedOn { get; set; }

        [DisplayName("Umpire Fee")]
        public int UmpireFeeId { get; set; }

        public GameStatus Status { get; set; }

        [DisplayName("Field Name/Number")]
        public string Field { get; set; }
    
        public int LeagueId { get; set; }
    }

    public class GameMassModel
    {
        [Required]
        [DisplayName("League/Tournament")]
        public int LeagueId { get; set; }

        [Required]
        [DisplayName("Division")]
        public int DivisionId { get; set; }

        public IList<Team> Teams { get; set; }

        public IList<GameMassNewGameModel> Games { get; set; }
    }

    public class GameMassNewGameModel
    {
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public string GameDate { get; set; }
        public string GameTime { get; set; }
        public int LocationId { get; set; }
        public string Field { get; set; }
    }

    public class GameChallengeNewGameModel
    {
        [DisplayName("Home Team")]
        public int HomeTeamId { get; set; }

        [DisplayName("Away Team")]
        public int AwayTeamId { get; set; }

        [DisplayName("Date")]
        public string GameDate { get; set; }

        [DisplayName("Time")]
        public string GameTime { get; set; }

        [DisplayName("Available Locations")]
        [Required]
        public int LocationId { get; set; }

        public AvailableDates AvailableDate { get; set; }
    }

    public class GameUpdateModel : GameNewModel
    {
        public int Id { get; set; }
        public string ReturnUrl { get; set; }
        
        [DisplayName("Notify Teams Of Changes")]
        public string NotifyTeams { get; set; }

        public Game Game { get; set; }
    }

    public class GameListModel
    {
        public UserRole UserRole { get; set; }
        public IList<TeamGameModel> TeamGames { get; set; }
        public IList<Game> Games { get; set; }
        public IList<GameInfoModel> GamesInfo { get; set; }
    }

    public class GameInfoModel
    {
        public Game Game { get; set; }
        // we need to supply the columns for classes that might be nullable
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public string Type { get; set; }
    }

    public class AvailableDateResult
    {
        public AvailableDates AvailableDate { get; set; }
        public double Distance { get; set; }
    }

    public class AvailableLocationResult
    {
        public Location Location { get; set; }
        public double Distance { get; set; }
    }

    public class TeamAvailableDates
    {
        public int TeamId { get; set; }
        public IList<AvailableDateResultDTO> AvailableDates { get; set; }
        public String TableHeader { get; set; }
    }

    public class AvailableDateResultDTO
    {
        public AvailableDateResult AvailableDateResult { get; set; }
        public int Id { get; set; }
        public string Status { get; set; }
        public Team Team { get; set; }
        public string TeamName { get; set; }
        public DateTime Date { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public int? DistanceFromLocation { get; set; }
        public Game GameScheduled { get; set; }
        public double Distance { get; set; }

        public static IList<AvailableDateResultDTO> MapAvailableDateResults(IList<AvailableDateResult> items)
        {
            var model = new List<AvailableDateResultDTO>();
            foreach (var item in items)
            {
                model.Add(MapAvailableDateResult(item));
            }
            return model;
        }

        public static AvailableDateResultDTO MapAvailableDateResult(AvailableDateResult item)
        {
            //return new AvailableDateResultDTO
            //{
            //    AvailableDate = AvailableDatesDTO.MapAvailableDate(item.AvailableDate),
            //    Distance = item.Distance
            //};
            var scheduled = AvailableDates.GetGameScheduled(item.AvailableDate);
            return new AvailableDateResultDTO
            {
                Id = item.AvailableDate.Id,
                AvailableDateResult = item,
                Date = item.AvailableDate.Date,
                GameScheduled = scheduled,
                LocationId = item.AvailableDate.Location.Id,
                Location = item.AvailableDate.Location.Name,
                Team = item.AvailableDate.Team,
                TeamName =  Web.Models.Team.PrettyName(item.AvailableDate.Team),
                Type = (item.AvailableDate.IsHome && item.AvailableDate.IsAway) ? "Home Or Away" : (item.AvailableDate.IsHome ? "Home" : "Away"),
                DistanceFromLocation = item.AvailableDate.DistanceFromLocation,
                Status = (scheduled == null ? "Available" : scheduled.Status.ToString()),
                Distance = item.Distance
            };
        }
    }

    public class TeamGameModel
    {
        public Team Team { get; set; }
        public IList<Game> Games { get; set; }
        public IList<GameInfoModel> GamesInfo { get; set; }
        public TeamAvailableDates AvailableDates { get; set; }
    }


    public class GameRequestModel
    {
        [DisplayName("Home Team")]
        [Required]
        public int HomeTeamId { get; set; }

        [DisplayName("Away Team")]
        [Required]
        public int AwayTeamId { get; set; }

        [Required]
        [DisplayName("Date and Location")]
        public int AvailableDateId { get; set; }
    }

    public class GameSpecificRequestModel
    {
        [DisplayName("Home Team")]
        [Required]
        public int HomeTeamId { get; set; }

        [DisplayName("Away Team")]
        [Required]
        public int AwayTeamId { get; set; }

        [Required]
        [DisplayName("Game Date")]
        public string GameDate { get; set; }

        [Required]
        [DisplayName("Game Time")]
        public string GameTime { get; set; }

        [Required]
        [DisplayName("Location")]
        public int LocationId { get; set; }
    }

    public class GameUpdateUmpireModel
    {
        public int Id { get; set; }

        [DisplayName("Plate Umpire")]
        public int PlateUmpireId { get; set; }

        [DisplayName("Field Umpire")]
        public int FieldUmpireId { get; set; }
    }

    public class GameUpdateScoreModel
    {
        public int Id { get; set; }

        [DisplayName("Home Team Score")]
        public int? HomeTeamScore { get; set; }

        [DisplayName("Away Team Score")]
        public int? AwayTeamScore { get; set; }

        public GameStatus Status { get; set; }

        public Game Game { get; set; }

        public IList<GameUpdatePlayerStatModel> HomePlayerStats { get; set; }
        public IList<GameUpdatePlayerStatModel> AwayPlayerStats { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class GameUpdatePlayerStatModel
    {
        public TeamPlayer Player { get; set; }
        public int PlayerId { get; set; }
        public double? InningsPitched { get; set; }
        public double? InningsOuts { get; set; }
        public double? PitchesThrown { get; set; }        
    }
}