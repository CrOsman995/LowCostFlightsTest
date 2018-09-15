using AmadeusFlightAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static AmadeusFlightAPI.FlightControl;

namespace KingICTExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            load_data();

        }
         
        
       
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            start();
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }


        private void load_data()
        {
            StreamReader sr = new StreamReader(@"IATACodes.txt");
            string x = sr.ReadToEnd();
            string[] y = x.Split('\n');
            foreach (string s in y)
            {
                cmbOrigin.Items.Add(s);
            }
            StreamReader sr1 = new StreamReader(@"IATACodes.txt");
            string x1 = sr1.ReadToEnd();
            string[] y1 = x1.Split('\n');
            foreach (string s in y1)
            {
                cmbDestination.Items.Add(s);
            }
            StreamReader sr2 = new StreamReader(@"numberOdAdults.txt");
            string x2 = sr2.ReadToEnd();
            string[] y2 = x2.Split('\n');
            foreach (string s in y2)
            {
                cmbAdults.Items.Add(s);
            }
            StreamReader sr3 = new StreamReader(@"currencies.txt");
            string x3 = sr3.ReadToEnd();
            string[] y3 = x3.Split('\n');
            foreach (string s in y3)
            {
                cmbCurrency.Items.Add(s);
            }
            StreamReader sr4 = new StreamReader(@"numberOfResults.txt");
            string x4 = sr4.ReadToEnd();
            string[] y4 = x4.Split('\n');
            foreach (string s in y4)
            {
                cmbNumberOfResult.Items.Add(s);
            }
        }



       

        private void datePickStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime DateLeave = Convert.ToDateTime(datePickStart.SelectedDate.ToString());
            if (DateLeave.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Departure date needs to be in the future!");
                datePickStart.SelectedDate = DateTime.Today;
            }
        }

        private void datePickEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime DateReturn = Convert.ToDateTime(datePickEnd.SelectedDate.ToString());
            DateTime DateLeave = Convert.ToDateTime(datePickStart.SelectedDate.ToString());
            if (DateReturn.Date <= DateLeave.Date)
            {
                MessageBox.Show("Return date needs to be after departure date!");
               
            }
        }

        private void start()
        {
            string QryAPIKey = "JXQ3tAn4sVXSn7ahn7UJSEyQOcDIUxpd";
            var Controls = new FlightControl(QryAPIKey);
            if (cmbOrigin.SelectedItem == null || cmbDestination.SelectedItem==null || datePickStart.SelectedDate==null || datePickEnd.SelectedDate == null)
            {


                MessageBox.Show("Please fill all fields!");
            }
            else
            {

                string QryOrigin = cmbOrigin.SelectedItem.ToString();
                string QryDestination = cmbDestination.SelectedItem.ToString();
                DateTime DateLeave = Convert.ToDateTime(datePickStart.SelectedDate.ToString());
                DateTime DateReturn = Convert.ToDateTime(datePickEnd.SelectedDate.ToString());

                int QryPeople = Convert.ToInt32(cmbAdults.SelectedItem.ToString());
                if (QryPeople == 0)
                {
                    QryPeople = 1;
                }
                else QryPeople = QryPeople;

                string QryCurrency = cmbCurrency.SelectedItem.ToString();
                if (QryCurrency == "")
                {
                    QryCurrency = "USD";
                }
                else QryCurrency = QryCurrency;

                int QryResultsNum = Convert.ToInt32(cmbNumberOfResult.SelectedItem.ToString());

                if (QryResultsNum == 0)
                {
                    QryResultsNum = 5;
                }
                else QryResultsNum = QryResultsNum;

                var Flights = Controls.GetFlights(QryOrigin, QryDestination, DateLeave, DateReturn, QryPeople, QryCurrency, QryResultsNum);
                BindingOperations.ClearAllBindings(lbResults);
                foreach (var flight in Flights.Results)
                {
                    

                    lbResults.Items.Add(new
                    {
                        
                        Currency = Flights.Currency,

                        Origin = flight.Itineraries[0].Outbound.Flights[0].Origin.Airport,
                        Destination = flight.Itineraries[0].Outbound.Flights[flight.Itineraries[0].Inbound.Flights.Count - 1].Destination.Airport,
                        LeaveDate = flight.Itineraries[0].Outbound.Flights[0].Departs_at,
                        ReturnDate = flight.Itineraries[0].Inbound.Flights[0].Departs_at,
                        Transfers = flight.Itineraries[0].Outbound.Flights.Count - 1,
                        Fare = flight.Fare.Total_price,
                        Passangers = QryPeople


                    });
                }





              
            }
        }
    }
    
}
