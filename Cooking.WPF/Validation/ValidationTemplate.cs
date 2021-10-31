using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using Cooking.WPF.Services;
using Cooking.WPF.Validation;
using FluentValidation;
using FluentValidation.Results;
using Prism.Unity;

namespace Cooking;

/// <summary>
/// Validar Validation template for FluentValidation. See https://github.com/Fody/Validar#validation-template-implementations.
/// </summary>
/// <typeparam name="T">Type of validated object.</typeparam>
public class ValidationTemplate<T> : IDataErrorInfo, INotifyDataErrorInfo, IValidationTemplate
    where T : INotifyPropertyChanged
{
    private static readonly IValidator<T> Validator;
    private readonly T target;

    /// <summary>
    /// Event subsctiprion optimization
    /// See https://stackoverflow.com/a/24239037.
    /// </summary>
    private readonly PropertyChangedEventHandler sharedHandler;
    private ValidationResult validationResult = new();

    static ValidationTemplate()
    {
        Type modelType = typeof(T);
        string typeName = $"{modelType.Namespace}.{modelType.Name}Validator";
        Type? type = modelType.Assembly.GetType(typeName, throwOnError: true);

        Validator = type != null && Application.Current is PrismApplication app
                        ? app.Container.Resolve(type) as IValidator<T> ?? new ValidatorNullObject<T>()
                        : throw new InvalidOperationException($"Provide validator for type {modelType.FullName}!");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationTemplate{T}"/> class.
    /// </summary>
    /// <param name="target">Object which will be validated. Injected by Validar into classes.</param>
    public ValidationTemplate(T target)
    {
        this.target = target;
        sharedHandler = OnPropertyChanged;

        // If parent of current class also has validation, we skip initialization for it
        // Effectively, we override parent's validation
        if (target.GetType() == typeof(T))
        {
            ValidateInternal();
            EnableValidation();
        }
    }

    /// <inheritdoc/>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <inheritdoc/>
    public bool HasErrors => validationResult.Errors.Count > 0;

    /// <inheritdoc/>
    public string Error => string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage));

    /// <inheritdoc/>
    public string this[string columnName]
    {
        get
        {
            IEnumerable errors = GetErrors(columnName);

            return errors.GetEnumerator().MoveNext()
                        ? string.Join(Environment.NewLine, errors)
                        : string.Empty;
        }
    }

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
        ValidateInternal(propertyName);
    }

    /// <summary>
    /// Stop validating object on PropertyChanged event.
    /// </summary>
    public void DisableValidation()
    {
        target.PropertyChanged -= sharedHandler;
    }

    /// <summary>
    /// Start validating object on PropertyChanged event.
    /// </summary>
    public void EnableValidation()
    {
        target.PropertyChanged += sharedHandler;
    }

    /// <summary>
    /// Actually perform validation.
    /// </summary>
    public void ForceValidate() => ValidateInternal();

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        ValidateInternal(eventArgs.PropertyName);
    }

    private void ValidateInternal(string? propertyName = null)
    {
        if (propertyName == null)
        {
            // Validate everything
            ValidationResult previousErrors = validationResult;
            validationResult = Validator.Validate(target);

            IEnumerable<string> prevErrors = previousErrors.Errors.Select(x => x.PropertyName);
            IEnumerable<string> currentErrors = validationResult.Errors.Select(x => x.PropertyName);

            foreach (string error in prevErrors.SymmetricException(currentErrors))
            {
                RaiseErrorsChanged(error);
            }
        }
        else
        {
            // Validate single property
            ValidationResult propertyResult = Validator.Validate(target, opts => opts.IncludeProperties(propertyName));

            var previousErrors = validationResult.Errors.Where(x => x.PropertyName == propertyName).ToList();

            bool wasValidationError = previousErrors.Count > 0;
            bool isValidationError = propertyResult.Errors.Count > 0;
            bool propertyValidityChanged = wasValidationError ^ isValidationError;

            if (propertyValidityChanged)
            {
                if (wasValidationError)
                {
                    validationResult.Errors.RemoveAll(x => x.PropertyName == propertyName);
                }
                else
                {
                    validationResult.Errors.AddRange(propertyResult.Errors);
                }

                RaiseErrorsChanged(propertyName);
            }
            else
            {
                // Error still present, but it may be another error
                if (isValidationError && propertyResult.Errors.Any(x => !previousErrors.Any(p => p.ErrorMessage == x.ErrorMessage)))
                {
                    validationResult.Errors.RemoveAll(x => x.PropertyName == propertyName);
                    validationResult.Errors.AddRange(propertyResult.Errors);
                    RaiseErrorsChanged(propertyName);
                }
            }
        }
    }

    private void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(target, new DataErrorsChangedEventArgs(propertyName));
}
