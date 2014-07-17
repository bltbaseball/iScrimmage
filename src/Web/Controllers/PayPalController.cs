using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class PayPalController : Controller
    {

        public ActionResult IPN()
        {
            try
            {
                string strFormValues = Encoding.ASCII.GetString(this.Request.BinaryRead(this.Request.ContentLength));
                string requestUriString = "https://www.paypal.com/cgi-bin/webscr";
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["PayPalSandbox"]) == true)
                    requestUriString = "https://www.sandbox.paypal.com/cgi-bin/webscr";

                // post form back to PayPal
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUriString);

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                byte[] param = Request.BinaryRead(Request.ContentLength);
                string strRequest = Encoding.ASCII.GetString(param);
                strRequest += "&cmd=_notify-validate";
                req.ContentLength = strRequest.Length;

                //Send the request to PayPal and get the response
                using (var streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
                {
                    streamOut.Write(strRequest);
                    streamOut.Close();
                }
                string strResponse = null;
                using (var streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    strResponse = streamIn.ReadToEnd();
                    streamIn.Close();
                }
                var paymentStatus = Request.Form["payment_status"];
                var txnId = Request.Form["txn_id"];
                var receiverEmail = Request.Form["receiver_email"];
                var paymentAmount = Request.Form["payment_amount"];
                var currency = Request.Form["payment_currency"];

                if (strResponse == "VERIFIED")
                {
                    //check the payment_status is Completed                
                    if (Request.Form["payment_status"] == "completed")
                    {
                        //check that txn_id has not been previously processed

                        var existingPayment = PayPalPayment.GetPayPalPaymentByTransactionId(txnId);
                        if (existingPayment != null)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(string.Format("PayPal Payment: Already processed: {0}", txnId)));
                            return View();
                        }

                        //check that receiver_email is your Primary PayPal email
                        if (receiverEmail != ConfigurationManager.AppSettings["PayPalEmail"])
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(string.Format("PayPal Payment: Receiver Email Not Matched: {0} vs {1}", receiverEmail, ConfigurationManager.AppSettings["PayPalEmail"])));
                            return View();
                        }

                        //check that payment_amount/payment_currency are correct
                        if (currency != "USD")
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(string.Format("PayPal Payment: Currency invalid: {0}", currency)));
                            return View();
                        }


                        // for the moment, option selection 1 is the Division Name
                        // and option selection 2 is the Team Name for tournament registrations
                        string option1 = Request.Form["option_selection1"];
                        string option2 = Request.Form["option_selection2"];

                        string invoice = Request.Form["invoice"]; // {feeId}_{teamId} is the current assumption

                        // process payment
                        var payment = new PayPalPayment();
                        payment.Amount = Convert.ToDecimal(Request.Form["mc_gross"]);
                        payment.Status = paymentStatus;
                        payment.TransactionId = txnId;
                        payment.Currency = currency;
                        payment.CreatedOn = DateTime.Now;
                        payment.Email = Request.Form["payer_email"];
                        payment.FirstName = Request.Form["first_name"];
                        payment.LastName = Request.Form["last_name"];
                        payment.Address = Request.Form["address_street"];
                        payment.City = Request.Form["address_city"];
                        payment.State = Request.Form["address_state"];
                        payment.Zip = Request.Form["address_zip"];
                        payment.Country = Request.Form["address_country"];
                        payment.Raw = Request.Form.ToString();
                        payment.InvoiceId = invoice;
                        payment.Option1 = option1;
                        payment.Option2 = option2;

                        var session = MvcApplication.SessionFactory.GetCurrentSession();
                        using (var tx = session.BeginTransaction())
                        {
                            session.Save(payment);

                            if (!string.IsNullOrEmpty(invoice))
                            {
                                string[] invoiceParts = invoice.Split('_');
                                int feeId = int.Parse(invoiceParts[0]);
                                int teamId = int.Parse(invoiceParts[1]);

                                var fee = Fee.GetFeeById(feeId);
                                var team = Team.GetTeamById(teamId);

                                // record the fee payment
                                var feePayment = new FeePayment();
                                feePayment.Amount = decimal.Parse(paymentAmount);

                                if (feePayment.Amount != fee.Amount)
                                {
                                    Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(string.Format("Fee payment of {0} not equal to fee amount of {1}. Payment Id #{2}", feePayment.Amount, fee.Amount, payment.Id)));
                                }
                                else
                                {
                                    feePayment.Status = FeePaymentStatus.Completed;
                                    feePayment.CompletedOn = DateTime.Now;
                                }

                                feePayment.Fee = fee;
                                feePayment.Method = FeePaymentMethod.PayPal;
                                feePayment.Payment = payment;
                                feePayment.Team = team;
                                feePayment.TransactionId = txnId;
                                feePayment.Type = FeePaymentType.Payment;                                
                                feePayment.CreatedOn = DateTime.Now;                                
                                session.Save(feePayment);
                            }
                            tx.Commit();
                        }
                    }
                    else
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(string.Format("PayPal Payment: Incomplete - {0}", Request.Form.ToString())));
                    }
                }
                else if (strResponse == "INVALID")
                {
                    //log for manual investigation
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(string.Format("PayPal Payment: INVALID - {0}", Request.Form.ToString())));
                }
                else
                {
                    //log response/ipn data for manual investigation
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(string.Format("PayPal Payment: UNKNOWN - {0}", Request.Form.ToString())));
                }
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }

            return View();
        }
 
    }
}
