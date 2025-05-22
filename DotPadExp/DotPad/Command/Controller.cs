using DotPadExp.Data;
using DotPadExp.DotPad.PadArea;
using DotPadExp.DotPad.Protocol;

namespace DotPadExp.DotPad.Command
{
    public class Controller : IDisposable
    {
        private readonly ComSerial _comSerial;
        private readonly ICommandFactory _commandFactory;
        private readonly IDrawingStrategyFactory _drawingStrategyFactory;
        private readonly Predefined _predefined = new();

        public Controller(
            ComSerial comSerial, 
            ICommandFactory commandFactory, 
            IDrawingStrategyFactory drawingStrategyFactory)
        {
            _comSerial = comSerial;
            _commandFactory = commandFactory;
            _drawingStrategyFactory = drawingStrategyFactory;
            Init();
        }

        public void Dispose()
        {
            AllDown();
            Release();
            GC.SuppressFinalize(this);
        }

        public void Init()
        {
            if (!_comSerial.IsOpen)
            {
                _comSerial.SerialOpen();
            }
        }

        public void Release()
        {
            if (_comSerial.IsOpen)
            {
                _comSerial.SerialClose();
            }
        }

        public void AllDown()
        {
            var command = _commandFactory.CreateBasicCommand(_comSerial, _predefined.AllDownBytes);
            command.Execute();
        }

        public void AllUp()
        {
            var command = _commandFactory.CreateBasicCommand(_comSerial, _predefined.AllUpBytes);
            command.Execute();
        }

        public void LineDown(byte lineID)
        {
            var command = _commandFactory.CreateLineCommand(_comSerial, _predefined.LineDownBytes, lineID);
            command.Execute();
        }

        public void LineDown(byte lineID, byte startOffset)
        {
            var command = _commandFactory.CreateLineCommand(_comSerial, _predefined.LineDownBytes, lineID, startOffset);
            command.Execute();
        }
        public void LineUp(byte lineID)
        {
            var command = _commandFactory.CreateLineCommand(_comSerial, _predefined.LineUpBytes, lineID);
            command.Execute();
        }

        public void LineUp(byte lineID, byte startOffset)
        {
            var command = _commandFactory.CreateLineCommand(_comSerial, _predefined.LineUpBytes, lineID, startOffset);
            command.Execute();
        }

        public void PartialDown(byte[] cellOffset)
        {
            var command = _commandFactory.CreatePartialCommand(_comSerial, _predefined.PartialDownBytes, cellOffset);
            command.Execute();
        }

        public void PartialUp(byte[] cellOffset)
        {
            var command = _commandFactory.CreatePartialCommand(_comSerial, _predefined.PartialUpBytes, cellOffset);
            command.Execute();
        }

        private void DrawCommon(
            DrawingParameters drawingParameters,
            Func<int[,], Area, int, int, (byte[], byte[])> dataGenerator,
            Func<IDrawingStrategy> strategySelector)
        {
            int[,] dots = Conversion.ImageToDots(
                drawingParameters.Image, 
                drawingParameters.XDotRes, 
                drawingParameters.YDotRes, 
                drawingParameters.Weight
            );

            (byte[] argCellData, byte[] cellOffset) = dataGenerator(
                dots,
                drawingParameters.Area,
                drawingParameters.XDotRes,
                drawingParameters.YDotRes
            );

            var command = strategySelector().CreateDrawingCommand(
                _comSerial, 
                argCellData, 
                cellOffset
            );
            command.Execute();
        }

        public void DrawLines(DrawingParameters drawingParameters)
        {
            DrawCommon(
                drawingParameters,
                Transmit.SetTxLineData,
                _drawingStrategyFactory.GetLineDrawingStrategy
            );
        }

        public void DrawPartial(DrawingParameters drawingParameters)
        {
            DrawCommon(
                drawingParameters,
                Transmit.SetTxPartialData,
                _drawingStrategyFactory.GetPartialDrawingStrategy
            );
        }
    }
}   