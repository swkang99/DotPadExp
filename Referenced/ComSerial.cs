using System;
using System.IO.Ports;
using System.Threading;
using DotPadPlugin;
using System.Diagnostics;
using Dot_Test_APP;
using System.Collections.Generic;

namespace Dot320A
{
    public class ComSerial
    {
        public SerialPort sPort = new SerialPort();
        private Thread mSerialThread = null;

        private Thread mRXThread = null;
        private Thread mProcessingThread = null;
        private Thread mProcessingLP = null;


        private List<byte> medBuffer = new List<byte>();
        private List<Tuple<int, byte[]>> rxBufferLogs = new List<Tuple<int, byte[]>>();

        public Dot_Packet dot_packet = new Dot_Packet();

        public static bool[] shortPressedKeys = { false, false, false, false, false, false };
        public static bool[] longPressedKeys = { false, false, false, false, false, false };

        public static bool[] longPressedBeforeRelease_pann= { false, false};
        public static bool[] longPressedBeforeRelease_func = { false, false, false, false};

        public static long thresholdLongPress = 900;

        private static ushort R_Idx = 0;
        private static ushort P_Idx = 0;
        private static Byte[] RxBuffer = new byte[400];

        private static bool[] panningKeys = { false, false };
        private static Stopwatch[] stopWatchPanns = new Stopwatch[2] {new Stopwatch(), new Stopwatch() };
        private static bool[] functionKeys = { false, false, false, false};
        private static Stopwatch[] stopWatchFuncs = new Stopwatch[4] { new Stopwatch(), new Stopwatch(), new Stopwatch(), new Stopwatch()};

        private static byte[] dotpadBytes_Static = new byte[] { 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0xF1, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8 };
        private static byte[] dotpadBytes_INIT = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private static Stopwatch stopWatchDisp = new Stopwatch();

        private static Stopwatch stopWatchTest = new Stopwatch();

        public static List<Dot_Packet> RSP_packets = new List<Dot_Packet>();
        public static List<Dot_Packet> NTF_packets = new List<Dot_Packet>();
        private static void ClearRxBuffer()
        {
            Array.Clear(RxBuffer, 0, RxBuffer.Length);
            R_Idx = P_Idx = 0;
        }
        public string[] getAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }
        public void test(string argPortName)
        {
            sPort.PortName = argPortName;
            sPort.BaudRate = 115200;
            sPort.DataBits = 8;
            sPort.StopBits = StopBits.One;
            sPort.Parity = Parity.None;

            try
            {
                sPort.Open();
            }
            catch (Exception)
            {
                Console.WriteLine("Port Open Failed!");
            }

            try
            {
                sPort.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Port Close Failed!");
            }
            try
            {
                sPort.Open();
                
            }
            catch (Exception)
            {
                Console.WriteLine("Port ReOpen Failed!");
            }
        }
        public bool openSerialPort(string argPortName)
        {
            sPort.PortName = argPortName;
            sPort.BaudRate = 115200;
            sPort.DataBits = 8;
            sPort.StopBits = StopBits.One;
            sPort.Parity = Parity.None;

            try
            {
                sPort.Open();
            }
            catch (Exception)
            {
                return false;
            }

            Console.WriteLine("Port Open");
            Console.WriteLine(sPort.BytesToRead);

            //mSerialThread = new Thread(new ThreadStart(FuncSerialThread));
            //mSerialThread.Start();

            mRXThread = new Thread(new ThreadStart(FuncRXThread));
            mRXThread.Start();
            mProcessingThread = new Thread(new ThreadStart(FuncProcessingThread));
            mProcessingThread.Start();
            mProcessingLP = new Thread(new ThreadStart(FuncProcessingLP));
            mProcessingLP.Start();

            return checkOpenPort();
        }
        public void closeThreads()
        {
            mRXThread.Join();
            mProcessingThread.Join();
            mProcessingLP.Join();
        }

        public bool openSerialPort()
        {
            try
            {
                sPort.Open();
            }
            catch (Exception)
            {
                return false;
            }


            return checkOpenPort();
        }

