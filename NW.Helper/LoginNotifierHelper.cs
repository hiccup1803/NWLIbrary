using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper
{
    public class LoginNotifierHelper
    {
        public static void Notifier(int memberId, string hash, DateTime date, string message)
        {
            try
            {

                MongoClient dbClient = new MongoClient("mongodb://SMSServiceDBUser:nXm43AFqSWgVeaUF@104.45.22.68/SMSService");
                var database = dbClient.GetDatabase("SMSService");
                var collection = database.GetCollection<BsonDocument>("LoginLog");
                var documnt = new BsonDocument
                {
                    {"MemberId",memberId},
                    {"Hash",hash},
                    {"Date",date},
                    {"Timestamp",(date - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    {"Message",message}
                };
                collection.InsertOneAsync(documnt);



                //HttpHelper.PostRequest("https://hooks.slack.com/services/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", JsonConvert.SerializeObject(new { text = message }), "application/json");

            }
            catch (Exception)
            {

            }
        }
    }

}
