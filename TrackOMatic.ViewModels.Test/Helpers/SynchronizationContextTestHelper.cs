using System.Collections.Concurrent;

namespace TrackOMatic.ViewModels.Test.Helpers;

/// <summary>
/// Test helper for managing SynchronizationContext in unit tests.
/// Provides utilities to run tests on a UI context, background thread, and verify context marshaling behavior.
/// </summary>
public static class SynchronizationContextTestHelper
{
    /// <summary>
    /// Sets up a SynchronizationContext for the current test thread and runs the given action.
    /// Ensures context is properly cleaned up after the action completes.
    /// </summary>
    /// <param name="action">The action to run on the UI context thread.</param>
    public static void RunWithUIContext(Action action)
    {
        var previousContext = SynchronizationContext.Current;
        try
        {
            var uiContext = new TestSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(uiContext);
            action();
            uiContext.RunPendingCallbacks();
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }

    /// <summary>
    /// Sets up a SynchronizationContext for the current test thread and runs the given action with a return value.
    /// Ensures context is properly cleaned up after the action completes.
    /// </summary>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <param name="action">The action to run on the UI context thread.</param>
    /// <returns>The result from the action.</returns>
    public static T RunWithUIContext<T>(Func<T> action)
    {
        var previousContext = SynchronizationContext.Current;
        try
        {
            var uiContext = new TestSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(uiContext);
            var result = action();
            uiContext.RunPendingCallbacks();
            return result;
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }

    /// <summary>
    /// Sets up a SynchronizationContext for the current test thread and runs the given async action.
    /// Ensures context is properly cleaned up after the action completes.
    /// </summary>
    /// <param name="action">The async action to run on the UI context thread.</param>
    public static async Task RunWithUIContextAsync(Func<Task> action)
    {
        var previousContext = SynchronizationContext.Current;
        try
        {
            var uiContext = new TestSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(uiContext);
            await action();
            uiContext.RunPendingCallbacks();
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }

    /// <summary>
    /// Sets up a SynchronizationContext for the current test thread and runs the given async action with a return value.
    /// Ensures context is properly cleaned up after the action completes.
    /// </summary>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <param name="action">The async action to run on the UI context thread.</param>
    /// <returns>The result from the action.</returns>
    public static async Task<T> RunWithUIContextAsync<T>(Func<Task<T>> action)
    {
        var previousContext = SynchronizationContext.Current;
        try
        {
            var uiContext = new TestSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(uiContext);
            var result = await action();
            uiContext.RunPendingCallbacks();
            return result;
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }

    /// <summary>
    /// Runs a callback on a background thread to simulate cross-context event handling.
    /// Returns a task that completes when the background work is done.
    /// </summary>
    /// <param name="action">The action to run on a background thread.</param>
    /// <returns>A task that completes when the background work is done.</returns>
    public static async Task RunOnBackgroundThreadAsync(Action action)
    {
        await Task.Run(() =>
        {
            // Clear the synchronization context to simulate a background thread
            SynchronizationContext.SetSynchronizationContext(null);
            action();
        });
    }

    /// <summary>
    /// Runs a callback on a background thread and returns a result.
    /// </summary>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <param name="action">The action to run on a background thread.</param>
    /// <returns>A task that completes with the result when the background work is done.</returns>
    public static async Task<T> RunOnBackgroundThreadAsync<T>(Func<T> action)
    {
        return await Task.Run(() =>
        {
            // Clear the synchronization context to simulate a background thread
            SynchronizationContext.SetSynchronizationContext(null);
            return action();
        });
    }

    /// <summary>
    /// A test synchronization context that captures Post/Send calls for inspection.
    /// Allows tests to verify that marshaling occurred and to run pending callbacks.
    /// </summary>
    public class TestSynchronizationContext : SynchronizationContext
    {
        private readonly ConcurrentQueue<(SendOrPostCallback callback, object? state)> _callbacks =
            new ConcurrentQueue<(SendOrPostCallback, object?)>();

        public override void Post(SendOrPostCallback d, object? state)
        {
            _callbacks.Enqueue((d, state));
        }

        public override void Send(SendOrPostCallback d, object? state)
        {
            // Send is synchronous, so execute immediately
            d(state);
        }

        /// <summary>
        /// Executes all pending callbacks that were posted to this context.
        /// </summary>
        public void RunPendingCallbacks()
        {
            while (_callbacks.TryDequeue(out var item))
            {
                item.callback(item.state);
            }
        }

        /// <summary>
        /// Gets the number of pending callbacks.
        /// </summary>
        public int PendingCallbackCount => _callbacks.Count;
    }
}
