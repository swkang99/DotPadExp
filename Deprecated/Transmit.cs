namespace DotPadExp.Deprecated
{
    public static byte[] SetTxAllData(int[,] dots, byte[,] canvas, Area area, int dotResX, int dotResY)
    {
        (int cellCountX, int cellCountY) = Conversion.GetCellCount(dotResX, dotResY);
        int cellStartX = area.CellStartX;
        int cellStartY = area.CellStartY;
        int cellEndX = cellStartX + cellCountX;
        int cellEndY = cellStartY + cellCountY;
        
        string[,] bytes = Conversion.DotToByte(dots, cellCountX, cellCountY);
        
        byte[,] txData = (byte[,])canvas.Clone(); // Deep Copy : Don't Change Canvas.

        int i = 0;
        int j = 0;

        for (int m = cellStartY; m < cellEndY; m++)
        {
            for (int n = cellStartX; n < cellEndX; n++)
            {
                txData[m, n] = Convert.ToByte(bytes[i, j++], 16);
            }
            i++;
            j = 0;
        }
        
        return Conversion.Flatten(txData);
    }
}