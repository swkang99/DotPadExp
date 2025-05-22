namespace DotPadExp.Deprecated
{
    public class WeightStudy
    {
        public static void Program()
        {
            Controller controller = new();
            controller.Init();

            (int X, int Y) = SelectResolution(Predefined.dotResolutions);

            Area[] areas = Area.InitAreas(X, Y);

            Random random = new();

            // MethodAdjustmentRandom(images, X, Y, areas, controller, random);
            MethodAdjustmentBound(Predefined.images, X, Y, areas, controller);

            controller.Release();
        }

        public static int[,][,] ConvertAllImage(string[] images, double[] weights, int dotResX, int dotResY)
        {            
            int[,][,] dotsAll = new int[images.Length, weights.Length][,];
            
            for (int i = 0; i < images.Length; i++)
            {
                for (int j = 0; j < weights.Length; j++)
                {
                    dotsAll[i, j] = Conversion.ImageToDots(
                        images[i], 
                        dotResX, 
                        dotResY, 
                        weights[j]);
                }
            }
            return dotsAll;
        }

        public static (int, int) SelectResolution((int X, int Y)[] dotResolutions)
        {
            while (true)
            {
                Console.WriteLine($"--[해상도 선택] 1:{dotResolutions[0]} , 2:{dotResolutions[1]}, 3:{dotResolutions[2]}--");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.D1)
                {   
                    return dotResolutions[0];
                }
                else if (keyInfo.Key == ConsoleKey.D2)
                {
                    return dotResolutions[1];
                }
                else if (keyInfo.Key == ConsoleKey.D3)
                {
                    return dotResolutions[2];
                }
            }
        }

        public static void RandomDraw(
            string[] images, 
            int dotResX, 
            int dotResY, 
            Area[] areas, 
            Controller controller)
        {
            double[] weights = [0.0, 0.3, 0.5, 0.7];
            int[,][,] dotsAll = ConvertAllImage(images, weights, dotResX, dotResY);
            
            for (int i = 0; i < images.Length; i++)
            {
                int[] random = [.. Enumerable.Range(0, Area.areaCount)]; // Area.areaCount == weights.Length
                byte[,] argCellData_0 = Predefined.init2DBytes;
                byte[,] argCellData_1 = Conversion.SetTxData(dotsAll[i, random[0]], argCellData_0, areas[random[0]], dotResX, dotResY);
                byte[,] argCellData_2 = Conversion.SetTxData(dotsAll[i, random[1]], argCellData_1, areas[random[1]], dotResX, dotResY);
                byte[,] argCellData_3 = Conversion.SetTxData(dotsAll[i, random[2]], argCellData_2, areas[random[2]], dotResX, dotResY);
                byte[,] argCellData_4 = Conversion.SetTxData(dotsAll[i, random[3]], argCellData_3, areas[random[3]], dotResX, dotResY);

                controller.Draw(argCellData_4);

                Console.WriteLine($"--출력중:{images[i]}--");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
            }
        }
         
        
        public static void MethodAdjustmentRandom ( // Method of Adjustment: Random Weight
            string[] images, 
            int dotResX, 
            int dotResY, 
            Area[] areas, 
            Controller controller,
            Random random) 
        {
            double weight = Math.Round(random.NextDouble(), 1);
            double weight_interval = 0.1;
            
            for (int i = 0; i < images.Length; i++)
            {
                controller.AllDown();
                
                int[,] dots = Conversion.ImageToDots(images[i], dotResX, dotResY, weight);
                Conversion.PrintDotsImage(dots);
                byte[,] argCellData = Conversion.SetTxData(dots, Predefined.init2DBytes, areas[(int)AreaNumber.TopLeft], dotResX, dotResY);
                controller.Draw(argCellData);

                Console.WriteLine($"----------------------------------------[가중치 조정]--------------------------------------------");    
                Console.WriteLine($"--현재 가중치:{weight} , 현재 이미지:{images[i]} , 현재 해상도:({dotResX},{dotResY})--");
                Console.WriteLine($"--위 방향키:+{weight_interval} , 아래 방향키:-{weight_interval} , 왼쪽 방향키:이전 이미지 , 오른쪽 방향키:다음 이미지 , Enter:가중치 기록--");
                Console.WriteLine($"-------------------------------------------------------------------------------------------------\n"); 
                ConsoleKeyInfo weightKeyInfo = Console.ReadKey();
                if (weightKeyInfo.Key == ConsoleKey.UpArrow)
                {
                    weight = Math.Round(weight + weight_interval, 1);
                    if (weight > 1.0)
                    {
                        Console.WriteLine($"[경고] 가중치는 1을 초과할 수 없습니다.");
                        weight = Math.Round(weight - weight_interval, 1);
                    }
                    i -= 1;
                    continue;
                }
                else if (weightKeyInfo.Key == ConsoleKey.DownArrow)
                {
                    weight = Math.Round(weight - weight_interval, 1);
                    if (weight < 0)
                    {
                        Console.WriteLine($"[경고] 가중치는 0 미만일 수 없습니다.");
                        weight = Math.Round(weight + weight_interval, 1);
                    }
                    i -= 1; 
                    continue;
                }
                else if (weightKeyInfo.Key == ConsoleKey.LeftArrow)
                {
                    i -= 2;
                    continue;
                }
                else if (weightKeyInfo.Key == ConsoleKey.RightArrow)
                {
                    weight = Math.Round(random.NextDouble(), 1);
                    continue;
                }
            }   
        }

        
        public static void MethodAdjustmentBound ( // Method of Adjustment: Lower & Upper Bound
            string[] images, 
            int dotResX, 
            int dotResY, 
            Area[] areas, 
            Controller controller) 
        {
            double weight_interval = 0.1;
            double weight = 0.0;
            bool isFindingWeight = false;
            var experiments = images.SelectMany(image => new[] 
            { 
                (image, true), 
                (image, false) 
            })
            .OrderBy(x => Guid.NewGuid())
            .ToArray();

            string filePath = $"C:/Users/kseon/DotPadExp/DotPadExp/Results/result-({dotResX},{dotResY}).csv";
            using (StreamWriter writer = new(filePath))
            {
                writer.WriteLine("image,isForward,weight");
            }
            
            for (int i = 0; i < experiments.Length; i++)
            {
                var (image, isForward) = experiments[i];
                if (!isFindingWeight)
                {
                    weight = isForward? 0.0 : 1.0;
                    isFindingWeight = true;
                }

                controller.AllDown();
                Console.WriteLine($"Enter키를 눌러 렌더링\n"); 
                _ = Console.ReadKey();

                int[,] dots = Conversion.ImageToDots(image, dotResX, dotResY, weight);
                Conversion.PrintDotsImage(dots);
                byte[,] argCellData = Conversion.SetTxData(dots, Predefined.init2DBytes, areas[(int)AreaNumber.MiddleCenter], dotResX, dotResY);
                controller.Draw(argCellData);  

                int index;
                (index, weight) = MethodAdjustmentBoundConsole(image, dotResX, dotResY, weight, weight_interval, isForward, filePath);
                if (weight < 0)
                    isFindingWeight = false;
                i += index;
            }  
        }

        public static (int, double) MethodAdjustmentBoundConsole (
            string image, 
            int dotResX, 
            int dotResY, 
            double weight, 
            double weight_interval, 
            bool isForward,
            string filePath)
        {
            Console.WriteLine($"----------------------------------------[가중치 조정]--------------------------------------------");
            Console.WriteLine($"--현재 가중치:{weight} , 현재 이미지:{image} , 현재 해상도:({dotResX},{dotResY})--");

            if (isForward)
                Console.WriteLine($"--위 방향키:+{weight_interval} , 왼쪽 방향키:이전 이미지 , 오른쪽 방향키:다음 이미지 , Enter:가중치 기록--");
            else
                Console.WriteLine($"--아래 방향키:-{weight_interval} , 왼쪽 방향키:이전 이미지 , 오른쪽 방향키:다음 이미지 , Enter:가중치 기록--");

            Console.WriteLine($"-------------------------------------------------------------------------------------------------\n"); 
            ConsoleKeyInfo weightKeyInfo = Console.ReadKey();

            int index;
            if (weightKeyInfo.Key == ConsoleKey.UpArrow && isForward)
            {
                weight = Math.Round(weight + weight_interval, 1);
                if (weight > 1.0)
                {
                    Console.WriteLine($"[경고] 가중치는 1을 초과할 수 없습니다.");
                    weight = Math.Round(weight - weight_interval, 1);
                }
                Console.WriteLine($"가중치 조정: +{weight_interval}\n");
                index = -1;
            }
            else if (weightKeyInfo.Key == ConsoleKey.DownArrow && !isForward)
            {
                weight = Math.Round(weight - weight_interval, 1);
                if (weight < 0)
                {
                    Console.WriteLine($"[경고] 가중치는 0 미만일 수 없습니다.");
                    weight = Math.Round(weight + weight_interval, 1);
                }
                Console.WriteLine($"가중치 조정: -{weight_interval}\n");
                index = -1;
            }
            else if (weightKeyInfo.Key == ConsoleKey.LeftArrow)
            {
                Console.WriteLine($"이전 이미지\n");
                index = -2;
            }
            else if (weightKeyInfo.Key == ConsoleKey.RightArrow)
            {
                Console.WriteLine($"다음 이미지\n");
                index = 0;
            }
            else if (weightKeyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine($"가중치 저장\n");
                index = 0;
                weight = SaveResult(image, weight, isForward, filePath);
            }
            else
            {
                Console.WriteLine($"다시 그리기\n");
                index = -1;
            }
            return (index, weight);
        }

        public static double SaveResult (string image, double weight, bool isForward, string filePath)
        {
            using StreamWriter writer = File.AppendText(filePath);
            string forward = isForward ? "forward (0->1)" : "backward (1->0)";
            writer.WriteLine($"{Path.GetFileName(image)},{forward},{weight}");
            return -1;
        }
    }
}