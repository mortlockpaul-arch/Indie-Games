using Microsoft.Xna.Framework;

namespace OluXNA;

internal class ChannelEffect
{
	public int channel;

	public ChannelEffect()
	{
		channel = 0;
	}

	public virtual void Update(GameTime gametime)
	{
	}

	public virtual void Draw(GameTime gametime, float curVal, ref EffectParameterCollectionRedux[] toUpdate)
	{
	}
}
