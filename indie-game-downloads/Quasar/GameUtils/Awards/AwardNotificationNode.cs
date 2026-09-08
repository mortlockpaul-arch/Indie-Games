namespace Quasar.GameUtils.Awards;

public abstract class AwardNotificationNode : Element
{
	public abstract AwardsScene.NotificationState State { get; }

	public abstract void Notify(AwardsScene.PendingNotification notification);
}
