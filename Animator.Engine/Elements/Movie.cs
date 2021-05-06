using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Root element of the movie.
    /// </summary>
    [ContentProperty(nameof(Scenes))]
    public class Movie : ManagedObject
    {
        #region Config managed property

        /// <summary>
        /// Contains configuration of the movie.
        /// </summary>
        public MovieConfig Config
        {
            get => (MovieConfig)GetValue(ConfigProperty);
            set => SetValue(ConfigProperty, value);
        }

        public static readonly ManagedProperty ConfigProperty = ManagedProperty.RegisterReference(typeof(Movie),
            nameof(Config),
            typeof(MovieConfig));

        #endregion

        #region Scenes managed collection

        /// <summary>
        /// Contains scenes of the movie.
        /// </summary>
        public ManagedCollection<Scene> Scenes
        {
            get => (ManagedCollection<Scene>)GetValue(ScenesProperty);
        }

        public static readonly ManagedProperty ScenesProperty = ManagedProperty.RegisterCollection(typeof(Movie),
            nameof(Scenes),
            typeof(ManagedCollection<Scene>));

        #endregion
    }
}
