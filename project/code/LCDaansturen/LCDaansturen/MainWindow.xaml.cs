using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Schema;

namespace LCDaansturen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Klasses
        SerialPort serialport;
        DispatcherTimer klok;
        DispatcherTimer timer;
        DispatcherTimer datum;
        Tekst tekst;
        Alarmklok alarmklok;
        DispatcherTimer wekker;
        int count = 0;
        string clock;
        int minuten = 0;
        int start = 0;

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

            /////////////////////////////////////////////////////////////////

            //een object aanmaken voor de datum en tijd
            klok = new DispatcherTimer();

            //event aanmaken
            klok.Tick += Timer_Tick;

            //ticken om de seconde
            klok.Interval = TimeSpan.FromSeconds(1);

            ////////////////////////////////////////////////////////////////

            //een object aanmaken voor de datum en tijd
            timer = new DispatcherTimer();

            //event aanmaken
            timer.Tick += Timer1_Tick;

            //ticken om de seconde
            timer.Interval = TimeSpan.FromSeconds(1);

            ////////////////////////////////////////////////////////////////

            datum = new DispatcherTimer();

            datum.Tick += Datum_Tick;

            datum.Interval = TimeSpan.FromSeconds(1);

            ////////////////////////////////////////////////////////
            
            alarmklok = new Alarmklok();

            wekker = new DispatcherTimer();

            wekker.Tick += Wekker_Tick;

            wekker.Interval = TimeSpan.FromSeconds(1);

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

                    //Als je een com-poort hebt aangeduid dan moeten ze aangaan
                    grpclk.IsEnabled=true;
                    grpdtm.IsEnabled=true;
                    grptim.IsEnabled=true;
                    grptkst.IsEnabled=true;
                    resetknop.IsEnabled=true;
                    grpwkkr.IsEnabled=true;

                }
                if (cbxComPorts.SelectedItem.ToString() == "None")
                {
                    //Als er geen com-poort is aangeduid alles uitzetten
                    grpclk.IsEnabled = false;
                    grpdtm.IsEnabled = false;
                    grptim.IsEnabled = false;
                    grptkst.IsEnabled = false;
                    resetknop.IsEnabled = false;
                    grpwkkr.IsEnabled = false;
                }
            }
        }
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            //je moet eerst op stuur klikken vooraleer je op start kan klikken
            start++;
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString($"{count}s"));

            }
        }

        private void Timer1_Tick(object? sender, EventArgs e)
        {

                //per tick count optellen voor de timer
                count++;

                //Als er 1 seconde voorbij is dan moet er 1 minuut op komen
                if (count > 60)
                {
                    count = 0;
                    minuten++;
                }

                if ((serialport != null) && (serialport.IsOpen) && (minuten == 0))
                {
                    serialport.WriteLine(Convert.ToString($"{count}s"));

                }
                if ((serialport != null) && (serialport.IsOpen) && (minuten > 0))
                {

                    serialport.WriteLine(Convert.ToString($"{minuten}m:{count}s"));

                }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Als je op stuur geklikt hebt dan kan je op start drukken
            if (start > 0)
            {
                //timer start
                timer.Start();
            }
            if(start == 0)
            {
                //Je moet eerst op stuur drukken
                MessageBox.Show("Eerst op stuur drukken!","Fout",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //timer stoppen
           timer.Stop();
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            count = 0;
            minuten = 0;
            timer.Stop();

            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString($"{count}s"));

            }
     
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (txtbxtekst.Text != "")
            {
                //als je een tekst wil doorsturen
                tekst = new Tekst();

                tekst.Info = txtbxtekst.Text;

                lbltekst.Content = tekst.toevoegen();


                if ((serialport != null) && (serialport.IsOpen))
                {
                    serialport.WriteLine(tekst.toevoegen());
                }

                //maakt de textbox weer leeg
                txtbxtekst.Clear();
            }
            else 
            //als er niets instaat dan een melding geven
            MessageBox.Show("Tekstvak is leeg!","Fout",MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
           
            //klok
            lblclock.Content = DateTime.Now.ToLongTimeString();
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString(DateTime.Now.ToLongTimeString()));

            }

        }

        private void Wekker_Tick(object? sender, EventArgs e)
        {
            string klok = DateTime.Now.ToLongTimeString();

            lblwkkrklok.Content = klok;

            string wekker1 = alarmklok.Alarmtime.ToLongTimeString();
            lblwkkr.Content = wekker1;

            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString(DateTime.Now.ToLongTimeString()));
            }

            if (wekker1 == klok)
            {
                wekker.Stop();
                if ((serialport != null) && (serialport.IsOpen))
                {
                    serialport.WriteLine(Convert.ToString("WEKKER!"));
                }
               

             
            }
        }
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {

            txtbxwkkr.Clear();
            wekker.Start();

            if(DateTime.TryParse(txtbxwkkr.Text, out DateTime tijd))  
             alarmklok.startalarm(tijd);
        }



        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            klok.Start();
        }

        private void Datum_Tick(object? sender, EventArgs e)
        {
            //datum
            lbldatum.Content = DateTime.Now.ToShortDateString();
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString(DateTime.Now.ToShortDateString()));
            }
          
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
           
           datum.Start();

         
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //ALs je het scherm sluit mag er niets meer op het scherm staan
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString(""));
            }

            if ((serialport != null) && serialport.IsOpen)
            {
                serialport.Write(new byte[] { 0 }, 0, 1);
                serialport.Dispose();
            }

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            //LCD resetten
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString(""));
            }
            //timer resetten
            start = 0;
            timer.Stop();
            count = 0;
            minuten = 0;

            //klok resetten
            klok.Stop();
            lblclock.Content = "";
            //datum resetten
            datum.Stop();
            lbldatum.Content = "";

            //tekst resetten
            lbltekst.Content = "";

            wekker.Stop();

        }

     
    }
}
