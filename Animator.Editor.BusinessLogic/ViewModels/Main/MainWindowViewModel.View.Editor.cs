using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        private void HandleWordWrapChanged()
        {
            var wordWrapSetting = configurationService.Configuration.Editor.WordWrap;

            if (wordWrapSetting.Value != wordWrap)
                wordWrapSetting.Value = wordWrap;
        }

        private void HandleLineNumbersChanged()
        {
            Configuration.ConfigValue<bool> lineNumbersSetting = configurationService.Configuration.Editor.LineNumbers;

            if (lineNumbersSetting.Value != lineNumbers)
                lineNumbersSetting.Value = lineNumbers;
        }

        public bool WordWrap
        {
            get => wordWrap;
            set
            {
                Set(ref wordWrap, () => WordWrap, value, HandleWordWrapChanged);
            }
        }

        public bool LineNumbers
        {
            get => lineNumbers;
            set
            {
                Set(ref lineNumbers, () => LineNumbers, value, HandleLineNumbersChanged);
            }
        }
    }
}
