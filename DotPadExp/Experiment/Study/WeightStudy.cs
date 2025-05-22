using DotPadExp.Data;
using DotPadExp.DotPad.Command;
using DotPadExp.DotPad.PadArea;

namespace DotPadExp.Experiment.Study
{
    public class WeightStudy(Controller controller) : Study(controller)
    {
        public override void Program(string[] message)
        {
            ParseMessage(message);

            Area areaMiddleCenter = Area.Create(AreaNumber.MiddleCenter, _xDotRes, _yDotRes);

            DrawingParameters drawingParameters = new(
                Predefined.SortedImages[_imageNumber],
                areaMiddleCenter,
                _xDotRes,
                _yDotRes,
                _weight
            );
            
            _controller.DrawLines(drawingParameters);
        }
    }
}