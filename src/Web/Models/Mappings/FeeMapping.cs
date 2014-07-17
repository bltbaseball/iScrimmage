using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class FeeMapping : ClassMap<Fee>
    {
        public FeeMapping()
        {
            Table("Fees");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Amount).Not.Nullable();
            Map(c => c.Notes);
            Map(c => c.Description);
            Map(c => c.Name).Not.Nullable();
            Map(c => c.IsRequired).Not.Nullable();
            Map(c => c.CreatedOn).Not.Nullable();

            References(c => c.League);
        }
    }

    public class FeePaymentMapping : ClassMap<FeePayment>
    {
        public FeePaymentMapping()
        {
            Table("FeePayments");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Amount).Not.Nullable();
            Map(c => c.Note);
            Map(c => c.Type);
            Map(c => c.Method);
            Map(c => c.Status);
            Map(c => c.TransactionId);
            Map(c => c.CreatedOn).Not.Nullable();
            Map(c => c.CompletedOn);

            References(c => c.Fee);
            References(c => c.Team);
            References(c => c.Payment);
        }
    }
}