using DotPadExp.Data;
using DotPadExp.DotPad.Command;
using DotPadExp.Experiment.Study;
using System.Net.Sockets;
using System.Text;

namespace DotPadExp.Experiment
{
    public class Client(Controller controller)
    {
        private readonly Controller _controller = controller;
        private readonly string _ipAddress = "127.0.0.1";
        private readonly int _port = 7777;
        private readonly Socket _clientSocket = new(
            AddressFamily.InterNetwork, 
            SocketType.Stream, 
            ProtocolType.Tcp
        );

        public void ConnectToServer(string serverIP, int port)
        {
            try
            {
                _clientSocket.Connect(serverIP, port);
                Console.WriteLine("Connected to server");
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Connection failed: " + ex.Message);
            }
        }
        public void SendMessage(string message)
        {
            if (_clientSocket.Connected)
            {
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                _clientSocket.Send(messageBytes);
                Console.WriteLine("Message sent: " + message);
            }
            else
            {
                Console.WriteLine("Not connected to server");
            }
        }

        public string ReceiveMessage()
        {
            string receivedMessage = "";
            if (_clientSocket.Connected)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = _clientSocket.Receive(buffer);
                receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received message: " + receivedMessage);
            }
            else
            {
                Console.WriteLine("Not connected to server");
            }

            return receivedMessage;
        }
        public void CloseConnection()
        {
            _controller.Release();
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public void Listen()
        {
            ConnectToServer(_ipAddress, _port);

            SendMessage("Hello from client!");

            WeightStudy weightStudy = new(_controller);
            
            while (true)
            {
                string[] message = ReceiveMessage().Split(' ');

                if (message != null)
                {
                    _controller.LineDown(Conversion.IntToByte(4));
                    weightStudy.Program(message);

                    if (message.Equals("disconnect"))
                    {
                        break;
                    }
                }
            }
            
            CloseConnection();
        }
    }
}
