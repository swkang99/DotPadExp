using DotPadExp.Data;
using DotPadExp.DotPad.Command;
using DotPadExp.DotPad.PadArea;

namespace DotPadExp.Experiment
{
    public class Test(Controller controller)
    {
        public void TestAll()
        {
            bool isUp = false;

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    if (isUp)
                    {
                        controller.AllDown();
                        isUp = false;
                    }
                    else
                    {
                        controller.AllUp();
                        isUp = true;
                    }
                }
                else
                    break;
            }
        }

        public void TestLine(int lineID)
        {
            controller.LineUp(Conversion.IntToByte(lineID));
            Console.WriteLine("Line Up: " + lineID.ToString());

            _ = Console.ReadKey();

            controller.LineDown(Conversion.IntToByte(lineID));
            Console.WriteLine("Line Down: " + lineID.ToString());
        }

        public void TestLine(int lineID, int startOffset)
        {
            controller.LineUp(Conversion.IntToByte(lineID), Conversion.IntToByte(startOffset));
            Console.WriteLine("Line Up: " + lineID.ToString());

            _ = Console.ReadKey();

            controller.LineDown(Conversion.IntToByte(lineID), Conversion.IntToByte(startOffset));
            Console.WriteLine("Line Down: " + lineID.ToString());
        }
        
        

        public void TestDotPadArea(int xDotRes, int yDotRes, double weight = 0.7)
        {
            Area[] areas = Area.InitAreas(xDotRes, yDotRes);
            
            foreach (Area area in areas)
            {
                DrawingParameters drawingParameters = new(Predefined.PixelImages[0], area, xDotRes, yDotRes, weight);
                controller.DrawLines(drawingParameters);
                _ = Console.ReadKey();
            }
        }

        public void TestDotPadAreaPartial(int xDotRes, int yDotRes, double weight = 0.7)
        {
            Area[] areas = Area.InitAreas(xDotRes, yDotRes);
            
            foreach (Area area in areas)
            {
                DrawingParameters drawingParameters = new(Predefined.PixelImages[0], area, xDotRes, yDotRes, weight);
                controller.DrawPartial(drawingParameters);
                _ = Console.ReadKey();
            }
        }

        public void TestDotPadDrawLines(int xDotRes, int yDotRes, double weight = 0.7)
        {
            Area area = Area.Create(AreaNumber.MiddleCenter, xDotRes, yDotRes);

            foreach (string image in Predefined.PixelImages)
            {
                DrawingParameters drawingParameters = new(image, area, xDotRes, yDotRes, weight);
                controller.DrawLines(drawingParameters);
                Console.WriteLine(image);

                _ = Console.ReadKey();
            }
        }

        public void TestDotPadDrawPartial(int xDotRes, int yDotRes, double weight = 0.7)
        {
            Area area = Area.Create(AreaNumber.MiddleCenter, xDotRes, yDotRes);

            foreach (string image in Predefined.PixelImages)
            {
                DrawingParameters drawingParameters = new(image, area, xDotRes, yDotRes, weight);
                controller.DrawPartial(drawingParameters);
                Console.WriteLine(image);

                _ = Console.ReadKey();
            }
        }
    }
}