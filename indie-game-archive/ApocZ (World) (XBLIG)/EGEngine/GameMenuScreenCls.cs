using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class GameMenuScreenCls
{
	public enum GMSCState
	{
		Hidden,
		TransitionOn,
		TransitionOff,
		Active
	}

	public class Entry : EventArgs
	{
		public bool valid = true;

		public int cost;

		public string message = "";

		public Vector2 messagesize = Vector2.Zero;

		public event EventHandler<Entry> SelectedFunction;

		public void TryDelegate()
		{
			if (SelectedFunction != null)
			{
				SelectedFunction(this, this);
			}
		}
	}

	public List<Entry> MenuEntries = new List<Entry>();

	public GMSCState State;

	public float Timer;

	public int Selected;

	public int TextJustify;

	public bool EnableXButton = true;

	public Color SelectedColor = Color.Black;

	public Color DiffuseColor = Color.Black;

	public Color ShadowColor = Color.Black;

	public BoundingSphere BndSphere = default(BoundingSphere);

	protected bool MakeAvailabe;

	public static bool MenusActive = false;

	private static Vector2 msgPos = Vector2.Zero;

	private static Vector2 msgPosSdw = new Vector2(2f, 2f);

	public bool IsActive
	{
		get
		{
			return State != GMSCState.Hidden;
		}
		set
		{
		}
	}

	public void SetBoundingSphere(BoundingSphere bs)
	{
		BndSphere = bs;
		BndSphere.Radius *= 1.15f;
	}

	public virtual void Update(PlayerBase playerRef, int qIndex)
	{
		if (IsActive)
		{
			MenusActive = true;
		}
		float num = (BndSphere.Center - playerRef.vecPosition).LengthSquared();
		if (num < BndSphere.Radius * BndSphere.Radius)
		{
			MakeAvailabe = true;
		}
		else
		{
			MakeAvailabe = false;
		}
		if (State == GMSCState.Hidden)
		{
			if (MakeAvailabe && EnableXButton && playerRef.currentGamePadState.IsButtonDown(Buttons.X) && playerRef.lastGamePadState.IsButtonUp(Buttons.X))
			{
				Timer = 0f;
				State = GMSCState.TransitionOn;
			}
			return;
		}
		playerRef.Speed *= 0.5f;
		playerRef.lastSpeed *= 0.5f;
		playerRef.OverrideInput = true;
		playerRef.OverrideProjection = true;
		playerRef.ZoomOverride = (float)Math.PI / 3f;
		if (GenericMessages.IsActive())
		{
			return;
		}
		if (State == GMSCState.TransitionOn)
		{
			Timer += 0.03f;
			if (Timer >= 1f)
			{
				State = GMSCState.Active;
			}
		}
		else if (State == GMSCState.TransitionOff)
		{
			Timer -= 0.03f;
			if (Timer <= 0f)
			{
				State = GMSCState.Hidden;
			}
		}
		else if (State == GMSCState.Active && EnableXButton && !GenericMessages.IsActive())
		{
			if (playerRef.currentGamePadState.IsButtonDown(Buttons.A) && playerRef.lastGamePadState.IsButtonUp(Buttons.A))
			{
				MenuEntries[Selected].TryDelegate();
			}
			else if (playerRef.currentGamePadState.IsButtonDown(Buttons.DPadDown) && playerRef.lastGamePadState.IsButtonUp(Buttons.DPadDown))
			{
				NextSelected();
			}
			else if (playerRef.currentGamePadState.IsButtonDown(Buttons.DPadUp) && playerRef.lastGamePadState.IsButtonUp(Buttons.DPadUp))
			{
				PrevSelected();
			}
		}
	}

	public virtual void DrawPost(PlayerBase playerRef, int qIndex)
	{
		if (GenericMessages.IsActive())
		{
			return;
		}
		float num = 1.2f;
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		if (IsActive)
		{
			Timer = ((Timer > 1f) ? 1f : Timer);
			Timer = ((Timer < 0f) ? 0f : Timer);
			msgPos.Y = viewport.TitleSafeArea.Top + 128;
			ShadowColor = Color.Black;
			ref Color shadowColor = ref ShadowColor;
			ref Color shadowColor2 = ref ShadowColor;
			byte b = (ShadowColor.B = 0);
			byte r = (shadowColor2.G = b);
			shadowColor.R = r;
			ShadowColor.A = (byte)(Timer * 255f);
			DiffuseColor = Color.Black;
			ref Color diffuseColor = ref DiffuseColor;
			ref Color diffuseColor2 = ref DiffuseColor;
			byte b4 = (DiffuseColor.B = (byte)(Timer * 120f));
			byte r2 = (diffuseColor2.G = b4);
			diffuseColor.R = r2;
			DiffuseColor.A = (byte)(Timer * 255f);
			SelectedColor = Color.Black;
			ref Color selectedColor = ref SelectedColor;
			ref Color selectedColor2 = ref SelectedColor;
			byte b7 = (SelectedColor.B = (byte)(Timer * 211f));
			byte r3 = (selectedColor2.G = b7);
			selectedColor.R = r3;
			SelectedColor.A = (byte)(Timer * 255f);
			Menu.spriteBatch.Begin();
			for (int i = 0; i < MenuEntries.Count; i++)
			{
				if (Selected == i)
				{
					if (TextJustify == 0)
					{
						msgPos.X = (float)viewport.TitleSafeArea.Center.X - MenuEntries[i].messagesize.X * 0.5f * (num + 0.2f);
					}
					else if (TextJustify == 1)
					{
						msgPos.X = (float)viewport.TitleSafeArea.Left + 300f;
					}
					Menu.spriteBatch.DrawString(Menu.defaultFont, MenuEntries[i].message, msgPos, ShadowColor, 0f, Vector2.Zero, num + 0.2f, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, MenuEntries[i].message, msgPos, SelectedColor, 0f, msgPosSdw, num + 0.2f, SpriteEffects.None, 0);
				}
				else
				{
					if (TextJustify == 0)
					{
						msgPos.X = (float)viewport.TitleSafeArea.Center.X - MenuEntries[i].messagesize.X * 0.5f * num;
					}
					else if (TextJustify == 1)
					{
						msgPos.X = (float)viewport.TitleSafeArea.Left + 300f;
					}
					Menu.spriteBatch.DrawString(Menu.defaultFont, MenuEntries[i].message, msgPos, ShadowColor, 0f, Vector2.Zero, num, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, MenuEntries[i].message, msgPos, DiffuseColor, 0f, msgPosSdw, num, SpriteEffects.None, 0);
				}
				msgPos.Y += 36f;
			}
			Menu.spriteBatch.End();
		}
		else if (MakeAvailabe)
		{
			string text = "Press X To Enter Menu";
			msgPos.X = (float)viewport.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(text).X * 0.5f * (num + 0.2f);
			msgPos.Y = viewport.TitleSafeArea.Top + 256;
			ShadowColor = Color.Black;
			DiffuseColor = Color.LightGray;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, msgPos, ShadowColor, 0f, Vector2.Zero, num + 0.2f, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, msgPos, DiffuseColor, 0f, msgPosSdw, num + 0.2f, SpriteEffects.None, 0);
			Menu.spriteBatch.End();
		}
	}

	public void NextSelected()
	{
		int i = 0;
		int num = Selected + 1;
		for (; i < MenuEntries.Count; i++)
		{
			if (num >= MenuEntries.Count)
			{
				num = 0;
			}
			if (MenuEntries[num].valid)
			{
				break;
			}
			num++;
		}
		Selected = num;
	}

	public void PrevSelected()
	{
		int i = 0;
		int num = Selected - 1;
		for (; i < MenuEntries.Count; i++)
		{
			if (num < 0)
			{
				num = MenuEntries.Count - 1;
			}
			if (MenuEntries[num].valid)
			{
				break;
			}
			num--;
		}
		Selected = num;
	}
}
