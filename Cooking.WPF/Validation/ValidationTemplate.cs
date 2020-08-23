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
using System.Diagnostics;
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
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IValidator<T>> Validators = new ConcurrentDictionary<RuntimeTypeHandle, IValidator<T>>();

        private readonly T target;
        private readonly IValidator<T> validator;
        private ValidationResult validationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationTemplate{T}"/> class.
        /// </summary>
        /// <param name="target">Object which will be validated. Injected by Validar into classes.</param>
        public ValidationTemplate(T target)
        {
            this.target = target;
            validator = GetValidator(target.GetType());
            validationResult = validator.Validate(target);
            target.PropertyChanged += Validate;
        }

        /// <inheritdoc/>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

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
        /// Internal factory method for FluentValidation validators.
        /// </summary>
        /// <param name="modelType">Type of object to validate.</param>
        /// <returns>Instance of validator.</returns>
        private static IValidator<T> GetValidator(Type modelType)
        {
            if (!Validators.TryGetValue(modelType.TypeHandle, out IValidator<T>? validator))
            {
                string typeName = $"{modelType.Namespace}.{modelType.Name}Validator";
                Type? type = modelType.Assembly.GetType(typeName, true);
                if (type != null && Application.Current is PrismApplication app)
                {
                    validator = app.Container.Resolve(type) as IValidator<T>;

                    if (validator == null)
                    {
                        throw new InvalidOperationException("Provide validator for type!");
                    }

                    Validators[modelType.TypeHandle] = validator;
                }
                else
                {
                    throw new InvalidOperationException("Provide validator for type!");
                }
            }

            return validator;
        }

        private void Validate(object? sender, PropertyChangedEventArgs e)
        {
            validationResult = validator.Validate(target);
            if (validationResult != null)
            {
                foreach (ValidationFailure error in validationResult.Errors)
                {
                    RaiseErrorsChanged(error.PropertyName);
                }
            }
        }

        private void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
