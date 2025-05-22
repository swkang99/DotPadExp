using DotPadExp.DotPad.PadArea;

namespace DotPadExp.Data
{
    public class Transmit
    {
        public static (byte[], byte[]) SetTxPartialData(int[,] dots, Area area, int xDotRes, int yDotRes)
        {
            (int cellCountX, int cellCountY) = Conversion.GetCellCount(xDotRes, yDotRes);
            
            byte startX = Conversion.IntToByte(area.CellStartX);
            byte endX = Conversion.IntToByte(area.CellStartX + cellCountX - 1);
            byte startY = Conversion.IntToByte(area.CellStartY + 1);

            byte[] cellOffSet = [startY, startX, endX];
            
            string[,] bytes = Conversion.DotToByte(dots, cellCountX, cellCountY);
            byte[,] txPartialData = new byte[cellCountY, cellCountX];

            for (int i = 0; i < cellCountY; i++)
            {
                for (int j = 0; j < cellCountX; j++)
                {
                    txPartialData[i, j] = Convert.ToByte(bytes[i, j], 16);
                }
            }

            return (Conversion.Flatten(txPartialData), cellOffSet);
        }

        public static (byte[], byte[]) SetTxLineData(int[,] dots, Area area, int xDotRes, int yDotRes)
        {
            (int cellCountX, int cellCountY) = Conversion.GetCellCount(xDotRes, yDotRes);  
            
            byte startX = Conversion.IntToByte(area.CellStartX);
            byte startY = Conversion.IntToByte(area.CellStartY + 1);
            byte[] cellOffSet = [startY, startX];

            byte[] bytes = GetBytes(Conversion.Flatten(Conversion.DotToByte(dots, cellCountX, cellCountY)));
            int marginLength = GetXMargin(xDotRes);
            byte[] txLineData = new byte[bytes.Length + (cellCountY - 1) * marginLength];

            for (int i = 0; i < cellCountY; i++)
            {
                for (int j = 0; j < cellCountX; j++)
                {
                    txLineData[i * (cellCountX + marginLength) + j] = Convert.ToByte(bytes[i * cellCountX + j]);
                }
            }

            return (txLineData, cellOffSet);
        }

        public static int GetXMargin(int x) => x switch
        {
            // The number of X-axis cells in a line that do not fit into the Middle Center Area. 
            40 => 10, 
            30 => 15, 
            20 => 20,
            15 => 22,
            10 => 25,
            _ => throw new ArgumentException("Invalid input")
        };

        public static byte[] GetBytes(string[] strs)
        {
            byte[] result = new byte[strs.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Conversion.StringToByte(strs[i]);
            }

            return result;
        }
    }
}