using DataContent;

namespace EGEngine;

public struct TriggerStruct
{
	public TriggerFlags flag;

	public OOBB oobb;

	public TriangleData[] mesh;
}
