using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace NewtonicServer.Server
{
    public class Server
    {
        object lockObj = new object();

        private Socket Sock;
        private SocketAsyncEventArgs AcceptAsyncArgs;

        private Dictionary<Guid, ClientConnection> connected = new Dictionary<Guid, ClientConnection>();

        public event EventHandler<GuidEventArgs> ClientConnected;
        public event EventHandler<GuidEventArgs> ClientDisonnected;
        public event EventHandler<MessageEventArgs> Received;

        internal List<Guid> Ignored = new List<Guid>();

        private Guid _selectedClient;
        public Guid SelectedClient
        {
            get
            {
                return _selectedClient;
            }
            set
            {
                _selectedClient = value;
            }
        }

        private void OnClientConnected(Guid id)
        {
            if (ClientConnected != null) ClientConnected.Invoke(this, new GuidEventArgs { Id = id });
        }

        private void OnClientDisonnected(Guid id)
        {
            if (ClientDisonnected != null) ClientDisonnected.Invoke(this, new GuidEventArgs { Id = id });
        }

        public Server()
        {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            AcceptAsyncArgs = new SocketAsyncEventArgs();
            AcceptAsyncArgs.Completed += AcceptCompleted;
        }

        public void Start(int Port)
        {
            Sock.Bind(new IPEndPoint(IPAddress.Any, Port));
            Sock.Listen(150);
            Sock.ReceiveTimeout = 500;
            Sock.SendTimeout = 500;
            AcceptAsync(AcceptAsyncArgs);
        }

        private void AcceptAsync(SocketAsyncEventArgs e)
        {
            try
            {
                bool willRaiseEvent = Sock.AcceptAsync(e);
                if (!willRaiseEvent)
                    AcceptCompleted(Sock, e);
            }
            catch(ObjectDisposedException)
            {
            }
        }

        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                var id = Guid.NewGuid();
                connected.Add(id, new ClientConnection(e.AcceptSocket, id));
                OnClientConnected(id);
                connected[id].Received += new EventHandler<ByteArrayEventArgs>(Server_Received);
            }
            e.AcceptSocket = null;
            AcceptAsync(AcceptAsyncArgs);
        }

        void Server_Received(object sender, ByteArrayEventArgs e)
        {
            if (Ignored.Contains(((ClientConnection)sender).ClientId)) return;
            if (Received != null)
            {
                Received.Invoke(this, new MessageEventArgs { Id = ((ClientConnection)sender).ClientId, Bytes = e.Bytes });
            }
        }

        public void Stop()
        {
            Sock.Close();
        }

        public void Send(Guid clientId, byte[] data)
        {
            if (connected.ContainsKey(clientId))
            {
                connected[clientId].Send(data);
            }
        }

        internal void SendText(Guid clientId, string line1, string line2)
        {
            if (connected.ContainsKey(clientId))
            {

                List<byte> bytes = new List<byte> { 3 };
                var bts = ASCIIEncoding.Default.GetBytes(line1 ?? "");
                if (bts.Length > 255)
                {
                    bts = bts.Take(255).ToArray();
                }
                bytes.Add((byte)bts.Length);
                bytes.AddRange(bts);
                bts = ASCIIEncoding.Default.GetBytes(line2 ?? "");
                if (bts.Length > 255)
                {
                    bts = bts.Take(255).ToArray();
                }
                bytes.Add((byte)bts.Length);
                bytes.AddRange(bts);

                try
                {
                    connected[clientId].Send(bytes.ToArray());
                }
                catch (SocketException)
                {
                    connected.Remove(clientId);
                    Logger.Log("Клиент " + clientId + " отвалился!");
                    OnClientDisonnected(clientId);
                }
            }
        }

        internal void IgnoreClient(Guid hardwareId)
        {
            Ignored.Add(hardwareId);
        }
    }
}
