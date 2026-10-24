namespace Sharphy.Core;

/// <summary>
/// Informasi arsitektur dan kapabilitas runtime yang aktif di Sharphy.
/// </summary>
public static class RuntimeEnvironment
{
    public static string FrameworkVersion =>
#if NET10_0_OR_GREATER
        ".NET 10.0 (Latest)";
#elif NET9_0_OR_GREATER
        ".NET 9.0 (STS)";
#else
        ".NET 8.0 (LTS)";
#endif

    public static bool SupportsUnsafeAccessor => true;
}

