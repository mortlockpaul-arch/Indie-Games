using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Kobingo.Xna.Library.Data;
using Kobingo.Xna.Library.Game;
using Kobingo.Xna.Library.Graphics;
using Kobingo.Xna.Library.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Kobingo.Xna.Games.Painter;

internal class PainterPictureGallery : GameScreen
{
	public const float ANIMATION_SPEED = 50f;

	public const float SPACE = 600f;

	private int m_LoadingPictureIndex;

	private int m_LoadingPictureCount;

	public List<Picture> Pictures { get; private set; }

	public int SelectedPictureIndex { get; private set; }

	public float Animation { get; private set; }

	public bool IsLoading { get; private set; }

	public float Progress { get; set; }

	public PainterPlayScreen PainterPlayScreen { get; set; }

	public PainterPictureGallery(ScreenManager screenManager)
		: base(screenManager)
	{
		Pictures = new List<Picture>();
	}

	public override void HandleInput()
	{
		if (IsLoading)
		{
			return;
		}
		if (ScreenInput.Right)
		{
			if (++SelectedPictureIndex > Pictures.Count - 1)
			{
				SelectedPictureIndex = Pictures.Count - 1;
			}
			else
			{
				Animation += 600f;
			}
		}
		if (ScreenInput.Left)
		{
			if (--SelectedPictureIndex < 0)
			{
				SelectedPictureIndex = 0;
			}
			else
			{
				Animation -= 600f;
			}
		}
		if (ScreenInput.Back)
		{
			Close();
		}
		if ((KeyboardManager.IsKeyPress((Keys)46) || GamepadManager.IsButtonPressed((Buttons)32768)) && Pictures.Count > 0)
		{
			Delete();
		}
		if (ScreenInput.Select && Pictures.Count > 0)
		{
			PainterPlayScreen.Show(PainterSessionType.Local, Pictures[SelectedPictureIndex].PictureTexture);
		}
		base.HandleInput();
	}

	public override void Update(GameTime gameTime, bool active)
	{
		if (Animation > 0f)
		{
			if ((Animation -= 50f) < 0f)
			{
				Animation = 0f;
			}
		}
		else if ((Animation += 50f) > 0f)
		{
			Animation = 0f;
		}
		Progress += 0.03f;
		base.Update(gameTime, active);
	}

