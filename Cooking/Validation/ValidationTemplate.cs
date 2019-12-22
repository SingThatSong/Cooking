using FluentValidation;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using FluentValidation.Results;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Cooking
{
    public class ValidationTemplate : IDataErrorInfo, INotifyDataErrorInfo
    {
        readonly INotifyPropertyChanged target;
        readonly IValidator? validator;
        ValidationResult? validationResult;
        static ConcurrentDictionary<RuntimeTypeHandle, IValidator?> validators = new ConcurrentDictionary<RuntimeTypeHandle, IValidator>();

        public ValidationTemplate(INotifyPropertyChanged target)
        {
            this.target = target;
            validator = GetValidator(target.GetType());
            validationResult = validator?.Validate(target);
            target.PropertyChanged += Validate;
        }

        static IValidator? GetValidator(Type modelType)
        {
            if (!validators.TryGetValue(modelType.TypeHandle, out var validator))
            {
                var typeName = $"{modelType.Namespace}.{modelType.Name}Validator";
                var type = modelType.Assembly.GetType(typeName, true);
                if (type != null)
                {
                    validator = Activator.CreateInstance(type) as IValidator;
                    validators[modelType.TypeHandle] = validator;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            return validator;
        }

        void Validate(object sender, PropertyChangedEventArgs e)
        {
            validationResult = validator.Validate(target);
            foreach (var error in validationResult.Errors)
            {
                RaiseErrorsChanged(error.PropertyName);
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return validationResult.Errors
                                   .Where(x => x.PropertyName == propertyName)
                                   .Select(x => x.ErrorMessage);
        }

        public bool HasErrors
        {
            get 
            {
                return validationResult.Errors.Count > 0; 
            }
        }

        public string Error
        {
            get
            {
                var strings = validationResult.Errors.Select(x => x.ErrorMessage)
                                              .ToArray();
                return string.Join(Environment.NewLine, strings);
            }
        }

        // Duplicates INotifyDataErrorInfo.GetErrors
        public string? this[string propertyName] => null;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
