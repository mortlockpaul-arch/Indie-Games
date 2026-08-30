using System;
using System.Runtime.CompilerServices;
using Kobingo.Xna.Library.Common;
using Kobingo.Xna.Library.Graphics;
using Kobingo.Xna.Library.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace Kobingo.Xna.Games.Painter;

internal class PainterPlayer
{
	public const float TYPE_TRANSITION_TIME = 0.2f;

	[CompilerGenerated]
	private Vector2 _003CCursor_003Ek__BackingField;

	[CompilerGenerated]
	private Vector2 _003CMovingTo_003Ek__BackingField;

	public Vector2 Cursor
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CCursor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CCursor_003Ek__BackingField = value;
		}
	}

	public PainterColor Color { get; set; }

	public float Size { get; set; }

	public float Speed { get; set; }

	public NetworkGamer NetworkGamer { get; set; }

	public LocalNetworkGamer LocalNetworkGamer { get; set; }

	public PainterPlayScreen PlayScreen { get; private set; }

	public string Name
	{
		get
		{
			if (LocalNetworkGamer != null)
			{
				return ((Gamer)LocalNetworkGamer.SignedInGamer).Gamertag;
			}
			if (NetworkGamer != null)
			{
				return ((Gamer)NetworkGamer).Gamertag;
			}
			return "Unknown player";
		}
	}

	public Vector2 MovingTo
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CMovingTo_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CMovingTo_003Ek__BackingField = value;
		}
	}

	public Transition<PainterType> TypeTransition { get; set; }

	public bool IsPainting { get; set; }

	public event Painting Painting;

	public event EventHandler StoppedPainting;

	public PainterPlayer(NetworkGamer gamer, PainterPlayScreen playScreen)
	{
		NetworkGamer = gamer;
		Size = 1f;
		Speed = 3f;
		PlayScreen = playScreen;
		TypeTransition = new Transition<PainterType>();
		TypeTransition.Change(PainterType.Pencil, TimeSpan.FromSeconds(0.20000000298023224), wait: true, TimeSpan.Zero);
	}

	public PainterPlayer(LocalNetworkGamer gamer, PainterPlayScreen playScreen)
	{
		LocalNetworkGamer = gamer;
		Size = 1f;
		Speed = 3f;
		PlayScreen = playScreen;
		TypeTransition = new Transition<PainterType>();
		TypeTransition.Change(PainterType.Pencil, TimeSpan.FromSeconds(0.20000000298023224), wait: true, TimeSpan.Zero);
	}

	public void ProcessInput(Rectangle drawingRectangle)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		if (LocalNetworkGamer == null)
		{
			return;
		}
		PlayerIndex playerIndex = LocalNetworkGamer.SignedInGamer.PlayerIndex;
		GamePadState state = GamePad.GetState(playerIndex, (GamePadDeadZone)2);
		Vector2 cursor = Cursor;
		GamePadThumbSticks thumbSticks = ((GamePadState)(ref state)).ThumbSticks;
		float num = ((GamePadThumbSticks)(ref thumbSticks)).Left.X * Speed;
		GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state)).ThumbSticks;
		Cursor = cursor + new Vector2(num, (0f - ((GamePadThumbSticks)(ref thumbSticks2)).Left.Y) * Speed);
		if (GamepadManager.IsButtonDown(playerIndex, (Buttons)1))
		{
			Cursor += new Vector2(0f, 0f - Speed);
		}
		if (GamepadManager.IsButtonDown(playerIndex, (Buttons)2))
		{
			Cursor += new Vector2(0f, Speed);
		}
		if (GamepadManager.IsButtonDown(playerIndex, (Buttons)4))
		{
			Cursor += new Vector2(0f - Speed, 0f);
		}
		if (GamepadManager.IsButtonDown(playerIndex, (Buttons)8))
		{
			Cursor += new Vector2(Speed, 0f);
		}
		if (Cursor.X < (float)((Rectangle)(ref drawingRectangle)).Left)
		{
			Cursor = new Vector2((float)((Rectangle)(ref drawingRectangle)).Left, Cursor.Y);
		}
		if (Cursor.X >= (float)((Rectangle)(ref drawingRectangle)).Right)
		{
			Cursor = new Vector2((float)(((Rectangle)(ref drawingRectangle)).Right - 1), Cursor.Y);
		}
		if (Cursor.Y < (float)((Rectangle)(ref drawingRectangle)).Top)
		{
			Cursor = new Vector2(Cursor.X, (float)((Rectangle)(ref drawingRectangle)).Top);
		}
		if (Cursor.Y >= (float)((Rectangle)(ref drawingRectangle)).Bottom)
		{
			Cursor = new Vector2(Cursor.X, (float)(((Rectangle)(ref drawingRectangle)).Bottom - 1));
		}
		if (GamepadManager.IsButtonDown(playerIndex, (Buttons)16384) && TypeTransition.Current != PainterType.Pencil)
		{
			TypeTransition.Change(PainterType.Pencil, TimeSpan.FromSeconds(0.20000000298023224), wait: true, TimeSpan.Zero);
		}
		if (GamepadManager.IsButtonDown(playerIndex, (Buttons)32768) && TypeTransition.Current != PainterType.Brush)
		{
			TypeTransition.Change(PainterType.Brush, TimeSpan.FromSeconds(0.20000000298023224), wait: true, TimeSpan.Zero);
		}
		if (GamepadManager.IsButtonDown(playerIndex, (Buttons)8192) && TypeTransition.Current != PainterType.Bucket)
		{
			TypeTransition.Change(PainterType.Bucket, TimeSpan.FromSeconds(0.20000000298023224), wait: true, TimeSpan.Zero);
		}
		if (TypeTransition.Current == PainterType.Bucket)
		{
			if (GamepadManager.IsButtonPressed(playerIndex, (Buttons)4096) && Painting != null)
			{
				Painting(this, TypeTransition.Current, Cursor, Size, Color);
			}
		}
		else
		{
			if (GamepadManager.IsButtonPressed(playerIndex, (Buttons)4096))
			{
				IsPainting = true;
			}
			if (IsPainting && GamepadManager.IsButtonDown(playerIndex, (Buttons)4096) && Painting != null)
			{
				Painting(this, TypeTransition.Current, Cursor, Size, Color);
			}
		}
		if (GamepadManager.IsButtonReleased(playerIndex, (Buttons)4096))
		{
			if (StoppedPainting != null)
			{
				StoppedPainting(this, EventArgs.Empty);
			}
			IsPainting = false;
		}
		if (!GamepadManager.IsButtonPressed(playerIndex, (Buttons)256) && !GamepadManager.IsButtonPressed(playerIndex, (Buttons)512))
		{
			return;
		}
		if (GamepadManager.IsButtonPressed(playerIndex, (Buttons)256))
		{
			do
			{
				if (--Color < PainterColor.Red)
				{
					Color = PainterColor.White;
				}
			}
			while (ColorPalette.IsLocked(Color));
		}
		if (GamepadManager.IsButtonPressed(playerIndex, (Buttons)512))
		{
			do
			{
				if (++Color > PainterColor.White)
				{
					Color = PainterColor.Red;
				}
			}
			while (ColorPalette.IsLocked(Color));
		}
		foreach (ColorPalette palette in PlayScreen.Palettes)
		{
			if (palette.Color == Color && PlayScreen.PaletteTransition.Current != palette)
			{
				PlayScreen.PaletteTransition.Change(palette, TimeSpan.FromSeconds(0.10000000149011612), wait: false, TimeSpan.Zero);
			}
		}
		if (PlayScreen.DisplayPaletteTransition.Current != 1)
		{
			PlayScreen.DisplayPaletteTransition.Change(1, TimeSpan.FromSeconds(0.20000000298023224), wait: false, TimeSpan.FromSeconds(3.0));
		}
		else
		{
			PlayScreen.DisplayPaletteTransition.ResetCurrent();
		}
	}

	public void Update(GameTime gameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Vector2 movingTo = MovingTo;
		if (((Vector2)(ref movingTo)).Length() > 1f)
		{
			Vector2 cursor = Cursor;
			float num = ((Vector2)(ref cursor)).Length();
			Vector2 movingTo2 = MovingTo;
			float num2 = Math.Abs(num - ((Vector2)(ref movingTo2)).Length());
			if (num2 > 1f)
			{
				Cursor += VectorHelper.GetDirection(VectorHelper.GetAngle(Cursor, MovingTo)) * (num2 / 3f);
			}
		}
		TypeTransition.Update(gameTime);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(0f, (float)(Graphics.Brush.Height - 5));
		foreach (TransitionState<PainterType> state in TypeTransition.States)
		{
			Texture2D val2 = null;
			switch (state.Value)
			{
			case PainterType.Pencil:
				val2 = Graphics.Pencil;
				break;
			case PainterType.Brush:
				val2 = Graphics.Brush;
				break;
			case PainterType.Bucket:
				val2 = Graphics.Bucket;
				break;
			}
			spriteBatch.Draw(val2, Cursor, (Rectangle?)null, new Color(Color.White, state.Transition), 0f, val, 1f, (SpriteEffects)0, 0f);
		}
		spriteBatch.DrawAlignedString(Fonts.DefaultFont, Name, Cursor + new Vector2(32f, -80f), Align.Center, Color.DimGray);
	}
}
