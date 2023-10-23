using Node;

var arguments = args[0].Split(',');

var writerPortName = arguments[0].Trim();
var readerPortName = arguments[1].Trim();

var speed = int.Parse(arguments[2].Trim());
var portId = (byte)int.Parse(arguments[3].Trim());

var nodeManager = new NodeManager(writerPortName, readerPortName, speed, portId, isMonitor: portId == 1);

Console.WriteLine("Node writer port is " + writerPortName + " Node reader port is " + readerPortName + " is monitor " + (portId == 1));

nodeManager.Process();