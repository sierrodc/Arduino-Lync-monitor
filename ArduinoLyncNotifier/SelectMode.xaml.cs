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
    /// Interaction logic for SelectMode.xaml
    /// </summary>
    public partial class SelectMode : Window
    {
        public SelectMode()
        {
            InitializeComponent();
        }

        private void StartMonitor_Click(object sender, RoutedEventArgs e)
        {
            new Monitor().Show();
            this.Close(); 
        }

        private void StartSimulator_Click(object sender, RoutedEventArgs e)
        {
            new SimulatorWindow().Show();
            this.Close();
        }
    }
}
