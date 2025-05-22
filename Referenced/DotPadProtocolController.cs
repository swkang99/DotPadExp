using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MainThread;
using MarkerPlugin;
using Microsoft.Office.Interop.PowerPoint;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MyUtility;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using Dot320A;
using Dot_Test_APP;
using MarkerPlugin;
using LiblouisPlugin;

namespace DotPadPlugin
{
    public class DotPadProtocolController
    {
        #region Member Variables
        public static int portNumber = 3;

        private static Stopwatch stopWatchDisp = new Stopwatch();
        private static long thresholdTime_char = 100;
        private static long thresholdTime_detail = 250;
        private static long thresholdTime_Part = 80;
        private static long thresholdTime_All = 2500;

        private static long thresholdTime_All_detail = 230;

        private static Stopwatch stopWatchTest = new Stopwatch();


        private static ComSerial serial = new ComSerial();
        public static List<int> updatedCells = new List<int>();
        public static List<int> updatedCells_text = new List<int>();

        private static bool isInitialized = false;
        public static bool isConnected = false;

        public static byte[] dotpadBytes = new byte[] { 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0xF1, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8 };
        public static byte[] dotpadBytes_Static = new byte[] { 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0xF1, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8 };
        
        public static byte[] dotpadBytes_Static_NormalUI = new byte[] { 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0xF1, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8 };
        public static byte[] dotpadBytes_Static_RollbackUI = new byte[] { 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0xF1, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x2F, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x32, 0x23, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0xF2, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8 };
        
