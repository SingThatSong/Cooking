using System.ComponentModel;
using System.Reflection;
using FluentValidation;

namespace Cooking.WPF.Validation;

/// <summary>
/// Static class for validation extention methods.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Get <see cref="ValidationTemplate{T}"/> for an object.
    /// </summary>
    /// <param name="obj">An object with a template.</param>
    /// <returns>Validation template object or null if object don't have one.</returns>
    public static IValidationTemplate? GetValidationTemplate(this object obj)
    {
        return obj.GetPrivateField("validationTemplate") as IValidationTemplate;
    }

    /// <summary>
    /// Determines whether object is valid.
    /// </summary>
    /// <param name="obj">Object to validate.</param>
    /// <param name="forceValidation">Parameter indicating whether this method should trigger object's validation method instead of just checking it's status.</param>
    /// <returns>Valie indicating if object is valid.</returns>
    public static bool IsValid(this object? obj, bool forceValidation = false)
    {
        if (obj is INotifyDataErrorInfo errorInfo)
        {
            if (forceValidation)
            {
                obj.GetValidationTemplate()?.ForceValidate();
            }

            return !errorInfo.HasErrors;
        }
        else
        {
            // Object does not support validation, hence it's valid
            return true;
        }
    }

    private static object? GetPrivateField(this object obj, string name)
    {
        return obj.GetType()
                     .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)?
                     .GetValue(obj);
    }
}
