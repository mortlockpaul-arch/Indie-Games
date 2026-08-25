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

	private PlannedPath m_Path = new PlannedPath();

	public PlannedPath PlannedPath => m_Path;

	public AIAsyncJobPlanPath(ref Vector2 from, ref Vector2 to, int shipWidth)
	{
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
