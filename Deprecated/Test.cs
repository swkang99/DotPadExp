namespace DotPadExp.Deprecated
{
    public class Test
    {
        public static void TestDotPadConsoleApp() 
        {
            Controller controller = new();
            controller.Init();

            (int dotResX, int dotResY) = GetResolution();
            Area[] areas = Area.InitAreas(dotResX, dotResY);

            while (true) 
            {
                Console.WriteLine("--[메뉴 선택] R:초기화 , A:전체 켬 , T:테스트 , F:그리기 모드 , ESC:종료--");
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.R)
                {
                    controller.AllDown();
                }
                else if (keyInfo.Key == ConsoleKey.A)
                {
                    controller.AllUp();
                }
                else if (keyInfo.Key == ConsoleKey.T)
                {
                    controller.TestDisplay();
                }
                else if (keyInfo.Key == ConsoleKey.F)
                {
                    while (true)
                    {
                        Console.WriteLine("--[그리기 모드] 1:왼쪽 위 , 2:왼쪽 밑 , 3:오른쪽 위 , 4:오른쪽 밑 , ESC:나가기--");
                        ConsoleKeyInfo drawKeyInfo = Console.ReadKey();
                        
                        if (drawKeyInfo.Key == ConsoleKey.D1)
                        {
                            controller.AllDown();
                            controller.Draw(areas[(int)AreaNumber.TopLeft], dotResX, dotResY);
                        }
                        else if (drawKeyInfo.Key == ConsoleKey.D2)
                        {
                            controller.AllDown();
                            controller.Draw(areas[(int)AreaNumber.BottomLeft], dotResX, dotResY);
                        }
                        else if (drawKeyInfo.Key == ConsoleKey.D3)
                        {
                            controller.AllDown();
                            controller.Draw(areas[(int)AreaNumber.TopRight], dotResX, dotResY);
                        }
                        else if (drawKeyInfo.Key == ConsoleKey.D4)
                        {
                            controller.AllDown();
                            controller.Draw(areas[(int)AreaNumber.BottomRight], dotResX, dotResY);
                        }
                        else if (drawKeyInfo.Key == ConsoleKey.Escape)
                        {
                            break;
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
            
            controller.Release();
        }

        public static (int x, int y) GetResolution()
        {
            int x = 0;
            int y = 0;
            Console.Write("해상도(x, y)를 공백으로 구분하여 입력: ");
            string? input = Console.ReadLine();
            string[] inputs = input?.Split() ?? [];

            if (inputs.Length == 2 && int.TryParse(inputs[0], out x) && int.TryParse(inputs[1], out y))
            {
                Console.WriteLine($"X축 해상도: {x}, Y축 해상도: {y}");
            }
            else
            {
                Console.WriteLine("올바른 형식으로 입력해주세요.");
            }

            return (x, y);
        }
    }   
}


