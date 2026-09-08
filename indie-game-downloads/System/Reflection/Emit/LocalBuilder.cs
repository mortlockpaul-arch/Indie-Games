namespace System.Reflection.Emit;

public abstract class LocalBuilder : LocalVariableInfo
{
	public void SetLocalSymInfo(string name)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		SetLocalSymInfoCore(name);
	}

	protected virtual void SetLocalSymInfoCore(string name)
	{
		throw new NotSupportedException(SR.NotSupported_EmitDebugInfo);
	}
}
