using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LCDaansturen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Klasses
        SerialPort serialport;
        DispatcherTimer timer;
        DispatcherTimer timer1;
        public MainWindow()
        {
            InitializeComponent();

            //Maak een object aan van SerialPort 
            serialport = new SerialPort();

            //je kan kiezen uit none
            cbxComPorts.Items.Add("None");
            
            //je kan ook kiezen tussen de beschikbare COM-poorten
            foreach (string poort in SerialPort.GetPortNames())
                cbxComPorts.Items.Add(poort);
            
            //een object aanmaken voor de datum en tijd
            timer = new DispatcherTimer();

            //event aanmaken
            timer.Tick += Timer_Tick;
            
            //ticken om de seconde
            timer.Interval = TimeSpan.FromSeconds(1);

            //timer starten
            timer.Start();

            timer1 = new DispatcherTimer();

            timer1.Tick += Timer_Tick;

            timer1.Interval = TimeSpan.FromSeconds(1);

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            //klok
            lblclock.Content = DateTime.Now.ToLongTimeString();
            //datum
            lbldatum.Content = DateTime.Now.ToLongDateString();
        }

        private void cbxComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Als er geen COM-poort is dan mag het niet doorgaan
            if(serialport != null)
            {
                //kijken of de seriële poort open is
                if(serialport.IsOpen)
                {
                    //seriële poort sluiten
                    serialport.Close();
                }

                //Als none niet aangeduid is dan mag je doorgaan
                if (cbxComPorts.SelectedItem.ToString() != "None")
                {
                    //De gekozen COM-poort uit de combobox in PortName zetten
                    serialport.PortName = cbxComPorts.SelectedItem.ToString();
                    //seriële poort openen
                    serialport.Open();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //window
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer1.Start();
            

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            timer1.Stop();
        }
    }
}
