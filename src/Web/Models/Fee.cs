using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    /// <summary>
    /// A fee that is applied to a team of players.
    /// </summary>
    public class Fee
    {
        public virtual int Id { get; set; }
        public virtual League League { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool IsRequired { get; set; }
        public virtual DateTime CreatedOn { get; set; }

        public static IList<Fee> GetRequiredFeesForLeague(League league)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Fee>()
                        .Where(c => c.League == league && c.IsRequired == true)
                        .List();
        }

        public static IList<Fee> GetFeesForLeague(League league)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Fee>()
                        .Where(c => c.League == league)
                        .List();
        }

        public static Fee GetFeeById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<Fee>(id);
        }
    }

    public enum FeePaymentType
    {
        Payment = 0,
        Adjustment = 1,
        NotApplicable = 2,
    }

    public enum FeePaymentMethod
    {
        Manual = 0,
        CreditCard = 1,
        PayPal = 2,
    }

    public enum FeePaymentStatus
    {
        Pending = 0,
        Completed = 1
    }

    /// <summary>
    /// Payment received from a team applied towards a specific fee.
    /// </summary>
    public class FeePayment
    {
        public virtual int Id { get; set; }
        public virtual Fee Fee { get; set; }
        public virtual Team Team { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Note { get; set; }
        public virtual FeePaymentType Type { get; set; }
        public virtual FeePaymentMethod Method { get; set; }
        public virtual FeePaymentStatus Status { get; set; }
        public virtual string TransactionId { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime? CompletedOn { get; set; }
        public virtual PayPalPayment Payment { get; set; }

        public static FeePayment GetFeePaymentById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<FeePayment>(id);
        }

        public static IList<FeePayment> GetFeePaymentsForTeam(Team team)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<FeePayment>()
                        .Where(c => c.Team == team)
                        .List();
        }
        public static bool GetFeesPaidStatus(Team team)
        {
            if (team.Id == 193)
            {
                int i = 48329;
            }
            var fees = Fee.GetRequiredFeesForLeague(team.League);
            var paidFees = FeePayment.GetFeePaymentsForTeam(team);
            bool isPaid = true;
            foreach (var fee in fees)
            {
                var paidFee = paidFees.Where(f => f.Fee == fee && f.Status == FeePaymentStatus.Completed).SingleOrDefault();
                if (paidFee == null)
                {
                    isPaid = false;
                    break;
                }
            }
            return isPaid;
        }
    }
}