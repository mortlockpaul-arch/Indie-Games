using System;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class Intro
{
	protected Game m_game;

	public Intro(Game game)
	{
		try
		{
			m_game = game;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void Clear()
	{
		try
		{
			m_game = null;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void Start()
	{
	}

	public virtual void Update(TimeSpan elapsed)
	{
	}

	public virtual void Draw(SpriteBatch SB)
	{
	}
}
