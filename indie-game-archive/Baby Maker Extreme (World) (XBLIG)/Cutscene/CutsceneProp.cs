using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ImportData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Renderer;
using Skeleton;

namespace Cutscene;

public class CutsceneProp
{
	private CharacterManager m_character;

	private List<CutsceneAction> m_actions;

	private int m_iLastAnimIndex;

	public int LastAnim => m_iLastAnimIndex;

	public CutsceneProp(CharacterManager character)
	{
		m_character = character;
		m_character.SetWorldScale(0.25f);
		m_actions = new List<CutsceneAction>();
		m_iLastAnimIndex = -1;
	}

	public CutsceneProp(PropImportData data)
	{
		m_character = new CharacterManager(data.CharacterFile);
		m_character.SetWorldScale(0.25f);
		m_actions = new List<CutsceneAction>();
		m_iLastAnimIndex = -1;
		foreach (PropActionImportData action in data.GetActions())
		{
			m_actions.Add(new CutsceneAction(action));
		}
	}

	public void DrawData(float yOff)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		foreach (CutsceneAction action in m_actions)
		{
			SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, action.Time.ToString(), new Vector2(400f, (float)(action.Time / 2) - yOff), Color.Black, 200f);
		}
		SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, "animSp:" + m_character.AnimSpeed, new Vector2(500f, 100f), Color.Black, 200f);
		SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, "Depth:" + m_character.Depth, new Vector2(500f, 130f), Color.Black, 200f);
		SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, "Mouth:" + m_character.GetMouth(), new Vector2(500f, 160f), Color.Black, 200f);
		SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, "EyeAim:" + ((object)m_character.GetEyeAimAt()/*cast due to constrained. prefix*/).ToString(), new Vector2(500f, 190f), Color.Black, 200f);
		SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, "WorldSc:" + m_character.GetWorldScale(), new Vector2(500f, 220f), Color.Black, 200f);
		SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, "Mirr:" + m_character.Mirror, new Vector2(500f, 250f), Color.Black, 200f);
		SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, "Pos:" + ((object)m_character.WorldPosition/*cast due to constrained. prefix*/).ToString(), new Vector2(500f, 280f), Color.Black, 200f);
		SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, "Rot:" + m_character.WorldRotation, new Vector2(500f, 310f), Color.Black, 200f);
	}

	public int NextKeyframe(int time)
	{
		for (int i = 0; i < m_actions.Count; i++)
		{
			if (m_actions[i].Time > time)
			{
				return m_actions[i].Time;
			}
		}
		if (m_actions.Count > 0)
		{
			return m_actions.Last().Time;
		}
		return 0;
	}

	public int PrevKeyframe(int time)
	{
		for (int num = m_actions.Count - 1; num >= 0; num--)
		{
			if (m_actions[num].Time < time)
			{
				return m_actions[num].Time;
			}
		}
		if (m_actions.Count > 0)
		{
			return m_actions.First().Time;
		}
		return 0;
	}

	public void DeleteActionAtTime(int time)
	{
		for (int i = 0; i < m_actions.Count; i++)
		{
			if (m_actions[i].Time >= time)
			{
				if (i > 0 && time - m_actions[i - 1].Time < m_actions[i].Time - time)
				{
					m_actions.RemoveAt(i - 1);
				}
				else
				{
					m_actions.RemoveAt(i);
				}
				return;
			}
		}
		if (m_actions.Count > 0)
		{
			m_actions.RemoveAt(m_actions.Count - 1);
		}
	}

	public void SaveProp(XmlWriter writer)
	{
		writer.WriteStartElement("prop");
		writer.WriteElementString("character", m_character.GetResourceFilename());
		foreach (CutsceneAction action in m_actions)
		{
			action.SaveAction(writer);
		}
		writer.WriteEndElement();
	}

	public void AddSceneAction(int time, int anim)
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		int num;
		for (int i = 0; i < m_actions.Count - 1; i++)
		{
			if (m_actions[i].Time < time && m_actions[i + 1].Time > time)
			{
				num = m_actions.Count - 1;
				while (num > 0 && m_actions[num].Anim == -1)
				{
					num--;
				}
				m_actions.Insert(i + 1, new CutsceneAction(time, anim, m_character.WorldPosition, m_character.WorldRotation, m_character.GetEyeAimAt(), m_character.GetMouth(), m_character.Depth, m_character.GetWorldScale(), m_character.AnimSpeed, m_character.Mirror));
				return;
			}
		}
		num = m_actions.Count - 1;
		while (num > 0 && m_actions[num].Anim == -1)
		{
			num--;
		}
		m_actions.Add(new CutsceneAction(time, anim, m_character.WorldPosition, m_character.WorldRotation, m_character.GetEyeAimAt(), m_character.GetMouth(), m_character.Depth, m_character.GetWorldScale(), m_character.AnimSpeed, m_character.Mirror));
	}

	public CharacterManager GetCharacter()
	{
		return m_character;
	}

	public CutsceneAction GetFirstAction()
	{
		if (m_actions.Count > 0)
		{
			return m_actions.First();
		}
		return null;
	}

	public CutsceneAction GetFinalAction()
	{
		if (m_actions.Count > 0)
		{
			return m_actions.Last();
		}
		return null;
	}

	public void GetStateAtTime(int time)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_actions.Count - 1; i++)
		{
			int time2 = m_actions[i].Time;
			int time3 = m_actions[i + 1].Time;
			if (time2 >= time || time3 <= time)
			{
				continue;
			}
			if (m_actions[i].Anim != m_iLastAnimIndex && m_actions[i].Anim != -1)
			{
				int timer = 200;
				if (time2 == 0)
				{
					timer = 0;
				}
				m_character.SetAnimation(m_actions[i].Anim, timer);
				m_iLastAnimIndex = m_actions[i].Anim;
			}
			float num = 1f - ((float)time3 - (float)time) / (float)(time3 - time2);
			Vector2 worldPosition = m_actions[i].Position + (m_actions[i + 1].Position - m_actions[i].Position) * num;
			float worldRotation = m_actions[i].Rotation + (m_actions[i + 1].Rotation - m_actions[i].Rotation) * num;
			m_character.WorldPosition = worldPosition;
			m_character.WorldRotation = worldRotation;
			Vector2 eyeAimAt = m_actions[i].LookAt + (m_actions[i + 1].LookAt - m_actions[i].LookAt) * num;
			m_character.SetEyeAimAt(eyeAimAt);
			m_character.SetMouth(m_actions[i].MouthPose);
			float depth = m_actions[i].Depth + (m_actions[i + 1].Depth - m_actions[i].Depth) * num;
			m_character.Depth = depth;
			float worldScale = m_actions[i].Scale + (m_actions[i + 1].Scale - m_actions[i].Scale) * num;
			m_character.SetWorldScale(worldScale);
			_ = m_actions[i].AnimSpeed;
			m_character.AnimSpeed = m_actions[i].AnimSpeed;
			m_character.Mirror = m_actions[i].Mirror;
			m_character.RecalcPos();
		}
	}

	public void Update(TimeTracker gameTime)
	{
	}

	public void Draw(TimeTracker gameTime)
	{
		m_character.DrawUpdater(gameTime);
		m_character.Draw(gameTime);
	}
}
