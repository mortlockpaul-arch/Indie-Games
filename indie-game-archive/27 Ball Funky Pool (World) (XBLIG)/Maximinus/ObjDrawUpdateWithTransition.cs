using Microsoft.Xna.Framework;

namespace Maximinus;

public class ObjDrawUpdateWithTransition : ObjDrawUpdate
{
	private bool shown;

	private float transition;

	private readonly float transitionByFrame;

	public float Transition => transition;

	public bool Shown
	{
		get
		{
			return shown;
		}
		set
		{
			if (!shown && value)
			{
				OnShown();
			}
			shown = value;
		}
	}

	public ObjDrawUpdateWithTransition(float transitionTimeSeconds, bool initialShownState)
		: this(transitionTimeSeconds, initialShownState, useAutoUpdate: true, useAutoDraw: true)
	{
	}

	public ObjDrawUpdateWithTransition(float transitionTimeSeconds, bool initialShownState, bool useAutoUpdate, bool useAutoDraw)
		: base(useAutoUpdate, useAutoDraw)
	{
		transitionByFrame = 1f / 60f / transitionTimeSeconds;
		shown = initialShownState;
		transition = (shown ? 1 : 0);
	}

	public override void Update(GameTime gameTime)
	{
		if ((shown && transition < 1f) || (!shown && transition > 0f))
		{
			transition += transitionByFrame * (float)(shown ? 1 : (-1));
			transition = Utils.clampRatio(transition);
		}
	}

	protected virtual void OnShown()
	{
	}
}
