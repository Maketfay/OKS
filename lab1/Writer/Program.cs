
using Writer;

var argument = args[0].Split(',');

var portName = argument[0].Trim();

var speed = int.Parse(argument[1].Trim());

var destinationPort = int.Parse(argument[2].Replace("COM", "").Trim());

var readerPort = new WriterPort(portName, speed);

readerPort.WriteProccess(destinationPort);