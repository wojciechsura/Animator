using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class Element : ManagedObject
    {
        protected string ResolvePath(string path)
        {
            // If path is rooted, return it without changes
            if (System.IO.Path.IsPathRooted(path))
                return path;

            // Find path relative to movie's location (if set)
            ManagedObject obj = this;
            while (obj != null && obj is not Movie)
                obj = obj.Parent;

            if (obj is Movie movie && !string.IsNullOrEmpty(movie.Path))
            {
                var directory = System.IO.Path.GetDirectoryName(movie.Path);
                var fullpath = System.IO.Path.Combine(directory, path);
                return fullpath;
            }

            // As fallback, return file in the main executable path
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
        }

        public string GetPath()
        {
            ManagedObject obj = this;
            List<string> parents = new();

            while (obj != null)
            {
                if (obj is SceneElement sceneElement)
                    parents.Add($"{sceneElement.GetType().Name} ({sceneElement.Name})");
                else
                    parents.Add($"{obj.GetType().Name}");

                obj = obj.Parent;
            }

            parents.Reverse();

            return string.Join(" -> ", parents);
        }
    }
}
