using FarseerGames.FarseerPhysics.Controllers;

namespace FarseerGames.FarseerPhysics;

public class ScalingController : Controller
{
	private float _elapsedTime;

	private float _maximumUpdateInterval;

	private float _scalingPenalty;

	private float _updateInterval;

	public float MaximumUpdateInterval
	{
		get
		{
			return _maximumUpdateInterval;
		}
		set
		{
			if (value > _updateInterval)
			{
				_maximumUpdateInterval = value;
			}
		}
	}

	public float UpdateInterval
	{
		get
		{
			return _updateInterval + _scalingPenalty;
		}
		set
		{
			_updateInterval = value;
			if (_updateInterval > _maximumUpdateInterval)
			{
				_maximumUpdateInterval = value;
			}
		}
	}

	public ScalingController(float preferredUpdateInterval, float maximumUpdateInterval)
	{
		_updateInterval = preferredUpdateInterval;
		_maximumUpdateInterval = maximumUpdateInterval;
	}

	public float GetUpdateInterval(float dt)
	{
		if (_updateInterval > 0f && Enabled)
		{
			_elapsedTime += dt;
			if (_elapsedTime < UpdateInterval)
			{
				return 0f;
			}
			float elapsedTime = _elapsedTime;
			_elapsedTime = 0f;
			return elapsedTime;
		}
		return dt;
	}

	public void IncreaseUpdateInterval()
	{
		if (_scalingPenalty + _updateInterval / 4f <= _maximumUpdateInterval)
		{
			_scalingPenalty += _updateInterval / 4f;
		}
	}

	public void DecreaseUpdateInterval()
	{
		_scalingPenalty -= _updateInterval / 8f;
		if (_scalingPenalty < 0f)
		{
			_scalingPenalty = 0f;
		}
	}

	public override void Validate()
	{
	}

	public override void Update(float dt, float dtReal)
	{
		dt = GetUpdateInterval(dt);
		if (dt != 0f)
		{
			if (UpdateInterval < dtReal)
			{
				IncreaseUpdateInterval();
			}
			else
			{
				DecreaseUpdateInterval();
			}
		}
	}
}
