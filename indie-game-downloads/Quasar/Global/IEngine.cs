using System;
using Microsoft.Xna.Framework.Graphics;
using PerformanceMeasuring.GameDebugTools;

namespace Quasar.Global;

public interface IEngine : IDisposable
{
	TimeRuler TimeRuler { get; set; }

	void InitGame();

	void Initialize(GraphicsDevice device);

	void SetGame(Game game);

	void SetContent(ContentTracker tracker);

	void Update();

	void Draw();

	void SetActive(bool active);
}
