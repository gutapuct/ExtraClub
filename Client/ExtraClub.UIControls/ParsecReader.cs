using System;
using System.Threading;
using System.Runtime.InteropServices;
using ExtraClub.Infrastructure;

namespace ExtraClub.UIControls
{
    internal class ParsecReader
    {
        [DllImport("prx08.dll")]
        private static extern int Enumerate();
        [DllImport("prx08.dll")]
        private static extern int GetFirstReader(byte[] array, int outputLen);
        [DllImport("prx08.dll")]
        private static extern int OpenReader(int number);
        [DllImport("prx08.dll")]
        private static extern int CloseReader(int number);
        [DllImport("prx08.dll")]
        private static extern int BeepBlink(int number);
        [DllImport("prx08.dll")]
        private static extern int ReadCardNumber(int number, out Int32 code);

        public event EventHandler<CardEventArgs> CardChanged;
        Thread mainThread;

        private int _readerNumber;

        public ParsecReader(int readerNumber)
        {
            _readerNumber = readerNumber;
        }

        public int ReaderNumber
        {
            get
            {
                return _readerNumber;
            }
        }

        public void StartListening()
        {
            if (!IsListening)
            {
                if (Enumerate() < _readerNumber + 1)
                {
                    Logger.Log(DateTime.Now.ToString() + " No reader!");
                    throw new Exception("No reader!");
                }
                if (OpenReader(_readerNumber) != 0)
                {
                    Logger.Log(DateTime.Now.ToString() + " Can't open reader!");
                    throw new Exception("Can't open reader!");
                }
                mainThread = new Thread(new ThreadStart(ReaderCycle));
                mainThread.Start();
            }
        }

        public void StopListening()
        {
            if (mainThread == null) return;
            mainThread.Abort();
            CloseReader(_readerNumber);
        }

        public bool IsListening
        {
            get
            {
                if (mainThread == null) return false;
                return mainThread.IsAlive;
            }
        }

        private Int32 _oldCardNumber = 0;

        void ReaderCycle()
        {
            while (true)
            {
                Thread.Sleep(300);

                Int32 num;
                var res = ReadCardNumber(_readerNumber, out num);
                if (res != 0)
                {
                    _oldCardNumber = 0;
                    continue;
                }
                if (num != _oldCardNumber)
                {
                    _oldCardNumber = num;
                    if (CardChanged != null)
                    {
                        //BeepBlink(_readerNumber);
                        try
                        {
                            CardChanged.Invoke(this, new CardEventArgs { CardNumber = num });
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            CardChanged.Invoke(this, new CardEventArgs { CardNumber = num });
                        }
                    }
                }
            }
        }
    }
    public class CardEventArgs : EventArgs
    {
        public int CardNumber { get; set; }
    }
}
