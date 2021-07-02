using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Animator.Editor.BusinessLogic.Services.Paths
{
    class PathService : IPathService
    {
        private const string PUBLISHER = "Spooksoft";
        private const string APPNAME = "Animator.Editor";

        private const string CONFIG_FILENAME = "Config.xml";
        private const string STORED_FILES_FOLDER = "StoredFiles";

        private string appDataPath;
        private string configPath;
        private string storedFilesPath;

        public PathService()
        {
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PUBLISHER, APPNAME);
            Directory.CreateDirectory(appDataPath);

            storedFilesPath = Path.Combine(appDataPath, STORED_FILES_FOLDER);
            Directory.CreateDirectory(storedFilesPath);

            configPath = Path.Combine(appDataPath, CONFIG_FILENAME);
        }

        public string ConfigPath => configPath;

        public string StoredFilesPath => storedFilesPath;
    }
}
