using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GAOS.EventSystem
{
    /// <summary>
    /// Represents an ongoing event execution and provides access to its status and results.
    /// </summary>
    /// <typeparam name="T">The type of the event results, must implement IDataInterface</typeparam>
    public class EventTrigger<T> where T : IDataInterface
    {
        private readonly TaskCompletionSource<IReadOnlyList<T>> _taskCompletionSource;
        private readonly List<T> _results;
        private readonly CancellationTokenSource _cancellationSource;
        private readonly object _lock = new object();

        /// <summary>
        /// The task that completes when all listeners have finished
        /// </summary>
        public Task<IReadOnlyList<T>> Task { get; }

        /// <summary>
        /// Number of listeners that have completed
        /// </summary>
        public int CompletedCount { get; private set; }

        /// <summary>
        /// Total number of listeners for this event trigger
        /// </summary>
        public int TotalListeners { get; }

        /// <summary>
        /// Whether all listeners have completed
        /// </summary>
        public bool IsCompleted => CompletedCount == TotalListeners;

        /// <summary>
        /// Collection of results from completed listeners
        /// </summary>
        public IReadOnlyList<T> CompletedResults => _results.AsReadOnly();

        public EventTrigger(int totalListeners)
        {
            TotalListeners = totalListeners;
            _results = new List<T>(totalListeners);
            _taskCompletionSource = new TaskCompletionSource<IReadOnlyList<T>>();
            _cancellationSource = new CancellationTokenSource();
            Task = _taskCompletionSource.Task;
        }

        /// <summary>
        /// Adds a result from a completed listener
        /// </summary>
        internal void AddResult(T result)
        {
            lock (_lock)
            {
                _results.Add(result);
                CompletedCount++;

                if (IsCompleted)
                {
                    _taskCompletionSource.TrySetResult(CompletedResults);
                }
            }
        }

        /// <summary>
        /// Cancels remaining listeners if they support cancellation
        /// </summary>
        public void Cancel()
        {
            _cancellationSource.Cancel();
            _taskCompletionSource.TrySetCanceled();
        }

        /// <summary>
        /// Gets the cancellation token for this trigger
        /// </summary>
        internal CancellationToken CancellationToken => _cancellationSource.Token;

        /// <summary>
        /// Sets an error that occurred during event execution
        /// </summary>
        internal void SetError(Exception ex)
        {
            _taskCompletionSource.TrySetException(ex);
        }
    }
} 