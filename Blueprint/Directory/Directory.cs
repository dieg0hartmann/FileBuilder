﻿using System.Collections.Generic;
using System.Text;

namespace FileBuilder
{
    public abstract class Directory : Blueprint
    {







        #region ===========- PROTECTED FIELDS -=================================================
        protected override string UnbuildPath => $"{unbuildLocation}/{unbuildName}";
        #endregion _____________________________________________________________________________









        #region ===========- PUBLIC PROPERTIES -=================================================
        public override string Location
        {
            get => FolderParent?.Path ?? null;
            
            protected set
            {
                StringBuilder _value = new StringBuilder(value);
                _value.Replace("\\", "/");
                _value.Replace("//", "/");
                _value.Replace("/", "/");
                location = _value.ToString();
            }
        }

        public override string Path => $"{Location}/{Name}" ?? null;
        
        /// <summary> List of child files. </summary>
        public List<File> ChildFileList { get; protected set; } = new List<File>();

        /// <summary> List of child folders. </summary>
        public List<Directory> ChildFolderList { get; protected set; } = new List<Directory>();
        #endregion _______________________________________________________________________________









        #region ===========- PUBLIC METHODS -=====================================================
        /// <summary>
        /// Clears the while child list (files and folders).
        /// They will all be deleted once the Root calls the Build method again.
        /// </summary>
        public void Clear()
        {
            ChildFileList.Clear();
            ChildFolderList.Clear();
        }

        /// <summary> Adds content into child list (files or folders). </summary>
        public void Add(Blueprint content)
        {
            var helper = GetHelper();

            // Does not add Root since it is already the root.
            if (content is RootBlueprint) return;

            // Adds document.
            if (content is DocumentBlueprint)
            {
                helper.AddFile(content as DocumentBlueprint);
                return;
            }

            // Adds folder.
            helper.AddFolder(content as FolderBlueprint);
        }

        /// <summary> Removes content from child list (files or folders). </summary>
        public void Remove(Blueprint content)
        {
            var helper = GetHelper();

            // Does not remove Root since it is never added anyways.
            if (content is RootBlueprint) return;

            // Removes document.
            if (content is DocumentBlueprint)
            {
                helper.RemoveFile(content as DocumentBlueprint);
                return;
            }

            // Remove folder.
            helper.RemoveFolder(content as FolderBlueprint);
        }
        #endregion _______________________________________________________________________________









        #region ===========- INTERNAL METHODS -===================================================
        internal override void CheckForExistence()
        {
            var helper = GetHelper();
            string _path = $"{Path}/";

            // if the Root does not exist, don't do anything.
            if (!helper.CheckForSelfExistence(_path)) return;

            // otherwise, check if its content exists too.
            helper.CheckForContentExistence(_path);
        }
        #endregion _______________________________________________________________________________








        private DirectoryHelpers GetHelper() => new DirectoryHelpers(this);








        #region ===========- PROTECTED METHODS -==================================================
        protected override void OnBuild()
        {
            System.IO.Directory.CreateDirectory(Path);
            GetHelper().BuildAllContent();
        }

        protected override void OnUnbuild()
        {
            System.IO.Directory.Delete(UnbuildPath, true);
            GetHelper().UnbuildAllContent();
        }

        protected void ConstructorForFolder(string name)
        {
            Name = name;
            unbuildName = name;
        }

        protected void ConstructorForRoot(string name, string location)
        {
            ConstructorForFolder(name);
            Location = location;
            unbuildLocation = location;
            CheckForExistence();
        }
        #endregion _______________________________________________________________________________
    }
}
