using Microsoft.Xna.Framework;

namespace Quasar;

internal struct ElementAttribute
{
	public int IntValue;

	public long LongValue;

	public float FloatValue;

	public Vector4 Vector4Value;

	public object ObjectValue;

	public ElementAttribute(object objectValue)
	{
		ObjectValue = objectValue;
		IntValue = 0;
		LongValue = 0L;
		FloatValue = 0f;
		Vector4Value = Vector4.Zero;
	}

	public ElementAttribute(int intValue)
	{
		IntValue = intValue;
		LongValue = 0L;
		FloatValue = 0f;
		Vector4Value = Vector4.Zero;
		ObjectValue = null;
	}

	public ElementAttribute(long longValue)
	{
		LongValue = longValue;
		IntValue = 0;
		FloatValue = 0f;
		Vector4Value = Vector4.Zero;
		ObjectValue = null;
	}

	public ElementAttribute(float floatValue)
	{
		FloatValue = floatValue;
		LongValue = 0L;
		IntValue = 0;
		Vector4Value = Vector4.Zero;
		ObjectValue = null;
	}

	public ElementAttribute(Vector4 vectorValue)
	{
		Vector4Value = vectorValue;
		LongValue = 0L;
		FloatValue = 0f;
		IntValue = 0;
		ObjectValue = null;
	}
}
