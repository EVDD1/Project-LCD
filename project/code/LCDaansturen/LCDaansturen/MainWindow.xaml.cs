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
using System.Windows.Markup;
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
        Tekst tekst;
        int count = 0;
       
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

            //een object aanmaken voor de datum en tijd
            timer1 = new DispatcherTimer();

            //event aanmaken
            timer1.Tick += Timer1_Tick;

            //ticken om de seconde
            timer1.Interval = TimeSpan.FromSeconds(1);

            //Nul op de label zetten
            lbltimer.Content = "0";

        }

        private void Timer1_Tick(object? sender, EventArgs e)
        {
          lbltimer.Content =  count++;
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
            if ((serialport != null) && serialport.IsOpen)
            {
                serialport.Write(new byte[] { 0 }, 0, 1);
                serialport.Dispose();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer1.Start();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           timer1.Stop();
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            lbltimer.Content = "0";
            count = 0;
            timer1.Stop();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //als je een tekst wil doorsturen
            tekst = new Tekst();

            tekst.Info = txtbxtekst.Text;

            lbltekst.Content = tekst.toevoegen();

           
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(txtbxtekst.Text);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //ALs je de klok wil doorsturen
            byte data = Convert.ToByte( lblclock.Content);

            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.Write(new byte[] { data }, 0, 1);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //Als je de datum wil doorsturen
            byte data = Convert.ToByte(lbldatum.Content);

            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.Write(new byte[] { data }, 0, 1);
            }
        }

     
    }
}
