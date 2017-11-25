using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace NewtonicServer.Server
{
    class ClientConnection
    {
        public Guid ClientId;

        private Socket Sock;
        private SocketAsyncEventArgs SockAsyncEventArgs;
        private byte[] buff;

        public event EventHandler<ByteArrayEventArgs> Received;

        object lockObj = new object();

        protected void OnReceived(byte[] bytes)
        {
            if (Received != null) Received.Invoke(this, new ByteArrayEventArgs { Bytes = bytes });
        }

        public ClientConnection(Socket AcceptedSocket, Guid clientId)
        {
            AcceptedSocket.ReceiveTimeout = 500;
            AcceptedSocket.SendTimeout = 500;
            ClientId = clientId;
            buff = new byte[1024];
            Sock = AcceptedSocket;
            SockAsyncEventArgs = new SocketAsyncEventArgs();
            SockAsyncEventArgs.Completed += SockAsyncEventArgs_Completed;
            SockAsyncEventArgs.SetBuffer(buff, 0, buff.Length);

            ReceiveAsync(SockAsyncEventArgs);
        }

        private void SockAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
            }
        }

        private void ReceiveAsync(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent = Sock.ReceiveAsync(e);
            if (!willRaiseEvent)
                ProcessReceive(e);
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                OnReceived(e.Buffer.Take(e.BytesTransferred).ToArray());
                ReceiveAsync(SockAsyncEventArgs);
            }
        }

        public void Send(byte[] data)
        {
            lock (lockObj)
            {
                Sock.Send(data);
            }
        }


    }
}
