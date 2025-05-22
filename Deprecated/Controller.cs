namespace DotPadExp.Deprecated
{
    public class Controller : IDisposable
    {
        private readonly ComSerial _comSerial = new();
        private readonly Predefined _predefined = new();

        public Controller()
        {
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

        public class CMDExecutor(ComSerial comSerial)
        {
            public void Execute(byte[] data)
            {
                comSerial.SerialSend(data);
            }
        }

        public void AllDown()
        {   
            ReqBase reqCellDisplay = new ReqCellDisplay(new ReqCellParameters{
                ArgCellData = _predefined.AllDownBytes
            });
            _comSerial.SerialSend(reqCellDisplay.CreateReq());
        }

        public void AllUp()
        {
            ReqBase reqCellDisplay = new ReqCellDisplay(new ReqCellParameters{
                ArgCellData = _predefined.AllUpBytes
            });
            _comSerial.SerialSend(reqCellDisplay.CreateReq());
        }

        public void PartialDown(byte[] cellOffSet)
        {
            ReqBase reqPartialDisplay = new ReqPartialDisplay(new ReqCellParameters{
                ArgCellData = _predefined.PartialDownBytes, 
                ArgStartOffset = cellOffSet[0], 
                ArgEndOffset = cellOffSet[1]
            });
            _comSerial.SerialSend(reqPartialDisplay.CreateReq());
        }

        public void PartialUp(byte[] cellOffSet)
        {
            ReqBase reqPartialDisplay = new ReqPartialDisplay(new ReqCellParameters{
                ArgCellData = _predefined.PartialUpBytes, 
                ArgStartOffset = cellOffSet[0], 
                ArgEndOffset = cellOffSet[1]
            });
            _comSerial.SerialSend(reqPartialDisplay.CreateReq());
        }

        public void LineDown(byte lineID)
        {
            ReqBase reqCellDisplay = new ReqCellDisplay(new ReqCellParameters{
                ArgCellData = _predefined.LineDownBytes, 
                ArgDestID = lineID
            });
            _comSerial.SerialSend(reqCellDisplay.CreateReq());
        }

        public void LineDown(byte lineID, byte startOffset)
        {
            ReqBase reqCellDisplay = new ReqCellDisplay(new ReqCellParameters{
                ArgCellData = _predefined.LineDownBytes, 
                ArgDestID = lineID, 
                ArgStartOffset = startOffset
            });;
            _comSerial.SerialSend(reqCellDisplay.CreateReq());
        }

        public void LineUp(byte lineID)
        {
            ReqBase reqCellDisplay = new ReqCellDisplay(new ReqCellParameters{
                ArgCellData = _predefined.LineUpBytes, 
                ArgDestID = lineID
            });
            _comSerial.SerialSend(reqCellDisplay.CreateReq());
        }

        public void LineUp(byte lineID, byte startOffset)
        {
             ReqBase reqCellDisplay = new ReqCellDisplay(new ReqCellParameters{
                ArgCellData = _predefined.LineUpBytes, 
                ArgStartOffset = startOffset,
                ArgDestID = lineID
            });
            _comSerial.SerialSend(reqCellDisplay.CreateReq());
        }

        public void Draw(byte[] argCellData)
        {
            ReqBase reqCellDisplay = new ReqCellDisplay(new ReqCellParameters{
                ArgCellData = argCellData
            });
            _comSerial.SerialSend(reqCellDisplay.CreateReq());
        }
        
        /// <summary>
        /// Drow Dots Image, Updating partial cells
        /// </summary>
        public void DrawPartial(string image, Area area, int dotResX, int dotResY, double weight)
        {
            int[,] dots = Conversion.ImageToDots(image, dotResX, dotResY, weight);
            (byte[] argCellData, byte[] cellOffSet) = Transmit.SetTxPartialData(
                dots, 
                area, 
                dotResX, 
                dotResY
            );
            ReqBase reqPartialDisplay = new ReqPartialDisplay(new ReqCellParameters{
                ArgCellData = argCellData, 
                ArgStartOffset = cellOffSet[0], 
                ArgEndOffset = cellOffSet[1], 
                ArgDestID = cellOffSet[2]
            });
            _comSerial.SerialSend(reqPartialDisplay.CreateReq());
        }

        /// <summary>
        /// Drow Dots Image, Updating line cells
        /// </summary>
        /// <param name="image">filename of image</param>
        /// <param name="area">area object where target to drawing</param>
        /// <param name="dotResX">X resolution of dots</param>
        /// <param name="dotResY">Y resolution of dots</param>
        /// <param name="weight">ImageToDots conversion weight</param>
        public void DrawLines(string image, Area area, int dotResX, int dotResY, double weight)
        {
            int[,] dots = Conversion.ImageToDots(image, dotResX, dotResY, weight);
            (byte[] argCellData, byte[] cellOffSet) = Transmit.SetTxLineData(
                dots, 
                area, 
                dotResX, 
                dotResY
            );
            ReqBase reqCellDisplay = new ReqCellDisplay(new ReqCellParameters{
                ArgCellData = argCellData,
                ArgStartOffset = cellOffSet[0],
                ArgDestID = cellOffSet[1]
            });
            _comSerial.SerialSend(reqCellDisplay.CreateReq());
        }
    }
}