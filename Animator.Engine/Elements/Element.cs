using Animator.Engine.Base;
using Animator.Engine.Elements.Rendering;
using Animator.Engine.Elements.Types;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract partial class Element : ManagedObject
    {
        // *** Path resolution ***

        protected string ResolvePath(string path)
        {
            // If path is rooted, return it without changes
            if (System.IO.Path.IsPathRooted(path))
                return path;

            // Find path relative to movie's location (if set)
            ManagedObject obj = this;
            while (obj != null && obj is not Movie)
                obj = obj.ParentInfo?.Parent;

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
                if (obj.ParentInfo != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(obj.ParentInfo.Property.Name);
                    
                    if (obj.ParentInfo.Property is ManagedCollectionProperty collectionProperty)
                    {
                        IList items = (IList)obj.ParentInfo.Parent.GetValue(collectionProperty);
                        var index = items.IndexOf(obj);
                        if (index == -1)
                            throw new InvalidOperationException("Parenting broken: item is not in its parent's collection property!");

                        sb.Append($"[{index}]");
                    }

                    parents.Add(sb.ToString());
                }
                else
                {
                    parents.Add(obj.GetType().Name);
                }

                obj = obj.ParentInfo?.Parent;
            }

            parents.Reverse();

            return string.Join(".", parents);
        }

        public string GetHumanReadablePath()
        {
            ManagedObject obj = this;
            List<string> parents = new();

            while (obj != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(obj.GetType().Name);
                if (obj is SceneElement sceneElement)
                    sb.Append($"({sceneElement.Name})");

                sb.Append(obj.ParentInfo.Property.Name);

                if (obj.ParentInfo.Property is ManagedCollectionProperty collectionProperty)
                {
                    IList items = (IList)obj.ParentInfo.Parent.GetValue(collectionProperty);
                    var index = items.IndexOf(obj);
                    if (index == -1)
                        throw new InvalidOperationException("Parenting broken: item is not in its parent's collection property!");

                    sb.Append($"[{index}]");
                }

                parents.Add(sb.ToString());

                obj = obj.ParentInfo?.Parent;
            }

            parents.Reverse();

            return string.Join(" -> ", parents);
        }
    }
}
