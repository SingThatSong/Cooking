using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class IngredientGroupEditViewModel : OkCancelViewModel
    {
        public IngredientGroupEditViewModel() : this(null) { }
        public IngredientGroupEditViewModel(DTO.IngredientGroupEdit? ingredientGroup = null)
        {
            IngredientGroup = ingredientGroup ?? new DTO.IngredientGroupEdit();
        }

        public DTO.IngredientGroupEdit IngredientGroup { get; }
    }
}