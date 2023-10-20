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
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class EasingExpression : Element
    {
        // Private types ------------------------------------------------------

        private class IdentifierResolver : BaseExternalIdentifierResolver
        {
            public override BaseResolvedIdentifier ResolveIdentifier(string identifierName)
            {
                if (identifierName == "t")
                    return new ResolvedExternalVariable(identifierName);
                else
                    return new ResolvedVariable(identifierName);
            }
        }

        private class ExternalVariableResolver : BaseExternalVariableResolver
        {
            private readonly AnimateWithExpression animator;
            private readonly float factor;

            private (bool resolved, BaseNumeric result) KnownVariableToValue(string externalVariableName)
            {
                switch (externalVariableName)
                {
                    case "t":
                        return (true, new FloatNumeric(factor));
                    default:
                        return (false, null);
                }
            }

            public ExternalVariableResolver(float factor)
            {
                this.factor = factor;
            }

            public override BaseNumeric ResolveVariable(string externalVariableName)
            {
                (bool resolved, BaseNumeric result) = KnownVariableToValue(externalVariableName);

                if (resolved)
                    return result;

                throw new InvalidOperationException($"Cannot resolve {externalVariableName}.");
            }
        }

        // Private fields -----------------------------------------------------

        private readonly ProCalcCore proCalc = new();
        private CompiledExpression compiled = null;

        // Private methods ----------------------------------------------------

        private float ValueFromNumeric(BaseNumeric result)
        {
            if (result is FloatValueNumeric floatNumeric)
                return (float)floatNumeric.RealValue;

            throw new AnimationException("Expression does not evaluate to single floating-point value!", GetPath());
        }

        // Public methods -----------------------------------------------------

        public EasingExpression() 
        { 
        
        }

        public float CalculateFactor(float t)
        {
            if (compiled == null)
                throw new AnimationException("No expression provided for EasingExpression or expression is invalid!", GetPath());

            BaseNumeric result = proCalc.Execute(compiled, null, false, new ExternalVariableResolver(t));

            return ValueFromNumeric(result);
        }

        // Public properties --------------------------------------------------

        #region Formula managed property

        /// <summary>
        /// Function used for easing. It should be a valid formula,
        /// using t as a parameter (ranging from 0 to 1). It should 
        /// (but not have to) fulfill the following requirements:
        /// 
        /// <ul>
        ///   <li>f(0) = 0 and f(1) = 1</li>
        ///   <li>for t in range [0, 1], f(t) should be in range(0, 1)</li>
        /// </ul>
        /// </summary>
        public string Formula
        {
            get => (string)GetValue(FormulaProperty);
            set => SetValue(FormulaProperty, value);
        }

        public static readonly ManagedProperty FormulaProperty = ManagedProperty.Register(typeof(EasingExpression),
            nameof(Formula),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = string.Empty, ValueChangedHandler = HandleFormulaPropertyChanged });

        private static void HandleFormulaPropertyChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            if (sender is EasingExpression expression)
                expression.HandleFormulaChanged((string)args.NewValue);
        }

        private void HandleFormulaChanged(string newValue)
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
