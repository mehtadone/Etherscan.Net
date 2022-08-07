using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EthScanNet.Lib.Helper
{
    public abstract class RateLimiter
{
	#region Public Properties

	public int Count
	{
		get => _count;
		set
		{
			if (value <= 0)
				throw new ArgumentException($"{nameof(RateLimiter)} configured count must be greater than 0.",
					nameof(Count));

			_count = value;

			while (_timestamps.Count > _count)
			{
				_timestamps.Dequeue();
			}
		}
	}

	public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(1);

	#endregion Public Properties

	#region Private Fields

	private int _count;

	private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

	private readonly Queue<long> _timestamps = new Queue<long>();

	#endregion Private Fields

	#region Constructors


	#endregion Constructors

	#region Public Methods

	public async Task DelayAsync(int count = 1, CancellationToken token = default(CancellationToken))
	{
		if (count < 1)
			throw new ArgumentException(
				$"{nameof(RateLimiter)}.{nameof(DelayAsync)} {nameof(count)} must not be less than 1.", nameof(count));

		if (_count == 0)
			return;

		ThrowIfDisposed();

		await _syncLock.WaitAsync(token)
			.ConfigureAwait(false);

		try
		{
			var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

			do
			{
				if (_timestamps.Count < _count)
				{
					_timestamps.Enqueue(now);
					continue;
				}

				var then = _timestamps.Dequeue();

				var millisecondsDelay = 0;

				try
				{
					var time = Convert.ToInt32(now - then);

					if (time < Duration.TotalMilliseconds)
					{
						millisecondsDelay = Convert.ToInt32(Duration.TotalMilliseconds) - time;
					}
				}
				catch (OverflowException)
				{
					/* ignore */
				}

				_timestamps.Enqueue(now + millisecondsDelay);

				if (millisecondsDelay <= 0)
					continue;

				await Task.Delay(millisecondsDelay, token)
					.ConfigureAwait(false);

				if (count > 1)
				{
					now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
				}
			} while (--count > 0);
		}
		catch (Exception)
		{
			/* ignore */
		}
		finally
		{
			_syncLock.Release();
		}
	}

	#endregion Public Methods

	#region IDisposable

	private bool _disposed;

	protected void ThrowIfDisposed()
	{
		if (_disposed)
			throw new ObjectDisposedException(nameof(RateLimiter));
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
			return;

		if (disposing)
		{
			_syncLock?.Dispose();
		}

		_disposed = true;
	}

	public void Dispose()
	{
		Dispose(true);
	}

	#endregion IDisposable
}
}