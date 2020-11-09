using SlimWifiConfig.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SlimWifiConfig.View
{
    /// <summary>
    /// Interaction logic for RemoteTerminal.xaml
    /// </summary>
    public partial class RemoteTerminal : Page
    {
        private LocalServerConfiguration _ServerConfiguration;

        private TcpListener _TcpServer;
        NetworkStream _TcpServerStream;
        private bool _TcpServerRunning;

        private TcpClient _TcpClient;
        NetworkStream _TcpClientStream;
        private bool _TcpClientRunning;

        private UdpClient _UdpServer;
        private IPEndPoint groupEP;
        private bool _UdpServerRunning;

        public RemoteTerminal(Model.LocalServerConfiguration ServerConfiguration)
        {
            InitializeComponent();
            _ServerConfiguration = ServerConfiguration;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            ServerPortTextBox.Text = _ServerConfiguration.LocalPort;
        }

        private void ServerCheckButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!ServerSettingsStackPanel.IsVisible)
            {
                ClientSettingsStackPanel.Visibility = Visibility.Collapsed;
                ServerSettingsStackPanel.Visibility = Visibility.Visible;
            }
            
        }

        private void ClientCheckButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!ClientSettingsStackPanel.IsVisible)
            {
                ClientSettingsStackPanel.Visibility = Visibility.Visible;
                ServerSettingsStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (_TcpServerRunning)
            {
                _TcpServer.Stop();
                _TcpServerRunning = false;
                StartServerButton.Content = "Start";
                ServerTypeComboBox.IsEnabled = true;
                ServerPortTextBox.IsEnabled = true;
                ServerIpTextBlock.IsEnabled = true;
            }
            else if (_UdpServerRunning)
            {
                _UdpServer.Close();
                _UdpServerRunning = false;
                StartServerButton.Content = "Start";
                ServerTypeComboBox.IsEnabled = true;
                ServerPortTextBox.IsEnabled = true;
                ServerIpTextBlock.IsEnabled = true;
            }
            else
            {
                string ServerType = ServerTypeComboBox.Text;            
                if (ServerType.Equals("TCP"))
                {
                    try
                    {
                        int ServerPort = int.Parse(ServerPortTextBox.Text);
                        _TcpServer = new TcpListener(IPAddress.Parse(_ServerConfiguration.LocalAddress), ServerPort);
                        _TcpServer.Start();
                        Task.Run(async () =>
                        {
                            TcpServerListener();
                        });
                        _TcpServerRunning = true;
                        StartServerButton.Content = "Stop";
                        ServerErrorTextBlock.Text = "";
                        ServerTypeComboBox.IsEnabled = false;
                        ServerPortTextBox.IsEnabled = false;
                        ServerIpTextBlock.IsEnabled = false;
                    }
                    catch (Exception)
                    {
                        ServerErrorTextBlock.Text = "Cannot create server with this configuration";
                    }
                }
                else
                {
                    try
                    {
                        int ServerPort = int.Parse(ServerPortTextBox.Text);
                        _UdpServer = new UdpClient(ServerPort);
                        groupEP = new IPEndPoint(IPAddress.Parse(_ServerConfiguration.LocalAddress), ServerPort);
                        Task.Run(async () =>
                        {
                            UdpServerListener();
                        });
                        _UdpServerRunning = true;
                        StartServerButton.Content = "Stop";
                        ServerErrorTextBlock.Text = "";
                        ServerTypeComboBox.IsEnabled = false;
                        ServerPortTextBox.IsEnabled = false;
                        ServerIpTextBlock.IsEnabled = false;
                    }
                    catch (Exception)
                    {
                        ServerErrorTextBlock.Text = "Cannot create server with this configuration";
                    }
                }
            }
        }

        private void ClientConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_TcpClientRunning)
            {
                _TcpClient.Close();
                _TcpClientStream.Close();
                _TcpClientRunning = false;
                ClientConnectButton.Content = "Connect";
                ClientIpTextBox.IsEnabled = true;
                ClientPortTextBox.IsEnabled = true;
            }
            else
            {
                try
                {
                    string ServerIp = ClientIpTextBox.Text;
                    int ServerPort = int.Parse(ClientPortTextBox.Text);

                    _TcpClient = new TcpClient(ServerIp, ServerPort);
                    _TcpClientStream = _TcpClient.GetStream();
                    Task.Run(async () =>
                    {
                        TcpClientListener();
                    });
                    _TcpClientRunning = true;
                    ClientConnectButton.Content = "Disconnect";
                    ClientErrorTextBlock.Text = "";
                    ClientIpTextBox.IsEnabled = false;
                    ClientPortTextBox.IsEnabled = false;
                }
                catch (Exception)
                {
                    ClientErrorTextBlock.Text = "Cannot connect to this server";
                }
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string data = InputTextBox.Text; // Grab data to be sent
            InputTextBox.Text = "";
            byte[] msg = Encoding.ASCII.GetBytes(data);
            if (_TcpServerRunning)
            {
                if (null != _TcpServerStream)
                {
                    _TcpServerStream.Write(msg, 0, msg.Length);
                }
                else
                {
                    Console.WriteLine("No clients connected");
                }
            }
            else if (_UdpServerRunning)
            {
                // don't know 
            }
            else if (_TcpClientRunning)
            {
                if (null != _TcpClientStream)
                {
                    _TcpClientStream.Write(msg, 0, msg.Length);
                }
                else
                {
                    Console.WriteLine("No clients connected");
                }
            }
        }

        private void TcpServerListener()
        {
            try
            {
                TcpClient client = _TcpServer.AcceptTcpClient();
                _TcpServerStream = client.GetStream();
                Byte[] bytes = new Byte[1024];
                int bytesRead;
                while (0 != (bytesRead = _TcpServerStream.Read(bytes, 0, bytes.Length)))
                {
                    string data = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    Dispatcher.Invoke(() =>
                    {
                        OutputTextBox.Text += data;
                    });
                }
            }
            catch (Exception)
            {
                Console.WriteLine("TCP Server stopped");
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
                    Dispatcher.Invoke(() =>
                    {
                        OutputTextBox.Text += data;
                    });
                }
            }
            catch (Exception)
            {
                Console.WriteLine("UDP Server stopped");
            }
        }

        private void TcpClientListener()
        {
            try
            {
                Byte[] bytes = new Byte[1024];
                int bytesRead;
                while (0 != (bytesRead = _TcpClientStream.Read(bytes, 0, bytes.Length)))
                {
                    string data = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    Dispatcher.Invoke(() =>
                    {
                        OutputTextBox.Text += data;
                    });
                }
            }
            catch (Exception)
            {
                Console.WriteLine("TCP Client stopped");
            }
        }
    }
}
