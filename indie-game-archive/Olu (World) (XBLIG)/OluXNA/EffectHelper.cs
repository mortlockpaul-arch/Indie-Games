using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class EffectHelper
{
	public EffectParameterCollectionRedux[] toUpdate;

	public EffectParameterCollection fxUpdate;

	public virtual void Update(GameTime gametime)
	{
	}

	public virtual void Draw(GameTime gametime)
	{
	}
}
