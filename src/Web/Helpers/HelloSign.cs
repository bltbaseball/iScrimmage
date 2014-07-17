using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using Web.Models;

namespace Web.Helpers
{
    public class Hellosign
    {
        /* set callback URL
         * curl -k -u 'adrian@bltbaseball.com:BLTbaseball2013' 'https://api.hellosign.com/v3/account' -F 'callback_url=http://www.bltbaseball.com/HelloSign/Callback'
         */


        public static string RequestPlayerWaiverSignature(NHibernate.ISession session, TeamPlayer teamPlayer, Player player, Team team)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 1600;
            String username = "adrian@bltbaseball.com";
            String password = "BLTbaseball2013";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));

            string email = player.Email;
            string guardianName = "Guardian";
            var guardian = player.Guardian;
            if (guardian != null)
            {
                email = guardian.Email;
                guardianName = guardian.LastName + ", " + guardian.FirstName;
            }

            WebRequest request = WebRequest.Create("https://api.hellosign.com/v3/signature_request/send_with_reusable_form");
            request.Method = "POST";
            request.Headers.Add("Authorization", "Basic " + encoded);
            var postData = new StringBuilder();
            postData.AppendFormat(
                "reusable_form_id=af940406ea794e4b5c1a0db84c469f1c0c045c1f" + "&" +
                "subject=BLT Baseball Player Waiver" + "&" +
                "message=Please sign the player waiver to enter into BLT Baseball." + "&" +
                "signers[Parent or Guardian][email_address]={0}" + "&" +
                "signers[Parent or Guardian][name]={1}" + "&" +
                "custom_fields[PlayerName]={2}" + "&" +
                "custom_fields[PlayerID]={3}" + "&" +
                "custom_fields[GuardianName]={4}" + "&" +
                "custom_fields[TeamName]={5}" + "&" +
                "custom_fields[Class]={6}",
                HttpUtility.UrlEncode(email), HttpUtility.UrlEncode(guardianName), HttpUtility.UrlEncode(player.LastName + ", " + player.FirstName), 
                player.Id, HttpUtility.UrlEncode(guardianName), HttpUtility.UrlEncode(Team.PrettyName(team)), HttpUtility.UrlEncode(team.Class.Name));
                //SignerEmail, SignerName, player.LastName + ", " + player.FirstName, player.Id, GuardianName, team.Name, team.Class.ToString());
            byte[] byteArray = Encoding.UTF8.GetBytes(postData.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            reader.Close();
            dataStream.Close();
            response.Close();

            JObject o = JObject.Parse(responseFromServer);
            var RequestId = o["signature_request"]["signature_request_id"].ToString();

            using (var tx = session.BeginTransaction())
            {
                teamPlayer.WaiverStatus = SignStatus.RequestSent;
                teamPlayer.SignWaiverId = RequestId;
                session.Update(teamPlayer);
                tx.Commit();
            }

            return RequestId;
        }
    }
}