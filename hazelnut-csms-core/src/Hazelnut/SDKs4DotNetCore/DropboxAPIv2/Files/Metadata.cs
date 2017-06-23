using System.Text.RegularExpressions;

namespace Dropbox.Api.Files {
    
    public class Metadata {

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="Metadata" /> class.</para>
        /// </summary>
        /// <param name="name">The last component of the path (including extension). This never
        /// contains a slash.</param>
        /// <param name="pathLower">The lowercased full path in the user's Dropbox. This always
        /// starts with a slash. This field will be null if the file or folder is not
        /// mounted.</param>
        /// <param name="pathDisplay">The cased path to be used for display purposes only. In
        /// rare instances the casing will not correctly match the user's filesystem, but this
        /// behavior will match the path provided in the Core API v1, and at least the last
        /// path component will have the correct casing. Changes to only the casing of paths
        /// won't be returned by <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderContinueAsync" />. This
        /// field will be null if the file or folder is not mounted.</param>
        /// <param name="parentSharedFolderId">Deprecated. Please use <see
        /// cref="Dropbox.Api.Files.FileSharingInfo.ParentSharedFolderId" /> or <see
        /// cref="Dropbox.Api.Files.FolderSharingInfo.ParentSharedFolderId" /> instead.</param>
        protected Metadata(string name,
                           string pathLower = null,
                           string pathDisplay = null,
                           string parentSharedFolderId = null)
        {
            if (name == null)
            {
                throw new System.ArgumentNullException("name");
            }

            if (parentSharedFolderId != null)
            {
                if (!Regex.IsMatch(parentSharedFolderId, @"\A(?:[-_0-9a-zA-Z:]+)\z"))
                {
                    throw new System.ArgumentOutOfRangeException("parentSharedFolderId", @"Value should match pattern '\A(?:[-_0-9a-zA-Z:]+)\z'");
                }
            }

            this.Name = name;
            this.PathLower = pathLower;
            this.PathDisplay = pathDisplay;
            this.ParentSharedFolderId = parentSharedFolderId;
        }

        public Metadata()
        {
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is File</para>
        /// </summary>
        public bool IsFile
        {
            get
            {
                return this is FileMetadata;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a <see cref="FileMetadata" />, or <c>null</c>.</para>
        /// </summary>
        public FileMetadata AsFile
        {
            get
            {
                return this as FileMetadata;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is Folder</para>
        /// </summary>
        public bool IsFolder
        {
            get
            {
                return this is FolderMetadata;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a <see cref="FolderMetadata" />, or <c>null</c>.</para>
        /// </summary>
        public FolderMetadata AsFolder
        {
            get
            {
                return this as FolderMetadata;
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether this instance is Deleted</para>
        /// </summary>
        public bool IsDeleted
        {
            get
            {
                return this is DeletedMetadata;
            }
        }

        /// <summary>
        /// <para>Gets this instance as a <see cref="DeletedMetadata" />, or
        /// <c>null</c>.</para>
        /// </summary>
        public DeletedMetadata AsDeleted
        {
            get
            {
                return this as DeletedMetadata;
            }
        }

        /// <summary>
        /// <para>The last component of the path (including extension). This never contains a
        /// slash.</para>
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// <para>The lowercased full path in the user's Dropbox. This always starts with a
        /// slash. This field will be null if the file or folder is not mounted.</para>
        /// </summary>
        public string PathLower { get; protected set; }

        /// <summary>
        /// <para>The cased path to be used for display purposes only. In rare instances the
        /// casing will not correctly match the user's filesystem, but this behavior will match
        /// the path provided in the Core API v1, and at least the last path component will
        /// have the correct casing. Changes to only the casing of paths won't be returned by
        /// <see cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderContinueAsync" />.
        /// This field will be null if the file or folder is not mounted.</para>
        /// </summary>
        public string PathDisplay { get; protected set; }

        /// <summary>
        /// <para>Deprecated. Please use <see
        /// cref="Dropbox.Api.Files.FileSharingInfo.ParentSharedFolderId" /> or <see
        /// cref="Dropbox.Api.Files.FolderSharingInfo.ParentSharedFolderId" /> instead.</para>
        /// </summary>
        public string ParentSharedFolderId { get; protected set; }
        
    }
}