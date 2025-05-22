﻿using DotPadExp.DotPad.Protocol;
using DotPadExp.DotPad.Command;
using DotPadExp.Experiment;

namespace DotPadExp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ComSerial comSerial = new();
            ICommandFactory commandFactory = new CommandFactory();
            IDrawingStrategyFactory strategyFactory = new DrawingStrategyFactory();

            using Controller controller = new(
                comSerial,
                commandFactory,
                strategyFactory
            );

            // Client client = new(controller);
            // client.Listen();
           
            Test test = new(controller);
            test.TestDotPadDrawLines(40, 40);
        }
    }
}