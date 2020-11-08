// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Cooking.Avalonia.ViewModels;
using System;

namespace Cooking.Avalonia
{
    /// <summary>
    /// Avalonia view locator: https://avaloniaui.net/docs/tutorial/locating-views.
    /// </summary>
    public class ViewLocator : IDataTemplate
    {
        /// <inheritdoc/>
        public IControl Build(object data)
        {
            if (data != null)
            {
                string name = data.GetType().FullName.Replace("ViewModel", "View", StringComparison.OrdinalIgnoreCase);
                var type = Type.GetType(name);

                if (type != null)
                {
                    return (Control)Activator.CreateInstance(type);
                }
            }

            return new TextBlock();
        }

        /// <inheritdoc/>
        public bool Match(object data) => data is ViewModelBase;
    }
}