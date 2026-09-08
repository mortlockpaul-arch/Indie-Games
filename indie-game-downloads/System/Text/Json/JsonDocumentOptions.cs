using System.Runtime.CompilerServices;

namespace System.Text.Json;

public struct JsonDocumentOptions
{
	private int _maxDepth;

	private JsonCommentHandling _commentHandling;

	[CompilerGenerated]
	private bool _003CAllowDuplicateProperties_003Ek__BackingField;

	public JsonCommentHandling CommentHandling
	{
		readonly get
		{
			return _commentHandling;
		}
		set
		{
			if ((int)value > 1)
			{
				throw new ArgumentOutOfRangeException("value", System.SR.JsonDocumentDoesNotSupportComments);
			}
			_commentHandling = value;
		}
	}

	public int MaxDepth
	{
		readonly get
		{
			return _maxDepth;
		}
		set
		{
			if (value < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_MaxDepthMustBePositive("value");
			}
			_maxDepth = value;
		}
	}

	public bool AllowTrailingCommas { get; set; }

	public bool AllowDuplicateProperties
	{
		get
		{
			return !_003CAllowDuplicateProperties_003Ek__BackingField;
		}
		set
		{
			_003CAllowDuplicateProperties_003Ek__BackingField = !value;
		}
	}

	internal JsonReaderOptions GetReaderOptions()
	{
		return new JsonReaderOptions
		{
			AllowTrailingCommas = AllowTrailingCommas,
			CommentHandling = CommentHandling,
			MaxDepth = MaxDepth
		};
	}
}
