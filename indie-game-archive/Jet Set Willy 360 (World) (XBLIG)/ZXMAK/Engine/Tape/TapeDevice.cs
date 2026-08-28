using System;
using System.Collections.Generic;
using ZXBox.Hardware.Interfaces;

namespace ZXMAK.Engine.Tape;

public class TapeDevice : IInput
{
	private ulong _Z80FQ = 3500000uL;

	private List<TapeBlock> _blocks = new List<TapeBlock>();

	private int _index;

	private int _playPosition;

	private bool _play;

	private int _lastTact;

	private int _waitEdge;

	private int _state;

	public ulong Z80FQ => _Z80FQ;

	public List<TapeBlock> Blocks
	{
		get
		{
			return _blocks;
		}
		set
		{
			_blocks = value;
		}
	}

	public int CurrentBlock
	{
		get
		{
			if (_blocks.Count > 0)
			{
				return _index;
			}
			return -1;
		}
		set
		{
			if (_index >= 0 && _index < _blocks.Count)
			{
				_index = value;
				_playPosition = 0;
			}
		}
	}

	public int Position
	{
		get
		{
			if (_playPosition >= _blocks[_index].Periods.Count)
			{
				return 0;
			}
			return _playPosition;
		}
	}

	public bool IsPlay => _play;

	public event EventHandler TapeStateChanged;

	public TapeDevice()
	{
		_Z80FQ = 3500000uL;
	}

	private void raiseTapeStateChanged()
	{
		if (TapeStateChanged != null)
		{
			TapeStateChanged(this, new EventArgs());
		}
	}

	private int tape_bit(int globalTact)
	{
		int num = globalTact - _lastTact;
		if (!_play)
		{
			_lastTact = globalTact;
			return -1;
		}
		if (_index < 0)
		{
			_play = false;
			raiseTapeStateChanged();
			return _state;
		}
		while (num >= _waitEdge)
		{
			num -= _waitEdge;
			_state ^= -1;
			_playPosition++;
			if (_playPosition >= _blocks[_index].Periods.Count)
			{
				while (_playPosition >= _blocks[_index].Periods.Count)
				{
					_playPosition = 0;
					_index++;
					if (_index >= _blocks.Count)
					{
						break;
					}
				}
				if (_index >= _blocks.Count)
				{
					_lastTact = globalTact;
					_index = 0;
					_play = false;
					raiseTapeStateChanged();
					return _state;
				}
				raiseTapeStateChanged();
			}
			_waitEdge = _blocks[_index].Periods[_playPosition];
		}
		_lastTact = globalTact - num;
		return _state;
	}

	public byte GetTapeBit(int globalTact)
	{
		return (byte)(tape_bit(globalTact) & 0x40);
	}

	public void Reset()
	{
		_waitEdge = 0;
		_index = -1;
		if (_blocks.Count > 0)
		{
			_index = 0;
		}
		_playPosition = 0;
		_play = false;
		raiseTapeStateChanged();
	}

	public void Rewind(int globalTact)
	{
		_lastTact = globalTact;
		_waitEdge = 0;
		_index = -1;
		if (_blocks.Count > 0)
		{
			_index = 0;
		}
		_playPosition = 0;
		_play = false;
		raiseTapeStateChanged();
	}

	public void Play(int globalTact)
	{
		_lastTact = globalTact;
		if (_blocks.Count <= 0 || _index < 0)
		{
			return;
		}
		while (_playPosition >= _blocks[_index].Periods.Count)
		{
			_playPosition = 0;
			_index++;
			if (_index >= _blocks.Count)
			{
				break;
			}
		}
		if (_index >= _blocks.Count)
		{
			_index = -1;
			return;
		}
		_state ^= -1;
		_waitEdge = _blocks[_index].Periods[_playPosition];
		_play = true;
		raiseTapeStateChanged();
	}

	public void Stop(int globalTact)
	{
		_lastTact = globalTact;
		_play = false;
		raiseTapeStateChanged();
	}

	public int Input(int Port, int tstates)
	{
		if ((Port & 0xFF) == 254)
		{
			return GetTapeBit(tstates);
		}
		return 0;
	}
}
