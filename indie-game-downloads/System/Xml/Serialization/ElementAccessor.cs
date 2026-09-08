namespace System.Xml.Serialization;

internal sealed class ElementAccessor : Accessor
{
	private bool _nullable;

	private bool _isSoap;

	private bool _unbounded;

	internal bool IsSoap
	{
		get
		{
			return _isSoap;
		}
		set
		{
			_isSoap = value;
		}
	}

	internal bool IsNullable
	{
		get
		{
			return _nullable;
		}
		set
		{
			_nullable = value;
		}
	}

	internal bool IsUnbounded
	{
		get
		{
			return _unbounded;
		}
		set
		{
			_unbounded = value;
		}
	}

	internal ElementAccessor Clone()
	{
		return new ElementAccessor
		{
			_nullable = _nullable,
			IsTopLevelInSchema = base.IsTopLevelInSchema,
			Form = base.Form,
			_isSoap = _isSoap,
			Name = Name,
			Default = base.Default,
			Namespace = base.Namespace,
			Mapping = base.Mapping,
			Any = base.Any
		};
	}
}
