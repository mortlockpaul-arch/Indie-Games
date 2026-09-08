using System.Collections.Generic;
using Quasar.GameUtils.Template;
using Quasar.Scenes;

namespace Quasar.GameUtils.Awards;

public class AwardsScene : Scene2D
{
	public enum NotificationState
	{
		Hidden,
		Showing
	}

	public struct PendingNotification(PlayerAwardProgress progress, AwardProgress award)
	{
		public PlayerAwardProgress progress = progress;

		public AwardProgress award = award;
	}

	private static AwardsScene instance;

	private AwardNotificationNode notifications;

	private Queue<PendingNotification> pendingNotifications = new Queue<PendingNotification>(3);

	public static AwardsScene Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AwardsScene();
			}
			return instance;
		}
	}

	private AwardsScene()
	{
		AwardManager.Instance.OnAchievementNotify += OnAchievementNotify;
		AwardManager.Instance.OnAchievementUnlocked += OnAchievementUnlocked;
		notifications = GameTemplate.Instance.CreateAwardNode();
		Add(notifications);
	}

	private void OnAchievementUnlocked(PlayerAwardProgress progress, AwardProgress award)
	{
		pendingNotifications.Enqueue(new PendingNotification(progress, award));
	}

	private void OnAchievementNotify(PlayerAwardProgress progress, AwardProgress award)
	{
		pendingNotifications.Enqueue(new PendingNotification(progress, award));
	}

	public override void Update()
	{
		if (pendingNotifications.Count > 0 && notifications.State == NotificationState.Hidden)
		{
			notifications.Notify(pendingNotifications.Dequeue());
		}
		base.Update();
	}
}
