using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Diagnostics.Metrics;

internal sealed class AggregationManager
{
	private static readonly QuantileAggregation s_defaultHistogramConfig = new QuantileAggregation(0.5, 0.95, 0.99);

	private readonly List<Predicate<Instrument>> _instrumentConfigFuncs = new List<Predicate<Instrument>>();

	private Dictionary<Instrument, bool> _instruments = new Dictionary<Instrument, bool>();

	private readonly ConcurrentDictionary<Instrument, InstrumentState> _instrumentStates = new ConcurrentDictionary<Instrument, InstrumentState>();

	private readonly CancellationTokenSource _cts = new CancellationTokenSource();

	private Thread _collectThread;

	private Timer _pollingTimer;

	private readonly MeterListener _listener;

	private int _currentTimeSeries;

	private int _currentHistograms;

	private readonly Action<Instrument, LabeledAggregationStatistics, InstrumentState> _collectMeasurement;

	private readonly Action<DateTime, DateTime> _beginCollection;

	private readonly Action<DateTime, DateTime> _endCollection;

	private readonly Action<Instrument, InstrumentState> _beginInstrumentMeasurements;

	private readonly Action<Instrument, InstrumentState> _endInstrumentMeasurements;

	private readonly Action<Instrument, InstrumentState> _instrumentPublished;

	private readonly Action _initialInstrumentEnumerationComplete;

	private readonly Action<Exception> _collectionError;

	private readonly Action _timeSeriesLimitReached;

	private readonly Action _histogramLimitReached;

	private readonly Action<Exception> _observableInstrumentCallbackError;

	private DateTime _startTime;

	private DateTime _intervalStartTime;

	private DateTime _nextIntervalStartTime;

	private Func<Aggregator> _histogramAggregatorFactory = () => new ExponentialHistogramAggregator(s_defaultHistogramConfig);

	public TimeSpan CollectionPeriod { get; private set; }

	public int MaxTimeSeries { get; }

	public int MaxHistograms { get; }

	public AggregationManager(int maxTimeSeries, int maxHistograms, Action<Instrument, LabeledAggregationStatistics, InstrumentState> collectMeasurement, Action<DateTime, DateTime> beginCollection, Action<DateTime, DateTime> endCollection, Action<Instrument, InstrumentState> beginInstrumentMeasurements, Action<Instrument, InstrumentState> endInstrumentMeasurements, Action<Instrument, InstrumentState> instrumentPublished, Action initialInstrumentEnumerationComplete, Action<Exception> collectionError, Action timeSeriesLimitReached, Action histogramLimitReached, Action<Exception> observableInstrumentCallbackError)
	{
		MaxTimeSeries = maxTimeSeries;
		MaxHistograms = maxHistograms;
		_collectMeasurement = collectMeasurement;
		_beginCollection = beginCollection;
		_endCollection = endCollection;
		_beginInstrumentMeasurements = beginInstrumentMeasurements;
		_endInstrumentMeasurements = endInstrumentMeasurements;
		_instrumentPublished = instrumentPublished;
		_initialInstrumentEnumerationComplete = initialInstrumentEnumerationComplete;
		_collectionError = collectionError;
		_timeSeriesLimitReached = timeSeriesLimitReached;
		_histogramLimitReached = histogramLimitReached;
		_observableInstrumentCallbackError = observableInstrumentCallbackError;
		_listener = new MeterListener();
		MeterListener listener = _listener;
		listener.InstrumentPublished = (Action<Instrument, MeterListener>)Delegate.Combine(listener.InstrumentPublished, new Action<Instrument, MeterListener>(PublishedInstrument));
		MeterListener listener2 = _listener;
		listener2.MeasurementsCompleted = (Action<Instrument, object>)Delegate.Combine(listener2.MeasurementsCompleted, new Action<Instrument, object>(CompletedMeasurements));
		_listener.SetMeasurementEventCallback(delegate(Instrument i, double m, ReadOnlySpan<KeyValuePair<string, object>> l, object c)
		{
			((InstrumentState)c).Update(m, l);
		});
		_listener.SetMeasurementEventCallback(delegate(Instrument i, float m, ReadOnlySpan<KeyValuePair<string, object>> l, object c)
		{
			((InstrumentState)c).Update(m, l);
		});
		_listener.SetMeasurementEventCallback(delegate(Instrument i, long m, ReadOnlySpan<KeyValuePair<string, object>> l, object c)
		{
			((InstrumentState)c).Update(m, l);
		});
		_listener.SetMeasurementEventCallback(delegate(Instrument i, int m, ReadOnlySpan<KeyValuePair<string, object>> l, object c)
		{
			((InstrumentState)c).Update(m, l);
		});
		_listener.SetMeasurementEventCallback(delegate(Instrument i, short m, ReadOnlySpan<KeyValuePair<string, object>> l, object c)
		{
			((InstrumentState)c).Update(m, l);
		});
		_listener.SetMeasurementEventCallback(delegate(Instrument i, byte m, ReadOnlySpan<KeyValuePair<string, object>> l, object c)
		{
			((InstrumentState)c).Update((int)m, l);
		});
		_listener.SetMeasurementEventCallback(delegate(Instrument i, decimal m, ReadOnlySpan<KeyValuePair<string, object>> l, object c)
		{
			((InstrumentState)c).Update((double)m, l);
		});
	}

