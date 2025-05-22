using DotPadExp.DotPad.PadArea;
using DotPadExp.DotPad.Protocol;

namespace DotPadExp.DotPad.Command
{
    public interface IDrawingStrategy
    {
        ICommand CreateDrawingCommand(ComSerial comSerial, byte[] cellData, params byte[] parameters);
    }

    public class LineDrawingStrategy : IDrawingStrategy
    {
        public ICommand CreateDrawingCommand(ComSerial comSerial, byte[] cellData, params byte[] parameters)
        {
            return new CellDisplayCommand(comSerial, new ReqCellParameters {
                ArgCellData = cellData,
                ArgDestID = parameters[0],
                ArgStartOffset = parameters.Length > 1 ? parameters[1] : (byte)0
            });
        }
    }

    public class PartialDrawingStrategy : IDrawingStrategy
    {
        public ICommand CreateDrawingCommand(ComSerial comSerial, byte[] cellData, params byte[] parameters)
        {
            return new PartialDisplayCommand(comSerial, new ReqCellParameters {
                ArgCellData = cellData,
                ArgDestID = parameters[0],
                ArgStartOffset = parameters[1],
                ArgEndOffset = parameters[2]
            });
        }
    }

    public interface IDrawingStrategyFactory
    {
        IDrawingStrategy GetLineDrawingStrategy();
        IDrawingStrategy GetPartialDrawingStrategy();
    }

    public class DrawingStrategyFactory : IDrawingStrategyFactory
    {
        public IDrawingStrategy GetLineDrawingStrategy() => new LineDrawingStrategy();
        public IDrawingStrategy GetPartialDrawingStrategy() => new PartialDrawingStrategy();
    }

    public struct DrawingParameters(string image, Area area, int xDotRes, int yDotRes, double weight)
    {
        public string Image { get; set; } = image;
        public Area Area { get; set; } = area;
        public int XDotRes { get; set; } = xDotRes;
        public int YDotRes { get; set; } = yDotRes;
        public double Weight { get; set; } = weight;
    }
}