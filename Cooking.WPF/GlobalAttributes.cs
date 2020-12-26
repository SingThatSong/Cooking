using Fody;
using NullGuard;

// Set Null-check on all func arguments globally
[assembly: NullGuard(ValidationFlags.Arguments)]

// Set ConfigureAwait globally
[assembly: ConfigureAwait(false)]