using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Types;
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
    public partial class Movie : ManagedObject
    {
        public Movie()
        {
            Config = new MovieConfig();
        }

        [DoNotDocument]
        public string Path { get; set; }

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
            typeof(MovieConfig), new ManagedReferencePropertyMetadata { ValueValidationHandler = ValidateConfig });

        private static bool ValidateConfig(ManagedObject sender, ValueValidationEventArgs args)
        {
            return args.NewValue != null;
        }

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
