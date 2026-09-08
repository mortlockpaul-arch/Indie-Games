using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using SDL2;

namespace Microsoft.Xna.Framework.GamerServices;

public static class Guide
{
	internal class GuideAction : IAsyncResult
	{
		public readonly AsyncCallback Callback;

		public object AsyncState { get; private set; }

		public bool CompletedSynchronously => false;

		public bool IsCompleted { get; internal set; }

		public WaitHandle AsyncWaitHandle { get; private set; }

		public GuideAction(object state, AsyncCallback callback)
		{
			AsyncState = state;
			Callback = callback;
			IsCompleted = false;
			AsyncWaitHandle = new ManualResetEvent(initialState: true);
		}
	}

	private static NotificationPosition position;

	public static bool IsScreenSaverEnabled
	{
		get
		{
			return SDL.SDL_IsScreenSaverEnabled() == SDL.SDL_bool.SDL_TRUE;
		}
		set
		{
			if (value)
			{
				SDL.SDL_EnableScreenSaver();
			}
			else
			{
				SDL.SDL_DisableScreenSaver();
			}
		}
	}

	public static bool IsTrialMode { get; set; }

	public static bool IsVisible
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public static NotificationPosition NotificationPosition
	{
		get
		{
			return position;
		}
		set
		{
			if (value != position)
			{
				position = value;
			}
		}
	}

	public static bool SimulateTrialMode { get; set; }

	static Guide()
	{
		position = NotificationPosition.BottomRight;
		IsTrialMode = false;
		SimulateTrialMode = false;
	}

	public static IAsyncResult BeginShowKeyboardInput(PlayerIndex player, string title, string description, string defaultText, AsyncCallback callback, object state)
	{
		return BeginShowKeyboardInput(player, title, description, defaultText, callback, state, usePasswordMode: false);
	}

	public static IAsyncResult BeginShowKeyboardInput(PlayerIndex player, string title, string description, string defaultText, AsyncCallback callback, object state, bool usePasswordMode)
	{
		TextInputEXT.StartTextInput();
		return new GuideAction(state, callback)
		{
			IsCompleted = true
		};
	}

	public static string EndShowKeyboardInput(IAsyncResult result)
	{
		TextInputEXT.StopTextInput();
		return "";
	}

	public static IAsyncResult BeginShowMessageBox(string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
	{
		throw new NotSupportedException();
	}

	public static IAsyncResult BeginShowMessageBox(PlayerIndex player, string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
	{
		throw new NotSupportedException();
	}

	public static int? EndShowMessageBox(IAsyncResult result)
	{
		throw new NotSupportedException();
	}

	public static void DelayNotifications(TimeSpan delay)
	{
	}

	public static void ShowComposeMessage(PlayerIndex player, string text, IEnumerable<Gamer> recipients)
	{
	}

	public static void ShowFriendRequest(PlayerIndex player, Gamer gamer)
	{
	}

	public static void ShowFriends(PlayerIndex player)
	{
	}

	public static void ShowGameInvite(PlayerIndex player, IEnumerable<Gamer> recipients)
	{
	}

	public static void ShowGameInvite(string sessionId)
	{
	}

	public static void ShowGamerCard(PlayerIndex player, Gamer gamer)
	{
	}

	public static void ShowMarketplace(PlayerIndex player)
	{
	}

	public static void ShowMessages(PlayerIndex player)
	{
	}

	public static void ShowParty(PlayerIndex player)
	{
	}

	public static void ShowPartySessions(PlayerIndex player)
	{
	}

	public static void ShowPlayerReview(PlayerIndex player, Gamer gamer)
	{
	}

	public static void ShowPlayers(PlayerIndex player)
	{
	}

	public static void ShowSignIn(int paneCount, bool onlineOnly)
	{
	}

	public static void ShowAchievementsEXT(PlayerIndex player)
	{
	}
}
