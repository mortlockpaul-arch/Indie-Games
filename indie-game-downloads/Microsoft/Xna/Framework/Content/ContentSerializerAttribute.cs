using System;

namespace Microsoft.Xna.Framework.Content;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ContentSerializerAttribute : Attribute
{
	private string collectionItemName;

	public bool AllowNull { get; set; }

	public string CollectionItemName
	{
		get
		{
			return collectionItemName ?? "Item";
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			collectionItemName = value;
		}
	}

	public string ElementName { get; set; }

	public bool FlattenContent { get; set; }

	public bool HasCollectionItemName => collectionItemName != null;

	public bool Optional { get; set; }

	public bool SharedResource { get; set; }

	public ContentSerializerAttribute()
	{
		AllowNull = true;
	}

	public ContentSerializerAttribute Clone()
	{
		return new ContentSerializerAttribute
		{
			AllowNull = AllowNull,
			collectionItemName = collectionItemName,
			ElementName = ElementName,
			FlattenContent = FlattenContent,
			Optional = Optional,
			SharedResource = SharedResource
		};
	}
}
