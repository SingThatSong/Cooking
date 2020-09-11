using System.ComponentModel;
using System.Reflection;

namespace Cooking.WPF.Validation
{
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
        public static dynamic? GetValidationTemplate(this object obj)
        {
            return obj.GetType()
                      .GetField("validationTemplate", BindingFlags.NonPublic | BindingFlags.Instance)?
                      .GetValue(obj);
        }

        /// <summary>
        /// Determines whether object is valid.
        /// </summary>
        /// <param name="obj">Object to validate.</param>
        /// <param name="forceValidation">Parameter indicating whether this method should trigger object's validation method instead of just checking it's status.</param>
        /// <returns>If object is valid.</returns>
        public static bool IsValid(this object obj, bool forceValidation = false)
        {
            if (obj != null && obj is INotifyDataErrorInfo errorInfo)
            {
                if (forceValidation)
                {
                    obj.GetValidationTemplate()?.Validate();
                }

                return !errorInfo.HasErrors;
            }
            else
            {
                // Object does not support validation, hence it's valid
                return true;
            }
        }
    }
}
