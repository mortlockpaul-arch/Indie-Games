using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace XnaLibrary.Diagnostics;

public class TimeWatcher
{
	[CompilerGenerated]
	private Color _003CColor_003Ek__BackingField;

	public string Name { get; set; }

	public Stopwatch Time { get; private set; }

	public Color Color
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CColor_003Ek__BackingField = value;
		}
	}

	public TimeWatcher()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Time = new Stopwatch();
		Color = Color.Red;
	}
}
