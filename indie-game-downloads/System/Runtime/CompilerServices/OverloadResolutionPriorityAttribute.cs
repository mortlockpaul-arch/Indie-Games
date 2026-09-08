namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class OverloadResolutionPriorityAttribute : Attribute
{
	public int Priority { get; }

	public OverloadResolutionPriorityAttribute(int priority)
	{
		Priority = priority;
	}
}
