using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace NewtonicServer.Server
{
    class TextProcessor
    {
        private object lockObj = new object();

        private Server Server;

        Timer timer;

        Dictionary<Guid, TextInfo> currentText = new Dictionary<Guid,TextInfo>();

        public TextProcessor(Server srv)
        {
            Server = srv;
            timer = new Timer(335);
            timer.Elapsed += timer_Elapsed;

            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (lockObj)
            {
                foreach (var pair in currentText)
                {
                    pair.Value.Delta();
                    System.Diagnostics.Debug.Write("*");
                    System.Diagnostics.Debug.WriteLine(pair.Value.DeltedLine1);
                    Server.SendText(pair.Key, pair.Value.DeltedLine1, pair.Value.DeltedLine2);
                }
            }
        }

        internal void SetText(ClientInfo client, string line1, string line2, int timeout)
        {
            lock (lockObj)
            {
                if (currentText.ContainsKey(client.HardwareId))
                {
                    if (currentText[client.HardwareId].Line1 != line1 || currentText[client.HardwareId].Line2 != line2)
                    {
                        Logger.Log(client.Treatment.DisplayName + " (текст)", (line1 ?? "") + ((", " + line2) ?? ""));
                        currentText[client.HardwareId] = new TextInfo(line1, line2, timeout);
                    }
                }
                else
                {
                    currentText.Add(client.HardwareId, new TextInfo(line1, line2, timeout));
                    Logger.Log(client.Treatment.DisplayName + " (текст)", (line1 ?? "") + ((", " + line2) ?? ""));
                }
            }
        }
    }
}