	public void Include(string meterName)
	{
		Include((Instrument i) => i.Meter.Name.Equals(meterName, StringComparison.OrdinalIgnoreCase));
	}

	public void IncludeAll()
	{
		Include((Instrument i) => true);
	}

	public void IncludePrefix(string meterNamePrefix)
	{
		Include((Instrument i) => i.Meter.Name.StartsWith(meterNamePrefix, StringComparison.OrdinalIgnoreCase));
	}

	public void Include(string meterName, string instrumentName)
	{
		Include((Instrument i) => i.Meter.Name.Equals(meterName, StringComparison.OrdinalIgnoreCase) && i.Name.Equals(instrumentName, StringComparison.OrdinalIgnoreCase));
	}

	private void Include(Predicate<Instrument> instrumentFilter)
	{
		lock (this)
		{
			_instrumentConfigFuncs.Add(instrumentFilter);
		}
	}

	public void SetHistogramAggregation(Func<Aggregator> histogramAggregatorFactory)
	{
		lock (this)
		{
			_histogramAggregatorFactory = histogramAggregatorFactory;
		}
	}

	public AggregationManager SetCollectionPeriod(TimeSpan collectionPeriod)
	{
		lock (this)
		{
			CollectionPeriod = collectionPeriod;
			return this;
		}
	}

	private void CompletedMeasurements(Instrument instrument, object cookie)
	{
		_instruments.Remove(instrument);
		_endInstrumentMeasurements(instrument, (InstrumentState)cookie);
		RemoveInstrumentState(instrument);
	}

	private void PublishedInstrument(Instrument instrument, MeterListener _)
	{
		InstrumentState instrumentState = GetInstrumentState(instrument);
		_instrumentPublished(instrument, instrumentState);
		if (instrumentState != null)
		{
			_beginInstrumentMeasurements(instrument, instrumentState);
			if (!_instruments.ContainsKey(instrument))
			{
				_listener.EnableMeasurementEvents(instrument, instrumentState);
				_instruments.Add(instrument, value: true);
			}
		}
	}

	public void Start()
	{
		_intervalStartTime = (_nextIntervalStartTime = (_startTime = DateTime.UtcNow));
		if (OperatingSystem.IsBrowser())
		{
			TimeSpan timeSpan = CalculateDelayTime(CollectionPeriod.TotalSeconds);
			_pollingTimer = new Timer(CollectOnTimer, null, (int)timeSpan.TotalMilliseconds, 0);
		}
		else
		{
			_collectThread = new Thread(CollectWorker);
			_collectThread.IsBackground = true;
			_collectThread.Name = "MetricsEventSource CollectWorker";
			_collectThread.Start();
		}
		_listener.Start();
		_initialInstrumentEnumerationComplete();
	}

