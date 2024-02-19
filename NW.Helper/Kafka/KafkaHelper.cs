using NW.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper.Kafka
{
    public class KafkaHelper
    {
        public static void InsertQueue(string kafkaConfig, string topic, int partition, string message)//don't use parttion for now
        {
            //string kafkaConfig = "104.40.212.156:9092";
            //if (!string.IsNullOrEmpty(kafkaConfig))
            //{
            //string key = "jJm9fGRPxcRPWxfFdfx2M9xSqrs9LR4ZgHCJ3LncnRGncZHagNujKCbGmxrn7Qvr5vvd6bkntPQSQkknSUEp9fMBV4VNff2XkVFqpz8cBrsEmrN7ugMnRG5nkTpNWZCwgjkd5JrPpXkbEZ79XYyHWrETgDP9bgTHV2pTKBRTFmphx26XerC8VwYJH6QjyENRQeFuQVndjXmyZRkE6EmQmUW4fK7wYpfNY9dJzMRc4pLCAXMecccNkWEbBP6bhpH6yzzXpUNLQWdUkBvvB6BghJDX2jLK75RGz3r75GytfTEp6tFZuUHNqkRgHbrNv8fcrQUtfKAFUYSpvNfMnQAe8rFhqWuXa2bZJqJdwEcwyga99CFGf3TEEsfJVf8rDVSXbMMfmpx36wBrtA6NmKn9svYYgEuL4qtt4kPnmucdSsMtWcnEz5aE7ayptQmrhFqk7QZk7eQgP567eXDWkxNMVxNuWJTP33qHwGrhhrty8YLRwLCLbS3m";
            //HttpHelper.PostRequest("http://k1.nwservmodule.com/messagequeue/kafka?topic=" + topic + "&c=" + SecurityHelper.MD5Encryption(message + key), message, "application/json");
            //}
        }
    }
}
