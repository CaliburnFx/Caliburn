#if SILVERLIGHT

namespace Caliburn.ShellFramework
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Core;
    using PresentationFramework;

    public class BindableEnumCollection<T> : BindableCollection<BindableEnum> where T : struct
    {
        public BindableEnumCollection(params T[] values)
            : this()
        {
            var self = this.ToList();

            Clear();

            (from value in values
             from bindableEnum in self
             where bindableEnum.Value.Equals(value)
             select bindableEnum).Apply(Add);
        }

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

#endif