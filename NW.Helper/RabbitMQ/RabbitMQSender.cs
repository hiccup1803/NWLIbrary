using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper.RabbitMQ
{
    public class RabbitMQSender
    {
        public static void InsertQueue(string queueName, string message)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "40.115.59.239", UserName = "remoteuser", Password = "remoteuser123" };
                using (var connection = factory.CreateConnection())
                {

                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: queueName,
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: queueName,
                                             basicProperties: null,
                                             body: body);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
