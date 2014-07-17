using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class BracketResult
    {
        public virtual int Id { get; set; }
        public virtual int Team1 { get; set; }
        public virtual int Team2 { get; set; }
        public virtual int Bracket { get; set; }
        public virtual int Position { get; set; }
        public virtual int? GameNumber { get; set; }
        public virtual BracketGenerator ConvertToBracketGenerator()
        {
            var bracketGenerator = new BracketGenerator();
            bracketGenerator.Sequence = this.Id;
            bracketGenerator.BracketBracket = this.Bracket;
            bracketGenerator.BracketPosition = this.Position;
            bracketGenerator.GameNumber = this.GameNumber;
            bracketGenerator.Team1 = this.Team1;
            bracketGenerator.Team2 = this.Team2;
            return bracketGenerator;
        }
    }

    public class BracketsListModel
    {
        public List<Bracket> Brackets { get; set; }
    }

    public class BracketInfoModel
    {
        public Bracket Bracket { get; set; }
        public IList<BracketGameModel> Games { get; set; }
    }

    public class BracketGamesModel
    {
        public int Id { get; set; }

        public BracketInfoModel Info { get; set; }

        public List<BracketGameModel> Games { get; set; }

        public IList<GameUpdateModel> PoolGames { get; set; }
    }

    public class BracketNewGameModel
    {
        public Team Team1 { get; set; }
        public int? Team1Seed { get; set; }
        public Team Team2 { get; set; }
        public int? Team2Seed { get; set; }
        public int Bracket { get; set; }
        public int Position { get; set; }
        public string GameDate { get; set; }
        public string GameTime { get; set; }
        [Required]
        [DisplayName("Location")]
        public int LocationId { get; set; }
        public string Field { get; set; }
        public int? GameNumber { get; set; }
    }

    public class BracketGameModel
    {
        public int Id { get; set; }
        public Game Game { get; set; }
        public Team Team1 { get; set; }
        public bool IsTeam1Bye { get; set; }
        public int? Team1Seed { get; set; }
        public Team Team2 { get; set; }
        public bool IsTeam2Bye { get; set; }
        public int? Team2Seed { get; set; }
        public int Bracket { get; set; }
        public int Position { get; set; }
        public string GameDate { get; set; }
        public string GameTime { get; set; }
        [Required]
        [DisplayName("Location")]
        public int LocationId { get; set; }
        public string Field { get; set; }
        public int? GameNumber { get; set; }
        public Team Winner { get; set; }
        public int? WinnerSeed { get; set; }
    }

    public class BracketPoolGameModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("League/Tournament")]
        public int LeagueId { get; set; }

        [Required]
        [DisplayName("Division")]
        public int DivisionId { get; set; }

        public IList<Team> Teams { get; set; }

        public IList<GameMassNewGameModel> Games { get; set; }
    }

    public class BracketNewModel
    {
        [Required]
        public string Name { get; set; }

        [DisplayName("League")]
        [Required]
        public int LeagueId { get; set; }

        public IList<BracketTeamModel> Teams { get; set; }
    }

    public class BracketSubmitPoolModel
    {
        public Bracket Bracket { get; set; }

        public IList<TeamRankingModel> PoolRankings { get; set; }
        public IList<BracketTeamModel> Teams { get; set; }
    }

    public class BracketTeamModel
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public int Standing { get; set; }
    }

    public class BracketUpdateModel : BracketNewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}