using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using ProCalc.NET;
using ProCalc.NET.Numerics;
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
    public class Eval : BaseMarkupExtension
    {
        private static readonly ProCalcCore proCalc = new ProCalcCore();

        private string formula;
        private CompiledExpression cachedFormula;

        private long NumericAsInt(BaseNumeric numeric)
        {
            return numeric switch
            {
                IntNumeric intNumeric => intNumeric.GetValue(),
                FloatNumeric floatNumeric => (long)floatNumeric.GetValue(),
                IntFractionNumeric intFractionNumeric => (long)intFractionNumeric.GetRealValue(),
                _ => throw new InvalidOperationException($"Cannot represent result of evaulation ({numeric.AsString}) as an int value!"),
            };
        }

        private double NumericAsFloat(BaseNumeric numeric)
        {
            return numeric switch
            {
                IntNumeric intNumeric => intNumeric.GetValue(),
                FloatNumeric floatNumeric => floatNumeric.GetValue(),
                IntFractionNumeric intFractionNumeric => intFractionNumeric.GetRealValue(),
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

            try
            {
                if (cachedFormula == null)
                {
                    cachedFormula = proCalc.Compile(formula);
                }

                result = proCalc.Execute(cachedFormula);
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
