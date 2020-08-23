using Cooking.Data.Model;
using System.ComponentModel;
using Validar;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// Dto for garnish editing.
    /// </summary>
    [InjectValidation]
    public class GarnishEdit : EntityNotify
    {
        /// <summary>
        /// Gets or sets garnish name.
        /// </summary>
        public string? Name { get; set; }
    }
}