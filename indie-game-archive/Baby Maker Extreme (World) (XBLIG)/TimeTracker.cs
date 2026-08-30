using Microsoft.Xna.Framework;

public class TimeTracker
{
	private const int timeStep = 10;

	private const float PHYSICS_TIME = 1f / 60f;

	private GameTime gameTime;

	private int steps;

	private int leftovers;

	private int m_iElapsedMilli;

	private float m_fFractionOfSec;

	public GameTime GameTime => gameTime;

	public int Steps => steps;

	public int StepSize => 10;

	public float FractionOfSecond => m_fFractionOfSec;

	public float PhysicsFractionOfSecond => m_fFractionOfSec;

	public int ElapsedMilli => m_iElapsedMilli;

	public TimeTracker()
	{
		steps = 0;
		gameTime = null;
		leftovers = 0;
		m_iElapsedMilli = 0;
		m_fFractionOfSec = 0f;
	}

	public void Update(GameTime g)
	{
		gameTime = g;
		leftovers += g.ElapsedGameTime.Milliseconds;
		steps = leftovers / 10;
		leftovers %= 10;
		m_iElapsedMilli = gameTime.ElapsedGameTime.Milliseconds;
		m_fFractionOfSec = (float)m_iElapsedMilli / 1000f;
	}
}
