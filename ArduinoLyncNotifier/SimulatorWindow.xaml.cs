using CommandMessenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArduinoLyncNotifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SimulatorWindow : Window
    {
        private ArduinoController arduino;
        

        public SimulatorWindow()
        {
            InitializeComponent();
            
            this.arduino = new ArduinoController("COM10", 9600, BoardType.Bit16);
            this.arduino.Start(
                    new CommandCallbackRegistration(OnUnknownCommand),
                    new CommandCallbackRegistration((int)CommandEnum.Ack, OnAck),
                    new CommandCallbackRegistration((int)CommandEnum.Error, OnError)
                    );
        }

        #region DEBUG
        private void OnError(ReceivedCommand receivedCommand)
        {
            throw new NotImplementedException();
        }

        private void OnAck(ReceivedCommand receivedCommand)
        {
            throw new NotImplementedException();
        }

        private void OnUnknownCommand(ReceivedCommand receivedCommand)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void SetBusy_Click(object sender, RoutedEventArgs e)
        {
            arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Busy));
        }

        private void SetAvailable_Click(object sender, RoutedEventArgs e)
        {
            arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Available));
        }

        private void SetDoNotDisturb_Click(object sender, RoutedEventArgs e)
        {
            arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.DoNotDisturb));
        }

        private void SetAway_Click(object sender, RoutedEventArgs e)
        {
            arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Away));
        }

        private void SimulateIncomingCalls_Click(object sender, RoutedEventArgs e)
        {
            this.arduino.SendCommand(new SendCommand((int)CommandEnum.IncomingCalls, 1));
        }

        private void SimulateNoIncomingCalls_Click(object sender, RoutedEventArgs e)
        {
            this.arduino.SendCommand(new SendCommand((int)CommandEnum.IncomingCalls, 0));
        }

        protected override void OnClosed(EventArgs e)
        {
            this.arduino.Dispose();
            this.arduino = null;

            base.OnClosed(e);
        }
    }
}
