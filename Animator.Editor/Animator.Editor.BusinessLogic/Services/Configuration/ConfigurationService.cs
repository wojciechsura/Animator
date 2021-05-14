using Animator.Editor.BusinessLogic.Models.Configuration;
using Animator.Editor.BusinessLogic.Services.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Services.Config
{
    class ConfigrationService : IConfigurationService
    {
        private readonly IPathService pathService;
        private readonly ConfigModel config;

        public ConfigrationService(IPathService pathService)
        {
            this.pathService = pathService;
            this.config = new ConfigModel();

            var configPath = pathService.ConfigPath;
            try
            {
                config.Load(configPath);
            }
            catch
            {
                config.SetDefaults();
            }
        }

        public bool Save()
        {
            try
            {
                config.Save(pathService.ConfigPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ConfigModel Configuration => config;
    }
}
