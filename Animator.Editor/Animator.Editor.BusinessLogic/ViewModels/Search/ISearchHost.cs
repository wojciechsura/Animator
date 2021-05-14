using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Animator.Editor.BusinessLogic.Models.Search;
using Animator.Editor.Common.Conditions;

namespace Animator.Editor.BusinessLogic.ViewModels.Search
{
    public interface ISearchHost
    {
        void ReplaceAll(ReplaceAllModel replaceModel);
        void Replace(ReplaceModel replaceModel);
        void FindNext(SearchModel searchModel);

        BaseCondition CanSearchCondition { get; }
        BaseCondition SelectionAvailableCondition { get; }
    }
}
