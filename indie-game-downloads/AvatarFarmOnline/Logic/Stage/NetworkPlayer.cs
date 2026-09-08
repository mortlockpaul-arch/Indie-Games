using System.Collections.Generic;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage;

internal class NetworkPlayer : AvatarFarmOnline.Logic.Stage.Player
{
	private AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection networkSelection;

	private long lastUpdate;

	private AvatarFarmOnline.Logic.Stage.PlayerSnapshot lastSnapshot;

	private Vector2 simulatedPosition;

	private float simulatedRotation;

	private int level;

	public AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection PlayerSelection => networkSelection;

	public override int Level => level;

	public NetworkPlayer(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection selection, AvatarFarmOnline.Logic.PlayerPermissions permissions)
		: base(stage, selection, permissions)
	{
		networkSelection = selection;
	}

	public override void Update()
	{
		switch (state)
		{
		case PlayerState.Idle:
		{
			base.Stage.Timer.TimeSince(lastUpdate);
			float lastIntervalSeconds = base.Stage.Timer.LastIntervalSeconds;
			simulatedPosition += lastSnapshot.speed * lastIntervalSeconds;
			entityBody.Position = Vector2.Lerp(entityBody.Position, simulatedPosition, 0.04f);
			entityBody.LinearVelocity = Vector2.Lerp(entityBody.LinearVelocity, lastSnapshot.speed, 0.04f);
			rotation = GameMath.Damping(rotation, simulatedRotation, 0.04f);
			break;
		}
		case PlayerState.Working:
			entityBody.LinearVelocity = Vector2.Zero;
			break;
		}
		base.Update();
	}

	public void SetLevel(int level)
	{
		this.level = level;
	}

	public override void EarnXp(int amount)
	{
	}

	public override void UpdateData(ref AvatarFarmOnline.Logic.Stage.PlayerSnapshot snapshot, bool fullUpdate)
	{
		lastSnapshot = snapshot;
		int num = (int)((AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection)base.Selection).NetworkGamer.RoundtripTime.TotalMilliseconds;
		float num2 = (float)num * 0.5f * 0.001f;
		simulatedPosition = snapshot.position + snapshot.speed * num2;
		simulatedRotation = snapshot.rotation;
		if (lastUpdate == 0)
		{
			entityBody.Position = simulatedPosition;
			entityBody.LinearVelocity = snapshot.speed;
			rotation = simulatedRotation;
		}
		lastUpdate = base.Stage.Timer.TotalTime - num / 2;
		isRunning = snapshot.isRunning;
		state = snapshot.playerState;
		base.UpdateData(ref snapshot, fullUpdate);
	}

	public void StartWork(AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, List<AvatarFarmOnline.Logic.Stage.FarmTile> farmTiles)
	{
		base.workType = workType;
		workItemDefinition = item;
		workTiles.Clear();
		workTiles.AddRange(farmTiles);
		InvokeStartWork();
	}

	public override void Dispose()
	{
		base.Dispose();
	}
}
