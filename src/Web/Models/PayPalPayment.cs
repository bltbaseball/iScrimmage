using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class PayPalPayment
    {
        public virtual int Id { get; set; }
        public virtual string TransactionId { get; set; }
        public virtual string Status { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Email { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string Country { get; set; }
        public virtual string Currency { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual string Raw { get; set; }
        public virtual string InvoiceId { get; set; }
        public virtual string Option1 { get; set; }
        public virtual string Option2 { get; set; }

        public static PayPalPayment GetPayPalPaymentByTransactionId(string transactionId)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<PayPalPayment>()
                .Where(p => p.TransactionId == transactionId).SingleOrDefault();
        }

        //public static PayPalPayment GetPayPalPaymentByInvoiceId(int invoiceId)
        //{
        //    var session = MvcApplication.SessionFactory.GetCurrentSession();
        //    return session.QueryOver<PayPalPayment>()
        //        .Where(p => p.InvoiceId == invoiceId).SingleOrDefault();
        //}
    }
}