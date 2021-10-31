namespace Cooking.WPF.Converters;

/// <summary>
/// Straignt implementation of BooleanConverter.
/// </summary>
public class SimpleBooleanConverter : BooleanConverter<bool>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleBooleanConverter"/> class.
    /// Simple implementation of BooleanConverter, when true is true and false is false.
    /// </summary>
    public SimpleBooleanConverter()
        : base(true, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleBooleanConverter"/> class.
    /// Simple implementation of BooleanConverter, but with ability to redefine bool values.
    /// </summary>
    /// <param name="trueValue">Bool value that will be used as true.</param>
    /// <param name="falseValue">Bool value that will be used as false.</param>
    public SimpleBooleanConverter(bool trueValue, bool falseValue)
        : base(trueValue, falseValue)
    {
    }
}
