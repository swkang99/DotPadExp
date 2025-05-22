using DotPadExp.Data;
using DotPadExp.DotPad.Command;
using DotPadExp.DotPad.PadArea;

namespace DotPadExp.Experiment.Study
{
    public class PixelStudy(Controller controller) : Study(controller)
    {
        public override void Program(string[] message)
        {
            ParseMessage(message);

            Area areaMiddleCenter = Area.Create(AreaNumber.MiddleCenter, _xDotRes, _yDotRes);
            
            // Weight Study로 얻은 최적의 weight 사용
            // _ weight = 이미지별 저장된 weight, _imageNumber를 이용해 참조
            
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