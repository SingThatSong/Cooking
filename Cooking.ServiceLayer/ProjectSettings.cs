using Fody;
using NullGuard;
using System.Runtime.CompilerServices;

/// <summary>
/// Global project settings. Replacement of ApplicationInfo.cs.
/// </summary>

// Set Null-check on all func arguments globally
[assembly: NullGuard(ValidationFlags.Arguments)]

// Set ConfigureAwait globally
[assembly: ConfigureAwait(false)]

[assembly: InternalsVisibleTo("Cooking.WPF.Tests")]