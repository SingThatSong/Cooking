using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooking.WPF.Validation
{
    /// <summary>
    /// Static class for validation extention methods.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Determines whether object is valid.
        /// </summary>
        /// <param name="obj">Object to validate.</param>
        /// <returns>If object is valid.</returns>
        public static bool IsValid(this object obj)
        {
            if (obj != null && obj is INotifyDataErrorInfo errorInfo)
            {
                return !errorInfo.HasErrors;
            }
            else
            {
                // Объект не поддерживает валидацию, значит валиден
                return true;
            }
        }
    }
}
