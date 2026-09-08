namespace Quasar.Particles;

public class EmitterElement : Element
{
	private ParticleEmitter emitter;

	public ParticleEmitter Emitter => emitter;

	public EmitterElement(ParticleEmitter emitter)
	{
		this.emitter = emitter;
	}

	protected override void DoUpdate()
	{
		emitter.transform.Translation = transform.WorldTranslation;
		base.DoUpdate();
	}
}
