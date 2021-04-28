using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using ProCalc.NET;
using ProCalc.NET.Numerics;
using ProCalc.NET.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class PropertyExpressionAnimator : TimeDurationNumericPropertyAnimator
    {
        // Private types ------------------------------------------------------

        private class ResolvedIdentifier : BaseResolvedIdentifier
        {
            private readonly IdentifierType identifierType;

            public ResolvedIdentifier(IdentifierType identifierType)
            {
                this.identifierType = identifierType;
            }

            public override IdentifierType GetIdentifierType() => identifierType;
        }

        private class IdentifierResolver : BaseExternalIdentifierResolver
        {
            public override BaseResolvedIdentifier ResolveIdentifier(string identifierName)
            {
                return new ResolvedIdentifier(IdentifierType.itExternalVariable);
            }
        }

        private class ExternalVariableResolver : BaseExternalVariableResolver
        {
            // Ulong is explicitly not supported, because long is the biggest type supported by ProCalc
            private readonly Type[] intTypes = new[] { typeof(char), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long) };
            private readonly Type[] floatTypes = new[] { typeof(float), typeof(double) };
            private readonly Type[] stringTypes = new[] { typeof(string) };
            private readonly Type[] boolTypes = new[] { typeof(bool) };

            private readonly PropertyExpressionAnimator animator;
            private readonly float currentTime;

            public ExternalVariableResolver(PropertyExpressionAnimator animator, float currentTime)
            {
                this.animator = animator;
                this.currentTime = currentTime;
            }

            public override BaseNumeric ResolveVariable(string externalVariableName)
            {
                switch (externalVariableName)
                {
                    case "_StartTime":
                        return new FloatNumeric(animator.StartTime.TotalMilliseconds);
                    case "_EndTime":
                        return new FloatNumeric(animator.EndTime.TotalMilliseconds);
                    case "_CurrentTime":
                        return new FloatNumeric(currentTime);
                    case "_EasedFactor":
                        return new FloatNumeric(EasingFunctionRepository.Ease(animator.EasingFunction, TimeCalculator.EvalAnimationFactor((float)animator.StartTime.TotalMilliseconds, (float)animator.EndTime.TotalMilliseconds, currentTime)));
                    default:
                        {
                            if (!externalVariableName.Contains('.') || externalVariableName.StartsWith(".") || externalVariableName.EndsWith("."))
                                throw new InvalidOperationException("Invalid reference in expression.");
                            int dotPos = externalVariableName.IndexOf('.');

                            string target = externalVariableName[0..dotPos];
                            string path = externalVariableName[(dotPos + 1)..];

                            (ManagedObject obj, ManagedProperty prop) = animator.Scene.FindProperty(target, path);

                            object value = obj.GetValue(prop);

                            if (value == null)
                                throw new InvalidOperationException($"Referenced path {externalVariableName} returned null!");

                            // Integers
                            if (intTypes.Contains(value.GetType()))
                                return new IntNumeric((long)Convert.ChangeType(value, typeof(long)));
                            else if (floatTypes.Contains(value.GetType()))
                                return new FloatNumeric((double)Convert.ChangeType(value, typeof(double)));
                            else if (stringTypes.Contains(value.GetType()))
                                return new StringNumeric((string)value);
                            else if (boolTypes.Contains(value.GetType()))
                                return new BoolNumeric((bool)value);
                            else
                                throw new InvalidOperationException($"Cannot convert value of type {value.GetType().Name} evaluated from {externalVariableName} to value, which can be processed in an expression.");
                        }
                }
            }
        }

        // Private static fields ----------------------------------------------

        private static readonly ProCalcCore proCalc;

        // Protected methods --------------------------------------------------

        public override void ApplyAnimation(float timeMs)
        {
            (var obj, var prop) = Scene.FindProperty(TargetName, Path);

            var compiled = proCalc.Compile(Expression, new IdentifierResolver());
            var result = proCalc.Execute(compiled, null, false, new ExternalVariableResolver(this, timeMs));

            object resultValue;

            if (result is IntNumeric intNumeric)
                resultValue = intNumeric.GetValue();
            else if (result is IntFractionNumeric intFractionNumeric)
                resultValue = intFractionNumeric.GetRealValue();
            else if (result is FloatNumeric floatNumeric)
                resultValue = floatNumeric.GetValue();
            else if (result is StringNumeric stringNumeric)
                resultValue = stringNumeric.GetValue();
            else if (result is BoolNumeric boolNumeric)
                resultValue = boolNumeric.GetValue();
            else
                throw new InvalidOperationException("Unsupported result type!");

            object targetValue = Convert.ChangeType(resultValue, prop.Type);
            obj.SetValue(prop, targetValue);
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