	public override void Draw(GameTime gameTime, float transition)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		base.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)1);
		base.ScreenManager.SpriteBatch.DrawAligned(Graphics.GalleryBack, base.ScreenManager.ScreenCenter, Align.Center, Color.White);
		base.ScreenManager.SpriteBatch.DrawAlignedString(Fonts.HeaderFont, "Picture Gallery", base.ScreenManager.ScreenCenter - new Vector2(0f, 230f), Align.Center, Color.Black);
		if (IsLoading)
		{
			if (m_LoadingPictureCount > 0)
			{
				string text = $"Loading picture {m_LoadingPictureIndex} of {m_LoadingPictureCount}";
				base.ScreenManager.SpriteBatch.DrawAligned(Graphics.Progress, base.ScreenManager.ScreenCenter - new Vector2(0f, 25f), Progress, 1f, Align.Center, Color.Black);
				base.ScreenManager.SpriteBatch.DrawAlignedString(GameManager.Font, text, base.ScreenManager.ScreenCenter + new Vector2(0f, 25f), Align.Center, Color.Black);
			}
			base.ScreenManager.SpriteBatch.End();
		}
		else
		{
			if (Pictures.Count == 0)
			{
				base.ScreenManager.SpriteBatch.DrawAlignedString(GameManager.Font, "Save your pictures to view and load from this gallery", base.ScreenManager.ScreenCenter, Align.Center, Color.Black);
			}
			else
			{
				_ = $"Viewing picture {SelectedPictureIndex + 1} of {Pictures.Count}";
				Rectangle titleSafeArea = base.ScreenManager.TitleSafeArea;
				float num = ((Rectangle)(ref titleSafeArea)).Left + 25;
				Rectangle titleSafeArea2 = base.ScreenManager.TitleSafeArea;
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector(num, (float)(((Rectangle)(ref titleSafeArea2)).Bottom - 25));
				base.ScreenManager.SpriteBatch.DrawAligned(Graphics.ButtonA, val, Align.Center, Color.White);
				val += new Vector2(30f, -17f);
				base.ScreenManager.SpriteBatch.DrawAlignedString(GameManager.Font, "Load picture", val, Align.Left, Color.Black);
				val += new Vector2(190f, 17f);
				base.ScreenManager.SpriteBatch.DrawAligned(Graphics.ButtonY, val, Align.Center, Color.White);
				val += new Vector2(30f, -17f);
				base.ScreenManager.SpriteBatch.DrawAlignedString(GameManager.Font, "Delete picture", val, Align.Left, Color.Black);
			}
			Rectangle titleSafeArea3 = base.ScreenManager.TitleSafeArea;
			float num2 = ((Rectangle)(ref titleSafeArea3)).Right;
			Rectangle titleSafeArea4 = base.ScreenManager.TitleSafeArea;
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector(num2, (float)(((Rectangle)(ref titleSafeArea4)).Bottom - 42));
			base.ScreenManager.SpriteBatch.DrawAlignedString(GameManager.Font, "Back to main", val2, Align.Right, Color.Black);
			val2 -= new Vector2(134f, 17f);
			base.ScreenManager.SpriteBatch.DrawAligned(Graphics.ButtonB, val2, Align.Right, Color.White);
			base.ScreenManager.SpriteBatch.End();
			Vector2 val3 = default(Vector2);
			for (int i = 0; i < Pictures.Count; i++)
			{
				((Vector2)(ref val3))._002Ector(base.ScreenManager.ScreenCenter.X - 600f * (float)SelectedPictureIndex, base.ScreenManager.ScreenCenter.Y);
				base.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)0);
				base.ScreenManager.SpriteBatch.DrawAligned(Pictures[i].PictureTexture, val3 + new Vector2((float)i * 600f + Animation, 0f), 0f, 0.5f, Align.Center, Color.White);
				base.ScreenManager.SpriteBatch.End();
				base.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)1);
				base.ScreenManager.SpriteBatch.DrawAligned(Graphics.Border, val3 + new Vector2((float)i * 600f + Animation, 0f), 0f, 0.5f, Align.Center, Color.SteelBlue);
				base.ScreenManager.SpriteBatch.End();
			}
		}
		base.Draw(gameTime, transition);
	}

	public override void Show()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		PainterPlayScreen = (PainterPlayScreen)GameManager.TitleScreen.MainMenu.PlayScreen;
		IsLoading = true;
		SelectedPictureIndex = 0;
		m_LoadingPictureIndex = 1;
		m_LoadingPictureCount = 0;
		Pictures.Clear();
		Rectangle titleSafeArea = base.ScreenManager.TitleSafeArea;
		StorageManager.PerformOperation(GameManager.ActiveGamer.PlayerIndex, delegate(StorageContainer container)
		{
			if (container != null)
			{
				string[] files = Directory.GetFiles(container.Path, "*.pic");
				m_LoadingPictureCount = files.Length;
				string[] array = files;
				foreach (string filepath in array)
				{
					try
					{
						Pictures.Add(new Picture
						{
							PictureTexture = PainterHelper.LoadPictureFromFile(((DrawableGameComponent)base.ScreenManager).GraphicsDevice, filepath, titleSafeArea.Width, titleSafeArea.Height),
							Filepath = filepath
						});
					}
					catch (Exception)
					{
					}
					Interlocked.Increment(ref m_LoadingPictureIndex);
				}
			}
			else
			{
				Close();
			}
			IsLoading = false;
		});
		base.Show();
	}

	private void Delete()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Picture picture = Pictures[SelectedPictureIndex];
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(picture.Filepath);
		Guide.BeginShowMessageBox(GameManager.ActiveGamer.PlayerIndex, "Delete picture", string.Format("Are you sure you want to delete the selected picture?", fileNameWithoutExtension), (IEnumerable<string>)new string[2] { "Yes", "No" }, 1, (MessageBoxIcon)3, (AsyncCallback)delegate(IAsyncResult result)
		{
			int? num = Guide.EndShowMessageBox(result);
			int? num2 = num;
			if (num2.GetValueOrDefault() == 0 && num2.HasValue)
			{
				DoDelete();
			}
		}, (object)null);
	}

	private void DoDelete()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Picture picture = Pictures[SelectedPictureIndex];
		try
		{
			StorageManager.PerformOperation(GameManager.ActiveGamer.PlayerIndex, delegate(StorageContainer container)
			{
				if (container != null)
				{
					File.Delete(picture.Filepath);
					RemovePicture();
				}
			});
		}
		catch (GuideAlreadyVisibleException)
		{
			RemovePicture();
		}
	}

	private void RemovePicture()
	{
		Pictures.RemoveAt(SelectedPictureIndex);
		if (SelectedPictureIndex > Pictures.Count - 1)
		{
			SelectedPictureIndex = Pictures.Count - 1;
		}
	}
}
