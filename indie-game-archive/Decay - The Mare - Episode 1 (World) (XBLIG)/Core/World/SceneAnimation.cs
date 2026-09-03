using System;
using Microsoft.Xna.Framework.Graphics;

namespace Core.World;

public class SceneAnimation : Scene
{
	public Animation2D m_animation;

	public Animation2D.LOOP_TYPE m_loop_type = Animation2D.LOOP_TYPE.PING_PONG;

	public SceneAnimation(Animation2D animation)
		: base(null)
	{
		if (animation != null)
		{
			m_animation = animation;
			m_animation.Play(m_loop_type);
		}
	}

	public SceneAnimation(Animation2D animation, Animation2D.LOOP_TYPE loop_type)
		: base(null)
	{
		if (animation != null)
		{
			m_loop_type = loop_type;
			m_animation = animation;
			m_animation.Play(m_loop_type);
		}
	}

	public override void Clear()
	{
		if (m_animation != null)
		{
			m_animation.Clear();
			m_animation = null;
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		if (m_animation != null)
		{
			m_animation.Update(elapsed);
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		if (SB != null && m_animation != null)
		{
			m_animation.Draw(SB, m_color);
		}
	}
}
