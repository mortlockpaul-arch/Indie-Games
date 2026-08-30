namespace Skeleton;

public class TypedAnimFrame
{
	private int m_iTime;

	private int m_iIndex;

	public int Time => m_iTime;

	public int Index => m_iIndex;

	public TypedAnimFrame(int time, int index)
	{
		m_iTime = time;
		m_iIndex = index;
	}
}
