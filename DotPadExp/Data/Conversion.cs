using OpenCvSharp;

namespace DotPadExp.Data
{
    public class Conversion
    {
        private readonly static int _cellWidth = 2;
        private readonly static int _cellHeight = 4;

        public static (int, int) GetCellCount(int xDotRes, int yDotRes)
        {
            int cellCountX = (int)Math.Ceiling((double)xDotRes / _cellWidth);
            int cellCountY = (int)Math.Ceiling((double)yDotRes / _cellHeight);
            return (cellCountX, cellCountY);
        }

        public static string[,] DotToByte(int[,] dots, int cellCountX, int cellCountY)
        {
            string byteLeft = "";
            string byteRight = "";

            int m = 0;
            int n = 0;

            string[,] results = new string[cellCountY, cellCountX];

            for (int v = 0; v < cellCountY; v++)
            {   
                for (int h = 0;  h < cellCountX; h++)
                {
                    for (int i = 0; i < _cellHeight; i++)
                    {
                        for (int j = 0; j < _cellWidth; j++)
                        {
                            int x = i + n;
                            int y = j + m;    
                            if (dots.GetLength(1) > x && dots.GetLength(0) > y)
                            {
                                if (y % 2 == 0)
                                    byteRight += dots[x, y].ToString();
                                else
                                    byteLeft += dots[x, y].ToString();
                            }
                            else
                            {
                                if (y % 2 == 0)
                                    byteRight += 0.ToString();
                                else
                                    byteLeft += 0.ToString();
                            }
                        }
                    }   
                    m += _cellWidth;

                    results[v, h] = MergeByte(byteLeft, byteRight);

                    byteLeft = "";
                    byteRight = "";
                }
                n += _cellHeight;
                m = 0;
            }

            return results;
        }

        private static string MergeByte(string byteLeft, string byteRight)
        {
            char[] byteLeftArr = byteLeft.ToCharArray();
            Array.Reverse(byteLeftArr);

            char[] byteRightArr = byteRight.ToCharArray();
            Array.Reverse(byteRightArr);

            string binByte = new string(byteLeftArr) + new string(byteRightArr);
            string hexByte = Convert.ToInt32(binByte, 2).ToString("X");

            return hexByte;
        }
        
        public static void PrintConversionResult(int[,] dots, int xDotRes, int yDotRes)
        {
            string[,] result = DotToByte(dots, xDotRes, yDotRes);
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Console.Write(result[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public static int[,] ImageToDots(string imgName, int xDotRes, int yDotRes, double weight)
        {
            Mat image = new(imgName, ImreadModes.Grayscale);
            Mat edges = new();
            Cv2.Canny(image, edges, 100, 200);

            int height = image.Rows;
            int width = image.Cols;
            int gridHeight = height / yDotRes;
            int gridWidth = width / xDotRes;

            int[,] dots = new int[xDotRes, yDotRes];

            double w = 1 - weight; // Use weight same as figure in PPT.

            for (int i = 0; i < xDotRes; i++)
            {
                for (int j = 0; j < yDotRes; j++)
                {
                    // Grid Coordinate : (x1, y1) , (x2, y2)
                    int y1 = i * gridHeight;
                    int y2 = (i + 1) * gridHeight;
                    
                    int x1 = j * gridWidth;
                    int x2 = (j + 1) * gridWidth;

                    int shrinkY = (int)(gridHeight * (w / 2));
                    int shrinkX = (int)(gridWidth * (w / 2));

                    int y1Shrink = y1 + shrinkY;
                    int y2Shrink = y2 - shrinkY;
                    int x1Shrink = x1 + shrinkX;
                    int x2Shrink = x2 - shrinkX;

                    Mat gridEdges = new(edges, new Rect(
                        x1Shrink, 
                        y1Shrink, 
                        x2Shrink - x1Shrink, 
                        y2Shrink - y1Shrink
                    ));
                    if (Cv2.CountNonZero(gridEdges) > 0)
                    {
                        dots[i, j] = 1;
                    }
                }
            }

            return dots;
        }

        public static void PrintDotsImage(int[,] img)
        {
            for (int i = 0; i < img.GetLength(0); i++)
            {
                for (int j = 0; j < img.GetLength(1); j++)
                {
                    Console.Write(img[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public static T[] Flatten<T>(T[,] twoDArray)
        {
            int rows = twoDArray.GetLength(0);
            int cols = twoDArray.GetLength(1);
            T[] oneDArray = new T[rows * cols];

            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    oneDArray[index++] = twoDArray[i, j];
                }
            }

            return oneDArray;
        }

        public static byte IntToByte(int number)
        {
            return Convert.ToByte(number.ToString("X2"), 16);
        }

        public static byte StringToByte(string str)
        {
            return Convert.ToByte(str, 16);
        }
    }
}