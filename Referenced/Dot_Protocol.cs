using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot_Test_APP
{
    class Dot_Protocol
    {
        /********************************************************************************/
        /* Serial Tx Data CheckSum Data Calculator Function								*/
        /********************************************************************************/
        private static Byte makeCheckSumData(byte[] argArray)
        {
            Byte result = 0xA5;

            for (UInt16 i = 4; i < argArray.Length - 1; i++)
            {
                result ^= argArray[i];
            }

            return result;
        }

        /********************************************************************************/
        /* Request Firmware Version CMD(0x00_00) Function								*/
        /********************************************************************************/
        public static Byte[] reqCMD_VerFW()
        {
            Byte[] txData = new byte[9];

            txData[0] = 0xAA;
            txData[1] = 0x55;
            txData[2] = 0x00;   //Length High Byte
            txData[3] = 0x05;   //Length Low Byte
            txData[4] = 0x00;   //Destination ID
            txData[5] = 0x00;   //Command-High
            txData[6] = 0x00;   //Command-Low
            txData[7] = 0x00;	//Sequence Number
            txData[txData.Length - 1] = makeCheckSumData(txData);	//CheckSum

            return txData;
        }

        /********************************************************************************/
        /* Request Hardware Version CMD(0x00_10) function								*/
        /********************************************************************************/
        public static Byte[] reqCMD_VerHW()
        {
            Byte[] txData = new byte[9];

            txData[0] = 0xAA;
            txData[1] = 0x55;
            txData[2] = 0x00;   //Length High Byte
            txData[3] = 0x05;   //Length Low Byte
            txData[4] = 0x00;   //Destination ID
            txData[5] = 0x00;   //Command-High
            txData[6] = 0x10;   //Command-Low
            txData[7] = 0x00;	//Sequence Number
            txData[txData.Length - 1] = makeCheckSumData(txData);	//CheckSum

            return txData;
        }

        /********************************************************************************/
        /* Request Device Name CMD(0x01_00) Function									*/
        /********************************************************************************/
        public static Byte[] reqCMD_DeviceName()
        {
            Byte[] txData = new byte[9];

            txData[0] = 0xAA;
            txData[1] = 0x55;
            txData[2] = 0x00;   //Length High Byte
            txData[3] = 0x05;   //Length Low Byte
            txData[4] = 0x00;   //Destination ID
            txData[5] = 0x01;   //Command-High
            txData[6] = 0x00;   //Command-Low
            txData[7] = 0x00;	//Sequence Number
            txData[txData.Length - 1] = makeCheckSumData(txData);	//CheckSum

            return txData;
        }

        /********************************************************************************/
        /* Request Board Information CMD(0x01_10) Function								*/
        /********************************************************************************/
        public static Byte[] reqCMD_BoardInfo()
        {
            Byte[] txData = new byte[9];

            txData[0] = 0xAA;
            txData[1] = 0x55;
            txData[2] = 0x00;   //Length High Byte
            txData[3] = 0x05;   //Length Low Byte
            txData[4] = 0x00;   //Destination ID
            txData[5] = 0x01;   //Command-High
            txData[6] = 0x10;   //Command-Low
            txData[7] = 0x00;	//Sequence Number
            txData[txData.Length - 1] = makeCheckSumData(txData);	//CheckSum

            return txData;
        }

        /********************************************************************************/
        /* Request Display Cell CMD(0x02_00) Function                                   */
        /********************************************************************************/
        public static Byte[] reqCMD_CellDisplay(byte argDestID, byte argMode, byte argStartOffset, ushort argLength, ref byte[] argCellData)
        {
            byte[] txData = new byte[10 + argLength];

            txData[0] = 0xAA;
            txData[1] = 0x55;
            txData[2] = (byte)((argLength & 0xFF00) >> 8);      //Length High Byte
            txData[3] = (byte)(0x06 + (argLength & 0x00FF));    //Length Low Byte
            txData[4] = argDestID;  //Destination ID
            txData[5] = 0x02;       //Command-High
            txData[6] = 0x00;       //Command-Low
            txData[7] = argMode;	//Sequence Number
            txData[8] = argStartOffset;	//Data[0] : StartOffset

            for (int i = 0; i < argLength; i++)
            {
                txData[9 + i] = argCellData[i];
            }

            txData[txData.Length - 1] = makeCheckSumData(txData);	//CheckSum

            return txData;
        }

        /********************************************************************************/
        /* Request Partial Display Cell CMD(0x02_20) Function                           */
        /********************************************************************************/
        public static Byte[] reqCMD_PartialDisplay(byte argDestID, byte argMode, byte argStartOffset, byte argEndOffset, ushort argLength, ref byte[] argCellData)
        {
            byte[] txData = new byte[11 + argLength];

            txData[0] = 0xAA;
            txData[1] = 0x55;
            txData[2] = (byte)((argLength & 0xFF00) >> 8);      //Length High Byte
            txData[3] = (byte)(0x07 + (argLength & 0x00FF));    //Length Low Byte
            txData[4] = argDestID;  //Destination ID
            txData[5] = 0x02;       //Command-High
            txData[6] = 0x20;       //Command-Low
            txData[7] = argMode;	//Sequence Number
            txData[8] = argStartOffset;	//Data[0] : StartOffset
            txData[9] = argEndOffset;	//Data[1] : EndOffset

            for (int i = 0; i < argLength; i++)
            {
                txData[10 + i] = argCellData[i];
            }

            txData[txData.Length - 1] = makeCheckSumData(txData);	//CheckSum

            return txData;
        }

        /********************************************************************************/
        /* Request Battery Level Status CMD(0x06_10) Function                           */
        /********************************************************************************/
        public static Byte[] reqCMD_BatteryStatus(byte argReqType)
        {
            byte[] txData = new byte[10];
            byte mIdx = 0;

            txData[mIdx++] = 0xAA;
            txData[mIdx++] = 0x55;
            txData[mIdx++] = 0x00;      //Length High Byte
            txData[mIdx++] = 0x00;      //Length Low Byte
            txData[mIdx++] = 0x00;      //Destination ID
            txData[mIdx++] = 0x06;      //Command-High
            txData[mIdx++] = 0x10;      //Command-Low
            txData[mIdx++] = 0x00;      //Sequence Number
            txData[mIdx++] = argReqType;    //Data[0] - Req Type : 0-Battery Level & 1-Battery SoC+mVolt
            txData[3] = (byte)(mIdx - 3);      //Length Low Byte
            txData[mIdx] = makeCheckSumData(txData);   //CheckSum

            return txData;
        }
    }
}
