using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class MinMaxClass : ManagedObject
    {

        #region Min managed property

        public int Min
        {
            get => (int)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public static readonly ManagedProperty MinProperty = ManagedProperty.Register(typeof(MinMaxClass),
            nameof(Min),
            typeof(int),
            new ManagedSimplePropertyMetadata(0, HandleMinChanged));

        private static void HandleMinChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            sender.CoerceValue(MaxProperty);
        }

        #endregion


        #region Max managed property

        public int Max
        {
            get => (int)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public static readonly ManagedProperty MaxProperty = ManagedProperty.Register(typeof(MinMaxClass),
            nameof(Max),
            typeof(int),
            new ManagedSimplePropertyMetadata(0, HandleMaxChanged, CoerceMax));

        private static void HandleMaxChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            sender.CoerceValue(MinProperty);            
        }

        private static object CoerceMax(ManagedObject obj, object baseValue)
        {
            var min = (int)obj.GetFinalBaseValue(MinProperty);
            var max = (int)baseValue;

            return Math.Max(min, max);
        }

        #endregion
    }
}
