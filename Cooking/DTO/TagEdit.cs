﻿using Data.Model;
using System;
using System.ComponentModel;
using Validar;

namespace Cooking.DTO
{
    [InjectValidation]
    public class TagEdit : INotifyPropertyChanged
    {
        public static readonly TagEdit Any = new TagEdit()
        {
            CanBeRemoved = false
        };

        public Guid ID { get; set; }

        public string? Name { get; set; }
        public TagType Type { get; set; }
        public string? Color { get; set; }

        public bool IsChecked { get; set; }
        public bool CanBeRemoved { get; set; } = true;

#pragma warning disable CS0067 // Событие не используется
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
    }
}