using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ImportData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Renderer;

namespace Cutscene;

public class CutsceneCamera
{
	private List<CutsceneCameraAction> m_actions;

	public CutsceneCamera()
	{
		m_actions = new List<CutsceneCameraAction>();
	}

	public void DrawData(float yOff)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		foreach (CutsceneCameraAction action in m_actions)
		{
			SceneRenderer.DrawPixelSpaceString(fonts.BASE_FONT, action.Time.ToString(), new Vector2(200f, (float)action.Time / 2f - yOff), Color.Black, 1000f);
		}
	}

	public void SaveCamera(XmlWriter writer, Cutscene scene)
	{
		foreach (CutsceneCameraAction action in m_actions)
		{
			action.SaveCameraAction(writer, scene);
		}
	}

	public void ReadCamera(CameraImportData data, Cutscene scene)
	{
		m_actions.Clear();
		foreach (CamActionImportData action in data.GetActions())
		{
			m_actions.Add(new CutsceneCameraAction(action, scene));
		}
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

	public void AddCameraAction(CutsceneCameraAction action)
	{
		for (int i = 0; i < m_actions.Count - 1; i++)
		{
			if (m_actions[i].Time < action.Time && m_actions[i + 1].Time > action.Time)
			{
				m_actions.Insert(i + 1, action);
				return;
			}
		}
		m_actions.Add(action);
	}

	public int GetNextTimeAction(int time)
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

	public int GetPrevTimeAction(int time)
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

	public void GetStateAtTime(int time)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_actions.Count - 1; i++)
		{
			int time2 = m_actions[i].Time;
			int time3 = m_actions[i + 1].Time;
			if (time2 < time && time3 > time)
			{
				float num = 1f - ((float)time3 - (float)time) / (float)(time3 - time2);
				Vector2 pos = m_actions[i].Position + (m_actions[i + 1].Position - m_actions[i].Position) * num;
				float zoom = m_actions[i].Zoom + (m_actions[i + 1].Zoom - m_actions[i].Zoom) * num;
				SceneRenderer.MoveCamera(pos, 0f, zoom);
			}
		}
	}
}
