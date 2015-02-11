using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoLyncNotifier
{
    public enum CommandEnum : int
    {
        Ack = 1,
        Error = 2,

        Availability = 3,
        IncomingCalls = 4
    }
}
