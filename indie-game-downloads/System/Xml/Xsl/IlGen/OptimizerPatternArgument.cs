namespace System.Xml.Xsl.IlGen;

internal enum OptimizerPatternArgument
{
	StepNode = 0,
	StepInput = 1,
	ElementQName = 2,
	KindTestType = ElementQName,
	IndexedNodes = StepNode,
	KeyExpression = StepInput,
	DodStep = ElementQName,
	MaxPosition = ElementQName,
	RtfText = ElementQName
}
