namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Core;

    /// <summary>
    /// A collection of <see cref="BindableEnum"/> based on an <see cref="Enum"/>.
    /// </summary>
    /// <typeparam name="T">The Enum type.</typeparam>
    public class BindableEnumCollection<T> : BindableCollection<BindableEnum> where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableEnumCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public BindableEnumCollection(params T[] values)
            : this()
        {
            var toRemove = from bindableEnum in this
                           where !values.Contains((T)bindableEnum.Value)
                           select bindableEnum;

            toRemove.ToList().Apply(x => Remove(x));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableEnumCollection&lt;T&gt;"/> class.
        /// </summary>
        public BindableEnumCollection()
        {
            var type = typeof(T);

            if(!type.IsEnum)
                throw new ArgumentException("This class only supports Enum types.");

            var fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach(var field in fields)
            {
                var att = field.GetAttributes<DescriptionAttribute>(false)
                    .FirstOrDefault();

                var bindableEnum = new BindableEnum
                {
                    Value = field.GetValue(null),
                    UnderlyingValue = (int)field.GetValue(null),
                    DisplayName = att != null ? att.Description : field.Name
                };

                Add(bindableEnum);
            }
        }
    }
}