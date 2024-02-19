using System;
using System.Collections.Generic;
using System.Text;

namespace Logging
{
    public enum LogLevel : int
    {
        None = 0,
        Debug = 10,
        Info = 20,
        Warn = 30,
        Error = 40,
        Fatal = 50
    }
}
