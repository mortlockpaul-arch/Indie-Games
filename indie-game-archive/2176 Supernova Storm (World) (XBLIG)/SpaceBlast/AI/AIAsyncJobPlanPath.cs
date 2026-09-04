using System;
using Microsoft.Xna.Framework;
using SpaceBlast.AsyncJobManager;
using SpaceBlast.PathFinding;

namespace SpaceBlast.AI;

internal class AIAsyncJobPlanPath : AsyncJob
{
	private Vector2 m_FromPos;

	private Vector2 m_ToPos;

	private int m_ShipWidth;

	private PlannedPath m_Path;

	public PlannedPath PlannedPath => m_Path;

	public AIAsyncJobPlanPath(ref Vector2 from, ref Vector2 to, int shipWidth)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		m_Path = new PlannedPath();
		base._002Ector();
		m_FromPos = from;
		m_ToPos = to;
		m_ShipWidth = shipWidth;
	}

	public override void ExecuteJob()
	{
		try
		{
			m_Path.CreatePath(ref m_FromPos, ref m_ToPos, m_ShipWidth);
		}
		catch (Exception)
		{
		}
	}
}
