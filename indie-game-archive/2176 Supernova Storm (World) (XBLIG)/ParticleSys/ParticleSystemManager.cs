using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ParticleSys;

internal class ParticleSystemManager
{
	private ExplosionParticleSystem m_ExplosionParticles;

	private SmallExplosionParticleSystem m_SmallExplosionParticles;

	private RedPowerUpCollectedParticleSystem m_RedPowerupParticles;

	private GreenPowerUpCollectedParticleSystem m_GreenPowerupParticles;

	private BluePowerUpCollectedParticleSystem m_BluePowerupParticles;

	public ParticleSystemManager(Game game, ContentManager content)
	{
		m_ExplosionParticles = new ExplosionParticleSystem(game, content);
		m_ExplosionParticles.Initialize();
		m_SmallExplosionParticles = new SmallExplosionParticleSystem(game, content);
		m_SmallExplosionParticles.Initialize();
		m_RedPowerupParticles = new RedPowerUpCollectedParticleSystem(game, content);
		m_RedPowerupParticles.Initialize();
		m_GreenPowerupParticles = new GreenPowerUpCollectedParticleSystem(game, content);
		m_GreenPowerupParticles.Initialize();
		m_BluePowerupParticles = new BluePowerUpCollectedParticleSystem(game, content);
		m_BluePowerupParticles.Initialize();
	}

	public void LoadContent()
	{
		m_ExplosionParticles.LoadContent();
		m_SmallExplosionParticles.LoadContent();
		m_RedPowerupParticles.LoadContent();
		m_GreenPowerupParticles.LoadContent();
		m_BluePowerupParticles.LoadContent();
	}

	public void Update(GameTime gametime)
	{
		m_ExplosionParticles.Update(gametime);
		m_SmallExplosionParticles.Update(gametime);
		m_RedPowerupParticles.Update(gametime);
		m_GreenPowerupParticles.Update(gametime);
		m_BluePowerupParticles.Update(gametime);
	}

	public void CreateExplosion(Vector3 position, Vector3 velocity)
	{
		m_ExplosionParticles.CreateExplosion(position, velocity);
	}

	public void CreateSmallExplosion(Vector3 position, Vector3 velocity)
	{
		m_SmallExplosionParticles.CreateExplosion(position, velocity);
	}

	public void CreateRedPowerUpPlasma(Vector3 position)
	{
		m_RedPowerupParticles.CreatePowerupCollectedParticles(position);
	}

	public void CreateGreenPowerUpPlasma(Vector3 position)
	{
		m_GreenPowerupParticles.CreatePowerupCollectedParticles(position);
	}

	public void CreateBluePowerUpPlasma(Vector3 position)
	{
		m_BluePowerupParticles.CreatePowerupCollectedParticles(position);
	}

	public void Draw(ref Matrix view, ref Matrix proj)
	{
		m_ExplosionParticles.SetCamera(view, proj);
		m_SmallExplosionParticles.SetCamera(view, proj);
		m_RedPowerupParticles.SetCamera(view, proj);
		m_GreenPowerupParticles.SetCamera(view, proj);
		m_BluePowerupParticles.SetCamera(view, proj);
		m_ExplosionParticles.Draw();
		m_SmallExplosionParticles.Draw();
		m_RedPowerupParticles.Draw();
		m_GreenPowerupParticles.Draw();
		m_BluePowerupParticles.Draw();
	}
}
