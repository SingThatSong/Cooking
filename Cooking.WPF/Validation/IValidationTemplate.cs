using System;
using System.Collections;
using System.ComponentModel;

namespace Cooking;

/// <summary>
/// Common validation template interface.
/// </summary>
public interface IValidationTemplate
{
    /// <summary>
    /// Disable validation.
    /// </summary>
    void DisableValidation();

    /// <summary>
    /// Enable validation.
    /// </summary>
    void EnableValidation();

    /// <summary>
    /// Execute validation forcefully.
    /// </summary>
    void ForceValidate();
}
