using System.Runtime.CompilerServices;
using Fody;
using NullGuard;

// Global project settings. Replacement of ApplicationInfo.cs.

// Set Null-check on all func arguments globally
[assembly: NullGuard(ValidationFlags.Arguments)]

// Set ConfigureAwait globally
[assembly: ConfigureAwait(false)]

[assembly: InternalsVisibleTo("Cooking.WPF.Tests")]