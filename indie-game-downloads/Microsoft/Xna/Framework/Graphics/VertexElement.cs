namespace Microsoft.Xna.Framework.Graphics;

public struct VertexElement
{
	private int offset;

	private VertexElementFormat vertexElementFormat;

	private VertexElementUsage vertexElementUsage;

	private int usageIndex;

	public int Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
		}
	}

	public VertexElementFormat VertexElementFormat
	{
		get
		{
			return vertexElementFormat;
		}
		set
		{
			vertexElementFormat = value;
		}
	}

	public VertexElementUsage VertexElementUsage
	{
		get
		{
			return vertexElementUsage;
		}
		set
		{
			vertexElementUsage = value;
		}
	}

	public int UsageIndex
	{
		get
		{
			return usageIndex;
		}
		set
		{
			usageIndex = value;
		}
	}

	public VertexElement(int offset, VertexElementFormat elementFormat, VertexElementUsage elementUsage, int usageIndex)
	{
		this = default(VertexElement);
		Offset = offset;
		UsageIndex = usageIndex;
		VertexElementFormat = elementFormat;
		VertexElementUsage = elementUsage;
	}

	public override int GetHashCode()
	{
		return offset ^ usageIndex ^ vertexElementFormat.GetHashCode() ^ vertexElementUsage.GetHashCode();
	}

	public override string ToString()
	{
		return "{{Offset:" + Offset + " Format:" + VertexElementFormat.ToString() + " Usage:" + VertexElementUsage.ToString() + " UsageIndex: " + UsageIndex + "}}";
	}

	public override bool Equals(object obj)
	{
		return obj is VertexElement && this == (VertexElement)obj;
	}

	public static bool operator ==(VertexElement left, VertexElement right)
	{
		return left.Offset == right.Offset && left.UsageIndex == right.UsageIndex && left.VertexElementUsage == right.VertexElementUsage && left.VertexElementFormat == right.VertexElementFormat;
	}

	public static bool operator !=(VertexElement left, VertexElement right)
	{
		return !(left == right);
	}
}
