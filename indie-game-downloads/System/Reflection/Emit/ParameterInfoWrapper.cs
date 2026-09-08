namespace System.Reflection.Emit;

internal sealed class ParameterInfoWrapper : ParameterInfo
{
	private readonly ParameterBuilderImpl _pb;

	private readonly Type _type;

	public override ParameterAttributes Attributes => _pb._attributes;

	public override string Name => _pb.Name;

	public override int Position => _pb.Position;

	public override Type ParameterType => _type;

	public override bool HasDefaultValue => _pb._defaultValue != DBNull.Value;

	public override object DefaultValue
	{
		get
		{
			if (!HasDefaultValue)
			{
				return null;
			}
			return _pb._defaultValue;
		}
	}

	public ParameterInfoWrapper(ParameterBuilderImpl pb, Type type)
	{
		_pb = pb;
		_type = type;
	}

	public override Type GetModifiedParameterType()
	{
		return ParameterType;
	}
}
