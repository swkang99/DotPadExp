namespace DotPadExp.DotPad.PadArea
{
    public enum AreaNumber
    {
        TopLeft,
        MiddleLeft,
        BottomLeft,
        MiddleCenter,
        TopRight,
        MiddleRight,
        BottomRight,
    }
    
    public class Area
    {
        public int CellStartX;
        public int CellStartY;
        public static readonly int areaCount = Enum.GetValues<AreaNumber>().Length;
        private readonly static string[] areaNames = [
            "top left", 
            "middle left", 
            "bottom left",
            "middle center",
            "top right", 
            "middle right", 
            "bottom right"
        ];

        private Area(string areaName, int xDotRes, int yDotRes)
        {
            string[] parts = areaName.Split(" ");
            string vertical = parts[0];
            string horizontal = parts[1];

            var mapper = new DotResolutionMapper();

            // Vertical position
            CellStartY = vertical switch
            {
                "top" => 0,
                "middle" => mapper.GetValue("MiddleStartY", xDotRes, yDotRes),
                "bottom" => mapper.GetValue("BottomStartY", xDotRes, yDotRes),
                _ => throw new ArgumentException($"Invalid vertical position: {vertical}")
            };

            // Horizontal position
            CellStartX = horizontal switch
            {
                "left" => 0,
                "center" => mapper.GetValue("CenterStartX", xDotRes, yDotRes),
                "right" => mapper.GetValue("RightStartX", xDotRes, yDotRes),
                _ => throw new ArgumentException($"Invalid horizontal position: {horizontal}")
            };
        }

        public static Area Create(AreaNumber areaNumber, int xDotRes, int yDotRes)
        {
            string areaName = areaNames[(int)areaNumber];
            return new Area(areaName, xDotRes, yDotRes);
        }

        public static Area[] InitAreas(int xDotRes, int yDotRes)
        {
            Area[] areas = new Area[areaCount];

            for (int i = 0; i < areaCount; i++)
            {
                areas[i] = new Area(areaNames[i], xDotRes, yDotRes);    
            }

            return areas;
        }
    }
}