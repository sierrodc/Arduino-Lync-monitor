using CommandMessenger;
using CommandMessenger.Serialport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoLyncNotifier
{
    public class ArduinoController : IDisposable
    {
        private SerialTransport serialTransport;
        private CmdMessenger cmdMessenger;

        public ArduinoController(string portName, int baudRate, CommandMessenger.BoardType boardType)
        {
            this.serialTransport = new SerialTransport()
            {
                CurrentSerialSettings = { PortName = portName, BaudRate = baudRate, DtrEnable = false } // object initializer
            };

            this.cmdMessenger = new CmdMessenger(this.serialTransport)
            {
                BoardType = boardType
            };
        }

        public bool Start(params CommandCallbackRegistration[] registrations)
        {
            this.cmdMessenger.NewLineReceived += NewLineReceived;
            this.cmdMessenger.NewLineSent += NewLineSent;

            foreach (var registration in registrations)
            {
                if (!registration.Command.HasValue)
                {
                    this.cmdMessenger.Attach(registration.Callback);
                }
                else
                {
                    this.cmdMessenger.Attach(registration.Command.Value, registration.Callback);
                }
            }

            return this.cmdMessenger.Connect();
        }

        public void SendCommand(SendCommand command)
        {
            this.cmdMessenger.SendCommand(command);
        }

        #region DEBUGGING

        private void NewLineSent(object sender, NewLineEvent.NewLineArgs e)
        {
            Console.WriteLine(@"Sent > " + e.Command.CommandString());
        }

        private void NewLineReceived(object sender, NewLineEvent.NewLineArgs e)
        {
            Console.WriteLine(@"Received > " + e.Command.CommandString());
        }
        
        #endregion

        public void Dispose()
        {
            this.cmdMessenger.Disconnect();
            this.cmdMessenger.Dispose();
            this.serialTransport.Dispose();
        }
    }
}