	public void Update()
	{
		using (MeterListener meterListener = new MeterListener())
		{
			meterListener.InstrumentPublished = (Action<Instrument, MeterListener>)Delegate.Combine(meterListener.InstrumentPublished, new Action<Instrument, MeterListener>(PublishedInstrument));
			meterListener.MeasurementsCompleted = (Action<Instrument, object>)Delegate.Combine(meterListener.MeasurementsCompleted, new Action<Instrument, object>(CompletedMeasurements));
			meterListener.Start();
		}
		_initialInstrumentEnumerationComplete();
	}

	private TimeSpan CalculateDelayTime(double collectionIntervalSecs)
	{
		_intervalStartTime = _nextIntervalStartTime;
		DateTime utcNow = DateTime.UtcNow;
		double value = Math.Ceiling((utcNow - _startTime).TotalSeconds / collectionIntervalSecs) * collectionIntervalSecs;
		_nextIntervalStartTime = _startTime.AddSeconds(value);
		DateTime dateTime = _intervalStartTime.AddSeconds(collectionIntervalSecs);
		if (_nextIntervalStartTime <= dateTime)
		{
			_nextIntervalStartTime = dateTime;
		}
		return _nextIntervalStartTime - utcNow;
	}

	private void CollectWorker()
	{
		try
		{
			double collectionIntervalSecs = -1.0;
			CancellationToken token;
			lock (this)
			{
				collectionIntervalSecs = CollectionPeriod.TotalSeconds;
				token = _cts.Token;
			}
			_ = DateTime.UtcNow;
			while (!_cts.Token.IsCancellationRequested)
			{
				TimeSpan timeout = CalculateDelayTime(collectionIntervalSecs);
				if (!token.WaitHandle.WaitOne(timeout))
				{
					_beginCollection(_intervalStartTime, _nextIntervalStartTime);
					Collect();
					_endCollection(_intervalStartTime, _nextIntervalStartTime);
					continue;
				}
				break;
			}
		}
		catch (Exception obj)
		{
			_collectionError(obj);
		}
	}

	private void CollectOnTimer(object _)
	{
		try
		{
			CancellationToken token = _cts.Token;
			double totalSeconds = CollectionPeriod.TotalSeconds;
			if (!token.IsCancellationRequested)
			{
				_beginCollection(_intervalStartTime, _nextIntervalStartTime);
				Collect();
				_endCollection(_intervalStartTime, _nextIntervalStartTime);
				TimeSpan timeSpan = CalculateDelayTime(totalSeconds);
				_pollingTimer.Change((int)timeSpan.TotalMilliseconds, 0);
			}
		}
		catch (Exception obj)
		{
			_collectionError(obj);
		}
	}

	public void Dispose()
	{
		_cts.Cancel();
		if (OperatingSystem.IsBrowser())
		{
			_pollingTimer?.Dispose();
			_pollingTimer = null;
		}
		else
		{
			_collectThread?.Join();
			_collectThread = null;
		}
		_listener.Dispose();
	}

	private void RemoveInstrumentState(Instrument instrument)
	{
		_instrumentStates.TryRemove(instrument, out var _);
	}

	private InstrumentState GetInstrumentState(Instrument instrument)
	{
		if (!_instrumentStates.TryGetValue(instrument, out var value))
		{
			lock (this)
			{
				foreach (Predicate<Instrument> instrumentConfigFunc in _instrumentConfigFuncs)
				{
					if (instrumentConfigFunc(instrument))
					{
						value = BuildInstrumentState(instrument);
						if (value != null)
						{
							_instrumentStates.TryAdd(instrument, value);
							_instrumentStates.TryGetValue(instrument, out value);
						}
						break;
					}
				}
			}
		}
		return value;
	}