        public bool closeSerialPort()
        {
            try
            {
                sPort.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return checkOpenPort();
        }


        public void writeSerialPort(byte[] argData)
        {
            try
            {
                sPort.Write(argData, 0, argData.Length);
            }
            catch (System.InvalidOperationException)
            {

            }
        }

        public bool checkOpenPort()
        {
            if (sPort.IsOpen)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void FuncRXThread()
        {
            while (true)
            {
                if (sPort.IsOpen)
                {
                    try
                    {
                        int numBytes = sPort.BytesToRead;
                        if(numBytes > 0)
                        {
                            medBuffer.Add((byte)sPort.ReadByte());
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    break;
                }
                
            }
        }
        private void FuncProcessingLP()
        {
            while(true)
            {
                if (sPort.IsOpen)
                {
                    try
                    {
                        for (int i = 0; i < stopWatchFuncs.Length; i++)
                        {
                            if (longPressedBeforeRelease_func[i]) continue;

                            Stopwatch watch = stopWatchFuncs[i];
                            if (watch.IsRunning && watch.ElapsedMilliseconds > thresholdLongPress)
                            {
                                longPressedBeforeRelease_func[i] = true;
                                watch.Stop();
                            }
                        }
                        for (int i = 0; i < stopWatchPanns.Length; i++)
                        {
                            if (longPressedBeforeRelease_pann[i]) continue;

                            Stopwatch watch = stopWatchPanns[i];
                            if (watch.IsRunning && watch.ElapsedMilliseconds > thresholdLongPress)
                            {
                                longPressedBeforeRelease_pann[i] = true;
                                watch.Stop();
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    break;
                }
            }
        }
        private void FuncProcessingThread()
        {
            string mRxData = "";

            while (true)
            {

                

                if (sPort.IsOpen)
                {
                    try
                    {
                        if(medBuffer.Count > 0)
                        {
                            RxBuffer[R_Idx++] = medBuffer[0];
                            medBuffer.RemoveAt(0);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    while (R_Idx != P_Idx)
                    {
                        if (dot_packet.SYNC_1 == false && dot_packet.SYNC_2 == false)
                        {
                            if (RxBuffer[P_Idx++] == 0xAA)
                            {
                                dot_packet.SYNC_1 = true;
                                mRxData = "";
                            }
                        }
                        else if (dot_packet.SYNC_1 == true && dot_packet.SYNC_2 == false)
                        {
                            if (RxBuffer[P_Idx++] == 0x55)
                            {
                                dot_packet.SYNC_2 = true;
                                dot_packet.IDX_PARSE = 0;
                                Array.Clear(dot_packet.DATA, 0, dot_packet.DATA.Length);
                            }
                            else
                            {
                                dot_packet.SYNC_1 = false;
                            }
                        }
                        else if (dot_packet.SYNC_1 == true && dot_packet.SYNC_2 == true)
                        {
                            switch (dot_packet.IDX_PARSE)
                            {
                                case 0:     //Length High Byte.
                                    dot_packet.Length = (ushort)(RxBuffer[P_Idx++] << 8);
                                    break;
                                case 1:     //Length Low Byte.
                                    dot_packet.Length += RxBuffer[P_Idx++];
                                    //workaround: fix invalid length for noise data, min /max length
                                    if ((dot_packet.Length < 5) || (dot_packet.Length > dot_packet.DATA.Length))
                                    {
                                        dot_packet.SYNC_1 = dot_packet.SYNC_2 = false;
                                    }
                                    break;
                                case 2:     //DEST_ID
                                    dot_packet.DEST_ID = RxBuffer[P_Idx++];
                                    break;
                                case 3:     //CMD High Byte.
                                    dot_packet.CMD_H = RxBuffer[P_Idx++];
                                    break;
                                case 4:     //CMD Low Byte.
                                    dot_packet.CMD_L = RxBuffer[P_Idx++];
                                    break;
                                case 5:     //SEQ NUM.
                                    dot_packet.SEQ_NUM = RxBuffer[P_Idx++];
                                    break;
                                default:    //Data.
                                    if (dot_packet.IDX_PARSE > dot_packet.Length)   //CheckSum.
                                    {
                                        dot_packet.CS_DATA = RxBuffer[P_Idx++];

                                        //if ((dot_packet.CMD_L & 0x01) != 0)
                                        //{
                                        //    mRxData += "RESP: AA55";
                                        //}
                                        //else if ((dot_packet.CMD_L & 0x02) != 0)
                                        //{
                                        //    mRxData += "NOTI: AA55";
                                        //}

                                        //mRxData += dot_packet.Length.ToString("X4");
                                        //mRxData += dot_packet.DEST_ID.ToString("X2");
                                        //mRxData += dot_packet.CMD_H.ToString("X2");
                                        //mRxData += dot_packet.CMD_L.ToString("X2");
                                        //mRxData += dot_packet.SEQ_NUM.ToString("X2");

                                        //for (int i = 0; i < (dot_packet.Length - 5); i++)
                                        //{
                                        //    mRxData += dot_packet.DATA[i].ToString("X2");
                                        //}


                                        //mRxData += dot_packet.CS_DATA.ToString("X2");

                                        if (dot_packet.CMD_H == 0x03 && dot_packet.CMD_L == 0x12)
                                        {
                                            //Console.WriteLine("It is NOTI_KEY_PERKINS!");
                                            for (int i = 0; i < panningKeys.Length; i++)
                                            {
                                                bool Pi = (dot_packet.DATA[1] & (1 << (panningKeys.Length - i))) != 0;
                                                if (panningKeys[i] == true && !Pi)
                                                {
                                                    if(stopWatchPanns[i].IsRunning) stopWatchPanns[i].Stop();
                                                    //if (i == 0) Console.WriteLine("LEFT PANN IS RELEASED!");
                                                    //else Console.WriteLine("RIGHT PANN IS RELEASED!");
                                                    //Console.WriteLine("The duration is " + stopWatchPanns[i].ElapsedMilliseconds + "ms");

                                                    panningKeys[i] = false;

                                                    if (stopWatchPanns[i].ElapsedMilliseconds > thresholdLongPress)
                                                    {
                                                        if (i == 0) longPressedKeys[0] = true;
                                                        else longPressedKeys[5] = true;
                                                    }
                                                    else
                                                    {
                                                        if (i == 0) shortPressedKeys[0] = true;
                                                        else shortPressedKeys[5] = true;
                                                    }
                                                }
                                                else if (panningKeys[i] == false && Pi)
                                                {
                                                    //if (i == 0) Console.WriteLine("LEFT PANN IS PRESSED!");
                                                    //else Console.WriteLine("RIGHT PANN IS PRESSED!"); ;

                                                    panningKeys[i] = true;
                                                    if (i == 0)
                                                    {
                                                        longPressedKeys[0] = false;
                                                        shortPressedKeys[0] = false;
                                                    }
                                                    else
                                                    {
                                                        longPressedKeys[5] = false;
                                                        shortPressedKeys[5] = false;
                                                    }
                                                    stopWatchPanns[i].Restart();
                                                }
                                            }
                                        }
                                        else if (dot_packet.CMD_H == 0x03 && dot_packet.CMD_L == 0x32)
                                        {
                                            //Console.WriteLine("It is NOTI_KEY_FUNCTION!");
                                            for (int i = 0; i < functionKeys.Length; i++)
                                            {
                                                bool Fi = (dot_packet.DATA[0] & (1 << (4 + functionKeys.Length - 1 - i))) != 0;
                                                if (functionKeys[i] == true && !Fi)
                                                {
                                                    if (stopWatchFuncs[i].IsRunning) stopWatchFuncs[i].Stop();
                                                    //Console.WriteLine("F" + (i+1) + " IS RELEASED!");
                                                    //Console.WriteLine("The duration is " + stopWatchFuncs[i].ElapsedMilliseconds + "ms");
                                                    
                                                    functionKeys[i] = false;
                                                    if (stopWatchFuncs[i].ElapsedMilliseconds > thresholdLongPress)
                                                    {
                                                        longPressedKeys[i + 1] = true;
                                                    }
                                                    else
                                                    {
                                                        shortPressedKeys[i + 1] = true;
                                                    }
                                                }
                                                else if (functionKeys[i] == false && Fi)
                                                {
                                                    //Console.WriteLine("F" + (i+1) + " IS PRESSED!");
                                                    functionKeys[i] = true;
                                                    longPressedKeys[i+1] = false;
                                                    shortPressedKeys[i+1] = false;

                                                    stopWatchFuncs[i].Restart();
                                                }
                                            }
                                        }
                                        else if (dot_packet.CMD_H == 0x02 && dot_packet.CMD_L == 0x01)
                                        {
                                            RSP_packets.Add(dot_packet);
                                        }
                                        else if (dot_packet.CMD_H == 0x02 && dot_packet.CMD_L == 0x02)
                                        {
                                            NTF_packets.Add(dot_packet);
                                        }
                                        //else if (dot_packet.CMD_H == 0x02 && dot_packet.CMD_L == 0x22)
                                        //{
                                        //    stopWatchDisp.Stop();
                                        //    Console.WriteLine("The duration is " + stopWatchDisp.ElapsedMilliseconds + "ms");
                                        //}
                                        //Console.WriteLine(mRxData);

                                        dot_packet.SYNC_1 = dot_packet.SYNC_2 = false;
                                        //Console.WriteLine(mRxData);


                                        ClearRxBuffer();
                                    }
                                    else
                                    {
                                        dot_packet.DATA[dot_packet.IDX_PARSE - 6] = RxBuffer[P_Idx++];
                                    }
                                    break;
                            }

                            if (dot_packet.SYNC_1 && dot_packet.SYNC_2)
                            {
                                dot_packet.IDX_PARSE++;
                            }
                        }
                    }

                }
                else
                {
                    break;
                }
            }
        }

    }
}
