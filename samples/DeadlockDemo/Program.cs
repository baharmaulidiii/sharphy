namespace DeadlockDemo;

/// <summary>
/// Reproducible concurrency testbed modeling AB-BA Monitor deadlocks and sync-over-async ThreadPool starvation.
/// </summary>
public static class Program
{
    private static readonly object LockA = new();
    private static readonly object LockB = new();

    public static void Main(string[] args)
    {
        var scenario = args.FirstOrDefault()?.ToLowerInvariant() ?? "--dry-run";
        switch (scenario)
        {
            case "--deadlock":
                RunDeadlockScenario(timeoutMs: 3000);
                break;
            case "--starvation":
                RunStarvationScenario(workerCount: 8);
                break;
            default:
                Console.WriteLine("DeadlockDemo ready. Use '--deadlock' or '--starvation' to simulate runtime contention.");
                break;
        }
    }

    public static void RunDeadlockScenario(int timeoutMs)
    {
        using var barrier = new Barrier(2);
        var worker1 = new Thread(() =>
        {
            lock (LockA)
            {
                barrier.SignalAndWait(timeoutMs);
                if (Monitor.TryEnter(LockB, timeoutMs))
                {
                    Monitor.Exit(LockB);
                }
            }
        }) { IsBackground = true, Name = "DeadlockWorker1" };

        var worker2 = new Thread(() =>
        {
            lock (LockB)
            {
                barrier.SignalAndWait(timeoutMs);
                if (Monitor.TryEnter(LockA, timeoutMs))
                {
                    Monitor.Exit(LockA);
                }
            }
        }) { IsBackground = true, Name = "DeadlockWorker2" };

        worker1.Start();
        worker2.Start();
        worker1.Join(timeoutMs + 500);
        worker2.Join(timeoutMs + 500);
    }

    public static void RunStarvationScenario(int workerCount)
    {
        var tasks = new Task[workerCount];
        for (int i = 0; i < workerCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                SimulateAsyncWork().Wait();
            });
        }

        Task.WaitAll(tasks);
    }

    private static async Task SimulateAsyncWork()
    {
        await Task.Delay(25).ConfigureAwait(false);
    }
}