	[UnconditionalSuppressMessage("AotAnalysis", "IL3050:RequiresDynamicCode", Justification = "MakeGenericType is creating instances over reference types that works fine in AOT.")]
	internal InstrumentState BuildInstrumentState(Instrument instrument)
	{
		Func<Aggregator> aggregatorFactory = GetAggregatorFactory(instrument);
		if (aggregatorFactory == null)
		{
			return null;
		}
		Type type = aggregatorFactory.GetType().GenericTypeArguments[0];
		return (InstrumentState)Activator.CreateInstance(typeof(InstrumentState<>).MakeGenericType(type), aggregatorFactory);
	}

	private Func<Aggregator> GetAggregatorFactory(Instrument instrument)
	{
		Type type = instrument.GetType();
		Type type2 = null;
		type2 = (type.IsGenericType ? type.GetGenericTypeDefinition() : null);
		if (type2 == typeof(Counter<>))
		{
			return delegate
			{
				lock (this)
				{
					return CheckTimeSeriesAllowed() ? new CounterAggregator(isMonotonic: true) : null;
				}
			};
		}
		if (type2 == typeof(ObservableCounter<>))
		{
			return delegate
			{
				lock (this)
				{
					return CheckTimeSeriesAllowed() ? new ObservableCounterAggregator(isMonotonic: true) : null;
				}
			};
		}
		if (type2 == typeof(ObservableGauge<>))
		{
			return delegate
			{
				lock (this)
				{
					return CheckTimeSeriesAllowed() ? new LastValue() : null;
				}
			};
		}
		if (type2 == typeof(Gauge<>))
		{
			return delegate
			{
				lock (this)
				{
					return CheckTimeSeriesAllowed() ? new SynchronousLastValue() : null;
				}
			};
		}
		if (type2 == typeof(Histogram<>))
		{
			return delegate
			{
				lock (this)
				{
					return (!CheckHistogramAllowed() || !CheckTimeSeriesAllowed()) ? null : _histogramAggregatorFactory();
				}
			};
		}
		if (type2 == typeof(UpDownCounter<>))
		{
			return delegate
			{
				lock (this)
				{
					return CheckTimeSeriesAllowed() ? new CounterAggregator(isMonotonic: false) : null;
				}
			};
		}
		if (type2 == typeof(ObservableUpDownCounter<>))
		{
			return delegate
			{
				lock (this)
				{
					return CheckTimeSeriesAllowed() ? new ObservableCounterAggregator(isMonotonic: false) : null;
				}
			};
		}
		return null;
	}

	private bool CheckTimeSeriesAllowed()
	{
		if (_currentTimeSeries < MaxTimeSeries)
		{
			_currentTimeSeries++;
			return true;
		}
		if (_currentTimeSeries == MaxTimeSeries)
		{
			_currentTimeSeries++;
			_timeSeriesLimitReached();
			return false;
		}
		return false;
	}

	private bool CheckHistogramAllowed()
	{
		if (_currentHistograms < MaxHistograms)
		{
			_currentHistograms++;
			return true;
		}
		if (_currentHistograms == MaxHistograms)
		{
			_currentHistograms++;
			_histogramLimitReached();
			return false;
		}
		return false;
	}

	internal void Collect()
	{
		try
		{
			_listener.RecordObservableInstruments();
		}
		catch (Exception obj)
		{
			_observableInstrumentCallbackError(obj);
		}
		foreach (KeyValuePair<Instrument, InstrumentState> kv in _instrumentStates)
		{
			kv.Value.Collect(kv.Key, delegate(LabeledAggregationStatistics labeledAggStats)
			{
				_collectMeasurement(kv.Key, labeledAggStats, kv.Value);
			});
		}
	}
}
