using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class PayPalPaymentMapping : ClassMap<PayPalPayment>
    {
        public PayPalPaymentMapping()
        {
            Table("PayPalPayments");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Address);
            Map(c => c.Amount);
            Map(c => c.City);
            Map(c => c.Country);
            Map(c => c.CreatedOn);
            Map(c => c.Currency);
            Map(c => c.Email);
            Map(c => c.FirstName);
            Map(c => c.LastName);
            Map(c => c.State);
            Map(c => c.Status);
            Map(c => c.TransactionId);
            Map(c => c.Zip);
            Map(c => c.Raw);
            Map(c => c.InvoiceId);
            Map(c => c.Option1);
            Map(c => c.Option2);
        }
    }
}