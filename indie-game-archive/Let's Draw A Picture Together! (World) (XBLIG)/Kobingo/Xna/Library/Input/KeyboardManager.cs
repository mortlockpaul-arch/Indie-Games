using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Kobingo.Xna.Library.Input;

public class KeyboardManager
{
	private class KeyboardManagerComponent : GameComponent
	{
		public KeyboardState[] PreviousStates;

		public KeyboardState[] CurrentStates;

		public KeyboardManagerComponent(Game game)
			: base(game)
		{
			PreviousStates = (KeyboardState[])(object)new KeyboardState[4];
			CurrentStates = (KeyboardState[])(object)new KeyboardState[4];
		}

		public override void Update(GameTime gameTime)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < 4; i++)
			{
				ref KeyboardState reference = ref PreviousStates[i];
				reference = CurrentStates[i];
				ref KeyboardState reference2 = ref CurrentStates[i];
				reference2 = Keyboard.GetState((PlayerIndex)i);
			}
			((GameComponent)this).Update(gameTime);
		}
	}

	private static KeyboardManagerComponent m_KeyboardManagerComponent;

	public static void Initialize(Game game)
	{
		if (m_KeyboardManagerComponent == null)
		{
			m_KeyboardManagerComponent = new KeyboardManagerComponent(game);
			((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)(object)m_KeyboardManagerComponent);
			return;
		}
		throw new InvalidOperationException("The keyboard manager has already been initialized.");
	}

	public static bool IsKeyDown(Keys key)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (m_KeyboardManagerComponent == null)
		{
			throw new InvalidOperationException("The keyboard manager has not been initialized.");
		}
		for (int i = 0; i < 4; i++)
		{
			if (IsKeyDown((PlayerIndex)i, key))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsKeyDown(PlayerIndex playerIndex, Keys key)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (m_KeyboardManagerComponent == null)
		{
			throw new InvalidOperationException("The keyboard manager has not been initialized.");
		}
		return ((KeyboardState)(ref m_KeyboardManagerComponent.CurrentStates[playerIndex])).IsKeyDown(key);
	}

	public static bool IsKeyPress(Keys key)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (m_KeyboardManagerComponent == null)
		{
			throw new InvalidOperationException("The keyboard manager has not been initialized.");
		}
		for (int i = 0; i < 4; i++)
		{
			if (IsKeyPress((PlayerIndex)i, key))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsKeyPress(PlayerIndex playerIndex, Keys key)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (m_KeyboardManagerComponent == null)
		{
			throw new InvalidOperationException("The keyboard manager has not been initialized.");
		}
		if (((KeyboardState)(ref m_KeyboardManagerComponent.CurrentStates[playerIndex])).IsKeyDown(key))
		{
			return ((KeyboardState)(ref m_KeyboardManagerComponent.PreviousStates[playerIndex])).IsKeyUp(key);
		}
		return false;
	}

	public static KeyboardState GetState(PlayerIndex playerIndex)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return m_KeyboardManagerComponent.CurrentStates[playerIndex];
	}
}
