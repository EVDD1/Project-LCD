using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace LCDaansturen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //nieuwe klasse SerialPort
        SerialPort serialport;
        public MainWindow()
        {
            InitializeComponent();

            // Maak een object aan van SerialPort
            serialport = new SerialPort();

            //je kan kiezen uit none
            cbxComPorts.Items.Add("None");
            
            //je kan ook kiezen tussen de beschikbare COM-poorten
            foreach (string poort in SerialPort.GetPortNames())
                cbxComPorts.Items.Add(poort);
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

        }
    }
}
