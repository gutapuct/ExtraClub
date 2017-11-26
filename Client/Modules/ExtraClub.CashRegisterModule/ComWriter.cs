using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ExtraClub.CashRegisterModule
{
    class ComWriter
    {
        int ComPortNumber = 3;

        public void WriteCommand()
        {
            SerialPort port = new SerialPort("COM" + ComPortNumber, 57600, Parity.None, 8, StopBits.One);
            port.Open();

            port.Close();
        }
    }
}