        public static byte[] dotpadBytes_INIT = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public static byte[] dotpadCapture_prev = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

       
        public static byte[] detailRegionBytes = new byte[] { 0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public static byte[] detailRegionBytes_Static = new byte[] { 0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public static byte[] detailRegionBytes_INIT = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public static byte[] detailRegionBytes_prev = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };





        public static int[,] dotpadPinOverlapped = new int[60, 40];
        public static int[,] dotpadPinOverlapped_text = new int[60, 40];


        public static int padWidth = 30;
        public static int padHeight = 10;
        public static int cellWidth = 2;
        public static int cellHeight = 4;

        public static float[] snd_myPosition_a = { 0f, 0f, 0f };
        public static Vector2 snd_myPosition_v = new Vector2(-1, -1);
        public static bool getIsInitialized()
        {
            return isInitialized;
        }
        public enum ERROR_CODE
        {
            DOT_ERROR_INVALIDE_LENGTH = 0x01, // LENGTH ERROR
            DOT_ERROR_INVALID_CMD, // CMD ERROR
            DOT_ERROR_DATA_CORRUPTED, // CRC ERROR
            DOT_ERROR_INVALID_PARAM, // PARAMETER ERROR
            DOT_ERROR_RESPONSE_TIMEOUT // TIMEOUT ERROR
        }

        public static Vector2[] leftTops_rollback = {
         new Vector2(1,2), new Vector2(31,2), new Vector2(1,8), new Vector2(31,8), new Vector2(1, 34), new Vector2(31, 34)
    };
        public static Vector2[] RightBtms_rollback = {
        new Vector2(28,6), new Vector2(58,6), new Vector2(28,32), new Vector2(58,32), new Vector2(28,38), new Vector2(58,38)
    };

        public static int getCellIndex(int x, int y, bool isInBigSlide = false , bool isInPrevSlide = false, bool isInCurSlide = false)
        {
            if (isInBigSlide)
            {
                if (x < 1 || x > 35 || y < 8 || y > 38) return -1;
            }
            else if (isInPrevSlide)
            {
                if (x < 1 || x > 28 || y < 8 || y > 32) return -1;
            }
            else if (isInCurSlide)
            {
                if (x < 31 || x > 58 || y < 8 || y > 32) return -1; 
            }
            else
            {
                if (x < 0 || x > 59 || y < 0 || y > 39) return -1;
            }
            
            return (y / cellHeight) * padWidth + x / cellWidth;
        }
        public static int getPinIndex(int x, int y)
        {
            return (x % cellWidth) * cellHeight + y % 4;
        }
        public static Vector2 convertDotToPinCoordinate(Vector2 pos_dot)
        {
            pos_dot = Utility.getFlooredVector(pos_dot);
            int x = (int)pos_dot.X;
            int y = (int)pos_dot.Y;

            int ci = getCellIndex(x, y);
            int pi = getPinIndex(x, y);

            Vector2 result = new Vector2(ci, pi);
            return result;
        }
        public static void setValueAtDotCoordinate(Vector2 pos_dot, int up , bool isInBigSlide = false, bool isInPrevSlide= false, bool isInCurSlide = false)
        {
            pos_dot = Utility.getFlooredVector(pos_dot);
            int x = (int)pos_dot.X;
            int y = (int)pos_dot.Y;
            int ci = getCellIndex(x, y, isInBigSlide, isInPrevSlide, isInCurSlide);
            if(ci == -1)
            {
                if(isInBigSlide) Console.WriteLine("x < 1 || x > 35 || y < 8 || y > 38 is true");
                else if(isInPrevSlide) Console.WriteLine("(x < 1 || x > 28 || y < 8 || y > 32 is true"); 
                else if(isInCurSlide) Console.WriteLine("x < 31 || x > 58 || y < 8 || y > 32 is true"); 
                else Console.WriteLine("x < 0 || x > 59 || y < 0 || y > 39 is true");
                return;
            }
            int pi = getPinIndex(x, y);

            if(ci < 0 || ci >= cellWidth)

            if (((dotpadBytes_Static[ci] >> pi) & 1) == 1) return; // boundary(static) 영역은 갱신하지 않음

            bool isAlreadyUp = dotpadPinOverlapped[x, y] > 0;

            if (up == 1)
            {
                dotpadPinOverlapped[x, y] = dotpadPinOverlapped[x, y] + 1;
                if(!isAlreadyUp)
                {
                    dotpadBytes[ci] |= (byte)(1 << pi);
                    if(!updatedCells.Contains(ci)) updatedCells.Add(ci);
                }
            }
            else
            {
                if (isAlreadyUp) dotpadPinOverlapped[x, y] = dotpadPinOverlapped[x, y] - 1;
                if (dotpadPinOverlapped[x, y] == 0)
                {
                    dotpadBytes[ci] &= (byte)~(1 << pi);
                    if (!updatedCells.Contains(ci)) updatedCells.Add(ci);
                }
            }
        }
        #endregion
        public static void OnApplicationQuit()
        {
            serial.closeSerialPort();
            serial.closeThreads();
            isInitialized = false;
        }
        public static bool displayInitData()
        {
            stopWatchTest.Restart();
            Byte[] partialDisp = Dot_Protocol.reqCMD_CellDisplay(0x01, 0x00, 0x00, 300, ref dotpadBytes_INIT);
            serial.writeSerialPort(partialDisp);
            stopWatchDisp.Restart();
            bool notf = false;
            while (!notf)
            {
                if (stopWatchDisp.ElapsedMilliseconds > thresholdTime_All)
                {
                    //Console.WriteLine("over thresholdTime_All");
                    break;
                }
                int i = 0;
                for (; i < ComSerial.NTF_packets.Count; i++)
                {
                    Dot_Packet packet = ComSerial.NTF_packets[i];
                    if (packet != null && packet.CMD_H == 0x02 && packet.CMD_L == 0x02 && packet.SEQ_NUM == 0x00 && packet.DATA[0] == 0x00)
                    {
                        notf = true;
                        break;
                    }
                }
                if (notf == true)
                {
                    ComSerial.NTF_packets.RemoveAt(i);
                }
            }
            stopWatchDisp.Stop();

            stopWatchTest.Stop();
            Console.WriteLine("displayInitData : " + stopWatchTest.ElapsedMilliseconds + "ms");
            return true;
        }
        public static bool displayAllData()
        {
            if (MarkerPluginController.getIsInitialized())
            {
                MarkerPluginController.snd_getMarkerPos_i(snd_myPosition_a, MarkerPluginController.xSignReverse, MarkerPluginController.ySignReverse);
                snd_myPosition_a[0] = MarkerPluginController.maxWidth + snd_myPosition_a[0];
                snd_myPosition_a[1] = MarkerPluginController.maxHeight + snd_myPosition_a[1];
                snd_myPosition_v.X = snd_myPosition_a[0];
                snd_myPosition_v.Y = snd_myPosition_a[1];
                snd_myPosition_v = Utility.getFlooredVector(snd_myPosition_v);
            }

            stopWatchTest.Restart();

            Byte[] partialDisp = Dot_Protocol.reqCMD_CellDisplay(0x01, 0x00, 0x00, 300, ref dotpadBytes);
            serial.writeSerialPort(partialDisp);

            stopWatchDisp.Restart();
            bool notf = false;
            while (!notf)
            {
                if (stopWatchDisp.ElapsedMilliseconds > thresholdTime_All)
                {
                    //Console.WriteLine("over thresholdTime_All");
                    break;
                }
                int i = 0;
                for (; i < ComSerial.NTF_packets.Count; i++)
                {
                    Dot_Packet packet = ComSerial.NTF_packets[i];
                    if (packet != null && packet.CMD_H == 0x02 && packet.CMD_L == 0x02 && packet.SEQ_NUM == 0x00 && packet.DATA[0] == 0x00)
                    {
                        notf = true;
                        break;
                    }
                }
                if (notf == true)
                {
                    ComSerial.NTF_packets.RemoveAt(i);
                }
            }
            stopWatchDisp.Stop();
            updatedCells.Clear();

            stopWatchTest.Stop();
            Console.WriteLine("displayAllData : " + stopWatchTest.ElapsedMilliseconds + "ms");
            return true;
        }
        public static bool displayPartData()
        {
            //Console.WriteLine("updatedCells.Count before comparison : " + updatedCells.Count);
            for (int ui = updatedCells.Count - 1; ui >= 0; ui--)
            {
                if (dotpadBytes[updatedCells[ui]] == dotpadCapture_prev[updatedCells[ui]]) updatedCells.RemoveAt(ui);
            }
            //Console.WriteLine("updatedCells.Count after comparison : " + updatedCells.Count);


            if (snd_myPosition_v.X > -1 && snd_myPosition_v.Y > -1)
            {
                //Console.WriteLine("updatedCells.Count before adding rerises : " + updatedCells.Count);
                int ci_marker_prv = getCellIndex((int)snd_myPosition_v.X, (int)snd_myPosition_v.Y);
                int ci_marker_prv_btm = getCellIndex((int)snd_myPosition_v.X, (int)snd_myPosition_v.Y + 3);
                int ci_marker_prv_top = getCellIndex((int)snd_myPosition_v.X, (int)snd_myPosition_v.Y - 3);
                int[] rerises = {ci_marker_prv - 1, ci_marker_prv_btm - 1, ci_marker_prv_top - 1, ci_marker_prv, ci_marker_prv_btm, ci_marker_prv_top, ci_marker_prv + 1, ci_marker_prv_btm + 1, ci_marker_prv_top + 1};
                for (int i = 0; i < rerises.Length; i++)
                {
                    int rerise = rerises[i];
                    if (rerise < 0 || rerise > 299) continue;
                    if (!updatedCells.Contains(rerises[i])) updatedCells.Add(rerises[i]);
                }
                //Console.WriteLine("updatedCells.Count after adding rerises : " + updatedCells.Count);
            }
            if (MarkerPluginController.getIsInitialized())
            {
                if(Program.currentMarker == null)
                {
                    MarkerPluginController.snd_getMarkerPos_i(snd_myPosition_a, MarkerPluginController.xSignReverse, MarkerPluginController.ySignReverse);
                    snd_myPosition_a[0] = MarkerPluginController.maxWidth + snd_myPosition_a[0];
                    snd_myPosition_a[1] = MarkerPluginController.maxHeight + snd_myPosition_a[1];
                    snd_myPosition_v.X = snd_myPosition_a[0];
                    snd_myPosition_v.Y = snd_myPosition_a[1];
                    snd_myPosition_v = Utility.getFlooredVector(snd_myPosition_v);
                }
                else snd_myPosition_v = Program.currentMarker.pos_dot;
            }

            if (updatedCells.Count > 50)
            {
                //Console.WriteLine("updatedCells.Count > 50!");
                List<Tuple<int, int>> coordinates = new List<Tuple<int, int>>();
                foreach (int cell in updatedCells)
                {
                    int x_i = cell % 30; // x값: 30으로 나눈 나머지
                    int y_i = cell / 30; // y값: 30으로 나눈 몫
                    coordinates.Add(Tuple.Create(x_i, y_i));
                }
                Vector2 leftTop = new Vector2(int.MaxValue, int.MaxValue);
                Vector2 rightBottom = new Vector2(int.MinValue, int.MinValue);

                // coordinates 리스트를 순회하면서 최소, 최대값을 찾습니다.
                foreach (var coordinate in coordinates)
                {
                    int x = coordinate.Item1;
                    int y = coordinate.Item2;

                    // x 좌표의 최소값 찾기
                    leftTop.X = Math.Min(leftTop.X, x);
                    // x 좌표의 최대값 찾기
                    rightBottom.X = Math.Max(rightBottom.X, x);
                    // y 좌표의 최소값 찾기
                    leftTop.Y = Math.Min(leftTop.Y, y);
                    // y 좌표의 최대값 찾기
                    rightBottom.Y = Math.Max(rightBottom.Y, y);
                }
                int ci_lt = (int)(leftTop.X  +  30 * leftTop.Y);
                byte lineID = (byte)(ci_lt / 30 + 1);
                byte offset_s = (byte)(ci_lt % 30);

                int ci_rb = (int)(rightBottom.X  + 30 * rightBottom.Y);
                ushort length = (ushort)(ci_rb - ci_lt + 1);
                //Console.WriteLine("dot length for display : " + length);
                long thresholdTime_temp = 50 + 8 * length + 30;
                
                long timeContinuous = 42 + 8 * length;
                long timeAllDiscrete = 50 * updatedCells.Count;
                if(timeContinuous < timeAllDiscrete)
                {
                    Console.WriteLine("timeAllDiscrete > timeContinuous");
                    byte[] data = new byte[length];
                    Array.Copy(dotpadBytes, ci_lt, data, 0, length);
                    Byte[] partialDisp = Dot_Protocol.reqCMD_CellDisplay(lineID, 0x00, offset_s, length, ref data);

                    stopWatchTest.Restart();

                    serial.writeSerialPort(partialDisp);
                    stopWatchDisp.Restart();
                    bool notf = false;
                    while (!notf)
                    {
                        if (stopWatchDisp.ElapsedMilliseconds > thresholdTime_temp)
                        {
                            //Console.WriteLine("over thresholdTime_temp");
                            break;
                        }
                        int i = 0;
                        for (; i < ComSerial.NTF_packets.Count; i++)
                        {
                            Dot_Packet packet = ComSerial.NTF_packets[i];
                            if (packet != null && packet.CMD_H == 0x02 && packet.CMD_L == 0x02 && packet.SEQ_NUM == 0x00 && packet.DATA[0] == 0x00)
                            {
                                notf = true;
                                break;
                            }
                        }
                        if (notf == true)
                        {
                            ComSerial.NTF_packets.RemoveAt(i);
                        }
                    }
                    stopWatchDisp.Stop();
                    updatedCells.Clear();

                    stopWatchTest.Stop();
                    Console.WriteLine("displayPartData of continuous area  : " + stopWatchTest.ElapsedMilliseconds + "ms");
                    return true;
                }
            }

            if (MarkerPluginController.getIsInitialized())
            {
                int ci_marker = getCellIndex((int)snd_myPosition_v.X, (int)snd_myPosition_v.Y);
                int x_m = ci_marker % 30;
                int y_m = ci_marker / 30;

                updatedCells.Sort((cell1, cell2) =>
                {
                    int x_i1 = cell1 % 30; // x값: 30으로 나눈 나머지
                    int y_i1 = cell1 / 30; // y값: 30으로 나눈 몫

                    int x_i2 = cell2 % 30;
                    int y_i2 = cell2 / 30;

                    int distance1 = Utility.CalculateSqrDistance(x_i1, y_i1, x_m, y_m);
                    int distance2 = Utility.CalculateSqrDistance(x_i2, y_i2, x_m, y_m);

                    return distance2.CompareTo(distance1);
                });
            }

            stopWatchTest.Restart();

            for (int ui = 0; ui < updatedCells.Count; ui++)
            {
                int ci = updatedCells[ui];
                byte lineID = (byte)(ci / 30 + 1);
                byte offset = (byte)(ci % 30);
                byte[] data = { dotpadBytes[ci] };
                Byte[] partialDisp = Dot_Protocol.reqCMD_CellDisplay(lineID, 0x00, offset, 1, ref data);
                serial.writeSerialPort(partialDisp);

                stopWatchDisp.Restart();
                bool notf = false;
                while (!notf)
                {
                    if (stopWatchDisp.ElapsedMilliseconds > thresholdTime_Part)
                    {
                        //Console.WriteLine("ui : " + ui);
                        ui--;
                        break;
                    }
                    int i = 0;
                    for (; i < ComSerial.NTF_packets.Count; i++)
                    {
                        Dot_Packet packet = ComSerial.NTF_packets[i];
                        if (packet != null && packet.CMD_H == 0x02 && packet.CMD_L == 0x02 && packet.SEQ_NUM == 0x00 && packet.DATA[0] == 0x00)
                        {
                            notf = true;
                            break;
                        }
                    }
                    if (notf == true)
                    {
                        ComSerial.NTF_packets.RemoveAt(i);
                    }
                }
                stopWatchDisp.Stop();
            }
            updatedCells.Clear();

            stopWatchTest.Stop();
            Console.WriteLine("displayPartData of discrete area : " + stopWatchTest.ElapsedMilliseconds + "ms");
            return true;
        }
        public static bool displayInitDetailRegion()
        {
            stopWatchTest.Restart();
            Byte[] partialDisp = Dot_Protocol.reqCMD_CellDisplay(0x00, 0x00, 0x00, 20, ref detailRegionBytes_INIT);
            serial.writeSerialPort(partialDisp);
            stopWatchDisp.Restart();
            bool notf = false;
            while (!notf)
            {
                if (stopWatchDisp.ElapsedMilliseconds > thresholdTime_detail)
                {
                    //Console.WriteLine("over thresholdTime_All_detail");
                    break;
                }
                int i = 0;
                for (; i < ComSerial.NTF_packets.Count; i++)
                {
                    Dot_Packet packet = ComSerial.NTF_packets[i];
                    if (packet != null && packet.CMD_H == 0x02 && packet.CMD_L == 0x02 && packet.SEQ_NUM == 0x00 && packet.DATA[0] == 0x00)
                    {
                        notf = true;
                        break;
                    }
                }
                if (notf == true)
                {
                    ComSerial.NTF_packets.RemoveAt(i);
                }
            }
            stopWatchDisp.Stop();

            stopWatchTest.Stop();
            Console.WriteLine("displayInitDetailRegion : " + stopWatchTest.ElapsedMilliseconds + "ms");
            return true;
        }
        public static bool displayTextDetailRegion(string text)
        {
            stopWatchTest.Restart();
            //Console.WriteLine("displayPageNumber : " + currentNum.ToString());
            byte[] data_o = LiblouisController.TranslateToBrailleData(text);
            byte[] data;
            if (data_o.Length > 20)
            {
                data = new byte[20];
            }
            else
            {
                data = new byte[data_o.Length];
            }
            Array.Copy(data_o, 0, data, 0, data.Length);
            ushort length = (ushort)data.Length;

            Byte[] partialDisp = Dot_Protocol.reqCMD_CellDisplay(0x00, 0x00, 0x00, length, ref data);
            serial.writeSerialPort(partialDisp);
            stopWatchDisp.Restart();
            bool notf = false;
            while (!notf)
            {
                if (stopWatchDisp.ElapsedMilliseconds > thresholdTime_char)
                {
                    //Console.WriteLine("over thresholdTime_All_detail");
                    break;
                }
                int i = 0;
                for (; i < ComSerial.NTF_packets.Count; i++)
                {
                    Dot_Packet packet = ComSerial.NTF_packets[i];
                    if (packet != null && packet.CMD_H == 0x02 && packet.CMD_L == 0x02 && packet.SEQ_NUM == 0x00 && packet.DATA[0] == 0x00)
                    {
                        notf = true;
                        break;
                    }
                }
                if (notf == true)
                {
                    ComSerial.NTF_packets.RemoveAt(i);
                }
            }
            stopWatchDisp.Stop();

            stopWatchTest.Stop();

            detailRegionBytes[1] = data[0];
            detailRegionBytes[2] = data[1];

            Console.WriteLine("displayText at detail region : " + stopWatchTest.ElapsedMilliseconds + "ms");
            return true;
        }


        public static bool displayAllDetailRegion()
        {
            stopWatchTest.Restart();
            Byte[] partialDisp = Dot_Protocol.reqCMD_CellDisplay(0x00, 0x00, 0x00, 20, ref detailRegionBytes);
            serial.writeSerialPort(partialDisp);
            stopWatchDisp.Restart();
            bool notf = false;
            while (!notf)
            {
                if (stopWatchDisp.ElapsedMilliseconds > thresholdTime_detail)
                {
                    //Console.WriteLine("over thresholdTime_All_detail");
                    break;
                }
                int i = 0;
                for (; i < ComSerial.NTF_packets.Count; i++)
                {
                    Dot_Packet packet = ComSerial.NTF_packets[i];
                    if (packet != null && packet.CMD_H == 0x02 && packet.CMD_L == 0x02 && packet.SEQ_NUM == 0x00 && packet.DATA[0] == 0x00)
                    {
                        notf = true;
                        break;
                    }
                }
                if (notf == true)
                {
                    ComSerial.NTF_packets.RemoveAt(i);
                }
            }
            stopWatchDisp.Stop();

            stopWatchTest.Stop();
            Console.WriteLine("displayInitDetailRegion : " + stopWatchTest.ElapsedMilliseconds + "ms");
            return true;
        }
        public static bool displayCharacter(Vector2 leftTop, byte character)
        {
            if(leftTop.X < 0 || leftTop.X > 58 || leftTop.Y < 0 || leftTop.Y > 36)
            {
                Console.WriteLine("The area for character is over the dot area");
                return false;
            }
            List<int> cellList = new List<int>(); 
            List<byte> cellData = new List<byte>();
            checkOverlappedCells(ref cellList,ref cellData, leftTop, character);
            int cellListLength = cellList.Count;
            List<byte> lineIDs = new List<byte>();
            List<byte> offsets = new List<byte>();
            List<ushort> lengths = new List<ushort>();
            List<byte[]> datas = new List<byte[]>();    

            if(cellListLength == 1)
            {
                int ci = cellList[0];
                lineIDs.Add((byte)(ci / 30 + 1));
                offsets.Add((byte)(ci % 30));
                lengths.Add(1);
                byte[] data = { cellData[0] };
                datas.Add(data);
            }
            else if(cellListLength == 2)
            {
                bool isSerial = true;
                if (cellList[1] - cellList[0] > 1) isSerial = false;
                if (isSerial)
                {
                    int ci = cellList[0];
                    lineIDs.Add((byte)(ci / 30 + 1));
                    offsets.Add((byte)(ci % 30));
                    lengths.Add(2);
                    byte[] data = { cellData[0], cellData[1] };
                    datas.Add(data);
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int ci = cellList[i];
                        lineIDs.Add((byte)(ci / 30 + 1));
                        offsets.Add((byte)(ci % 30));
                        lengths.Add(1);
                        byte[] data = {cellData[i]};
                        datas.Add(data);
                    }
                }
            }
            else if(cellListLength == 4)
            {
                for(int i = 0; i < 2; i ++)
                {
                    int ci = cellList[i*2];
                    lineIDs.Add((byte)(ci / 30 + 1));
                    offsets.Add((byte)(ci % 30));
                    lengths.Add(2);
                    byte[] data = { cellData[i*2], cellData[i*2+1]};
                    datas.Add(data);
                }
            }
            else
            {
                Console.WriteLine("overlapped cellListLength : " + cellListLength);
                return false;
            }
            stopWatchTest.Restart();

            for (int ui = 0; ui < datas.Count; ui++)
            {
                byte lineID = lineIDs[ui];
                byte offset = offsets[ui];
                byte[] data = new byte[datas[ui].Length];
                ushort length = lengths[ui];
                Byte[] partialDisp = Dot_Protocol.reqCMD_CellDisplay(lineID, 0x00, offset, length, ref data);
                serial.writeSerialPort(partialDisp);

                stopWatchDisp.Restart();
                bool notf = false;
                while (!notf)
                {
                    if (stopWatchDisp.ElapsedMilliseconds > thresholdTime_char)
                    {
                        //Console.WriteLine("ui : " + ui);
                        ui--;
                        break;
                    }
                    int i = 0;
                    for (; i < ComSerial.NTF_packets.Count; i++)
                    {
                        Dot_Packet packet = ComSerial.NTF_packets[i];
                        if (packet != null && packet.CMD_H == 0x02 && packet.CMD_L == 0x02 && packet.SEQ_NUM == 0x00 && packet.DATA[0] == 0x00)
                        {
                            notf = true;
                            break;
                        }
                    }
                    if (notf == true)
                    {
                        ComSerial.NTF_packets.RemoveAt(i);
                    }
                }
                stopWatchDisp.Stop();
            }

            stopWatchTest.Stop();
            Console.WriteLine("displayCharacter for erasing : " + stopWatchTest.ElapsedMilliseconds + "ms");


            stopWatchTest.Restart();

            for (int ui = 0; ui < datas.Count; ui++)
            {
                byte lineID = lineIDs[ui];
                byte offset = offsets[ui];
                byte[] data = datas[ui];
                ushort length = lengths[ui];
                Byte[] partialDisp = Dot_Protocol.reqCMD_CellDisplay(lineID, 0x00, offset, length, ref data);
                serial.writeSerialPort(partialDisp);

                stopWatchDisp.Restart();
                bool notf = false;
                while (!notf)
                {
                    if (stopWatchDisp.ElapsedMilliseconds > thresholdTime_char)
                    {
                        //Console.WriteLine("ui : " + ui);
                        ui--;
                        break;
                    }
                    int i = 0;
                    for (; i < ComSerial.NTF_packets.Count; i++)
                    {
                        Dot_Packet packet = ComSerial.NTF_packets[i];
                        if (packet != null && packet.CMD_H == 0x02 && packet.CMD_L == 0x02 && packet.SEQ_NUM == 0x00 && packet.DATA[0] == 0x00)
                        {
                            notf = true;
                            break;
                        }
                    }
                    if (notf == true)
                    {
                        ComSerial.NTF_packets.RemoveAt(i);
                    }
                }
                stopWatchDisp.Stop();
            }

            stopWatchTest.Stop();
            Console.WriteLine("displayCharacter : " + stopWatchTest.ElapsedMilliseconds + "ms");

            return true;
        }
        public static void checkOverlappedCells(ref List<int> cellList, ref List<byte> cellData, Vector2 pos, byte data)
        {
            for(int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 2; x++) 
                { 
                    Vector2 curPos = pos + new Vector2(x, y);
                    int cellIdx = getCellIndex((int)curPos.X, (int)curPos.Y);
                    int pinIdx = getPinIndex((int)curPos.X, (int)curPos.Y);
                    if (!cellList.Contains(cellIdx))
                    {
                        Console.WriteLine("cellIdx : " + cellIdx);
                        cellList.Add(cellIdx);
                        cellData.Add(0x00);
                    }
                    int bitIndex = x * 4 + y;
                    byte mask = (byte)(1 << bitIndex);
                    bool isSet = (data & mask) != 0;
                    if (isSet)
                    {
                        int order = cellList.IndexOf(cellIdx);
                        cellData[order] |= (byte)(1 << pinIdx);
                    }
                }
            }
        }
        public static bool displayPageNumber(int currentNum)
        {
            stopWatchTest.Restart();
            string text;
            if(currentNum > 9)
            {
                text = currentNum.ToString();
            }
            else 
            {
                text = "0" + currentNum.ToString();            
            }
            text = "p" + text;
            //Console.WriteLine("displayPageNumber : " + currentNum.ToString());
            byte[] data_o = LiblouisController.TranslateToBrailleData(text);
            byte[] data = new byte[data_o.Length];
            Array.Copy(data_o, 0, data, 0, data.Length);
            ushort length = (ushort)data.Length;

            Byte[] partialDisp = Dot_Protocol.reqCMD_CellDisplay(0x00, 0x00, 0x00, length, ref data);
            serial.writeSerialPort(partialDisp);
            stopWatchDisp.Restart();
            bool notf = false;
            while (!notf)
            {
                if (stopWatchDisp.ElapsedMilliseconds > thresholdTime_char)
                {
                    //Console.WriteLine("over thresholdTime_All_detail");
                    break;
                }
                int i = 0;
                for (; i < ComSerial.NTF_packets.Count; i++)
                {
                    Dot_Packet packet = ComSerial.NTF_packets[i];
                    if (packet != null && packet.CMD_H == 0x02 && packet.CMD_L == 0x02 && packet.SEQ_NUM == 0x00 && packet.DATA[0] == 0x00)
                    {
                        notf = true;
                        break;
                    }
                }
                if (notf == true)
                {
                    ComSerial.NTF_packets.RemoveAt(i);
                }
            }
            stopWatchDisp.Stop();

            stopWatchTest.Stop();

            detailRegionBytes[1] = data[0];
            detailRegionBytes[2] = data[1];

            Console.WriteLine("displayPageNumber : " + stopWatchTest.ElapsedMilliseconds + "ms");
            return true;
        }

        // Update is called once per frame
        public static void Initialization()
        {
            if (!isConnected)
            {
                bool result = serial.openSerialPort("COM"+ portNumber.ToString());
                isConnected = result;
            }
            if(isConnected && !isInitialized)
            {
                bool result = displayInitData();
                if (result) displayAllData();
                result = displayInitDetailRegion();
                if(result) displayAllDetailRegion();
                isInitialized = result;
            }
        }
    }
}
