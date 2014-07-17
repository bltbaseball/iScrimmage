using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class PlayerGameStat
    {
        public virtual int Id { get; set; }
        public virtual TeamPlayer TeamPlayer { get; set; }
        public virtual Game Game { get; set; }
        public virtual double? InningsPitched { get; set; }
        public virtual double? InningsOuts { get; set; }
        public virtual double? PitchesThrown { get; set; }

        public static IList<PlayerGameStat> GetPlayerGameStatsForGameAndTeam(Game game, Team team)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<PlayerGameStat>().Where(c => c.Game.AwayTeam == team || c.Game.HomeTeam == team && c.TeamPlayer.Team == team && c.Game == game).List();
        }

        public static IList<PlayerGameStat> GetPlayerGameStatsForGame(Game game)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<PlayerGameStat>().Where(c => c.Game == game).List();
        }

    }
}