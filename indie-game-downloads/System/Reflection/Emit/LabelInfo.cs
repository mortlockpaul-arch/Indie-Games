using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit;

internal sealed class LabelInfo
{
	internal int _position;

	internal int _startDepth;

	internal LabelHandle _metaLabel;

	internal LabelInfo(LabelHandle metaLabel)
	{
		_position = -1;
		_startDepth = -1;
		_metaLabel = metaLabel;
	}
}
