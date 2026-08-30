using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Graphics.Particles;

public class Particle : Entity
{
	[CompilerGenerated]
	private Color _003CColor_003Ek__BackingField;

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

	public Particle()
	{
		base.IsActive = false;
	}
}
