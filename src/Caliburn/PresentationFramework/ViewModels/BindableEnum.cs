namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Core;

    /// <summary>
    /// A databindable enum with a display name.
    /// </summary>
    public class BindableEnum
    {
        /// <summary>
        /// Gets or sets the underlying value.
        /// </summary>
        /// <value>The underlying value.</value>
        public int UnderlyingValue { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            var otherBindable = obj as BindableEnum;

            if(otherBindable != null)
                return UnderlyingValue == otherBindable.UnderlyingValue;

            if(obj is int)
                return UnderlyingValue.Equals((int)obj);

            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return UnderlyingValue.GetHashCode();
        }

        /// <summary>
        /// Creates a <see cref="BindableEnum"/> from the provided enum value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The <see cref="BindableEnum"/>.</returns>
        public static BindableEnum Create(object value)
        {
            var fields = value.GetType()
                .GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (var fieldInfo in fields)
            {
                var fieldValue = fieldInfo.GetValue(null);
                if (fieldValue.Equals(value))
                {
                    return Create(fieldInfo);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a <see cref="BindableEnum"/> from the <see cref="FieldInfo"/>.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/>.</param>
        /// <returns>The <see cref="BindableEnum"/>.</returns>
        public static BindableEnum Create(FieldInfo field)
        {
            var att = field.GetAttributes<DescriptionAttribute>(false)
                    .FirstOrDefault();
            var value = field.GetValue(null);

            return new BindableEnum
            {
                Value = value,
                UnderlyingValue = Convert.ToInt32(value),
                DisplayName = att != null ? att.Description : field.Name
            };
        }
    }
}