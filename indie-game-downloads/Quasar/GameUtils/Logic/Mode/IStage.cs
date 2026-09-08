using System;

namespace Quasar.GameUtils.Logic.Mode;

public interface IStage : IDisposable
{
	void Update();
}
