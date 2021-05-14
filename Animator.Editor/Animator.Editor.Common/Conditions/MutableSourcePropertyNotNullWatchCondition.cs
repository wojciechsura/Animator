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
    public class MutableSourcePropertyNotNullWatchCondition<TSourceProvider, TSource> : BaseCondition
        where TSource : class, INotifyPropertyChanged
        where TSourceProvider : class, INotifyPropertyChanged
    {
        private readonly TSourceProvider sourceProvider;
        private readonly Func<TSourceProvider, TSource> getSourceFunc;
        private readonly string sourcePropertyName;
        private TSource source;
        private readonly Func<TSource, object> getValueFunc;
        private readonly string valuePropertyName;        

        private PropertyInfo GetPropertyInfo<TClass, TProperty>(Expression<Func<TClass, TProperty>> propertyLambda)
        {
            Type type = typeof(TClass);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            var propInfo = member.Member as PropertyInfo;
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

        private void Detach(TSource source)
        {
            if (source != null)
                source.PropertyChanged -= HandleSourcePropertyChanged;
        }

        private void Attach(TSource source)
        {
            if (source != null)
                source.PropertyChanged += HandleSourcePropertyChanged;
        }

        private void HandleSourceProviderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(sourcePropertyName))
            {
                Detach(source);

                source = getSourceFunc(sourceProvider);

                Attach(source);

                OnValueChanged(GetValue());
            }
        }

        private void HandleSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(valuePropertyName))
                OnValueChanged(GetValue());
        }

        public MutableSourcePropertyNotNullWatchCondition(TSourceProvider sourceProvider,
            Expression<Func<TSourceProvider, TSource>> getSourceExpression,
            Expression<Func<TSource, object>> getValueExpression)
        {
            this.sourceProvider = sourceProvider ?? throw new ArgumentNullException(nameof(sourceProvider));
            sourceProvider.PropertyChanged += HandleSourceProviderPropertyChanged;

            var sourcePropInfo = GetPropertyInfo(getSourceExpression);
            sourcePropertyName = sourcePropInfo.Name;
            getSourceFunc = getSourceExpression.Compile();

            var propInfo = GetPropertyInfo(getValueExpression);
            valuePropertyName = propInfo.Name;
            getValueFunc = getValueExpression.Compile();

            source = getSourceFunc(sourceProvider);
            Attach(source);
        }

        public override bool GetValue() => source != null ? (getValueFunc(source) != null) : false;
    }
}
