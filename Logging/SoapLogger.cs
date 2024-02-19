using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Logging
{
    public enum LogTraceStatus
    {
        RequestTrace,
        ResponseTrace,
        AllTrace
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SoapLoggerAttribute : SoapExtensionAttribute
    {
        private int _priority = 0;
        public SoapLoggerAttribute(string methodName, LogTraceStatus traceStatus)
        {
            this.methodName = methodName;
            this.LogTraceable = traceStatus;

        }

        private string methodName;
        public string MethodName
        {
            get
            {
                return methodName;
            }
            set
            {
                methodName = value;
            }
        }

        private LogTraceStatus logTraceable;
        public LogTraceStatus LogTraceable
        {
            get
            {
                return logTraceable;
            }
            set
            {
                logTraceable = value;
            }
        }



        public override Type ExtensionType
        {
            get
            {
                return typeof(SoapLogger);
            }
        }

        public override int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }
    }

    public class SoapLogger : SoapExtension
    {

        private Stream soapStream;
        private Stream tempStream;

        private SoapLoggerAttribute attribute = null;
        public override object GetInitializer(Type serviceType)
        {
            return null;
        }
        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return ((SoapLoggerAttribute)attribute);
        }

        public override void Initialize(object initializer)
        {
            attribute = (SoapLoggerAttribute)initializer;
        }

        public override Stream ChainStream(Stream stream)
        {
            soapStream = stream;
            tempStream = new MemoryStream();
            return tempStream;
        }

        public void CopyTextStream(Stream src, Stream dest)
        {
            TextReader reader = new StreamReader(src);
            TextWriter writer = new StreamWriter(dest);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
        }

        
        public override void ProcessMessage(System.Web.Services.Protocols.SoapMessage message)
        {
            string header = string.Empty;
            string msg = string.Empty;


            switch (message.Stage)
            {
                case SoapMessageStage.BeforeDeserialize:
                    {

                        CopyTextStream(soapStream, tempStream);

                        header = "SOAP REQUEST -> " + attribute.MethodName;

                        tempStream.Position = 0;

                        StreamReader reader = new StreamReader(tempStream);

                        msg = reader.ReadToEnd();

                        if (attribute.LogTraceable == LogTraceStatus.AllTrace || attribute.LogTraceable == LogTraceStatus.RequestTrace)
                            //Logger.FileLog.Debug(string.Format("{0}{1}{2}", header, Environment.NewLine, msg));


                        tempStream.Position = 0;


                    }
                    break;
                case SoapMessageStage.AfterSerialize:
                    {

                        header = "SOAP RESPONSE -> " + attribute.MethodName;
                        tempStream.Position = 0;

                        StreamReader reader = new StreamReader(tempStream);
                        msg = reader.ReadToEnd();

                        if (attribute.LogTraceable == LogTraceStatus.AllTrace || attribute.LogTraceable == LogTraceStatus.ResponseTrace)
                            //Logger.FileLog.Debug(string.Format("{0}{1}{2}", header, Environment.NewLine, msg));


                        tempStream.Position = 0;

                        CopyTextStream(tempStream, soapStream);


                    }
                    break;
            }

            return;
        }
    }
}
