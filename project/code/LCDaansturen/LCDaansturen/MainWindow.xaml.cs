using System;
using System.IO.Ports;
using System.Threading;
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
        //variablen
        int seconden = 0;
        int minuten = 0;
        int start = 0;
        int wekkerduur = 10;
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

            //een object aanmaken voor de klok
            klok = new DispatcherTimer();

            //event aanmaken
            klok.Tick += Timer_Tick;

            //ticken om de seconde
            klok.Interval = TimeSpan.FromSeconds(1);

            ////////////////////////////////////////////////////////////////

            //een object aanmaken voor de timer
            timer = new DispatcherTimer();

            //event aanmaken
            timer.Tick += Timer1_Tick;

            //ticken om de seconde
            timer.Interval = TimeSpan.FromSeconds(1);

            ////////////////////////////////////////////////////////////////

            //een object aanmaken voor de datum
            datum = new DispatcherTimer();

            datum.Tick += Datum_Tick;

            datum.Interval = TimeSpan.FromSeconds(1);

            ////////////////////////////////////////////////////////
            
            //een object aanmaken voor de wekker
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


        /////////////////////////////////////////////////////////////////////////////////
        //TIMER
        /////////////////////////////////////////////////////////////////////////////////
        private void STUUR_Click(object sender, RoutedEventArgs e)
        {
            //je moet eerst op stuur klikken vooraleer je op start kan klikken
            start++;
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString($"{seconden}s"));

            }
        }

        private void Timer1_Tick(object? sender, EventArgs e)
        {

                //per tick count optellen voor de timer
                seconden++;

                //Als er 1 seconde voorbij is dan moet er 1 minuut op komen
                if (seconden > 60)
                {
                    seconden = 0;
                    minuten++;
                }
          
                //De seconden 
                if ((serialport != null) && (serialport.IsOpen) && (minuten == 0))
                {
                    serialport.WriteLine(Convert.ToString($"{seconden}s"));

                }

                //De minuten en seconden
                if ((serialport != null) && (serialport.IsOpen) && (minuten > 0))
                {

                    serialport.WriteLine(Convert.ToString($"{minuten}m:{seconden}s"));

                }
         
        }

        private void START_Click(object sender, RoutedEventArgs e)
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

        private void STOP_Click(object sender, RoutedEventArgs e)
        {
            //timer stoppen
           timer.Stop();
        }
        private void RESET_Click(object sender, RoutedEventArgs e)
        {
            //variabelen resetten
            seconden = 0;
            minuten = 0;

            //timer stoppen
            timer.Stop();

            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString($"{seconden}s"));

            }
     
        }

        /////////////////////////////////////////////////////////////////////////////////
        //TEKST STUREN
        /////////////////////////////////////////////////////////////////////////////////
        private void TEKST_Click(object sender, RoutedEventArgs e)
        {
            //Kijken of er tekst instaat
            if (txtbxtekst.Text != "")
            {
                //object aanmaken
                tekst = new Tekst();

                //De tekst van in de textbox in de propertie info zetten
                tekst.Info = txtbxtekst.Text;

                //De methode oproepen om dan de tekst in de label te zetten
                lbltekst.Content = tekst.toevoegen();

                //de tekst naar de lcd sturen
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

        /////////////////////////////////////////////////////////////////////////////////
        //KLOK
        /////////////////////////////////////////////////////////////////////////////////
        private void Timer_Tick(object? sender, EventArgs e)
        {
            //Het uur op de label zetten
            lblclock.Content = DateTime.Now.ToLongTimeString();

            //Het uur op het lcd-scherm zetten
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString(DateTime.Now.ToLongTimeString()));

            }

        }
        private void CLOCK_Click(object sender, RoutedEventArgs e)
        {
            //De timer starten van klok
            klok.Start();
        }

        /////////////////////////////////////////////////////////////////////////////////
        //WEKKER
        /////////////////////////////////////////////////////////////////////////////////
        private void Wekker_Tick(object? sender, EventArgs e)
        {
        

            //De tijd dat er is ingesteld komt in de string wekker1
            lblwkkr.Content = alarmklok.Alarmtime.ToLongTimeString();


            //De tijd op de lcd zetten
            if ((serialport != null) && (serialport.IsOpen))
            {
               serialport.WriteLine(Convert.ToString(DateTime.Now.ToLongTimeString()));
            }
            
            //Als het TRUE is dan komt er wekker op de lcd
            if(alarmklok.IsAlarmTijdKlaar())
            {

                lblwkkr.Content = "";
                //timer stoppen
                wekker.Stop();
           

                for (int i = 0; i < wekkerduur; i++)
                {
                    serialport.WriteLine(Convert.ToString("WEKKER!"));

                    Thread.Sleep(500);

                    serialport.WriteLine(Convert.ToString(""));

                    Thread.Sleep(500);
                }
              
            }
        }
        private void WEKKER_Click(object sender, RoutedEventArgs e)
        {

            //De waarde van in de textbox omzetten naar een datetime
            if (DateTime.TryParse(txtbxwkkr.Text, out DateTime tijd))
            {
                //Je mag 00:00:00 niet invullen
                if (tijd != Convert.ToDateTime("00:00:00"))
                {
                    //De timer starten
                    wekker.Start();
                    alarmklok.Startalarm(tijd);
                }
                else
                    MessageBox.Show("Ongeldige tijdnotatie!","Fout",MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Ongeldige tijdnotatie!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
          

            //Als je op de knop drukt dan reset je de textbox
            txtbxwkkr.Clear();
        }

        /////////////////////////////////////////////////////////////////////////////////
        //DATUM
        /////////////////////////////////////////////////////////////////////////////////
        private void Datum_Tick(object? sender, EventArgs e)
        {
            //datum op label zetten
            lbldatum.Content = DateTime.Now.ToShortDateString();

            //Datum via seriële communicatie doorsturen naar de lcd
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString(DateTime.Now.ToShortDateString()));
            }
          
        }

        private void DATUM_Click(object sender, RoutedEventArgs e)
        {
           //De timer starten van de datum
           datum.Start(); 
        }

        /////////////////////////////////////////////////////////////////////////////////
        //VENSTER SLUITEN
        /////////////////////////////////////////////////////////////////////////////////
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //ALs je het scherm sluit mag er niets meer op het scherm staan
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString(""));
            }

            //Controleerd of het object serialport niet nul is en of de seriële
            //poort nog open is
            if ((serialport != null) && serialport.IsOpen)
            {
                //Er wordt een byte met waarde nul geschreven naar de seriële poort
                serialport.Write(new byte[] { 0 }, 0, 1);

                //Hier sluit je de seriële poort, je kan het dan niet meer gebruiken
                serialport.Dispose();
            }

        }

        /////////////////////////////////////////////////////////////////////////////////
        //RESETTEN
        /////////////////////////////////////////////////////////////////////////////////
        private void RESET1_Click(object sender, RoutedEventArgs e)
        {
            //LCD resetten, niet meer op het scherm zetten
            if ((serialport != null) && (serialport.IsOpen))
            {
                serialport.WriteLine(Convert.ToString(""));
            }
            //timer resetten
            start = 0;
            timer.Stop();
            seconden = 0;
            minuten = 0;

            //klok resetten
            klok.Stop();
            lblclock.Content = "";

            //datum resetten
            datum.Stop();
            lbldatum.Content = "";

            //tekst resetten
            lbltekst.Content = "";

            //wekker resetten
            wekker.Stop();
            lblwkkr.Content = "";
            lblwkkrklok.Content = "";
        }

     
    }
}
