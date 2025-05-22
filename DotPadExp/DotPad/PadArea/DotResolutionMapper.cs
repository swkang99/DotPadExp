namespace DotPadExp.DotPad.PadArea
{
    public class DotResolutionMap(Dictionary<(int, int), int> resolutionMap)
    {
        private readonly Dictionary<(int, int), int> resolutionMap = resolutionMap;

        public int GetValue(int xDotRes, int yDotRes)
        {
            return resolutionMap.TryGetValue((xDotRes, yDotRes), out int value) ? value : 0;
        }
    }

    public class DotResolutionMapper
    {
        private readonly Dictionary<string, DotResolutionMap> maps;

        public DotResolutionMapper()
        {
            maps = new Dictionary<string, DotResolutionMap>
            {
                //  {( m,  n), k}  ->  k = X or Y Index of Start Cell to draw each Side in (m x n) Dot Resolution
                ["BottomStartY"] = new DotResolutionMap(new Dictionary<(int, int), int>
                {
                    {(20, 20), 5},
                    {(15, 15), 6},
                    {(10, 10), 7}
                }),
                ["RightStartX"] = new DotResolutionMap(new Dictionary<(int, int), int>
                {
                    {(20, 20), 20},
                    {(15, 15), 22},
                    {(10, 10), 25}
                }),
                ["MiddleStartY"] = new DotResolutionMap(new Dictionary<(int, int), int>
                {
                    {(40, 40), 0},
                    {(30, 30), 1},
                    {(20, 20), 3},
                    {(15, 15), 3},
                    {(10, 10), 4}
                }),
                ["CenterStartX"] = new DotResolutionMap(new Dictionary<(int, int), int>
                {
                    {(40, 40), 5},
                    {(30, 30), 8},
                    {(20, 20), 10},
                    {(15, 15), 11},
                    {(10, 10), 12}
                })
            };
        }

        public int GetValue(string mapName, int xDotRes, int yDotRes)
        {
            if (maps.TryGetValue(mapName, out DotResolutionMap? map))
            {
                return map.GetValue(xDotRes, yDotRes);
            }
            else
            {
                throw new ArgumentException("Invalid map name", nameof(mapName));
            }
        }
    }
}