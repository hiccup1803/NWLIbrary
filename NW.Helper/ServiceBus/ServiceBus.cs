using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper.ServiceBus
{
    public static class ServiceBusHelper
    {
        static Dictionary<string, ServiceBusSender> senderList = new Dictionary<string, ServiceBusSender>();
        static ServiceBusClient client = null;
        static ServiceBusHelper()
        {
            if (client == null)
                client = new ServiceBusClient("Endpoint=sb://b6premiumservicebuswesteu.servicebus.windows.net/;SharedAccessKeyName=MainProducer;SharedAccessKey=0vQbrdmOExK0x0vldXU/6w8mv33S4jRkg9nzsx2VWVo=");
        }
        public static void InsertQueue(bool isProduction, string topicName, string data)
        {
            if(isProduction)
            {
                if (!senderList.ContainsKey(topicName))
                    senderList.Add(topicName, client.CreateSender(topicName));

                senderList[topicName].SendMessageAsync(new ServiceBusMessage(data));
            }
        }
    }
}
