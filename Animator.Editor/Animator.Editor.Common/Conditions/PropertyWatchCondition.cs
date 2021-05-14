using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.Common.Conditions
{
    /// <summary>
    /// Condition, which reflects to some object's bool-type property.
    /// </summary>
    /// <typeparam name="TSource">Source object containing watched property. Must implement <see cref="INotifyPropertyChanged"/> interface.</typeparam>
    public class PropertyWatchCondition<TSource> : BaseCondition
        where TSource : class, INotifyPropertyChanged
    {
        private readonly Func<TSource, bool> getValueFunc;
        private readonly string valuePropertyName;
        private readonly bool defaultValue;
        private TSource source;

        private void SetSource(TSource value)
        {
            if (source != null)
                source.PropertyChanged -= HandleSourcePropertyChanged;

            source = value;

            if (source != null)
                source.PropertyChanged += HandleSourcePropertyChanged;

            OnValueChanged(GetValue());
        }

        private PropertyInfo GetPropertyInfo<TClass, TProperty>(Expression<Func<TClass, TProperty>> propertyLambda)
        {
            Type type = typeof(TClass);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        private void HandleSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(valuePropertyName))
                OnValueChanged(GetValue());
        }

        public PropertyWatchCondition(TSource source, Expression<Func<TSource, bool>> expression, bool defaultValue)
        {
            this.source = source;
            this.defaultValue = defaultValue;

            if (source != null)
                source.PropertyChanged += HandleSourcePropertyChanged;

            var propInfo = GetPropertyInfo(expression);
            valuePropertyName = propInfo.Name;

            getValueFunc = expression.Compile();
        }

        public PropertyWatchCondition(Expression<Func<TSource, bool>> expression, bool defaultValue)
            : this(null, expression, defaultValue)
        {
            
        }

        public override bool GetValue() => source != null ? getValueFunc(source) : defaultValue;

        public TSource Source
        {
            get => source;
            set
            {
                SetSource(value);
            }
        }
    }
}
