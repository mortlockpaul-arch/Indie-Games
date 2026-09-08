namespace System.Runtime.CompilerServices;

internal sealed class Continuation
{
	public Continuation Next;

	public unsafe delegate*<Continuation, Continuation> Resume;

	public uint State;

	public CorInfoContinuationFlags Flags;

	public byte[] Data;

	public object[] GCData;

	public object GetContinuationContext()
	{
		int num = 0;
		if ((Flags & CorInfoContinuationFlags.CORINFO_CONTINUATION_RESULT_IN_GCDATA) != 0)
		{
			num++;
		}
		if ((Flags & CorInfoContinuationFlags.CORINFO_CONTINUATION_NEEDS_EXCEPTION) != 0)
		{
			num++;
		}
		return GCData[num];
	}

	public void SetException(Exception ex)
	{
		int num = 0;
		if ((Flags & CorInfoContinuationFlags.CORINFO_CONTINUATION_RESULT_IN_GCDATA) != 0)
		{
			num++;
		}
		GCData[num] = ex;
	}
}
