using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Config;
using System.Web;
using System.Reflection;
using System.IO;
using log4net.Appender;
using System.Linq;
using System.Threading.Tasks;

namespace Logging
{
    public class Logger
    {
        private static string CONFIG_FILE_NAME = "log4net.config";

        public static Logger WalletServiceLog = new Logger(LogType.WalletServiceLogger);
        public static Logger UserTrack = new Logger(LogType.UserTrackLogger);


        private ILog logger;

        public static void Configure()
        {
            //XmlConfigurator.Configure();

            if (!LogManager.GetRepository().Configured)
            {
                string configFile = Assembly.GetExecutingAssembly().GetName().Name + "." + CONFIG_FILE_NAME;
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(configFile))
                {
                    XmlConfigurator.Configure(stream);
                }
            }
        }


        public ILog Log
        {
            get { return this.logger; }
        }


        public Logger(string loggerName)
        {

            Configure();
            this.logger = LogManager.GetLogger(loggerName);
        }

        public void Debug(object message)
        {
            Task.Factory.StartNew(() => this.Log.Debug(GetMessageFormat(message)));
        }
        public void Debug(object message, System.Exception t)
        {
            Task.Factory.StartNew(() => this.Log.Debug(GetMessageFormat(message), t));
        }

        public void Error(object message)
        {
            Task.Factory.StartNew(() => this.Log.Error(GetMessageFormat(message)));
        }

        public void Error(object message, System.Exception t)
        {
            Task.Factory.StartNew(() => this.Log.Error(GetMessageFormat(message), t));
        }
        public void Fatal(object message)
        {
            Task.Factory.StartNew(() => this.Log.Fatal(GetMessageFormat(message)));
        }
        public void Fatal(object message, System.Exception t)
        {
            Task.Factory.StartNew(() => this.Log.Fatal(GetMessageFormat(message), t));
        }
        public void Info(object message)
        {
            this.Log.Info(GetMessageFormat(message));
        }
        public void Track(string message)
        {
            IAppender[] appenders = LogManager.GetRepository().GetAppenders();
            BasicConfigurator.Configure(appenders.FirstOrDefault(a => a.Name == "RollingFileAppender"));
            this.Log.Info(GetMessageFormat(message));
        }



        public void Info(object message, System.Exception t)
        {
            this.Log.Info(GetMessageFormat(message), t);
        }
        public void Warn(object message)
        {
            this.Log.Warn(GetMessageFormat(message));
        }
        public void Warn(object message, System.Exception t)
        {
            this.Log.Warn(GetMessageFormat(message), t);
        }
        public bool IsDebugEnabled
        {
            get { return this.Log.IsDebugEnabled; }
        }
        public bool IsErrorEnabled
        {
            get { return this.Log.IsErrorEnabled; }
        }
        public bool IsFatalEnabled
        {
            get { return this.Log.IsFatalEnabled; }
        }
        public bool IsInfoEnabled
        {
            get { return this.Log.IsInfoEnabled; }
        }
        public bool IsWarnEnabled
        {
            get { return this.Log.IsWarnEnabled; }
        }

        public void Error(Exception ex, string p)
        {
            this.Log.Error(p, ex);
        }

        private string GetMessageFormat(object message)
        {
            //string userIdentity = string.Empty;
            //if (HttpContext.Current != null && HttpContext.Current.Session != null)
            //{
            //    if (HttpContext.Current.Session["UserIdentity"] != null)
            //    {
            //        userIdentity = HttpContext.Current.Session["UserIdentity"].ToString();
            //        userIdentity = userIdentity + "(" + HttpContext.Current.Session.SessionID + ")";
            //    }
            //}
            return message.ToString();
        }
    }
}
