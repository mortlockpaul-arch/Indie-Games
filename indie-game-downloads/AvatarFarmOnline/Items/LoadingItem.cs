using System;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace AvatarFarmOnline.Items;

internal class LoadingItem : RenderItem
{
	private enum TransitionState
	{
		Enter,
		Wait,
		Exit,
		Finished
	}

	private TransitionState state;

	private long startTime;

	private TextMesh loadingText;

	private bool endedLoading;

	public event Action OnTransitionEnd;

	public LoadingItem()
	{
		loadingText = new TextMesh(GameTemplate.TitleFont, new TextDrawProperties(new Vector2(Engine.GUIWidth * 0.4f - 300f, (0f - Engine.GUIHeight) * 0.3f), HorizontalAlignment.Left), 128, useStringBuilder: true);
		loadingText.Text = "LOADING_TITLE".Translate();
		addMesh(loadingText);
		startTime = Timer.DefaultTimer.TotalTime;
	}

	public void EndedLoading()
	{
		endedLoading = true;
		if (state == TransitionState.Wait)
		{
			state = TransitionState.Exit;
			startTime = Timer.DefaultTimer.TotalTime;
		}
	}

	protected override void DoUpdate()
	{
		if (Visible)
		{
			long num = Timer.DefaultTimer.TimeSince(startTime);
			switch (state)
			{
			case TransitionState.Enter:
				loadingText.Alpha = GameMath.Interpolate(0f, 1f, GameMath.Clamp(0f, 1f, (float)num / 800f));
				if (num > 1300)
				{
					if (!endedLoading)
					{
						startTime = Timer.DefaultTimer.TotalTime;
						state = TransitionState.Wait;
					}
					else
					{
						startTime = Timer.DefaultTimer.TotalTime;
						state = TransitionState.Exit;
					}
				}
				break;
			case TransitionState.Wait:
			{
				if (endedLoading)
				{
					startTime = Timer.DefaultTimer.TotalTime;
					state = TransitionState.Exit;
					loadingText.Text = "LOADING_TITLE".Translate();
					break;
				}
				int num2 = (int)(num / 500);
				int num3 = num2 % 4;
				loadingText.Text = "LOADING_TITLE".Translate();
				for (int i = 0; i < num3; i++)
				{
					loadingText.StringBuilder.Append('.');
				}
				break;
			}
			case TransitionState.Exit:
				loadingText.Alpha = GameMath.Interpolate(1f, 0f, GameMath.Clamp(0f, 1f, (float)num / 500f));
				if (num > 1000)
				{
					if (OnTransitionEnd != null)
					{
						OnTransitionEnd();
					}
					state = TransitionState.Finished;
					Visible = false;
				}
				break;
			}
		}
		base.DoUpdate();
	}
}
