﻿using System.IO;

namespace FileBuilder
{
    public class FileBlueprint
    {
     
        #region =========== CONSTRUCTORS ===============================================================

        /// <summary> Creates a virtual file that can be edited before being really created into your local machine. </summary>
        /// <param name="location">The path the real file will be created in.</param>
        /// <param name="name">The name the real file will have.</param>
        /// <param name="extention">The extention of the file.</param>
        public FileBlueprint(string location, string name, Extention extention)
        {
            this.FileBlueprintConstructor(location, name, extention);
        }

        /// <summary> Creates a virtual file that can be edited before being really created into your local machine. </summary>
        /// <param name="folder">The Folder enum value containing the path that the real file will be created in.</param>
        /// <param name="name">The name the real file will have.</param>
        /// <param name="extention">The extention of the file.</param>
        public FileBlueprint(Folder folder, string name, Extention extention)
        {
            this.FileBlueprintConstructor(folder.ToLocationString(), name, extention);
        }

        #endregion =======================================================================================


        #region =========== PRIVATE FIELDS ===============================================================
        private string location;
        private Extention extention;
        private string fileExtentionText;
        
        // these three data hold the last saved information to correctly unbuild the old version of the file through its old Path (UnbuildPath bellow).
        // they will be updated into the Build method as the last path version saved.
        private string unbuildLocation;
        private string unbuildName;
        private string unbuildFileExtentionText;
        // called on Unbuild method;
        private string UnbuildPath => $"{unbuildLocation}/{unbuildName}.{unbuildFileExtentionText}";
        #endregion =======================================================================================


        #region =========== PUBLIC PROPERTIES ====================================================

        /// <summary> Number of copies made with GetCopy method (readonly). </summary>
        public int NumberOfCopies { get; private set; }

        /// <summary> Specifies if this file is currently mounted with the Build method (readonly). </summary>
        public bool IsFileBuilded { get; private set; }

        /// <summary> Location of the file. </summary>
        public string Location
        {
            get => location;
            set => location = value.Replace("\\", "/");
        }

        /// <summary> Name of the file. </summary>
        public string Name { get; set; }

        /// <summary> Extention type of the file. </summary>
        public Extention Extention
        {
            get => extention;
            set
            {
                extention = value;
                fileExtentionText = extention.ToExtentionString();
            }
        }

        /// <summary> Complete path of the file, including location, name and extention (readonly). </summary>
        public string Path => $"{Location}/{Name}.{fileExtentionText}";

        /// <summary> The content inside the file (readonly). </summary>
        public string Content { get; private set; } = string.Empty;

        #endregion ===============================================================================================


        #region =========== PRIVATE METHODS ==============================================================
        private void FileBlueprintConstructor(string location, string name, Extention extention)
        {
            this.Location = location;
            this.Name = name;
            this.Extention = extention;
            this.unbuildLocation = location;
            this.unbuildName = name;
            this.unbuildFileExtentionText = extention.ToExtentionString();
            CheckIfFileAlreadyExistsAndUpdateThisBlueprint();
        }
        private void CheckIfFileAlreadyExistsAndUpdateThisBlueprint()
        {
            if (File.Exists(this.Path))
            {
                // gettingthe the real file content
                StreamReader fileReader = new StreamReader(this.Path);
                string fileContent = fileReader.ReadToEnd();
                fileReader.Close();
                
                // filling the object content with the real file content
                this.AddContent(fileContent);
                
                //saying to the algorithm that the file is already built
                this.IsFileBuilded = true;

            }
        }
        #endregion =======================================================================================


        #region =========== PUBLIC METHODS ===============================================================


        /// <summary> Adds text to the Content property. </summary>
        /// <param name="content">The content string to be added.</param>
        public void AddContent(string content) => Content += content;

        /// <summary> Adds text line to the Content property. </summary>
        /// <param name="content">The content string line to be added.</param>
        public void AddContentLine(string content) {
            if (Content.Length > 0) {
                Content += $"\n{content}";
                return;
            }
            Content += content;
        }

        /// <summary> Makes Content property empty. </summary>
        public void ClearContent() => Content = "";


        /// <summary> Creates the real file or updates the existing one. </summary>
        public void Build() {
            Unbuild();
            
            // updating the UnbuildPath property through the three fields that compose it.  
            this.unbuildLocation = this.Location;
            this.unbuildName = this.Name;
            this.unbuildFileExtentionText = this.fileExtentionText;
            
            StreamWriter writer = new StreamWriter(Path, false);
            writer.Write(Content);
            writer.Close();
            IsFileBuilded = true;
        }

        /// <summary> Deletes the real file. </summary>
        /// <param name="fileBlueprint">The blueprint of the real file to be deleted.</param>
        public void Unbuild() {
            if (!IsFileBuilded) return;
            // using the update UnbuildPath to delete the last version of the file
            File.Delete(UnbuildPath);
            IsFileBuilded = false;
        }

        /// <summary> Creates another blueprint based on this. </summary>
        /// <returns>Returns a new object with the same properties.</returns>
        public FileBlueprint GetCopy() {
            NumberOfCopies++;
            string newName = $"{Name}_{NumberOfCopies}";
            FileBlueprint duplication = new FileBlueprint(Location, newName, Extention);
            duplication.AddContent(Content);
            return duplication;
        }

        #endregion =======================================================================================

    }
}
