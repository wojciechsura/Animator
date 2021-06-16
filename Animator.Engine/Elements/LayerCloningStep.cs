using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    [ContentProperty(nameof(Transforms))]
    public class LayerCloningStep : SceneElement
    {
        // Internal methods ---------------------------------------------------
        
        internal List<Matrix> BuildMatrices()
        {
            Matrix singleTransform = new Matrix();

            for (int i = 0; i < Transforms.Count; i++)
            {
                singleTransform.Multiply(Transforms[i].GetMatrix());
            }

            Matrix runningMatrix = new Matrix();
            List<Matrix> result = new();
            for (int i = 0; i < Count; i++)
            {
                result.Add(runningMatrix.Clone());
                runningMatrix.Multiply(singleTransform, MatrixOrder.Append);
            }

            return result;
        }

        // Public properties --------------------------------------------------

        #region Count managed property

        /// <summary>
        /// Defines, how many times layer should be cloned in this step
        /// </summary>
        public int Count
        {
            get => (int)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        public static readonly ManagedProperty CountProperty = ManagedProperty.Register(typeof(LayerCloningStep),
            nameof(Count),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0, CoerceValueHandler = CoerceCount });

        private static object CoerceCount(ManagedObject obj, object baseValue)
        {
            var count = (int)baseValue;
            return Math.Max(0, count);
        }

        #endregion

        #region ReverseOrder managed property

        /// <summary>
        /// If set to true, causes clones to be rendered from last to first.
        /// </summary>
        public bool ReverseOrder
        {
            get => (bool)GetValue(ReverseOrderProperty);
            set => SetValue(ReverseOrderProperty, value);
        }

        public static readonly ManagedProperty ReverseOrderProperty = ManagedProperty.Register(typeof(LayerCloningStep),
            nameof(ReverseOrder),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

        #endregion

        #region Transforms managed collection

        /// <summary>
        /// Defines transforms added to each clone.
        /// </summary>
        public ManagedCollection<Transform> Transforms
        {
            get => (ManagedCollection<Transform>)GetValue(TransformsProperty);
        }

        public static readonly ManagedProperty TransformsProperty = ManagedProperty.RegisterCollection(typeof(LayerCloningStep),
            nameof(Transforms),
            typeof(ManagedCollection<Transform>));

        #endregion
    }
}
