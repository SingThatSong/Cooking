using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cooking.WPF.Validation
{
    /// <summary>
    /// Class representing Null Object pattern fo <see cref="IValidator"/>.
    /// </summary>
    /// <typeparam name="T">Type of instance to validate.</typeparam>
    public class ValidatorNullObject<T> : IValidator<T>
    {
        /// <inheritdoc/>
        public bool CanValidateInstancesOfType(Type type) => true;

        /// <inheritdoc/>
        public IValidatorDescriptor? CreateDescriptor() => null;

        /// <inheritdoc/>
        public ValidationResult? Validate(T instance) => null;

        /// <inheritdoc/>
        public ValidationResult? Validate(IValidationContext context) => null;

        /// <inheritdoc/>
        public Task<ValidationResult>? ValidateAsync(T instance, CancellationToken cancellation = default) => (Task<ValidationResult>)Task.CompletedTask;

        /// <inheritdoc/>
        public Task<ValidationResult>? ValidateAsync(IValidationContext context, CancellationToken cancellation = default) => (Task<ValidationResult>)Task.CompletedTask;
    }
}
