using SlimWifiConfig.Model;
using SlimWifiConfig.Model.DataBase;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SlimWifiConfig.View
{
    /// <summary>
    /// Interaction logic for DataLogging.xaml
    /// </summary>
    public partial class DataLogging : Page
    {
        private LocalServerConfiguration _ServerConfiguration;
        private DataLoggerDbContext _DataBase;

        private UdpClient _UdpServer;
        private IPEndPoint groupEP;
        private bool _UdpServerRunning;

        public DataLogging(LocalServerConfiguration ServerConfiguration)
        {
            InitializeComponent();
            _ServerConfiguration = ServerConfiguration;
            _DataBase = new DataLoggerDbContext();   
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        { 
            Task.Run(async () =>
            {
                Dispatcher.Invoke(() =>
                {
                    _DataBase.Records.Load();
                    RecordsDataGrid.ItemsSource = _DataBase.Records.Local;
                });         
            });
        }

        private void StartUdpServerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_UdpServerRunning)
            {
                _UdpServer.Close();
                _UdpServerRunning = false;
                StartUdpServerButton.Content = "Start";
                ServerInformationStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                try
                {
                    int port = int.Parse(_ServerConfiguration.LocalPort);
                    _UdpServer = new UdpClient(port);
                    groupEP = new IPEndPoint(IPAddress.Parse(_ServerConfiguration.LocalAddress), port);
                    Task.Run(async () =>
                    {
                        UdpServerListener();
                    });
                    _UdpServerRunning = true;
                    StartUdpServerButton.Content = "Stop";
                    ServerIpTextBox.Text = _ServerConfiguration.LocalAddress;
                    ServerPortTextBox.Text = _ServerConfiguration.LocalPort;
                    ServerInformationStackPanel.Visibility = Visibility.Visible;
                    Console.WriteLine("UDP Server started");
                }
                catch (Exception)
                {
                    Console.WriteLine("UDP Server ERROR");
                }
            }
            
        }

        private void UdpServerListener()
        {
            try
            {
                while (true)
                {
                    byte[] bytes = _UdpServer.Receive(ref groupEP);
                    string data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    
                    var rec = new Record();
                    rec.Data = data.Trim();
                    rec.TimeStamp = DateTime.Now;

                    Dispatcher.Invoke(() =>
                    {
                        _DataBase.Records.Add(rec);
                        _DataBase.SaveChanges();
                    });

                }
            }
            catch (Exception)
            {
                Console.WriteLine("UDP Server stopped");
            }
        }

        private void ClearRecordsButton_Click(object sender, RoutedEventArgs e)
        {
            _DataBase.Records.RemoveRange(_DataBase.Records);
            _DataBase.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Records',RESEED,0);");
            _DataBase.SaveChanges();
            _DataBase.Records.Load();
            RecordsDataGrid.ItemsSource = _DataBase.Records.Local;
        }
    }
}
