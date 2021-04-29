using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
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
    public class PropertyExpressionAnimator : TimeDurationNumericPropertyAnimator
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
            private readonly PropertyExpressionAnimator animator;
            private readonly float currentTime;

            private readonly Regex ReferenceRegex = new Regex("[a-zA-Z_][a-zA-Z0-9_]*(\\.[a-zA-Z_][a-zA-Z0-9_])+");

            private (bool resolved, BaseNumeric result) ReferenceToValue(string externalVariableName)
            {
                if (!ReferenceRegex.IsMatch(externalVariableName))
                    throw new InvalidOperationException($"The reference {externalVariableName} used in expression is not valid.");

                int dotPos = externalVariableName.IndexOf('.');

                string target = externalVariableName[0..dotPos];
                string path = externalVariableName[(dotPos + 1)..];

                (ManagedObject obj, ManagedProperty prop) = animator.Scene.FindProperty(target, path);

                object value = obj.GetValue(prop);
                if (value == null)
                    throw new InvalidOperationException($"Referenced path {externalVariableName} returned null!");

                Type valueType = value.GetType();

                if (value.GetType() == typeof(char))
                    return (true, new IntNumeric((char)value));
                else if (value.GetType() == typeof(byte))
                    return (true, new IntNumeric((byte)value));
                else if (value.GetType() == typeof(short))
                    return (true, new IntNumeric((short)value));
                else if (value.GetType() == typeof(ushort))
                    return (true, new IntNumeric((ushort)value));
                else if (value.GetType() == typeof(int))
                    return (true, new IntNumeric((int)value));
                else if (value.GetType() == typeof(uint))
                    return (true, new IntNumeric((uint)value));
                else if (value.GetType() == typeof(long))
                    return (true, new IntNumeric((long)value));
                else if (value.GetType() == typeof(float))
                    return (true, new FloatNumeric((float)value));
                else if (value.GetType() == typeof(double))
                    return (true, new FloatNumeric((double)value));
                else if (value.GetType() == typeof(string))
                    return (true, new StringNumeric((string)value));
                else if (value.GetType() == typeof(bool))
                    return (true, new BoolNumeric((bool)value));
                else if (value.GetType() == typeof(PointF))
                {
                    var point = (PointF)value;
                    return (true, new MatrixNumeric(2, 1, new[] { new FloatNumeric(point.X), new FloatNumeric(point.Y) }));
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
                        return (true, new FloatNumeric(EasingFunctionRepository.Ease(animator.EasingFunction,
                                TimeCalculator.EvalAnimationFactor((float)animator.StartTime.TotalMilliseconds,
                                    (float)animator.EndTime.TotalMilliseconds,
                                    currentTime))));
                    default:
                        return (false, null);
                }
            }

            public ExternalVariableResolver(PropertyExpressionAnimator animator, float currentTime)
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

        private static readonly ProCalcCore proCalc = new ProCalcCore();

        // Private methods ----------------------------------------------------

        private object ValueFromNumeric(BaseNumeric result, Type propType)
        {
            if (propType == typeof(char))
            {
                if (result is IntNumeric intNumeric)
                    return (char)intNumeric.GetValue();
            }
            else if (propType == typeof(byte))
            {
                if (result is IntNumeric intNumeric)
                    return (byte)intNumeric.GetValue();
            }
            else if (propType == typeof(short))
            {
                if (result is IntNumeric intNumeric)
                    return (short)intNumeric.GetValue();
            }
            else if (propType == typeof(ushort))
            {
                if (result is IntNumeric intNumeric)
                    return (ushort)intNumeric.GetValue();
            }
            else if (propType == typeof(int))
            {
                if (result is IntNumeric intNumeric)
                    return (int)intNumeric.GetValue();
            }
            else if (propType == typeof(uint))
            {
                if (result is IntNumeric intNumeric)
                    return (uint)intNumeric.GetValue();
            }
            else if (propType == typeof(float))
            {
                if (result is FloatValueNumeric floatNumeric)
                    return (float)floatNumeric.GetRealValue();
            }
            else if (propType == typeof(double))
            {
                if (result is FloatValueNumeric floatNumeric)
                    return (double)floatNumeric.GetRealValue();
            }
            else if (propType == typeof(string))
            {
                if (result is StringNumeric stringNumeric)
                    return (string)stringNumeric.AsString();
            }
            else if (propType == typeof(bool))
            {
                if (result is BoolNumeric boolNumeric)
                    return (bool)boolNumeric.GetValue();
            }
            else if (propType == typeof(PointF))
            {
                if (result is MatrixNumeric matrixNumeric &&
                    matrixNumeric.GetWidth() == 2 &&
                    matrixNumeric.GetHeight() == 1 &&
                    matrixNumeric.GetElementAt(0, 0) is FloatValueNumeric xNumeric &&
                    matrixNumeric.GetElementAt(1, 0) is FloatValueNumeric yNumeric)
                    return new PointF((float)xNumeric.GetRealValue(), (float)yNumeric.GetRealValue());
            }

            throw new InvalidOperationException($"Expression result {result.AsString()} cannot be converted into property type {propType.Name}");
        }

        // Protected methods --------------------------------------------------

        public override void ApplyAnimation(float timeMs)
        {
            (var obj, var prop) = Scene.FindProperty(TargetName, Path);

            var compiled = proCalc.Compile(Expression, new IdentifierResolver());
            var result = proCalc.Execute(compiled, null, false, new ExternalVariableResolver(this, timeMs));

            var propType = prop.Type;

            object resultValue = ValueFromNumeric(result, propType);
            
            obj.SetValue(prop, resultValue);
        }

        // Public properties --------------------------------------------------

        #region Expression managed property

        public string Expression
        {
            get => (string)GetValue(ExpressionProperty);
            set => SetValue(ExpressionProperty, value);
        }

        public static readonly ManagedProperty ExpressionProperty = ManagedProperty.Register(typeof(PropertyExpressionAnimator),
            nameof(Expression),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = "0" });

        #endregion
    }
}
