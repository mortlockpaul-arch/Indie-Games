using System;

namespace Microsoft.Xna.Framework.Content;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ContentSerializerCollectionItemNameAttribute : Attribute
{
	public string CollectionItemName { get; private set; }

	public ContentSerializerCollectionItemNameAttribute(string collectionItemName)
	{
		if (string.IsNullOrEmpty(collectionItemName))
		{
			throw new ArgumentNullException("collectionItemName");
		}
		CollectionItemName = collectionItemName;
	}
}
