using CommandMessenger;
using LyncModel = Microsoft.Lync.Model;
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
using System.Windows.Shapes;

namespace ArduinoLyncNotifier
{
    /// <summary>
    /// Interaction logic for Logger.xaml
    /// </summary>
    public partial class Monitor : Window
    {
        private LyncModel.LyncClient _LyncClient;
        private ArduinoController arduino;

        public bool IsLyncAvailable { get; set; }

        public Monitor()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;
            this.Closing += Monitor_Closing;
            incomintCallText.Visibility = System.Windows.Visibility.Hidden;


            this.arduino = new ArduinoController("COM10", 9600, BoardType.Bit16);
            var connected = this.arduino.Start(
                    new CommandCallbackRegistration(OnUnknownCommand),
                    new CommandCallbackRegistration((int)CommandEnum.Ack, OnAck),
                    new CommandCallbackRegistration((int)CommandEnum.Error, OnError)
                    );

            try
            {
                _LyncClient = LyncModel.LyncClient.GetClient();

                //Register for event that is raised when user availablity changes
                _LyncClient.Self.Contact.ContactInformationChanged += Contact_ContactInformationChanged;
                _LyncClient.ConversationManager.ConversationAdded += ConversationManager_ConversationAdded;
                this.IsLyncAvailable = true;
            }
            catch (LyncModel.ClientNotFoundException)
            {
                this.IsLyncAvailable = false;
                MessageBox.Show("Lync is not running");
            }
        }




        #region Arduino DEBUG
        private void OnError(ReceivedCommand receivedCommand)
        {
            Console.WriteLine("Error: {0}", receivedCommand.RawString);
        }

        private void OnAck(ReceivedCommand receivedCommand)
        {
            Console.WriteLine("Acknowledged: {0}", receivedCommand.RawString);
        }

        private void OnUnknownCommand(ReceivedCommand receivedCommand)
        {
            Console.WriteLine("Unkown: {0}", receivedCommand.RawString);
        }
        #endregion

        /// <summary>
        /// New conversation available
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Additional info about the event</param>
        void ConversationManager_ConversationAdded(object sender, LyncModel.Conversation.ConversationManagerEventArgs e)
        {
            var newConversation = e.Conversation;

            //conversation window inactive == window doesn't require our action => conversation started by current user
            if (newConversation.State == Microsoft.Lync.Model.Conversation.ConversationState.Inactive)
                return;

            //check if it is audio/video call
            if (newConversation.Modalities.ContainsKey(LyncModel.Conversation.ModalityTypes.AudioVideo) &&
                newConversation.Modalities[LyncModel.Conversation.ModalityTypes.AudioVideo].State == LyncModel.Conversation.ModalityState.Notified)
            {
                //Show "Incoming call" message 
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    incomintCallText.Visibility = System.Windows.Visibility.Visible;
                }));
                
                //Start buzzer
                this.arduino.SendCommand(new SendCommand((int)CommandEnum.IncomingCalls, 1));

                //Register to AV conversation change
                newConversation.Modalities[LyncModel.Conversation.ModalityTypes.AudioVideo].ModalityStateChanged += (se, ea) =>
                {
                    //we are now connected to the call, no more buzz needed
                    if (ea.NewState == LyncModel.Conversation.ModalityState.Connected ||
                        ea.NewState == LyncModel.Conversation.ModalityState.Disconnected ||
                        ea.NewState == LyncModel.Conversation.ModalityState.Joining)
                    {
                        //hide "Incoming call" message
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            incomintCallText.Visibility = System.Windows.Visibility.Hidden;
                        }));
                        //stop buzzer
                        this.arduino.SendCommand(new SendCommand((int)CommandEnum.IncomingCalls, 0));
                    }
                };

                //current user didn't joined the conversation (timout/close/voice message)
                newConversation.StateChanged += (se, ea) =>
                {
                    if (ea.NewState == LyncModel.Conversation.ConversationState.Parked ||
                        ea.NewState == LyncModel.Conversation.ConversationState.Terminated)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            incomintCallText.Visibility = System.Windows.Visibility.Hidden;
                        }));
                        this.arduino.SendCommand(new SendCommand((int)CommandEnum.IncomingCalls, 0));
                    }
                };
            }
        }

        private delegate void DisplayCurrentStateDelegate();

        #region My Availability

        /// <summary>
        /// Executed when lync status changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contact_ContactInformationChanged(object sender, LyncModel.ContactInformationChangedEventArgs e)
        {
            if (e.ChangedContactInformation.Contains(LyncModel.ContactInformationType.Availability))
            {
                //update the UI with the current state of the user
                this.Dispatcher.Invoke(new DisplayCurrentStateDelegate(DisplayCurrentState));
            }
        }

        /// <summary>
        /// Display the availability, activity, activity ID, and custom activity 
        /// of the currently signed in user.
        /// </summary>
        private void DisplayCurrentState()
        {
            //Current availability
            switch ((LyncModel.ContactAvailability)_LyncClient.Self.Contact.GetContactInformation(LyncModel.ContactInformationType.Availability))
            {
                case LyncModel.ContactAvailability.Away:
                    arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Away));
                    this.Background = Brushes.Yellow;
                    break;
                case LyncModel.ContactAvailability.Busy:
                    arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Busy));
                    this.Background = Brushes.Red;
                    break;
                case LyncModel.ContactAvailability.BusyIdle:
                    arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Busy));
                    this.Background = Brushes.Red;
                    break;
                case LyncModel.ContactAvailability.DoNotDisturb:
                    arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.DoNotDisturb));
                    this.Background = Brushes.Red;
                    break;
                case LyncModel.ContactAvailability.Free:
                    arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Available));
                    this.Background = Brushes.Green;
                    break;
                case LyncModel.ContactAvailability.FreeIdle:
                    arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Available));
                    this.Background = Brushes.Green;
                    break;
                case LyncModel.ContactAvailability.Invalid:
                    arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Available));
                    this.Background = Brushes.Green;
                    break;
                case LyncModel.ContactAvailability.None:
                    arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Available)); // TODO: change color
                    this.Background = Brushes.White;
                    break;
                case LyncModel.ContactAvailability.Offline:
                    arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Available)); // TODO: change color
                    this.Background = Brushes.White;
                    break;
                case LyncModel.ContactAvailability.TemporarilyAway:
                    arduino.SendCommand(new SendCommand((int)CommandEnum.Availability, (int)AvailabilityEnum.Away));
                    this.Background = Brushes.Yellow;
                    break;
            }

        }

        #endregion

        protected override void OnClosed(EventArgs e)
        {
            this.arduino.Dispose();
            this.arduino = null;

            base.OnClosed(e);
        }

        private bool isClosing = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsLyncAvailable)
            {
                // TODO: polling procedure because first message using CmdMessenger seems not working immediately after communication initialized (to fix) 
                Task.Factory.StartNew(() =>
                {
                    while (!this.isClosing)
                    {
                        this.Dispatcher.Invoke(new DisplayCurrentStateDelegate(DisplayCurrentState));
                        System.Threading.Thread.Sleep(5000);
                    }
                });
            }
        }

        /// <summary>
        /// Exit from application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Monitor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.isClosing = true;
        }
    }
}
