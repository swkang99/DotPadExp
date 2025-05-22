using DotPadExp.DotPad.Protocol;

namespace DotPadExp.DotPad.Command
{
    public interface ICommand
    {
        void Execute();
    }

    public abstract class DotPadCommand(ComSerial comSerial, ReqCellParameters parameters) : ICommand
    {
        protected readonly ComSerial _comSerial = comSerial;
        protected readonly ReqCellParameters _parameters = parameters;

        public abstract void Execute();
    }

    public class CellDisplayCommand(ComSerial comSerial, ReqCellParameters parameters) : DotPadCommand(comSerial, parameters)
    {
        public override void Execute()
        {
            var request = new ReqCellDisplay(_parameters);
            _comSerial.SerialSend(request.CreateReq());
        }
    }

    public class PartialDisplayCommand(ComSerial comSerial, ReqCellParameters parameters) : DotPadCommand(comSerial, parameters)
    {
        public override void Execute()
        {
            var request = new ReqPartialDisplay(_parameters);
            _comSerial.SerialSend(request.CreateReq());
        }
    }

    public interface ICommandFactory
    {
        ICommand CreateBasicCommand(ComSerial comSerial, byte[] cellData);
        ICommand CreateLineCommand(ComSerial comSerial, byte[] cellData, byte lineId, byte? offset = null);
        ICommand CreatePartialCommand(ComSerial comSerial, byte[] cellData, byte[] cellOffset);
    }

    public class CommandFactory : ICommandFactory
    {
        public ICommand CreateBasicCommand(ComSerial comSerial, byte[] cellData)
        {
            return new CellDisplayCommand(comSerial, new ReqCellParameters { ArgCellData = cellData });
        }

        public ICommand CreateLineCommand(ComSerial comSerial, byte[] cellData, byte lineId, byte? offset = null)
        {
            var parameters = new ReqCellParameters
            {
                ArgCellData = cellData,
                ArgDestID = lineId,
                ArgStartOffset = offset ?? 0
            };
            return new CellDisplayCommand(comSerial, parameters);
        }

        public ICommand CreatePartialCommand(ComSerial comSerial, byte[] cellData, byte[] cellOffset)
        {
            var parameters = new ReqCellParameters
            {
                ArgCellData = cellData,
                ArgDestID = cellOffset[0],
                ArgStartOffset = cellOffset[1],
                ArgEndOffset = cellOffset[2]
            };
            return new PartialDisplayCommand(comSerial, parameters);
        }
    }
}