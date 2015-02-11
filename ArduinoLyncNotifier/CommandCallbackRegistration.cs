using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandMessenger;

namespace ArduinoLyncNotifier
{
    public class CommandCallbackRegistration
    {
        public CommandCallbackRegistration(CmdMessenger.MessengerCallbackFunction callback)
        {
            this.Callback = callback;
        }

        public CommandCallbackRegistration(int command, CmdMessenger.MessengerCallbackFunction callback)
        {
            this.Command = command;
            this.Callback = callback;
        }

        public int? Command { get; private set; }

        public CmdMessenger.MessengerCallbackFunction Callback { get; private set; }
    }
}
