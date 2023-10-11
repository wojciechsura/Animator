using Animator.Designer.BusinessLogic.Models.AddNamespace;
using Animator.Designer.BusinessLogic.Services.Messaging;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
using Animator.Designer.Common.Helpers;
using Animator.Designer.Resources.Windows.AddNamespaceWindow;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.AddNamespace
{
    public class AddNamespaceWindowViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> availableAssemblies;
        private readonly ObservableCollection<string> availableNamespaces;
        private readonly WrapperContext wrapperContext;
        private readonly IAddNamespaceWindowAccess access;
        private readonly IMessagingService messagingService;
        private string selectedAssembly;
        private Assembly selectedAssemblyObject;
        private string selectedNamespace;
        private string prefix;

        private ChainedLambdaCondition<AddNamespaceWindowViewModel> prefixValidCondition;
        private ChainedLambdaCondition<AddNamespaceWindowViewModel> assemblyNamespaceUniqueCondition;
        private ChainedLambdaCondition<AddNamespaceWindowViewModel> assemblySelectedCondition;
        private ChainedLambdaCondition<AddNamespaceWindowViewModel> namespaceSelectedCondition;

        private void HandleSelectedNamespaceChanged()
        {
            availableNamespaces.Clear();
            SelectedAssemblyObject = null;

            try
            {
                SelectedAssemblyObject = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == selectedAssembly);

                if (SelectedAssemblyObject == null)
                {
                    SelectedAssemblyObject = Assembly.Load(selectedAssembly);

                    if (SelectedAssemblyObject == null)
                        throw new InvalidOperationException("Cannot load assembly!");
                }

                HashSet<string> namespaces = new();
                foreach (var type in selectedAssemblyObject.GetTypes())
                {
                    namespaces.Add(type.Namespace);
                }

                namespaces.OrderBy(n => n)
                    .ForEach(n => availableNamespaces.Add(n));
            }
            catch (Exception e)
            {
                messagingService.Warn(string.Format(Strings.Message_CannotLoadAssembly, e.Message));
            }
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        private void DoOk()
        {
            access.Close(true);
        }

        private bool PrefixAlreadyUsed(string prefix)
            => prefix == null || wrapperContext.Namespaces
                .OfType<AssemblyNamespaceViewModel>()
                .Any(ns => ns.Prefix?.ToLowerInvariant() == prefix.ToLowerInvariant());

        private bool AssemblyNamespaceExists(string assembly, string @namespace)
            => assembly == null || @namespace == null || wrapperContext.Namespaces
                .OfType<AssemblyNamespaceViewModel>()
                .Any(ns => ns.Assembly.GetName().Name == assembly && ns.Namespace == @namespace);

        public AddNamespaceWindowViewModel(WrapperContext wrapperContext, IAddNamespaceWindowAccess access, IMessagingService messagingService)
        {
            this.wrapperContext = wrapperContext;
            this.access = access;
            this.messagingService = messagingService;

            availableNamespaces = new();

            // Load existing namespaces

            ExistingNamespaces = wrapperContext.Namespaces
                .OfType<AssemblyNamespaceViewModel>()
                .ToList();

            // Load available namespaces

            var assemblyFolder = Path.GetDirectoryName(typeof(Animator.Engine.Elements.Movie).Assembly.Location);

            availableAssemblies = new();

            Directory.GetFiles(assemblyFolder, "*.dll")
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ForEach(f => availableAssemblies.Add(f));

            selectedAssembly = null;

            // Commands

            prefixValidCondition = Condition.ChainedLambda(this, vm => !PrefixAlreadyUsed(vm.Prefix), false);
            assemblyNamespaceUniqueCondition = Condition.ChainedLambda(this, vm => !AssemblyNamespaceExists(vm.SelectedAssembly, vm.SelectedNamespace), false);
            assemblySelectedCondition = Condition.ChainedLambda(this, vm => vm.SelectedAssemblyObject != null, false);
            namespaceSelectedCondition = Condition.ChainedLambda(this, vm => !string.IsNullOrEmpty(vm.SelectedNamespace), false);

            var okCondition = prefixValidCondition & assemblySelectedCondition & namespaceSelectedCondition & assemblyNamespaceUniqueCondition;

            OkCommand = new AppCommand(obj => DoOk(), okCondition);
            CancelCommand = new AppCommand(obj => DoCancel());
        }

        public IReadOnlyList<AssemblyNamespaceViewModel> ExistingNamespaces { get; }

        public ObservableCollection<string> AvailableAssemblies => availableAssemblies;

        public string SelectedAssembly
        {
            get => selectedAssembly;
            set => Set(ref selectedAssembly, value, changeHandler: HandleSelectedNamespaceChanged);
        }

        public Assembly SelectedAssemblyObject
        {
            get => selectedAssemblyObject;
            private set => Set(ref selectedAssemblyObject, value);
        }

        public ObservableCollection<string> AvailableNamespaces => availableNamespaces;

        public string SelectedNamespace
        {
            get => selectedNamespace;
            set => Set(ref selectedNamespace, value);
        }

        public string Prefix
        {
            get => prefix;
            set => Set(ref prefix, value);
        }

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        public AddNamespaceResultModel Result => new AddNamespaceResultModel(prefix, selectedAssemblyObject, selectedNamespace);
    }
}
