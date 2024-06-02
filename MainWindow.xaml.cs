using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.IO;
using System.Windows.Threading;

namespace TicketGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<int> distance = new List<int>();
        UInt64 count = 1;
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Set interval to 1 second
            timer.Tick += new EventHandler(Timeout);
            timer.Start();

            Initialize_CSV_Report();

            SetupUI();
            
        }

        private void Initialize_CSV_Report()
        {
            string filename = "tickets.csv";
            if (!File.Exists(filename))
            {
                string heading = "Id,Origin,Destination,Distance,Fare,Passengers,Amount,Date(yyyy-MM-dd),Time(HH:mm:ss)\n";
                File.AppendAllText(filename, heading);
            }
        }

        public void SetupUI()
        {
            Read_Route_File();
            DateLabel.Content = "Date: " + DateTime.Now.ToString("D");
            FromComboBox.SelectedIndex = 0;
            ToComboBox.SelectedIndex = ToComboBox.Items.Count-1;

            for(byte i = 1; i <= 10; i++)
            {
                PassengerComboBox.Items.Add(i.ToString());
            }
            PassengerComboBox.SelectedIndex = 0;

            RouteLabel.Content = "Route: " + FromComboBox.Text + " to " + ToComboBox.Text;

            Update_Total();
            
        }


        private void Timeout(Object source, EventArgs e)
        {
            TimeLabel.Content = "Time: " + DateTime.Now.ToString("HH:mm:ss");
            if(DateTime.Now.ToString("HH:mm:ss") == "00:00:00")
            {
                DateLabel.Content = "Date: " + DateTime.Now.ToString("D");
            }

        }

        private void Read_Route_File()
        {
            FromComboBox.Items.Clear();
            ToComboBox.Items.Clear();
            string filename = "Route.txt";
            foreach (string line in File.ReadAllLines(filename))
            {
                string[] parts = line.Split('-');
                if (parts[0] == "Fare")
                {
                    FareAmountLabel.Text = parts[1];
                    break;
                }
                FromComboBox.Items.Add(parts[0]);
                ToComboBox.Items.Add(parts[0]);
                distance.Add(Int32.Parse(parts[1]));
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            string origin = FromComboBox.Text;
            string destination = ToComboBox.Text;
            UInt64 distance = UInt64.Parse(CalculatedDistanceLabel.Text);
            UInt64 fare = UInt64.Parse(FareAmountLabel.Text);
            UInt64 amount = UInt64.Parse(TotalAmountLabel.Text);
            UInt64 num = count++;
            UInt64 passengers = (ulong)(PassengerComboBox.SelectedIndex + 1);
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = TimeLabel.Content.ToString().Remove(0, 6);

            TicketInfo tkt = new TicketInfo(origin, destination, distance, fare, amount, num, passengers, date, time);
            tkt.Insert_Details();
            Update_Details_Section(tkt);
            MainGroupBox.Header = "Last Generated Ticket";

        }

        private void Update_Details_Section(TicketInfo tkt)
        {
            TicketId.Text = tkt.Ticket_Id;
            AuthenticityStatus.Text = "GENERATED";
            TicketDate.Text = tkt.Date;
            TicketTime.Text = tkt.Time;
            TicketRoute.Text = tkt.Ticket_Origin + " to " + tkt.Ticket_Destination;
            TicketDistance.Text = tkt.Distance.ToString();
            TicketFare.Text = tkt.Fare.ToString();
            TicketPassengers.Text = tkt.Passengers.ToString();
            TicketTotal.Text = tkt.Amount.ToString();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            FromComboBox.SelectedIndex = 0;
            ToComboBox.SelectedIndex = ToComboBox.Items.Count - 1;
            PassengerComboBox.SelectedIndex = 0;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            MainGroupBox.Header = "Last Searched Ticket";
            string filename = "tickets.csv";
            string ticketid = TicketId.Text.ToString();
            foreach(string line in File.ReadLines(filename))
            {
                if (line.StartsWith(ticketid))
                {
                    string[] details = line.Split(',');
                    TicketId.Text = details[0];
                    AuthenticityStatus.Text = "FOUND";
                    TicketDate.Text = details[7];
                    TicketTime.Text = details[8];
                    TicketRoute.Text = details[1] + " to " + details[2];
                    TicketDistance.Text = details[3];
                    TicketFare.Text = details[4];
                    TicketPassengers.Text = details[5];
                    TicketTotal.Text = details[6];
                    return;
                }
                
            }
            AuthenticityStatus.Text = "NOT FOUND";
            TicketDate.Text = "";
            TicketTime.Text = "";
            TicketRoute.Text = "";
            TicketDistance.Text = "";
            TicketFare.Text = "";
            TicketPassengers.Text = "";
            TicketTotal.Text = "";

        }

        private void FromComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FromComboBox.SelectedIndex == FromComboBox.Items.Count - 1)
            {
                FromComboBox.SelectedIndex = FromComboBox.SelectedIndex - 1;
            }

            if (ToComboBox.SelectedIndex <= FromComboBox.SelectedIndex)
            {
                ToComboBox.SelectedIndex = FromComboBox.SelectedIndex + 1;
            }
            Update_Calculated_Distance();
            Update_Total();
        }

        private void ToComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToComboBox.SelectedIndex == 0)
            {
                ToComboBox.SelectedIndex = ToComboBox.SelectedIndex + 1;
            }

            if (ToComboBox.SelectedIndex <= FromComboBox.SelectedIndex)
            {
                ToComboBox.SelectedIndex = FromComboBox.SelectedIndex + 1;
            }
            Update_Calculated_Distance();
            Update_Total();
        }

        private void Update_Total()
        {
            if (FareAmountLabel.Text != "0" && CalculatedDistanceLabel.Text != "0" && PassengerComboBox.Text != "")
            {
                TotalAmountLabel.Text = (Int32.Parse(FareAmountLabel.Text) * Int32.Parse(CalculatedDistanceLabel.Text) * (PassengerComboBox.SelectedIndex + 1)).ToString();

            }
        }

        private void Update_Calculated_Distance()
        {


            CalculatedDistanceLabel.Text = (distance[ToComboBox.SelectedIndex] - distance[FromComboBox.SelectedIndex]).ToString();
        }

        private void PassengerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update_Total();

        }

        private void TicketId_TextChanged(object sender, TextChangedEventArgs e)
        {
            string curr_text = TicketId.Text.ToString();

            if (curr_text.Length == 0)
            {
                SearchButton.IsEnabled = false;
                return;
            }
            for(int i = 0; i < curr_text.Length; i++)
            {
                if (Char.IsDigit(curr_text[i]) == false)
                {
                    TicketId.Text = curr_text.Remove(i);
                    SearchButton.IsEnabled = false;
                    TicketId.CaretIndex = i;
                    return;
                }
            }
            if (curr_text.Length == 15)
            {
                SearchButton.IsEnabled = true;
            }
            else
            {
                SearchButton.IsEnabled = false;
            }


        }
    }
}