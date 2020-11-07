using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Windows.Gaming.Input;

namespace SlimWifiConfig.Service
{
    public class CommandProcessingService
    {
        private SerialPort _serialPort;
        private static bool _isExecuting;
        private static string _responseBuffer;

        public bool IsExecuting
        { 
            get 
            { 
                return _isExecuting;
            }
        }

        private static CancellationTokenSource _source;

        public delegate void OnParseSuccessResponseDelegate(string CommandSuccessResponse);

        private static event OnParseSuccessResponseDelegate _onParseSuccessResponse;

        public event OnParseSuccessResponseDelegate OnParseSuccessResponse
        {
            add { _onParseSuccessResponse += value; }
            remove { _onParseSuccessResponse -= value; }
        }


        public delegate void OnParseFailResponseDelegate(string CommandFailResponse);

        private static event OnParseFailResponseDelegate _onParseFailResponse;

        public event OnParseFailResponseDelegate OnParseFailResponse
        {
            add { _onParseFailResponse += value; }
            remove { _onParseFailResponse -= value; }
        }


        public delegate void OnCommandSuccessDelegate();

        private static event OnCommandSuccessDelegate _onCommandSuccess;

        public event OnCommandSuccessDelegate OnCommandSuccess
        {
            add { _onCommandSuccess += value; }
            remove { _onCommandSuccess -= value; }
        }


        public delegate void OnCommandFailDelegate();

        private static event OnCommandFailDelegate _onCommandFail;

        public event OnCommandFailDelegate OnCommandFail
        {
            add { _onCommandFail += value; }
            remove { _onCommandFail -= value; }
        }


        public delegate void OnCommandTimeoutDelegate();

        private static event OnCommandTimeoutDelegate _onCommandTimeout;

        public event OnCommandTimeoutDelegate OnCommandTimeout
        {
            add { _onCommandTimeout += value; }
            remove { _onCommandTimeout -= value; }
        }


        public CommandProcessingService(SerialPort Port)
        {
            _isExecuting = false;        
            _serialPort = Port;
            _responseBuffer = "";
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandler);
        }

        public void ExecuteCommand(string Command, int timeout)
        {
            if (!_isExecuting)
            {
                _isExecuting = true; // Command execution has started
                _source = new CancellationTokenSource();
                _serialPort.Write($"{Command}\r\n");
                StartCommandTimeout(timeout);
            }
            else
            {
                Console.WriteLine($"Cannot start executing command {Command}. There is an ongoing operation!");
            }
        }

        private void StartCommandTimeout(int timeout)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(timeout, _source.Token);
                    if (_isExecuting)
                    {
                        Console.WriteLine("TIMEOUT - No response received");
                        _isExecuting = false;
                        _onCommandTimeout?.Invoke();
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Time STOPED - Response received");
                }
            });   
        }

        private static string ClearResponse(string RawResponse, string EndingToken)
        {
            if (RawResponse.StartsWith("AT+") || RawResponse.StartsWith("AT\r\n"))  // Echoeing back is enabled
            {
                // Remove first line of response - issued command echoed back
                RawResponse = RawResponse.Substring(RawResponse.IndexOf("\n") + 1);
            }
            return RawResponse.Replace(EndingToken, "").Replace("\r\n",";");
        }

        private static void SerialDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort Port = (SerialPort)sender;
            if (_isExecuting)
            {
                _responseBuffer += Port.ReadExisting();
                if (_responseBuffer.Contains("\r\nOK\r\n"))
                {
                    _source.Cancel();
                    _isExecuting = false;
                    string CleanResponse = ClearResponse(_responseBuffer, "\r\nOK\r\n");
                    Console.WriteLine($"**OK**Response is: {CleanResponse}");
                    _onParseSuccessResponse?.Invoke(CleanResponse);
                    _onCommandSuccess?.Invoke();
                    _responseBuffer = "";
                }
                else if (_responseBuffer.Contains("\r\nERROR\r\n"))
                {
                    _source.Cancel();
                    _isExecuting = false;
                    string CleanResponse = ClearResponse(_responseBuffer, "\r\nERROR\r\n");
                    Console.WriteLine($"**ERROR**Response is: {CleanResponse}");
                    _onParseFailResponse?.Invoke(CleanResponse);
                    _onCommandFail?.Invoke();
                    _responseBuffer = "";
                }
                else if (_responseBuffer.Contains("\r\nFAIL\r\n"))
                {
                    _source.Cancel();
                    _isExecuting = false;
                    string CleanResponse = ClearResponse(_responseBuffer, "\r\nFAIL\r\n");
                    Console.WriteLine($"**FAIL**Response is: {CleanResponse}");
                    _onParseFailResponse?.Invoke(CleanResponse);
                    _onCommandFail?.Invoke();
                    _responseBuffer = "";
                }
                else if (_responseBuffer.Contains("ready"))
                {
                    /*Experimental*/
                    //_source.Cancel();
                    //_isExecuting = false;
                    _responseBuffer = "";
                }
            }
            else
            {
                Port.DiscardInBuffer();
            }
        }
    }
}
