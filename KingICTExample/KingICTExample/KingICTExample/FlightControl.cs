using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KingICTExample
{
    class FlightControl
    {
        public string APIKey { get; set; }
        public string RawJSONResponse { get; set; }

       
        public FlightControl(string apiKey)
        {
            if (String.IsNullOrWhiteSpace(apiKey)) throw new System.ArgumentException("API Key is require for all operations.");
            this.APIKey = apiKey;
        }


       
        public FlightResponse GetFlights(string qryOrigin, string qryDestination, DateTime DateLeave, DateTime DateReturn, int qryPeople, string qryCurrency, int qryResultsNum)
        {
            if (qryResultsNum > 250) throw new System.ArgumentException("Maximum number of results can't be over 250", "qryResultsNum");

           // if (DateLeave.Date <= DateTime.Now.Date) throw new System.ArgumentException("Departure date needs to in the future", "DateLeave");
            string qryDateLeave = DateLeave.ToString("yyyy-MM-dd");

            string qryDateReturn = String.Empty;
            if (DateReturn != null)
            {
                if (((DateTime)DateReturn).Date < DateLeave.Date) throw new System.ArgumentException("Return date can't be before departure date", "DateReturn");
                qryDateReturn = DateReturn.ToString("yyyy-MM-dd");
            }


            ;
            if (File.Exists($"{qryOrigin}-{qryDestination}-{qryDateLeave}-{qryDateReturn}-{qryPeople}-{qryCurrency}-{qryResultsNum}.json"))
            {
                using (StreamReader SR = new StreamReader($"{qryOrigin}-{qryDestination}-{qryDateLeave}-{qryDateReturn}-{qryPeople}-{qryCurrency}-{qryResultsNum}.json"))
                {
                    RawJSONResponse = SR.ReadToEnd();
                    return JsonConvert.DeserializeObject<FlightResponse>(RawJSONResponse);
                }
            }
            else
            {
                string TQryOorigin = qryOrigin.Trim();
                string TQryDestination = qryDestination.Trim();
                string TQryCurrency = qryCurrency.Trim();
                string URL = "https://" + $@"api.sandbox.amadeus.com/v1.2/flights/low-fare-search?apikey={APIKey}&origin={TQryOorigin}&destination={TQryDestination}&departure_date={qryDateLeave}&{((qryDateReturn == "") ? "" : $"return_date={qryDateReturn}")}&adults={qryPeople}&currency={TQryCurrency}&number_of_results={qryResultsNum}";
              
                HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(URL);
                Req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse Res = (HttpWebResponse)Req.GetResponse())
                {
                    using (Stream ResStream = Res.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(ResStream))
                        {
                            RawJSONResponse = reader.ReadToEnd();
                            using (StreamWriter SW = new StreamWriter($"{TQryOorigin}-{TQryDestination}-{qryDateLeave}-{qryDateReturn}-{qryPeople}-{TQryCurrency}-{qryResultsNum}.json"))
                            {
                                SW.Write(RawJSONResponse);
                            }
                            
                            return JsonConvert.DeserializeObject<FlightResponse>(RawJSONResponse);
                       
                        }
                    }
                }
              
            }
            
        }
   

      
        public class FlightResponse
        {
            public string Currency { get; set; }
            public List<FlightResults> Results { get; set; }
        }

    
        public class FlightResults
        {
            public List<FlightItineraries> Itineraries { get; set; }
            public FlightFare Fare { get; set; }
        }

       
        public class FlightItineraries
        {
            public FlightXbound Outbound { get; set; }
            public FlightXbound Inbound { get; set; }
        }

  
        public class FlightXbound
        {
            public string Duration { get; set; }
            public List<FlightFlights> Flights { get; set; }

        }

        public class FlightFlights
        {
            public DateTime Departs_at { get; set; }
            public DateTime Arrives_at { get; set; }
            public FlightTerminal Origin { get; set; }
            public FlightTerminal Destination { get; set; }
            public string Marketing_airline { get; set; }
            public string Operating_airline { get; set; }
            public string Flight_number { get; set; }
            public string Aircraft { get; set; }
            public FlightBookingInfo Booking_info { get; set; }
        }

        public class FlightTerminal
        {
            public string Airport { get; set; }
            public string Terminal { get; set; }
        }

        public class FlightBookingInfo
        {
            public string Travel_class { get; set; }
            public string Booking_code { get; set; }
            public int Seats_remaining { get; set; }
        }

        public class FlightFare
        {
            public double Total_price { get; set; }
            public FlightPricePerAdult Price_per_adult { get; set; }
            public FlightRestrictions Restrictions { get; set; }
        }

        public class FlightPricePerAdult
        {
            public string Total_fare { get; set; }
            public string Tax { get; set; }
        }

        public class FlightRestrictions
        {
            public bool Refundable { get; set; }
            public bool Change_penalties { get; set; }
        }
    }
}
