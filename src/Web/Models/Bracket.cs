using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class Bracket
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual League League { get; set; }
        public virtual Division Division { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual IList<BracketTeam> Standings { get; set; }
        public virtual IList<BracketGenerator> BracketGenerator { get; set; }

        public virtual string FullName()
        {
            return Name + " " + Division.Name;
        }
        public virtual string FullLongName()
        {
            return Name + " " + Division.Name + "-" + League.Name;
        }

        public static IList<Bracket> GetAllBrackets()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.Bracket>().List();
        }

        public static IList<Bracket> GetBracketsForLeague(League league)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.Bracket>().Where(r => r.League == league).List();
        }

        public static Bracket GetBracketById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<Bracket>(id);
        }

        /*public static IList<BracketResult> GetFullBracket(Bracket bracket)
        {
            if (bracket == null)
                return null;
            var session = MvcApplication.SessionFactory.OpenStatelessSession();
            var results = session.GetNamedQuery("BracketCreate")
                .SetParameter("BracketId", bracket.Id)
                .List<BracketResult>();
            session.Close();
            return results;
        }*/
        public static void DeleteBracket(Bracket bracket)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();

            var games = Game.GetGamesForBracket(bracket);

            /*if (games.Count > 0)
            {
                throw new ApplicationException("The bracket cannot be deleted because it has games associated with it.");
            }*/
            using (var tx = session.BeginTransaction())
            {
                foreach (var game in games)
                {
                    session.Delete(game);
                }
                session.Delete(bracket);
                tx.Commit();
            }
        }

        public static string TeamNameFromBracket(IList<BracketResult> brackets, Team team, Game game, bool isTeam1, string leader = "", string tail = "")
        {
            if (game.Bracket == null)
            {
                //Regular Game
                return leader + Team.PrettyNameWithoutLeague(team) + tail;
            }
            if (game.BracketBracket == null)
            {
                //Pool Game Game
                return leader + Team.PrettyNameWithoutLeague(team) + tail;
            }
            //else is Bracket Game

            if (brackets == null)
                return "";
            var bracket = brackets.Where<BracketResult>(c => c.Bracket == game.BracketBracket && c.Position == game.BracketPosition).FirstOrDefault<BracketResult>();
            if (bracket == null)
                return "";
            return TeamNameFromBracket(bracket.ConvertToBracketGenerator(), team, game, isTeam1, leader, tail);
        }
        public static string TeamNameFromBracket(BracketGenerator bracketGenerator, Team team, Game game, bool isTeam1, string leader = "", string tail = "")
        {
            if (game.Bracket == null)
            {
                //Regular Game
                return leader + Team.PrettyNameWithoutLeague(team) + tail;
            }
            if(game.BracketBracket == null)
            {
                //Pool Game Game
                return leader + Team.PrettyNameWithoutLeague(team) + tail;
            }
            //else is Bracket Game

            var bracketResultsGame = Bracket.PopulateBracketInfo(game.Bracket).Games.Where(i=>i.Game == game).FirstOrDefault();

            var bracket = bracketGenerator;
            if (bracket != null)
            {
                var myTeamBracket = BracketTeam.GetRankForTeam(bracket.Sequence.Value, team);
                if (myTeamBracket == null)
                {
                    if (isTeam1)
                    {
                        if (bracket.Team1 == -1)
                            return "No opponent";
                        else if (bracket.Team1 == -2)
                            return "TBD";
                        else if (bracketResultsGame.Team1 != null)
                            return Team.PrettyNameWithoutLeague(bracketResultsGame.Team1);
                        else
                            return "Seed #" + bracket.Team1.ToString();
                    }
                    else
                    {
                        if (bracket.Team2 == -1)
                            return "No opponent";
                        else if (bracket.Team2 == -2)
                            return "TBD";
                        else if (bracketResultsGame.Team2 != null)
                            return Team.PrettyNameWithoutLeague(bracketResultsGame.Team2);
                        else
                            return "Seed #" + bracket.Team2.ToString();
                    }
                }
                else
                {
                    var teamPosition = myTeamBracket.Standing;
                    return leader + Team.PrettyNameWithoutLeague(team) + tail;
                }
            }
            else
            {
                return "";
            }
        }

        public static Team TeamFromBracket(IList<BracketResult> brackets, Team team, Game game, bool isTeam1)
        {
            if (game.Bracket == null)
            {
                //Regular Game
                return team;
            }
            if (game.BracketBracket == null)
            {
                //Pool Game Game
                return team;
            }
            //else is Bracket Game

            if (brackets == null)
                return null;
            var bracket = brackets.Where<BracketResult>(c => c.Bracket == game.BracketBracket && c.Position == game.BracketPosition).FirstOrDefault<BracketResult>();
            if (bracket == null)
                return null;
            return TeamFromBracket(bracket.ConvertToBracketGenerator(), team, game, isTeam1);
        }

        public static Team TeamFromBracket(BracketGenerator bracketGenerator, Team team, Game game, bool isTeam1)
        {
            if (game.Bracket == null)
            {
                //Regular Game
                return team;
            }
            if (game.BracketBracket == null)
            {
                //Pool Game Game
                return team;
            }
            //else is Bracket Game

            var bracketResultsGame = Bracket.PopulateBracketInfo(game.Bracket).Games.Where(i => i.Game == game).FirstOrDefault();

            var bracket = bracketGenerator;
            if (bracket != null)
            {
                var myTeamBracket = BracketTeam.GetRankForTeam(bracket.Sequence.Value, team);
                if (myTeamBracket == null)
                {
                    if (isTeam1)
                    {
                        if (bracket.Team1 == -1)
                            return null;
                        else if (bracket.Team1 == -2)
                            return null;
                        else if (bracketResultsGame.Team1 != null)
                            return bracketResultsGame.Team1;
                        else
                            return null;
                    }
                    else
                    {
                        if (bracket.Team2 == -1)
                            return null;
                        else if (bracket.Team2 == -2)
                            return null;
                        else if (bracketResultsGame.Team2 != null)
                            return bracketResultsGame.Team2;
                        else
                            return null;
                    }
                }
                else
                {
                    var teamPosition = myTeamBracket.Standing;
                    return team;
                }
            }
            else
            {
                return null;
            }
        }

        public static BracketOverviewModel PopulateBracketOverview(Bracket bracket)
        {
            var bracketOverview = new BracketOverviewModel();
            bracketOverview.Bracket = bracket;
            var bracketGames = Game.GetGamesForBracket(bracket);
            bracketOverview.PoolGames = bracketGames.Where(g => g.BracketBracket == null).ToList();
            bracketOverview.BracketGames = bracketGames.Where(g => g.BracketBracket != null).ToList();
            bracketOverview.PoolRankings = Bracket.Rankings(bracket, true);
            bracketOverview.BracketRankings = Bracket.Rankings(bracket, false);
            return bracketOverview;
        }

        public virtual IList<BracketGenerator> GetBracketGenerator()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.BracketGenerator>()
                .Where(b => b.Bracket.Id == this.Id)
                .List();
        }

        public static BracketInfoModel PopulateBracketInfo(Bracket bracket)
        {
            var items = bracket.GetBracketGenerator(); // Bracket.GetFullBracket(bracket);

            var model = new BracketInfoModel();
            model.Bracket = bracket;
            var brackets = new List<BracketGameModel>();
            var bracketTeams = BracketTeam.GetBracketTeams(bracket);
            //int GameNumber = 1;
            foreach (var item in items)
            {
                var bracketGame = new BracketGameModel();
                bracketGame.Team1Seed = null;
                bracketGame.Team2Seed = null;
                bracketGame.Id = item.Sequence.Value;
                if (item.Team1 == -1)
                {
                    bracketGame.IsTeam1Bye = true;
                }
                else if (item.Team1 != -2)
                {
                    // get team by standing
                    var team1 = bracketTeams.Where(i => i.Standing == item.Team1).SingleOrDefault();
                    if (team1 != null)
                        bracketGame.Team1 = team1.Team;
                    if (item.Team1 > 0)
                        bracketGame.Team1Seed = item.Team1;
                }
                if (item.Team2 == -1)
                {
                    bracketGame.IsTeam2Bye = true;
                }
                else if (item.Team2 != -2)
                {
                    var team2 = bracketTeams.Where(i => i.Standing == item.Team2).SingleOrDefault();
                    if (team2 != null)
                        bracketGame.Team2 = team2.Team;
                    if (item.Team2 > 0)
                        bracketGame.Team2Seed = item.Team2;
                }
                bracketGame.Game = Game.GetGameByBracket(bracket, item.BracketBracket, item.BracketPosition);
                if (bracketGame.Game != null)
                {
                    bracketGame.Field = bracketGame.Game.Field;
                    bracketGame.LocationId = bracketGame.Game.Location.Id;
                    bracketGame.GameDate = bracketGame.Game.GameDate.ToShortDateString();
                    bracketGame.GameTime = bracketGame.Game.GameDate.ToString("h:mmtt");
                    if (bracketGame.Game.HomeTeamScore.HasValue && bracketGame.Game.AwayTeamScore.HasValue)
                    {
                        bracketGame.Winner = bracketGame.Game.HomeTeamScore.Value > bracketGame.Game.AwayTeamScore.Value ? bracketGame.Team1 : bracketGame.Team2;
                        bracketGame.WinnerSeed = bracketGame.Game.HomeTeamScore.Value > bracketGame.Game.AwayTeamScore.Value ? bracketGame.Team1Seed : bracketGame.Team2Seed;
                    }
                }
                bracketGame.Bracket = item.BracketBracket;
                bracketGame.Position = item.BracketPosition;
                //if (!bracketGame.IsTeam1Bye && !bracketGame.IsTeam2Bye)
                //    bracketGame.GameNumber = GameNumber++;
                bracketGame.GameNumber = item.GameNumber;
                brackets.Add(bracketGame);
                 
            }
            model.Games = brackets;
            return model;
        }


        public static IList<TeamRankingModel> Rankings(Bracket bracket, bool isPool)
        {
            var bracketGames = Game.GetGamesForBracket(bracket);
            var teams = bracket.League.Teams.Where(t => t.Division == bracket.Division).Distinct().ToList();

            //IList<Team> teams = ((from g in bracketGames select g.HomeTeam).Concat(from g in bracketGames select g.AwayTeam)).ToList();
            //teams = teams.AsEnumerable().GroupBy(t => t.Id).Select(t => t.First()).ToList();
            var rankings = new List<TeamRankingModel>();
            var games = new List<Game>();
            
            foreach (var team in teams)
            {
                var teamGames = bracketGames.Where(t => t.HomeTeam == team || t.AwayTeam == team).ToList();
                if (isPool)
                    teamGames = teamGames.Where(g => g.BracketBracket == null).ToList();
                else
                    teamGames = teamGames.Where(g => g.BracketBracket != null).ToList();

                //var teamGames = Game.GetGamesForTeam(team);
                
                games.AddRange(teamGames);

                // todo: include homeforfeit and awayforfeit in win/loss calculation
                var ranking = new TeamRankingModel();
                ranking.Team = team;
                ranking.TeamGames = teamGames;
                ranking.Wins = teamGames.Where(g => (((g.HomeTeam == team) && (g.HomeTeamScore > g.AwayTeamScore)) || ((g.AwayTeam == team) && (g.AwayTeamScore > g.HomeTeamScore))) && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
                ranking.Losses = teamGames.Where(g => (((g.HomeTeam == team) && (g.HomeTeamScore < g.AwayTeamScore)) || ((g.AwayTeam == team) && (g.AwayTeamScore < g.HomeTeamScore))) && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
                ranking.Ties = teamGames.Where(g => (((g.HomeTeam == team) && (g.HomeTeamScore == g.AwayTeamScore)) || ((g.AwayTeam == team) && (g.AwayTeamScore == g.HomeTeamScore))) && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
                ranking.Games = teamGames.Count;

                var totalRunsScored = new List<int>();
                totalRunsScored.AddRange(teamGames.Where(g => g.HomeTeam == team && g.HomeTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => g.HomeTeamScore.Value));
                totalRunsScored.AddRange(teamGames.Where(g => g.AwayTeam == team && g.AwayTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => g.AwayTeamScore.Value));
                ranking.RunsScored = totalRunsScored.Sum();

                var totalRunsAllowed = new List<int>();
                totalRunsAllowed.AddRange(teamGames.Where(g => g.HomeTeam == team && g.AwayTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => g.AwayTeamScore.Value));
                totalRunsAllowed.AddRange(teamGames.Where(g => g.AwayTeam == team && g.HomeTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => g.HomeTeamScore.Value));
                ranking.RunsAllowed = totalRunsAllowed.Sum();

                var totalRunsDifferential = new List<int>();
                totalRunsDifferential.AddRange(teamGames.Where(g => g.HomeTeam == team && g.AwayTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => (g.AwayTeamScore.Value - g.HomeTeamScore.Value)));
                totalRunsDifferential.AddRange(teamGames.Where(g => g.AwayTeam == team && g.HomeTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => (g.HomeTeamScore.Value - g.AwayTeamScore.Value)));
                ranking.RunDifferential = totalRunsDifferential.Sum();

                var totalRunsDifferentialMax8 = new List<int>();
                totalRunsDifferentialMax8.AddRange(teamGames.Where(g => g.HomeTeam == team && g.AwayTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => (g.AwayTeamScore.Value - g.HomeTeamScore.Value) > 8 ? 8 : (g.AwayTeamScore.Value - g.HomeTeamScore.Value)));
                totalRunsDifferentialMax8.AddRange(teamGames.Where(g => g.AwayTeam == team && g.HomeTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => (g.HomeTeamScore.Value - g.AwayTeamScore.Value) > 8 ? 8 : (g.HomeTeamScore.Value - g.AwayTeamScore.Value)));
                ranking.RunDifferentialMax8 = totalRunsDifferentialMax8.Sum();
                if (teamGames.Count > 0)
                {
                    rankings.Add(ranking);
                }
            }

            int i = 0;
            int iJump = 1;
            TeamRankingModel lastRanking = null;
            //Do initial ranking based on wins/losses
            foreach (var rank in (from r in rankings orderby (r.Wins/r.Games) descending, r.Wins descending, r.Losses ascending  select r))
            {
                if (lastRanking != null)
                {
                    if ((rank.Wins / rank.Games) == (lastRanking.Wins / lastRanking.Games) &&
                        rank.Wins == lastRanking.Wins &&
                        rank.Losses == lastRanking.Losses)
                    {
                        //Deal with matching ranks here
                        iJump++;
                    }
                    else
                    {
                        i += iJump;
                        iJump = 1;
                    }
                }
                rank.Ranking = i;
                lastRanking = rank;
            }
            //Formulate which teams beat which teams
            for(i=0; i < rankings.Count; )
            {
                i++;
                foreach (var rank in (from r in rankings where r.Ranking == i select r))
                {
                    int[] matchingTeams = (from r in rankings where r.Ranking == i && r.Team != rank.Team  select r.Team.Id).ToArray();
                    rank.MatchingRankLosses = 0 - rank.TeamGames.Where(g => (((g.HomeTeam == rank.Team) && (g.HomeTeamScore < g.AwayTeamScore) && matchingTeams.Contains(g.AwayTeam.Id)) ||
                                                                ((g.AwayTeam == rank.Team) && (g.AwayTeamScore < g.HomeTeamScore) && matchingTeams.Contains(g.HomeTeam.Id)))
                                                                && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
                    rank.MatchingRankWins = rank.TeamGames.Where(g => (((g.HomeTeam == rank.Team) && (g.HomeTeamScore > g.AwayTeamScore) && matchingTeams.Contains(g.AwayTeam.Id)) ||
                                                                ((g.AwayTeam == rank.Team) && (g.AwayTeamScore > g.HomeTeamScore) && matchingTeams.Contains(g.HomeTeam.Id)))
                                                                && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
                }
            }
            //Perform final ranking
            i = 0;
            iJump = 1;
            foreach (var rank in (from r in rankings orderby (r.Wins / r.Games) descending, r.Wins descending, r.Losses ascending, r.MatchingRankLosses descending, r.MatchingRankWins descending, r.RunsAllowed ascending, r.RunDifferentialMax8 ascending select r))
            {
                if (lastRanking != null)
                {
                    if ((rank.Wins / rank.Games) == (lastRanking.Wins / lastRanking.Games) &&
                        rank.Wins == lastRanking.Wins &&
                        rank.Losses == lastRanking.Losses &&
                        rank.MatchingRankLosses == lastRanking.MatchingRankLosses &&
                        rank.MatchingRankLosses == lastRanking.MatchingRankWins &&
                        rank.RunsAllowed == lastRanking.RunsAllowed &&
                        rank.RunDifferentialMax8 == lastRanking.RunDifferentialMax8)
                    {
                        //Deal with matching ranks here
                        iJump++;
                    }
                    else
                    {
                        i += iJump;
                        iJump = 1;
                    }
                }
                rank.Ranking = i;
                lastRanking = rank;
            }

            return (from r in rankings orderby r.Ranking select r).ToList();
        }
    }

    public class BracketTeam
    {
        public virtual int Id { get; set; }
        public virtual Team Team { get; set; }
        public virtual Bracket Bracket { get; set; }
        public virtual int Standing { get; set; }

        public static IList<BracketTeam> GetStandingsForTeam(Team team)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<BracketTeam>().Where(b => b.Team == team).List();
        }
        public static BracketTeam GetRankForTeam(int bracket_id, Team team)
        {
            if (team == null)
                return null;
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<BracketTeam>().Where(b => (b.Team == team) && (b.Bracket.Id == bracket_id)).SingleOrDefault();
        }
        public static IList<BracketTeam> GetBracketTeams(Bracket bracket)
        {
            if (bracket == null)
                return null;
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<BracketTeam>().Where(b => b.Bracket == bracket).List();
        }

    }
    public class BracketGenerator
    {
        public virtual int Id { get; set; }
        public virtual int? Sequence { get; set; }
        public virtual int? Team1 { get; set; }
        public virtual int? Team2 { get; set; }
        public virtual int BracketBracket { get; set; }
        public virtual int BracketPosition { get; set; }
        public virtual int? GameNumber { get; set; }
        public virtual Bracket Bracket { get; set; }
        public virtual Game Game { get; set; }
    }
}