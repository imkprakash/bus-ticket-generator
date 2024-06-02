using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TicketGen
{
    internal class TicketInfo
    {
        string id = "";
        string origin = "";
        string destination = "";
        UInt64 distance = 0;
        UInt64 fare = 0;
        UInt64 amount = 0;
        UInt64 num = 0;
        UInt64 passengers = 0;
        string date = "";
        string time = "";

        public TicketInfo(string origin, string destination, UInt64 distance, UInt64 fare, UInt64 amount, UInt64 num, UInt64 passengers, string date, string time)
        {

            this.date = date;
            this.time = time;

            this.id = this.date.Remove(0, 2).Remove(5, 1).Remove(2, 1) + this.time.Remove(5, 1).Remove(2, 1) + num.ToString("000");
            this.origin = origin;
            this.destination = destination;
            this.distance = distance;
            this.fare = fare;
            this.amount = amount;
            this.num = num;

            this.passengers = passengers;


        }

        public string Ticket_Id
        {
            get { return id; }
        }

        public string Ticket_Origin
        {
            get { return origin; }
        }

        public string Ticket_Destination
        {
            get { return destination; }

        }

        public UInt64 Distance
        {
            get { return distance; }
        }

        public UInt64 Fare
        {
            get { return fare; }
        }

        public UInt64 Amount
        {
            get { return amount; }
        }

        public UInt64 Num
        {
            get { return num; }
        }

        public UInt64 Passengers
        {
            get { return passengers; }
        }

        public string Date
        {
            get { return date; }
        }

        public string Time
        {
            get { return time; }
        }

        public void Insert_Details()
        {
            string filename = "tickets.csv";
            string details = this.id + "," + this.origin + "," + this.destination + "," + this.distance + "," + this.fare + "," + this.passengers + "," + this.amount + "," + this.date + "," + this.time + "\n";
            File.AppendAllText(filename, details);

        }
    }
}
