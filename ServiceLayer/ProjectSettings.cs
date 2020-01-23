using Fody;
using NullGuard;

/// <summary>
/// Global project settings. Replacement of ApplicationInfo.cs.
/// </summary>

// Set Null-check on all func arguments globally
[assembly: NullGuard(ValidationFlags.Arguments)]

// Set ConfigureAwait globally
[assembly: ConfigureAwait(false)]