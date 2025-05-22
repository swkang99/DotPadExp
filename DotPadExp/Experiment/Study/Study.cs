using DotPadExp.Data;
using DotPadExp.DotPad.Command;

namespace DotPadExp.Experiment.Study
{
    public abstract class Study(Controller controller)
    {
        protected Controller _controller = controller;
        protected int _resolutionNumber;
        protected int _imageNumber;
        protected int _xDotRes;
        protected int _yDotRes;
        protected double _weight;
        
        protected void ParseMessage(string[] message)
        {
            _resolutionNumber = int.Parse(message[0]);
            _imageNumber = int.Parse(message[1]);
            _weight = double.Parse(message[2]);

            (_xDotRes, _yDotRes) = Predefined.DotResolutions[_resolutionNumber];
        }

        public abstract void Program(string[] message);
    }
}