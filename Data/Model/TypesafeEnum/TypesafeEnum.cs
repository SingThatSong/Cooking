namespace Cooking.Data.Model
{
    /// <summary>
    /// Base class for Typesafe enum pattern.
    /// See https://www.infoworld.com/article/3198453/how-to-implement-a-type-safe-enum-pattern-in-c.html .
    /// </summary>
    public abstract class TypesafeEnum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypesafeEnum"/> class.
        /// </summary>
        /// <param name="id">Enum identificator.</param>
        public TypesafeEnum(int id)
        {
            ID = id;
        }

        /// <summary>
        /// Gets or sets iD for enum. Represents underlying type of enum.
        /// </summary>
        public int ID { get; set; }
    }
}
