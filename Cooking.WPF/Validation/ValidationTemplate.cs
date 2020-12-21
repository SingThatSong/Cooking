using Cooking.WPF.Validation;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using NullGuard;
using Prism.Unity;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace Cooking
{
    /// <summary>
    /// Validar Validation template for FluentValidation. See https://github.com/Fody/Validar#validation-template-implementations.
    /// </summary>
    /// <typeparam name="T">Type of validated object.</typeparam>
    [NullGuard(ValidationFlags.None)]
    public class ValidationTemplate<T> : IDataErrorInfo, INotifyDataErrorInfo
            where T : INotifyPropertyChanged
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, (IValidator<T>, List<string>)> Validators = new();

        private readonly T target;
        private readonly IValidator<T> validator;
        private readonly List<string> propertyNames;
        private ValidationResult validationResult = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationTemplate{T}"/> class.
        /// </summary>
        /// <param name="target">Object which will be validated. Injected by Validar into classes.</param>
        public ValidationTemplate(T target)
        {
            this.target = target;
            (validator, propertyNames) = GetValidator(target.GetType());
            ValidateInternal();
            target.PropertyChanged += (_, eventArgs) => Validate(eventArgs.PropertyName);
        }

        /// <inheritdoc/>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Gets or sets a value indicating whether mapping is ongoing.
        /// </summary>
        public bool ValidationSuspended { get; set; }

        /// <inheritdoc/>
        public bool HasErrors => validationResult.Errors.Count > 0;

        /// <inheritdoc/>
        public string Error
        {
            get
            {
                string[] strings = validationResult.Errors
                                                   .Select(x => x.ErrorMessage)
                                                   .ToArray();
                return string.Join(Environment.NewLine, strings);
            }
        }

        // Duplicates INotifyDataErrorInfo.GetErrors

        /// <inheritdoc/>
        public string? this[string propertyName] => null;

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

            if (propertyName != null)
            {
                if (propertyNames.Contains(propertyName))
                {
                    ValidateInternal(propertyName);
                }
            }
            else
            {
                ForceValidate();
            }
        }

        /// <summary>
        /// Actually perform validation.
        /// </summary>
        public void ForceValidate()
        {
            ValidateInternal();
        }

        /// <summary>
        /// Internal factory method for FluentValidation validators.
        /// </summary>
        /// <param name="modelType">Type of object to validate.</param>
        /// <returns>Instance of validator.</returns>
        private static (IValidator<T> Validator, List<string> PropertyNames) GetValidator(Type modelType)
        {
            if (!Validators.TryGetValue(modelType.TypeHandle, out (IValidator<T> Validator, List<string> PropertyNames) cachedValue))
            {
                string typeName = $"{modelType.Namespace}.{modelType.Name}Validator";
                Type type = modelType.Assembly.GetType(typeName, throwOnError: true)!;
                if (Application.Current is PrismApplication app)
                {
                    if (app.Container.Resolve(type) is IValidator<T> validator)
                    {
                        return Validators[modelType.TypeHandle] = (validator, GetPropertyNames(validator));
                    }
                    else
                    {
                        throw new InvalidOperationException($"Provide validator for type {modelType.FullName}!");
                    }
                }
            }

            return cachedValue;
        }

        private static List<string> GetPropertyNames(IValidator<T> validator)
        {
            dynamic? rules = validator.GetRules();

            var result = new List<string>();
            foreach (object rule in rules!)
            {
                if (rule is PropertyRule propertyRule)
                {
                    result.Add(propertyRule.PropertyName);
                }
            }

            return result;
        }

        private void ValidateInternal(string? propertyName = null)
        {
            validationResult = validator.Validate(target);
            foreach (ValidationFailure error in validationResult.Errors)
            {
                if (propertyName == null || error.PropertyName == propertyName)
                {
                    RaiseErrorsChanged(error.PropertyName);
                }
            }
        }

        private void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
