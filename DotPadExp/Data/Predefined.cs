namespace DotPadExp.Data
{
    public class Predefined
    {
        private readonly int _allCellWidth = 30;
        private readonly int _allCellHeight = 10;
        private readonly int _lineCellWidth = 30;
        private readonly int _lineCellHeight = 1;
        private readonly int _partialCellWidth = 10;
        private readonly int _partialCellHeight = 5;
        
        private static byte[] InitBytes(int cellWidth, int cellHeight, byte val = 0)
        {
            int bytesLength = cellWidth * cellHeight;
            byte[] bytes = new byte[bytesLength];

            for (int i = 0; i < bytesLength; i++)
            {
                bytes[i] = val;
            }

            return bytes;
        }

        private byte[]? _cachedAllDownBytes;
        public byte[] AllDownBytes
        {
            get
            {  
                _cachedAllDownBytes ??= InitBytes(_allCellWidth, _allCellHeight);
                return _cachedAllDownBytes;
            }
        }
        
        private byte[]? _cachedAllUpBytes;
        public byte[] AllUpBytes
        {
            get
            {
                _cachedAllUpBytes ??= InitBytes(_allCellWidth, _allCellHeight, 0xFF);
                return _cachedAllUpBytes;
            }
        }

        private byte[]? _cachedLineDownBytes;
        public byte[] LineDownBytes
        {
            get
            {
                _cachedLineDownBytes ??= InitBytes(_lineCellWidth, _lineCellHeight);
                return _cachedLineDownBytes;
            }
        }

        private byte[]? _cachedLineUpBytes;
        public byte[] LineUpBytes
        {
            get
            {
                _cachedLineUpBytes ??= InitBytes(_lineCellWidth, _lineCellHeight, 0xFF);
                return _cachedLineUpBytes;
            }
        }

        private byte[]? _cachedPartialDownBytes;
        public byte[] PartialDownBytes
        {
            get
            {
                _cachedPartialDownBytes ??= InitBytes(_partialCellWidth, _partialCellHeight);
                return _cachedPartialDownBytes;
            }
        }

        private byte[]? _cachedPartialUpBytes;
        public byte[] PartialUpBytes
        {
            get
            {
                _cachedPartialUpBytes ??= InitBytes(_partialCellWidth, _partialCellHeight, 0xFF);
                return _cachedPartialUpBytes;
            }
        }

        readonly public static string DirImage = @"C:/Users/kseon/DotPadExp/DotPadExp/Images/PixelStudy/";
        readonly public static string[] Images = Directory.GetFiles(DirImage, "*.*", SearchOption.AllDirectories);
        readonly public static string[] SortedImages = SortByNumber(Images);
        readonly public static (int X, int Y)[] DotResolutions = 
        [
            (X: 40, Y: 40),
            (X: 30, Y: 30),
            (X: 20, Y: 20), 
            (X: 15, Y: 15), 
            (X: 10, Y: 10)
        ];
        private static string[] SortByNumber (string[] data)
        {
            Array.Sort(data, (x, y) =>
            {
                string fileNameX = Path.GetFileName(x);
                string fileNameY = Path.GetFileName(y);

                int xNumber = int.Parse(fileNameX.Split('_')[0]);
                int yNumber = int.Parse(fileNameY.Split('_')[0]);

                return xNumber.CompareTo(yNumber);
            });
            return data;
        }
    }
}