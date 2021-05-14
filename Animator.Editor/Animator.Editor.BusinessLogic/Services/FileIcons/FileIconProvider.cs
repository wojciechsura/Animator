using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Animator.Editor.BusinessLogic.Services.FileIcons
{
    class FileIconProvider : IFileIconProvider
    {
        private const string Prefix = "Animator.Editor.BusinessLogic.Resources.FileTypeIcons.";

        private Dictionary<string, string> extensions = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {".a", "" },
            {".bash", ""},
            {".bin", ""},
            {".c", "CFile_16x_color.png"},
            {".cl", ""},
            {".clj", ""},
            {".cmake", "MakeFile_16x.png"},
            {".d", ""},
            {".di", ""},
            {".el", ""},
            {".elc", ""},
            {".elv", ""},
            {".erl", ""},
            {".ex", ""},
            {".fasl", ""},
            {".fish", ""},
            {".go", ""},
            {".haxe", ""},{"hbs", ""},
            {".hh", ""},
            {".hs", ""},
            {".hxml", ""},
            {".hxx", ""},
            {".jl", ""},
            {".l", ""},
            {".lhs", ""},
            {".lisp", ""},
            {".litcoffee", ""},
            {".lsp", ""},
            {".lua", ""},
            {".make", "MakeFile_16x.png"},
            {".mk", "MakeFile_16x.png"},
            {".o", ""},
            {".otd", ""},
            {".patel", ""},
            {".pdf", "PDFFile_16x.png"},
            {".ps", ""},
            {".ptl", ""},
            {".scala", ""},
            {".scm", ""},
            {".sfd", ""},
            {".skim", ""},
            {".slim", ""},
            {".so", ""},
            {".ss", ""},
            {".swf", ""},
            {".swift", ""},
            {".tex", ""},
            {".vb", ""},
            {".vue", ""},
            {".yaml", ""},
            {".yml", ""},
            {".zsh", ""},
            {".indd", "AdobeIndesign_16x.png"},
            {".indl", "AdobeIndesign_16x.png"},
            {".indt", "AdobeIndesign_16x.png"},
            {".inml", "AdobeIndesign_16x.png"},
            {".ai", "AdobeIllustrator_16x.png"},
            {".psb", "AdobePhotoshop_16x.png"},
            {".psd", "AdobePhotoshop_16x.png"},
            {".exe", "Application_16x.png"},
            {".aac", "AudioOn_16x.png"},
            {".mp3", "AudioOn_16x.png"},
            {".flac", "AudioOn_16x.png"},
            {".m4a", "AudioOn_16x.png"},
            {".wma", "AudioOn_16x.png"},
            {".aiff", "AudioOn_16x.png"},
            {".asp", "ASPXFile_16x.png"},
            {".aspx", "ASPXFile_16x.png"},
            {".asm", "AssemblerSourceFile_16x.png"},
            {".s", "AssemblerSourceFile_16x.png"},
            {".styl", "BuildStyle_16x.png"},
            {".cc", "CFile_16x_color.png"},
            {".cpp", "CPP_16x.png"},
            {".cxx", "CPP_16x.png"},
            {".h", "CPPHeaderFile_16x.png"},
            {".hpp", "CPPHeaderFile_16x.png"},
            {".cs", "CS_16x.png"},
            {".cert", "Certificate_16x.png"},
            {".cer", "Certificate_16x.png"},
            {".crt", "Certificate_16x.png"},
            {".config", "ConfigurationFile_16x.png"},
            {".conf", "ConfigurationFile_16x.png"},
            {".cmd", "Console_16x.png"},
            {".sh", "Console_16x.png"},
            {".bat", "Console_16x.png"},
            {".ps1", "Console_16x.png"},
            {".vcxproj", "CPPApplication_16x.png"},
            {".csproj", "CSApplication_16x.png"},
            {".cur", "CursorFile_16x.png"},
            {".sql", "Database_16x.png"},
            {".mdb", "Database_16x.png"},
            {".sqlite", "Database_16x.png"},
            {".pdb", "Database_16x.png"},
            {".vsix", "Extension_16x.png"},
            {".manifest", "ExtensionManagerManifest_16x.png"},
            {".resx", "FileGroup_16x.png"},
            {".fs", "FS_16x.png"},
            {".fsproj", "FSApplication_16x.png"},
            {".woff", "Font_16x.png"},
            {".woff2", "Font_16x.png"},
            {".ttf", "Font_16x.png"},
            {".eot", "Font_16x.png"},
            {".suit", "Font_16x.png"},
            {".otf", "Font_16x.png"},
            {".bmap", "Font_16x.png"},
            {".fnt", "Font_16x.png"},
            {".odttf", "Font_16x.png"},
            {".ttc", "Font_16x.png"},
            {".font", "Font_16x.png"},
            {".fonts", "Font_16x.png"},
            {".sui", "Font_16x.png"},
            {".ntf", "Font_16x.png"},
            {".mrf", "Font_16x.png"},
            {".htm", "HTMLFile_16x.png"},
            {".html", "HTMLFile_16x.png"},
            {".ico", "Image_16x.png"},
            {".png", "Image_16x.png"},
            {".jpeg", "Image_16x.png"},
            {".jpg", "Image_16x.png"},
            {".gif", "Image_16x.png"},
            {".svg", "Image_16x.png"},
            {".tif", "Image_16x.png"},
            {".tiff", "Image_16x.png"},
            {".ami", "Image_16x.png"},
            {".apx", "Image_16x.png"},
            {".bmp", "Image_16x.png"},
            {".bpg", "Image_16x.png"},
            {".brk", "Image_16x.png"},
            {".dds", "Image_16x.png"},
            {".dng", "Image_16x.png"},
            {".eps", "Image_16x.png"},
            {".exr", "Image_16x.png"},
            {".fpx", "Image_16x.png"},
            {".gbr", "Image_16x.png"},
            {".img", "Image_16x.png"},
            {".jbig2", "Image_16x.png"},
            {".jb2", "Image_16x.png"},
            {".jng", "Image_16x.png"},
            {".jxr", "Image_16x.png"},
            {".pbm", "Image_16x.png"},
            {".pgf", "Image_16x.png"},
            {".pic", "Image_16x.png"},
            {".raw", "Image_16x.png"},
            {".webp", "Image_16x.png"},
            {".map", "ImageMapFile_16x.png"},
            {".jade", "JADEScript_16x.png"},
            {".jar", "JARFile_16x.png"},
            {".java", "JavaFile_16x.png"},
            {".jsp", "JavaFile_16x.png"},
            {".coffee", "JSCoffeeScript_16x.png"},
            {".json", "JSONScript_16x.png"},
            {".graphql", "JSONScript_16x.png"},
            {".js", "JSScript_16x.png"},
            {".ejs", "JSScript_16x.png"},
            {".esx", "JSScript_16x.png"},
            {".jsx", "JSXScript_16x.png"},
            {".pub", "Key_16x.png"},
            {".key", "Key_16x.png"},
            {".pem", "Key_16x.png"},
            {".asc", "Key_16x.png"},
            {".less", "LessStyleSheet_16x.png"},
            {".lib", "Library_16x.png"},
            {".bib", "Library_16x.png"},
            {".lock", "Lock_16x.png"},
            {".ics", "Mail_16x.png"},
            {".mailmap", "Mail_16x.png"},
            {".nmake", "MakeFile_16x.png"},
            {".master", "MasterPage_16x.png"},
            {".webm", "Media_16x.png"},
            {".mkv", "Media_16x.png"},
            {".flv", "Media_16x.png"},
            {".f4v", "Media_16x.png"},
            {".vob", "Media_16x.png"},
            {".ogv", "Media_16x.png"},
            {".ogg", "Media_16x.png"},
            {".gifv", "Media_16x.png"},
            {".avi", "Media_16x.png"},
            {".mov", "Media_16x.png"},
            {".qt", "Media_16x.png"},
            {".wmv", "Media_16x.png"},
            {".yuv", "Media_16x.png"},
            {".rm", "Media_16x.png"},
            {".rmvb", "Media_16x.png"},
            {".mp4", "Media_16x.png"},
            {".m4v", "Media_16x.png"},
            {".mpg", "Media_16x.png"},
            {".mp2", "Media_16x.png"},
            {".mpeg", "Media_16x.png"},
            {".mpe", "Media_16x.png"},
            {".mpv", "Media_16x.png"},
            {".m2v", "Media_16x.png"},
            {".accdb", "OfficeAccess2013Logo_16x.png"},
            {".accda", "OfficeAccess2013Logo_16x.png"},
            {".accde", "OfficeAccess2013Logo_16x.png"},
            {".xsl", "XSLTTransformFile_16x.png"},
            {".xsls", "OfficeExcel2013Logo_16x.png"},
            {".ppt", "OfficePowerPoint2013Logo_16x.png"},
            {".pptx", "OfficePowerPoint2013Logo_16x.png"},
            {".mpd", "OfficeProject2013Logo_16x.png"},
            {".mpp", "OfficeProject2013Logo_16x.png"},
            {".mpt", "OfficeProject2013Logo_16x.png"},
            {".vsd", "OfficeVisio2013Logo_16x.png"},
            {".vsdx", "OfficeVisio2013Logo_16x.png"},
            {".doc", "OfficeWord2013Logo_16x.png"},
            {".docx", "OfficeWord2013Logo_16x.png"},
            {".cshtml", "Parameter_16x.png"},
            {".php", "PHPFile_16x.png"},
            {".properties", "Property_16x.png"},
            {".prop", "Property_16x.png"},
            {".props", "Property_16x.png"},
            {".pug", "PugScript_16x.png"},
            {".py", "PY_16x.png"},
            {".rb", "RB_FileSENode_16x.png"},
            {".rs", "RustFile_16x.png"},
            {".rlib", "RustFile_16x.png"},
            {".sass", "SassStyleSheet_16x.png"},
            {".scss", "SassStyleSheet_16x.png"},
            {".settings", "Settings_16x.png"},
            {".asax", "SettingsFile_16x.png"},
            {".ini", "SettingsFile_16x.png"},
            {".dlc", "SettingsFile_16x.png"},
            {".dll", "SettingsFile_16x.png"},
            {".toml", "SettingsFile_16x.png"},
            {".sitemap", "SitemapFile_16x.png"},
            {".css", "StyleSheet_16x.png"},
            {".ts", "TS_FileSENode_16x.png"},
            {".tsx", "TS_FileSENode_16x.png"},
            {".d.ts", "TS_FileSENode_16x.png"},
            {".xlsx", "Table_16x.png"},
            {".xls", "Table_16x.png"},
            {".csv", "Table_16x.png"},
            {".txt", "Text_16x.png"},
            {".rtf", "Text_16x.png"},
            {".md", "TextFile_16x.png"},
            {".md.rendered", "TextFile_16x.png"},
            {".vbproj", "VBApplication_16x.png"},
            {".vdi", "VirtualMachine_16x.png"},
            {".vbox", "VirtualMachine_16x.png"},
            {".vbox-prev", "VirtualMachine_16x.png"},
            {".vscodeignore", "VisualStudioSettingFile_16x.png"},
            {".suo", "VisualStudioSettingFile_16x.png"},
            {".sln", "VisualStudioSettingFile_16x.png"},
            {".asmx", "WebService_16x.png"},
            {".url", "WebURL_16x.png"},
            {".ascx", "WebUserControl_16x.png"},
            {".xml", "XMLFile_16x.png"},
            {".dtd", "XMLFile_16x.png"},
            {".iml", "XMLFile_16x.png"},
            {".vsixmanifest", "XMLFile_16x.png"},
            {".xquery", "XMLFile_16x.png"},
            {".xsd", "XMLSchema_16x.png"},
            {".zip", "ZipFile_16x.png"},
            {".tar", "ZipFile_16x.png"},
            {".gz", "ZipFile_16x.png"},
            {".xz", "ZipFile_16x.png"},
            {".bzip2", "ZipFile_16x.png"},
            {".7z", "ZipFile_16x.png"},
            {".rar", "ZipFile_16x.png"},
            {".tgz", "ZipFile_16x.png" }
        };

        private Dictionary<string, ImageSource> extensionCache = new Dictionary<string, ImageSource>(StringComparer.InvariantCultureIgnoreCase);

        private Dictionary<string, string> fileNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {".bowerrc", ""},
            {".esformatter", ""},
            { ".eslintrc", ""},
            {".npmignore", ""},
            {".tfignore", ""},
            {".travis.yml", ""},
            {"appveyor.yml", ""},
            {"bower.json", ""},
            {"composer.lock", ""},
            {"copying", ""},
            {"dockerfile", ""},
            {"dub.json", ""},
            {"dub.selections.json", ""},
            {"gruntfile", ""},
            {"gulpfile", ""},
            {"install", ""},
            {"mak", "MakeFile_16x.png"},
            {"make", "MakeFile_16x.png"},
            {"mk", ""},
            {"makefile", "MakeFile_16x.png"},
            {"package.json", "NodeJS_16x.png"},
            {"readme", ""},
            {"readme.md", ""},
            {"releases", ""},
            {"yarn.lock", ""},
            {"androidmanifest.xml", "AndroidFile_16x.png"},
            {"license", "Certificate_16x.png"},
            {"license.md", "Certificate_16x.png"},
            {"license.md.rendered", "Certificate_16x.png"},
            {"license.txt", "Certificate_16x.png"},
            {".editorconfig", "ConfigurationFile_16x.png"},
            {"dub.sdl", "CPPATLWebService_16x.png"},
            {".gitignore", "GitLogo_16x.png"},
            {".gitconfig", "GitLogo_16x.png"},
            {".gitattributes", "GitLogo_16x.png"},
            {".gitmodules", "GitLogo_16x.png"},
            {".gitkeep", "GitLogo_16x.png"},
            {".jscsrc", "JSONScript_16x.png"},
            {".jshintrc", "JSONScript_16x.png"},
            {".babelrc", "JSONScript_16x.png"},
            {"cmakelists.txt", "MakeFile_16x.png"},
            {"gnumakefile", "MakeFile_16x.png"},
            {"web.debug.config", "NextDocument_16x.png"},
            {"web.release.config", "NextDocument_16x.png"},
            {".jshintignore", "SettingsFile_16x.png"},
            {"procfile", "SettingsFile_16x.png"},
            {".buildignore", "SettingsFile_16x.png"},
            {"tsconfig.json", "TS_FileSENode_16x.png"},
            {"tslint.json", "TS_FileSENode_16x.png" }
        };

        private Dictionary<string, ImageSource> fileNameCache = new Dictionary<string, ImageSource>(StringComparer.InvariantCultureIgnoreCase);

        private Dictionary<string, string> folderNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"bin", "Folder_Bin_16x.png" },
            {"obj", "Folder_Obj_16x.png" },
            {"properties", "Property_16x.png" },
            {"references", "Reference_16x.png" },
            {"rootFolder", "SpecialFolder_16x.png" },
            {"wwwroot", "WebFolder_16x.png" }
        };

        private Dictionary<string, ImageSource> folderNameCache = new Dictionary<string, ImageSource>(StringComparer.InvariantCultureIgnoreCase);

        private ImageSource genericFile;
        private ImageSource genericFolder;

        private ImageSource GetFromResources(string resourceName)
        {
            using (var imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Prefix + resourceName))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = imageStream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();

                return image;
            }
        }

        public FileIconProvider()
        {
            genericFile = GetFromResources("Document_16x.png");
            genericFolder = GetFromResources("Folder_16x.png");
        }

        private ImageSource FindImage(string key, Dictionary<string, ImageSource> cache, Dictionary<string, string> resources)
        {
            if (cache.ContainsKey(key))
                return cache[key];

            if (resources.ContainsKey(key) && !string.IsNullOrEmpty(resources[key]))
            {
                ImageSource result = GetFromResources(resources[key]);
                cache[key] = result;

                return result;
            }

            return null;
        }

        public ImageSource GetImageForFile(string filename)
        {
            var result = FindImage(Path.GetFileName(filename), fileNameCache, fileNames);
            if (result != null)
                return result;

            result = FindImage(Path.GetExtension(filename), extensionCache, extensions);
            if (result != null)
                return result;

            return genericFile;
        }

        public ImageSource GetImageForFolder(string folderName)
        {
            var result = FindImage(Path.GetFileName(folderName), folderNameCache, folderNames);
            if (result != null)
                return result;

            return genericFolder;
        }

        public ImageSource GenericFileIcon { get; }
        public ImageSource GenericFolderIcon { get; }
    }
}
