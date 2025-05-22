namespace DotPadExp.DotPad.Protocol
{
    public class ReqCellParameters
    {
        public required byte[] ArgCellData { get; set; }
        public byte ArgStartOffset { get; set; } = 0x00;
        public byte ArgEndOffset { get; set; } = 0x00;
        public byte ArgDestID { get; set; } = 0x01;
        public byte ArgMode = MODE_GRAPHIC_INTERLACED;
        private const byte MODE_GRAPHIC_PROGRESSIVE = 0x00;
        private const byte MODE_GRAPHIC_INTERLACED = 0x08;
        private const byte MODE_TEXT = 0x80;
    }

    public enum ERROR_CODE
    {
        DOT_ERROR_INVALIDE_LENGTH = 0x01, // LENGTH ERROR
        DOT_ERROR_INVALID_CMD,            // CMD ERROR
        DOT_ERROR_DATA_CORRUPTED,         // CRC ERROR
        DOT_ERROR_INVALID_PARAM,          // PARAMETER ERROR
        DOT_ERROR_RESPONSE_TIMEOUT        // TIMEOUT ERROR
    }
    

    public abstract class Request
    {
        protected const byte SYNC_FIELD_1 = 0xAA;
        protected const byte SYNC_FIELD_2 = 0x55;

        /// <summary>
        /// Calculate CheckSum of Serial Tx Data
        /// </summary>
        /// <param name="argArray"></param>
        /// <returns></returns>
        protected static byte CheckSum(byte[] argArray)
        {
            byte result = 0xA5;

            for (ushort i = 4; i < argArray.Length - 1; i++)
            {
                result ^= argArray[i];
            }

            return result;
        }

        /// <summary>
        /// Create Command for Request
        /// </summary>
        /// <returns></returns>
        public abstract byte[] CreateReq();
    }
    
    public class ReqFWVer : Request
    {
        /// <summary>
        /// Request Firmware Version CMD(0x00_00)
        /// </summary>
        /// <returns></returns>
        public override byte[] CreateReq()
        {
            byte[] data = new byte[9];

            data[0] = SYNC_FIELD_1;
            data[1] = SYNC_FIELD_2;
            data[2] = 0x00;   // Length High Byte
            data[3] = 0x05;   // Length Low Byte
            data[4] = 0x00;   // Destination ID
            data[5] = 0x00;   // Command-High
            data[6] = 0x00;   // Command-Low
            data[7] = 0x00;	  // Sequence Number
            data[^1] = CheckSum(data);

            return data;
        }
    }

    public class ReqHWVer : Request
    {
        /// <summary>
        /// Request Hardware Version CMD(0x00_10)
        /// </summary>
        /// <returns></returns>
        public override byte[] CreateReq()
        {
            byte[] data = new byte[9];

            data[0] = SYNC_FIELD_1;
            data[1] = SYNC_FIELD_2;
            data[2] = 0x00;   // Length High Byte
            data[3] = 0x05;   // Length Low Byte
            data[4] = 0x00;   // Destination ID
            data[5] = 0x00;   // Command-High
            data[6] = 0x10;   // Command-Low
            data[7] = 0x00;	  // Sequence Number
            data[^1] = CheckSum(data);

            return data;
        }
    }
    
    public class ReqDeviceName : Request
    {
        /// <summary>
        /// Request Device Name CMD(0x01_00)
        /// </summary>
        /// <returns></returns>
        public override byte[] CreateReq()
        {
            byte[] data = new byte[9];

            data[0] = SYNC_FIELD_1;
            data[1] = SYNC_FIELD_2;
            data[2] = 0x00;   // Length High Byte
            data[3] = 0x05;   // Length Low Byte
            data[4] = 0x00;   // Destination ID
            data[5] = 0x01;   // Command-High
            data[6] = 0x00;   // Command-Low
            data[7] = 0x00;	  // Sequence Number
            data[^1] = CheckSum(data);

            return data;
        }
    }
    
    public class ReqBoardInfo : Request
    {
        /// <summary>
        /// Request Board Information CMD(0x01_10)
        /// </summary>
        /// <returns></returns>
        public override byte[] CreateReq()
        {
            byte[] data = new byte[9];

            data[0] = SYNC_FIELD_1;
            data[1] = SYNC_FIELD_2;
            data[2] = 0x00;   // Length High Byte
            data[3] = 0x05;   // Length Low Byte
            data[4] = 0x00;   // Destination ID
            data[5] = 0x01;   // Command-High
            data[6] = 0x10;   // Command-Low
            data[7] = 0x00;	  // Sequence Number
            data[^1] = CheckSum(data);

            return data;
        }
    }

    public class ReqCellDisplay(ReqCellParameters parameters) : Request
    {
        /// <summary>
        /// Request Display Cell Line CMD(0x02_20)
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override byte[] CreateReq()
        {
            ushort argLength = (ushort)parameters.ArgCellData.Length;
            byte[] data = new byte[10 + argLength];

            data[0] = SYNC_FIELD_1;
            data[1] = SYNC_FIELD_2;
            data[2] = (byte)((argLength & 0xFF00) >> 8);      // Length High Byte
            data[3] = (byte)(0x06 + (argLength & 0x00FF));    // Length Low Byte
            data[4] = parameters.ArgDestID;                   // Destination ID (Line ID)
            data[5] = 0x02;                                   // Command-High
            data[6] = 0x00;                                   // Command-Low
            data[7] = parameters.ArgMode;	                  // Sequence Number
            data[8] = parameters.ArgStartOffset;	          // Data[0] : StartOffset

            for (int i = 0; i < argLength; i++)
            {
                data[9 + i] = parameters.ArgCellData[i];
            }

            data[^1] = CheckSum(data);

            return data;
        }
    }

    public class ReqPartialDisplay(ReqCellParameters parameters) : Request
    {
        /// <summary>
        /// Request Partial Display Cell CMD(0x02_20)
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override byte[] CreateReq()
        {
            ushort argLength = (ushort)parameters.ArgCellData.Length;
            byte[] data = new byte[11 + argLength];

            data[0] = SYNC_FIELD_1;
            data[1] = SYNC_FIELD_2;
            data[2] = (byte)((argLength & 0xFF00) >> 8);      // Length High Byte
            data[3] = (byte)(0x07 + (argLength & 0x00FF));    // Length Low Byte
            data[4] = parameters.ArgDestID;                   // Destination ID
            data[5] = 0x02;                                   // Command-High
            data[6] = 0x20;                                   // Command-Low
            data[7] = parameters.ArgMode;	                  // Sequence Number
            data[8] = parameters.ArgStartOffset;	          // Data[0] : StartOffset
            data[9] = parameters.ArgEndOffset;	              // Data[1] : EndOffset

            for (int i = 0; i < argLength; i++)
            {
                data[10 + i] = parameters.ArgCellData[i];
            }

            data[^1] = CheckSum(data);

            return data;
        }
    }
}