using Newtonsoft.Json.Linq;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class HellosignController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        //
        // POST: /Hellosign/Callback
        private string TestSignedJson = "{\"event\":{\"event_type\":\"signature_request_signed\",\"event_time\":\"1371005680\",\"event_hash\":\"efdbbf6d8f9dbcf95057e1e3d634866604ec6f844d497205625bc8a6d8af3ab0\"},\"account_guid\":\"f2d48f1f5d8a6c77f8d8517659f825e1a8ed890a\",\"signature_request\":{\"signature_request_id\":\"2cad1a6b06a853b7fc237ba1de107db7703f605d\",\"title\":\"BLT Baseball Player Waiver\",\"original_title\":\"BLT Baseball Player Waiver\",\"subject\":\"BLT Baseball Player Waiver\",\"message\":\"Please sign the player waiver to enter into BLT Baseball.\",\"is_complete\":false,\"has_error\":false,\"custom_fields\":[{\"name\":\"PlayerID\",\"value\":\"1453\",\"type\":\"text\"},{\"name\":\"PlayerName\",\"value\":\"Little Farmer\",\"type\":\"text\"},{\"name\":\"GuardianName\",\"value\":\"Adrian Farmer\",\"type\":\"text\"},{\"name\":\"TeamName\",\"value\":\"Sunrise Sharks\",\"type\":\"text\"},{\"name\":\"Class\",\"value\":\"AAA\",\"type\":\"text\"}],\"response_data\":[{\"name\":null,\"value\":\"06\\/11\\/2013\",\"type\":\"date_signed\"}],\"signing_url\":\"https:\\/\\/www.hellosign.com\\/editor\\/sign?guid=2cad1a6b06a853b7fc237ba1de107db7703f605d\",\"final_copy_uri\":\"\\/v3\\/signature_request\\/final_copy\\/2cad1a6b06a853b7fc237ba1de107db7703f605d\",\"details_url\":\"https:\\/\\/www.hellosign.com\\/home\\/manage?guid=2cad1a6b06a853b7fc237ba1de107db7703f605d\",\"requester_email_address\":\"adrian@bltbaseball.com\",\"signatures\":[{\"signer_email_address\":\"jefferypalmer@hotmail.com\",\"signer_name\":\"Adrian\",\"order\":null,\"status_code\":\"signed\",\"signed_at\":null,\"last_viewed_at\":1371005556,\"last_reminded_at\":null}],\"cc_email_addresses\":[]}}";
        private string TestViewedJson = "{\"event\":{\"event_type\":\"signature_request_viewed\",\"event_time\":\"1371005556\",\"event_hash\":\"5714d43270fdd67d13be2265d9097e9f48dae2d2d70286b5f1ae554000091321\"},\"account_guid\":\"f2d48f1f5d8a6c77f8d8517659f825e1a8ed890a\",\"signature_request\":{\"signature_request_id\":\"2cad1a6b06a853b7fc237ba1de107db7703f605d\",\"title\":\"BLT Baseball Player Waiver\",\"original_title\":\"BLT Baseball Player Waiver\",\"subject\":\"BLT Baseball Player Waiver\",\"message\":\"Please sign the player waiver to enter into BLT Baseball.\",\"is_complete\":false,\"has_error\":false,\"custom_fields\":[{\"name\":\"PlayerID\",\"value\":\"1453\",\"type\":\"text\"},{\"name\":\"PlayerName\",\"value\":\"Little Farmer\",\"type\":\"text\"},{\"name\":\"GuardianName\",\"value\":\"Adrian Farmer\",\"type\":\"text\"},{\"name\":\"TeamName\",\"value\":\"Sunrise Sharks\",\"type\":\"text\"},{\"name\":\"Class\",\"value\":\"AAA\",\"type\":\"text\"}],\"response_data\":[],\"signing_url\":\"https:\\/\\/www.hellosign.com\\/editor\\/sign?guid=2cad1a6b06a853b7fc237ba1de107db7703f605d\",\"details_url\":\"https:\\/\\/www.hellosign.com\\/home\\/manage?guid=2cad1a6b06a853b7fc237ba1de107db7703f605d\",\"requester_email_address\":\"adrian@bltbaseball.com\",\"signatures\":[{\"signer_email_address\":\"jefferypalmer@hotmail.com\",\"signer_name\":\"Adrian\",\"order\":null,\"status_code\":\"awaiting_signature\",\"signed_at\":null,\"last_viewed_at\":1371005556,\"last_reminded_at\":null}],\"cc_email_addresses\":[]}}";
        private string TestSigningRequestResponseJson = "{\"signature_request\":{\"signature_request_id\":\"2cad1a6b06a853b7fc237ba1de107db7703f605d\",\"title\":\"BLT Baseball Player Waiver\",\"original_title\":\"BLT Baseball Player Waiver\",\"subject\":\"BLT Baseball Player Waiver\",\"message\":\"Please sign the player waiver to enter into BLT Baseball.\",\"is_complete\":false,\"has_error\":false,\"custom_fields\":[{\"name\":\"PlayerID\",\"value\":\"1453\",\"type\":\"text\"},{\"name\":\"PlayerName\",\"value\":\"Little Farmer\",\"type\":\"text\"},{\"name\":\"GuardianName\",\"value\":\"Adrian Farmer\",\"type\":\"text\"},{\"name\":\"TeamName\",\"value\":\"Sunrise Sharks\",\"type\":\"text\"},{\"name\":\"Class\",\"value\":\"AAA\",\"type\":\"text\"}],\"response_data\":[],\"signing_url\":\"https:\\/\\/www.hellosign.com\\/editor\\/sign?guid=2cad1a6b06a853b7fc237ba1de107db7703f605d\",\"details_url\":\"https:\\/\\/www.hellosign.com\\/home\\/manage?guid=2cad1a6b06a853b7fc237ba1de107db7703f605d\",\"requester_email_address\":\"adrian@bltbaseball.com\",\"signatures\":[{\"signer_email_address\":\"jefferypalmer@hotmail.com\",\"signer_name\":\"Adrian\",\"order\":null,\"status_code\":\"awaiting_signature\",\"signed_at\":null,\"last_viewed_at\":null,\"last_reminded_at\":null}],\"cc_email_addresses\":[]}}";
        public ActionResult Callback(String json)
        {
            /*var msg = new MailMessage();
            msg.To.Add(new MailAddress("jeff@triggerinc.com"));
            msg.From = new MailAddress("noreply@bltbaseball.com");
            msg.Subject = "HelloSign Results";
            var sb = new StringBuilder();
            sb.AppendFormat(
@"Hellosign returned json {0}", json);
            msg.Body = sb.ToString();
            msg.IsBodyHtml = false;
            using (var client = new SmtpClient())
            {
                client.Send(msg);
            }*/

            JObject o = JObject.Parse(json);
            var event_type = o["event"]["event_type"].ToString();
            var signature_request_id = o["signature_request"]["signature_request_id"].ToString();

            var item = session.QueryOver<TeamPlayer>()
                .Where(x => x.SignWaiverId == signature_request_id)
                .List().FirstOrDefault();
            if (item == null)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(string.Format("HelloSign: No Matching Signature Request Found #{0}\n{1}", signature_request_id, json)));
                return View("EventReceived");
            }
                
            //var item = searchList[0];
            
            if (event_type.Equals("signature_request_viewed"))
            {
                using (var tx = session.BeginTransaction())
                {
                    item.WaiverStatus = SignStatus.Viewed;
                    session.Update(item);
                    tx.Commit();
                }
            }
            if (event_type.Equals("signature_request_signed"))
            {
                using (var tx = session.BeginTransaction())
                {
                    item.WaiverStatus = SignStatus.Signed;
                    session.Update(item);
                    tx.Commit();
                }
            }
            return View("EventReceived");
        }

    }
}