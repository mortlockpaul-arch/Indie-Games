using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Kobingo.Xna.Library.Input;

public class GamepadManager
{
	private class GamepadManagerComponent : GameComponent
	{
		public GamePadState[] PreviousStates;

		public GamePadState[] CurrentStates;

		public GamepadManagerComponent(Game game)
			: base(game)
		{
			PreviousStates = (GamePadState[])(object)new GamePadState[4];
			CurrentStates = (GamePadState[])(object)new GamePadState[4];
		}

		public override void Update(GameTime gameTime)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < 4; i++)
			{
				ref GamePadState reference = ref PreviousStates[i];
				reference = CurrentStates[i];
				ref GamePadState reference2 = ref CurrentStates[i];
				reference2 = GamePad.GetState((PlayerIndex)i);
			}
			((GameComponent)this).Update(gameTime);
		}
	}

	private static GamepadManagerComponent m_GamepadManagerComponent;

	public static void Initialize(Game game)
	{
		if (m_GamepadManagerComponent == null)
		{
			m_GamepadManagerComponent = new GamepadManagerComponent(game);
			((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)(object)m_GamepadManagerComponent);
			return;
		}
		throw new InvalidOperationException("The gamepad manager has already been initialized.");
	}

	public static bool IsButtonDown(Buttons button)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (m_GamepadManagerComponent == null)
		{
			throw new InvalidOperationException("The gamepad manager has not been initialized.");
		}
		for (int i = 0; i < 4; i++)
		{
			if (IsButtonDown((PlayerIndex)i, button))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsButtonDown(PlayerIndex playerIndex, Buttons button)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (m_GamepadManagerComponent == null)
		{
			throw new InvalidOperationException("The gamepad manager has not been initialized.");
		}
		return ((GamePadState)(ref m_GamepadManagerComponent.CurrentStates[playerIndex])).IsButtonDown(button);
	}

	public static bool IsButtonPressed(Buttons button)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (m_GamepadManagerComponent == null)
		{
			throw new InvalidOperationException("The gamepad manager has not been initialized.");
		}
		for (int i = 0; i < 4; i++)
		{
			if (IsButtonPressed((PlayerIndex)i, button))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsButtonPressed(PlayerIndex playerIndex, Buttons button)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (m_GamepadManagerComponent == null)
		{
			throw new InvalidOperationException("The gamepad manager has not been initialized.");
		}
		if (((GamePadState)(ref m_GamepadManagerComponent.CurrentStates[playerIndex])).IsButtonDown(button))
		{
			return ((GamePadState)(ref m_GamepadManagerComponent.PreviousStates[playerIndex])).IsButtonUp(button);
		}
		return false;
	}

	public static bool IsButtonReleased(Buttons button)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (m_GamepadManagerComponent == null)
		{
			throw new InvalidOperationException("The gamepad manager has not been initialized.");
		}
		for (int i = 0; i < 4; i++)
		{
			if (IsButtonReleased((PlayerIndex)i, button))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsButtonReleased(PlayerIndex playerIndex, Buttons button)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (m_GamepadManagerComponent == null)
		{
			throw new InvalidOperationException("The gamepad manager has not been initialized.");
		}
		if (((GamePadState)(ref m_GamepadManagerComponent.CurrentStates[playerIndex])).IsButtonUp(button))
		{
			return ((GamePadState)(ref m_GamepadManagerComponent.PreviousStates[playerIndex])).IsButtonDown(button);
		}
		return false;
	}

	public static GamePadState GetState(PlayerIndex playerIndex)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return m_GamepadManagerComponent.CurrentStates[playerIndex];
	}
}
