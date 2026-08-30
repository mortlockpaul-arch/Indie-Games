using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Renderer;

namespace Cutscene;

public class TextBox
{
	private const int textSpeed = 10;

	private bool m_bAlignLeft;

	private bool m_bAlignBottom;

	private RenderSprite m_faceSpr;

	private string m_text;

	private StringBuilder m_textBuilder;

	private RenderSprite m_backdrop;

	private int m_iCharacterIndex;

	private int m_iTimer;

	private Vector2 startPos;

	private static float m_iBobTimer;

	private RenderSprite m_continueButton;

	public bool ShownAllText
	{
		get
		{
			return m_iCharacterIndex >= m_text.Length;
		}
		set
		{
			if (value)
			{
				m_textBuilder.Append(m_text, m_iCharacterIndex, m_text.Length - m_iCharacterIndex);
				m_iCharacterIndex = m_text.Length;
			}
		}
	}

	public TextBox(bool alignLeft, bool alignBottom, RenderSprite spr, string text)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		if (ControlManager.ControllerType() != 4)
		{
			m_continueButton = SpriteManager.GetSprite("images/Buttons-Small/small_face_a", new Vector2(1000f, 620f), 1f);
		}
		else
		{
			m_continueButton = SpriteManager.GetSprite("images/keyboardButtons/space", new Vector2(1000f, 620f), 1f);
		}
		m_continueButton.ScreenSpace = true;
		m_iCharacterIndex = 0;
		m_iTimer = 0;
		m_backdrop = SpriteManager.GetSprite("images/menus/textback", default(Vector2), 0.3f);
		m_backdrop.ScreenSpace = true;
		m_bAlignLeft = alignLeft;
		m_bAlignBottom = alignBottom;
		m_faceSpr = spr;
		m_faceSpr.Depth = 0.4f;
		m_faceSpr.ScreenSpace = true;
		if (alignBottom)
		{
			m_backdrop.Position = new Vector2(SceneRenderer.GetScreenDim().X / 2f, SceneRenderer.GetScreenDim().Y - m_backdrop.SurfaceScale.Y / 2f - 40f);
		}
		else
		{
			m_backdrop.Position = new Vector2(SceneRenderer.GetScreenDim().X / 2f, 10f);
		}
		if (alignLeft)
		{
			m_faceSpr.Position = m_backdrop.Position - new Vector2(m_backdrop.SurfaceScale.X - m_faceSpr.SurfaceScale.X, 0f) / 2f;
		}
		else
		{
			m_faceSpr.Position = m_backdrop.Position + new Vector2(m_backdrop.SurfaceScale.X - m_faceSpr.SurfaceScale.X, 0f) / 2f;
		}
		m_continueButton.Position = m_backdrop.Position + m_backdrop.SurfaceScale / 2f - m_continueButton.SurfaceScale;
		startPos = m_faceSpr.Position;
		m_text = text;
		m_textBuilder = new StringBuilder(null, m_text.Length);
	}

	public void ResetText()
	{
		m_textBuilder.Remove(0, m_textBuilder.Length);
		m_iCharacterIndex = 0;
		m_iTimer = 0;
	}

	public void Update(TimeTracker gameTime)
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		m_iTimer += gameTime.ElapsedMilli;
		m_iBobTimer += gameTime.FractionOfSecond / 0.3f;
		if ((double)m_iBobTimer > Math.PI * 2.0)
		{
			m_iBobTimer -= (float)Math.PI * 2f;
		}
		while (m_iTimer > m_iCharacterIndex * 10 && m_iCharacterIndex < m_text.Length)
		{
			if (m_iCharacterIndex < m_text.Length)
			{
				m_textBuilder.Append(m_text[m_iCharacterIndex]);
				m_iCharacterIndex++;
			}
		}
		m_faceSpr.Position = startPos + new Vector2(0f, 10f * (float)Math.Sin(m_iBobTimer));
	}

	public void Draw(TimeTracker gameTime)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (m_bAlignLeft)
		{
			SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, m_textBuilder, m_backdrop.Position + new Vector2(m_faceSpr.SurfaceScale.X - m_backdrop.SurfaceScale.X / 2f, 0f - (m_backdrop.SurfaceScale.Y / 2f - 60f)), Color.Black, m_backdrop.Depth + 0.001f);
		}
		else
		{
			SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, m_textBuilder, m_backdrop.Position - new Vector2(m_backdrop.SurfaceScale.X / 2f - 40f, m_backdrop.SurfaceScale.Y / 2f - 60f), Color.Black, m_backdrop.Depth + 0.001f);
		}
		m_faceSpr.Draw(gameTime);
		m_backdrop.Draw(gameTime);
		m_continueButton.Draw(gameTime);
	}
}
