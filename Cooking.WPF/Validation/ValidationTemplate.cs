using FluentValidation;
using FluentValidation.Results;
using NullGuard;
using Prism.Unity;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Cooking
{
    /// <summary>
    /// Validar Validation template for FluentValidation. See https://github.com/Fody/Validar#validation-template-implementations.
    /// </summary>
    /// <typeparam name="T">Type of validated object.</typeparam>
    public class ValidationTemplate<T> : IDataErrorInfo, INotifyDataErrorInfo
            where T : INotifyPropertyChanged
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IValidator<T>> Validators = new();

        private readonly T target;
        private readonly IValidator<T> validator;
        private ValidationResult validationResult = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationTemplate{T}"/> class.
        /// </summary>
        /// <param name="target">Object which will be validated. Injected by Validar into classes.</param>
        public ValidationTemplate(T target)
        {
            this.target = target;
            validator = GetValidator(target.GetType());
            ValidateInternal();
            target.PropertyChanged += (_, eventArgs) => Validate(eventArgs.PropertyName);
        }

        /// <inheritdoc/>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Gets or sets a value indicating whether validation should not occur.
        /// </summary>
        public bool ValidationSuspended { get; set; }

        /// <inheritdoc/>
        public bool HasErrors => validationResult.Errors.Count > 0;

        /// <inheritdoc/>
        public string Error => string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage));

        /// <inheritdoc/>
        public string this[string propertyName] => string.Join(Environment.NewLine, GetErrors(propertyName));

        /// <inheritdoc/>
        public IEnumerable GetErrors(string? propertyName) => validationResult.Errors
                                                                              .Where(x => x.PropertyName == propertyName)
                                                                              .Select(x => x.ErrorMessage);

        /// <summary>
        /// Actually perform validation.
        /// </summary>
        /// <param name="propertyName">Property to validate.</param>
        public void Validate(string? propertyName)
        {
            if (ValidationSuspended)
            {
                return;
            }

            ValidateInternal(propertyName);
        }

        /// <summary>
        /// Actually perform validation.
        /// </summary>
        public void ForceValidate() => ValidateInternal();

        /// <summary>
        /// Internal factory method for FluentValidation validators.
        /// </summary>
        /// <param name="modelType">Type of object to validate.</param>
        /// <returns>Instance of validator.</returns>
        private static IValidator<T> GetValidator(Type modelType)
        {
            if (Validators.TryGetValue(modelType.TypeHandle, out IValidator<T>? cachedValue))
            {
                return cachedValue!;
            }

            string typeName = $"{modelType.Namespace}.{modelType.Name}Validator";
            Type type = modelType.Assembly.GetType(typeName, throwOnError: true)!;
            if (Application.Current is PrismApplication app)
            {
                if (app.Container.Resolve(type) is IValidator<T> validator)
                {
                    return Validators[modelType.TypeHandle] = validator;
                }
            }

            throw new InvalidOperationException($"Provide validator for type {modelType.FullName}!");
        }

        private void ValidateInternal(string? propertyName = null)
        {
            validationResult = propertyName != null
                                    ? validator.Validate(target, opts => opts.IncludeProperties(propertyName))
                                    : validator.Validate(target);

            validationResult.Errors.ForEach(error => RaiseErrorsChanged(error.PropertyName));
        }

        private void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
