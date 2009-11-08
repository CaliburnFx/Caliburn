namespace Caliburn.ModelFramework
{
    using Core.Metadata;

    /// <summary>
    /// Implemented by types capable of converting property values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPropertyValueConverter<T> : IMetadata
    {
        /// <summary>
        /// Converts the proposed value to the property type.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="proposedValue">The proposed value.</param>
        /// <returns></returns>
        T Convert(IProperty<T> property, object proposedValue);

        /// <summary>
        /// Converts the property value to a generic type.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="currentValue">The current value.</param>
        /// <returns></returns>
        object ConvertBack(IProperty<T> property, T currentValue);
    }
}