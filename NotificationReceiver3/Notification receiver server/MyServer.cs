using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NotificationReceiver3
{
    class MyServer
    {
        static Socket ThisSocket;
        static List<string> ReceivedNotificationsList { get; set; }

        public static bool StatusChanged = false;

        public static void RunServer()
        {
            ReceivedNotificationsList = new List<string>();

            ThisSocket = new Socket(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 9090);
            ThisSocket.Bind(endPoint);
            Debug.WriteLine("Listening " + endPoint.ToString());
            ThisSocket.Listen(1);

            //---incoming client connected---
            Socket client = ThisSocket.Accept();
            Debug.WriteLine("Connected");
            while (true)
            {
                if (client.IsConnected())
                {
                    byte[] b = new byte[1024];
                    int k = client.Receive(b);
                    if (k != 0)
                    {
                        string szReceived = Encoding.UTF8.GetString(b, 0, k);
                        ReceivedNotificationsList.Add(szReceived);
                        StatusChanged = true;
                        Debug.WriteLine("data:\n\n" + szReceived + "\n");
                    }
                }
                else
                {
                    client = ThisSocket.Accept();
                    Debug.WriteLine("Reconnected");
                }
            }

            client.Close();
            ThisSocket.Close();
            Debug.WriteLine("Server stopped");
        }

        public static List<string> GetNotificationsList()
        {
            StatusChanged = false;
            List<string> ReceivedNotificationsListCopy = new List<string>();
            foreach (string s in ReceivedNotificationsList)
            {
                string ss = new string(s.ToCharArray());
                ReceivedNotificationsListCopy.Add(ss);
            }
            ReceivedNotificationsList.Clear();
            return ReceivedNotificationsListCopy;
        }
    }
    static class SocketExtensions
    {
        public static bool IsConnected(this Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }
    }

}