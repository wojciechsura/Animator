using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using ProCalc.NET;
using ProCalc.NET.Numerics;
using ProCalc.NET.Resolvers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    [DefaultProperty(nameof(Formula))]
    public partial class Eval : BaseMarkupExtension
    {
        private class IdentifierResolver : BaseExternalIdentifierResolver
        {
            public override BaseResolvedIdentifier ResolveIdentifier(string identifierName)
            {
                return new ResolvedExternalVariable(identifierName);
            }
        }

        private class VariableResolver : BaseExternalVariableResolver
        {
            private readonly SceneElement root;

            public VariableResolver(SceneElement root)
            {
                this.root = root;
            }

            public override BaseNumeric ResolveVariable(string externalVariableName)
            {
                var current = root;

                while (current != null) 
                {
                    var variable = current.Variables.FirstOrDefault(v => v.Name == externalVariableName);
                    if (variable != null)
                    {
                        return variable switch
                        {
                            IntVariable intVariable => new IntNumeric(intVariable.Value),
                            FloatVariable floatVariable => new FloatNumeric(floatVariable.Value),
                            PointVariable pointVariable => new VectorNumeric(new[] {
                                new FloatNumeric(pointVariable.Value.X),
                                new FloatNumeric(pointVariable.Value.Y) }
                            ),
                            _ => throw new InvalidOperationException("Unsupported variable type!")
                        };
                    }

                    current = current.ParentInfo?.Parent as SceneElement;
                }

                throw new InvalidOperationException($"Unknown variable: {externalVariableName}!");
            }
        }

        private string formula;
        private CompiledExpression cachedFormula;

        private long NumericAsInt(BaseNumeric numeric)
        {
            return numeric switch
            {
                IntNumeric intNumeric => intNumeric.Value,
                FloatNumeric floatNumeric => (long)floatNumeric.Value,
                IntFractionNumeric intFractionNumeric => (long)intFractionNumeric.RealValue,
                _ => throw new InvalidOperationException($"Cannot represent result of evaulation ({numeric.AsString}) as an int value!"),
            };
        }

        private double NumericAsFloat(BaseNumeric numeric)
        {
            return numeric switch
            {
                IntNumeric intNumeric => intNumeric.Value,
                FloatNumeric floatNumeric => floatNumeric.Value,
                IntFractionNumeric intFractionNumeric => intFractionNumeric.RealValue,
                _ => throw new InvalidOperationException($"Cannot represent result of evaulation ({numeric.AsString}) as a float value!"),
            };
        }

        private PointF NumericAsPointF(BaseNumeric result)
        {
            if (result is VectorNumeric vectorNumeric && vectorNumeric.Count == 2)
            {
                return new PointF((float)NumericAsFloat(vectorNumeric[0]), (float)NumericAsFloat(vectorNumeric[1]));
            }

            throw new InvalidOperationException($"Cannot represent result of evaulation ({result.AsString}) as a PointF value!");
        }

        public override void ProvideValue(ManagedObject @object, ManagedProperty property)
        {
            BaseNumeric result;

            var proCalc = new ProCalcCore();

            try
            {
                if (cachedFormula == null)
                {
                    cachedFormula = proCalc.Compile(formula, new IdentifierResolver());
                }

                result = proCalc.Execute(cachedFormula, externalVariableResolver: new VariableResolver(@object as SceneElement));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Failed to evaluate expression {formula}", e);
            }

            if (property.Type == typeof(int))
                @object.SetValue(property, (int)NumericAsInt(result));
            else if (property.Type == typeof(uint))
                @object.SetValue(property, (uint)NumericAsInt(result));
            else if (property.Type == typeof(short))
                @object.SetValue(property, (short)NumericAsInt(result));
            else if (property.Type == typeof(ushort))
                @object.SetValue(property, (ushort)NumericAsInt(result));
            else if (property.Type == typeof(long))
                @object.SetValue(property, (long)NumericAsInt(result));
            else if (property.Type == typeof(ulong))
                @object.SetValue(property, (ulong)NumericAsInt(result));
            else if (property.Type == typeof(byte))
                @object.SetValue(property, (byte)NumericAsInt(result));
            else if (property.Type == typeof(sbyte))
                @object.SetValue(property, (sbyte)NumericAsInt(result));

            else if (property.Type == typeof(float))
                @object.SetValue(property, (float)NumericAsFloat(result));
            else if (property.Type == typeof(double))
                @object.SetValue(property, (double)NumericAsFloat(result));

            else if (property.Type == typeof(string))
                @object.SetValue(property, result.AsString);

            else if (property.Type == typeof(PointF))
                @object.SetValue(property, NumericAsPointF(result));

            else 
                throw new InvalidOperationException($"Cannot set result of evaulation ({result.AsString}) to a property of type {property.Type}");            
        }

        public string Formula 
        { 
            get => formula;
            set
            {
                formula = value;
                cachedFormula = null;
            }
        }
    }
}
