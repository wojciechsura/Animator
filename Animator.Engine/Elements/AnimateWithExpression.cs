using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Exceptions;
using ProCalc.NET;
using ProCalc.NET.Numerics;
using ProCalc.NET.Resolvers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Allows evaluating animated value of an property basing on a mathematical expression.
    /// </summary>
    public class AnimateWithExpression : AnimateNumericPropertyInTime
    {
        // Private types ------------------------------------------------------

        private class IdentifierResolver : BaseExternalIdentifierResolver
        {
            public override BaseResolvedIdentifier ResolveIdentifier(string identifierName)
            {
                return new ResolvedExternalVariable(identifierName);
            }
        }

        private class ExternalVariableResolver : BaseExternalVariableResolver
        {
            private readonly AnimateWithExpression animator;
            private readonly float currentTime;

            private readonly Regex ReferenceRegex = new Regex("[a-zA-Z_@][a-zA-Z0-9_@]*(\\.[a-zA-Z_@][a-zA-Z0-9_@])+");

            private (bool resolved, BaseNumeric result) ReferenceToValue(string externalVariableName)
            {
                if (!ReferenceRegex.IsMatch(externalVariableName))
                    throw new InvalidOperationException($"The reference {externalVariableName} used in expression is not valid.");

                (var obj, var prop) = animator.AnimatedObject.FindProperty(externalVariableName);

                object value = obj.GetValue(prop);
                if (value == null)
                    throw new InvalidOperationException($"Referenced path {externalVariableName} returned null!");

                Type valueType = value.GetType();

                if (valueType == typeof(char))
                    return (true, new IntNumeric((char)value));
                else if (valueType == typeof(byte))
                    return (true, new IntNumeric((byte)value));
                else if (valueType == typeof(short))
                    return (true, new IntNumeric((short)value));
                else if (valueType == typeof(ushort))
                    return (true, new IntNumeric((ushort)value));
                else if (valueType == typeof(int))
                    return (true, new IntNumeric((int)value));
                else if (valueType == typeof(uint))
                    return (true, new IntNumeric((uint)value));
                else if (valueType == typeof(long))
                    return (true, new IntNumeric((long)value));
                else if (valueType == typeof(float))
                    return (true, new FloatNumeric((float)value));
                else if (valueType   == typeof(double))
                    return (true, new FloatNumeric((double)value));
                else if (valueType == typeof(string))
                    return (true, new StringNumeric((string)value));
                else if (valueType == typeof(bool))
                    return (true, new BoolNumeric((bool)value));
                else if (valueType == typeof(PointF))
                {
                    var point = (PointF)value;
                    return (true, new VectorNumeric(new[] { new FloatNumeric(point.X), new FloatNumeric(point.Y) }));
                }
                else
                    return (false, null);                    
            }

            private (bool resolved, BaseNumeric result) KnownVariableToValue(string externalVariableName)
            {
                switch (externalVariableName)
                {
                    case "StartTime":
                        return (true, new FloatNumeric(animator.StartTime.TotalMilliseconds));
                    case "EndTime":
                        return (true, new FloatNumeric(animator.EndTime.TotalMilliseconds));
                    case "CurrentTime":
                        return (true, new FloatNumeric(currentTime));
                    case "EasedFactor":
                        var factor = TimeCalculator.EvalAnimationFactor((float)animator.StartTime.TotalMilliseconds,
                                (float)animator.EndTime.TotalMilliseconds,
                                currentTime);
                        var easedFactor = animator.Ease(factor);
                        return (true, new FloatNumeric(easedFactor));
                    default:
                        return (false, null);
                }
            }

            public ExternalVariableResolver(AnimateWithExpression animator, float currentTime)
            {
                this.animator = animator;
                this.currentTime = currentTime;
            }

            public override BaseNumeric ResolveVariable(string externalVariableName)
            {
                (bool resolved, BaseNumeric result) = KnownVariableToValue(externalVariableName);

                if (resolved)
                    return result;

                (resolved, result) = ReferenceToValue(externalVariableName);

                if (resolved)
                    return result;
               
                throw new InvalidOperationException($"Cannot resolve {externalVariableName}. The reference description may be wrong or it may return type not supported for expressions.");
            }
        }

        // Private static fields ----------------------------------------------

        private readonly ProCalcCore proCalc = new ProCalcCore();

        // Private fields -----------------------------------------------------

        private CompiledExpression compiled;

        // Private methods ----------------------------------------------------

        private object ValueFromNumeric(BaseNumeric result, Type propType)
        {
            if (propType == typeof(char))
            {
                if (result is IntNumeric intNumeric)
                    return (char)intNumeric.Value;
            }
            else if (propType == typeof(byte))
            {
                if (result is IntNumeric intNumeric)
                    return (byte)intNumeric.Value;
            }
            else if (propType == typeof(short))
            {
                if (result is IntNumeric intNumeric)
                    return (short)intNumeric.Value;
            }
            else if (propType == typeof(ushort))
            {
                if (result is IntNumeric intNumeric)
                    return (ushort)intNumeric.Value;
            }
            else if (propType == typeof(int))
            {
                if (result is IntNumeric intNumeric)
                    return (int)intNumeric.Value;
            }
            else if (propType == typeof(uint))
            {
                if (result is IntNumeric intNumeric)
                    return (uint)intNumeric.Value;
            }
            else if (propType == typeof(float))
            {
                if (result is FloatValueNumeric floatNumeric)
                    return (float)floatNumeric.RealValue;
            }
            else if (propType == typeof(double))
            {
                if (result is FloatValueNumeric floatNumeric)
                    return (double)floatNumeric.RealValue;
            }
            else if (propType == typeof(string))
            {
                if (result is StringNumeric stringNumeric)
                    return (string)stringNumeric.AsString;
            }
            else if (propType == typeof(bool))
            {
                if (result is BoolNumeric boolNumeric)
                    return (bool)boolNumeric.Value;
            }
            else if (propType == typeof(PointF))
            {
                if (result is VectorNumeric vectorNumeric &&
                    vectorNumeric.Count == 2 &&
                    vectorNumeric[0] is FloatValueNumeric xNumeric &&
                    vectorNumeric[1] is FloatValueNumeric yNumeric)
                    return new PointF((float)xNumeric.RealValue, (float)yNumeric.RealValue);
            }

            throw new InvalidOperationException($"Expression result {result.AsString} cannot be converted into property type {propType.Name}");
        }

        // Protected methods --------------------------------------------------

        public override bool ApplyAnimation(float timeMs)
        {
            if (compiled == null)
                throw new AnimationException("No expression provided for PropertyExpressionAnimator or expression is invalid!", GetPath());

            (var obj, var prop) = AnimatedObject.FindProperty(PropertyRef);

            BaseNumeric result = proCalc.Execute(compiled, null, false, new ExternalVariableResolver(this, timeMs));

            var propType = prop.Type;
            object value = ValueFromNumeric(result, propType);

            var previous = obj.GetValue(prop);
            obj.SetAnimatedValue(prop, value);
            var next = obj.GetValue(prop);

            // if (!object.Equals(next, previous)) { Console.WriteLine($"{obj}.{prop} changed from {previous} to {next}"); }

            return !object.Equals(previous, next);
        }

        // Public properties --------------------------------------------------

        #region Expression managed property

        /// <summary>
        /// Expression for the property.
        /// 
        /// You may use the following pre-defined values:
        /// 
        /// <ul>
        /// <li><strong>StartTime</strong> - value of StartTime property expressed in milliseconds (1/1000 of a second)</li>
        /// <li><strong>EndTime</strong> - value of EndTime property expressed in milliseconds</li>
        /// <li><strong>CurrentTime</strong> - time of current frame expressed in milliseconds</li>
        /// <li><strong>EasedFactor</strong> - value ranging from 0 (when CurrentTime = StartTime) to 1 (when CurrentTime = EndTime)
        /// with easing function applied.</li>
        /// </ul>
        ///     
        /// In addition, you can reach value of any named element's property with the simple
        /// notation.
        /// </summary>
        /// <example>
        /// <code><pre>Expression="Rect.Position + [10,10] * EasedFactor"</pre></code>
        /// </example>
        public string Expression
        {
            get => (string)GetValue(ExpressionProperty);
            set => SetValue(ExpressionProperty, value);
        }

        public static readonly ManagedProperty ExpressionProperty = ManagedProperty.Register(typeof(AnimateWithExpression),
            nameof(Expression),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = "0", ValueChangedHandler = HandleExpressionPropertyChanged });

        private static void HandleExpressionPropertyChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            if (sender is AnimateWithExpression animator)
                animator.HandleExpressionChanged((string)args.NewValue);
        }

        private void HandleExpressionChanged(string newValue)
        {
            if (!string.IsNullOrEmpty(newValue))
            {
                compiled = proCalc.Compile(newValue, new IdentifierResolver());
            }
            else
            {
                compiled = null;
            }
        }

        #endregion
    }
}
