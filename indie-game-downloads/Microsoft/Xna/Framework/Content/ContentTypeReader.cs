using System;

namespace Microsoft.Xna.Framework.Content;

public abstract class ContentTypeReader
{
	private Type targetType;

	public virtual bool CanDeserializeIntoExistingObject => false;

	public Type TargetType => targetType;

	public virtual int TypeVersion => 0;

	protected ContentTypeReader(Type targetType)
	{
		this.targetType = targetType;
	}

	protected internal virtual void Initialize(ContentTypeReaderManager manager)
	{
	}

	protected internal abstract object Read(ContentReader input, object existingInstance);
}
public abstract class ContentTypeReader<T> : ContentTypeReader
{
	protected ContentTypeReader()
		: base(typeof(T))
	{
	}

	protected internal override object Read(ContentReader input, object existingInstance)
	{
		if (existingInstance == null)
		{
			return Read(input, default(T));
		}
		return Read(input, (T)existingInstance);
	}

	protected internal abstract T Read(ContentReader input, T existingInstance);
}
