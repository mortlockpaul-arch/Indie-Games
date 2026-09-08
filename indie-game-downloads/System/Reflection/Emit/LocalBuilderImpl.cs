namespace System.Reflection.Emit;

internal sealed class LocalBuilderImpl : LocalBuilder
{
	private readonly int _localIndex;

	private readonly Type _localType;

	private readonly MethodInfo _method;

	private readonly bool _isPinned;

	private string _name;

	internal string Name => _name;

	public override bool IsPinned => _isPinned;

	public override Type LocalType => _localType;

	public override int LocalIndex => _localIndex;

	internal LocalBuilderImpl(int index, Type type, MethodInfo method, bool isPinned)
	{
		_isPinned = isPinned;
		_localIndex = index;
		_localType = type;
		_method = method;
	}

	internal MethodInfo GetMethodBuilder()
	{
		return _method;
	}

	protected override void SetLocalSymInfoCore(string name)
	{
		if (_method.DeclaringType is TypeBuilder typeBuilder && typeBuilder.IsCreated())
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_TypeHasBeenCreated);
		}
		_name = name;
	}
}
