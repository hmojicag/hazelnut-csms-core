namespace Dropbox.Api.Files {
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    /// <summary>
    /// <para>The folder metadata object</para>
    /// </summary>
    /// <seealso cref="Global::Dropbox.Api.Files.Metadata" />
    public class FolderMetadata : Metadata
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FolderMetadata" /> class.</para>
        /// </summary>
        /// <param name="name">The last component of the path (including extension). This never
        /// contains a slash.</param>
        /// <param name="id">A unique identifier for the folder.</param>
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
        /// <param name="sharedFolderId">Deprecated. Please use <paramref name="sharingInfo" />
        /// instead.</param>
        public FolderMetadata(string name,
                              string id,
                              string pathLower = null,
                              string pathDisplay = null,
                              string parentSharedFolderId = null,
                              string sharedFolderId = null)
            : base(name, pathLower, pathDisplay, parentSharedFolderId)
        {
            if (id == null)
            {
                throw new sys.ArgumentNullException("id");
            }
            if (id.Length < 1)
            {
                throw new sys.ArgumentOutOfRangeException("id", "Length should be at least 1");
            }

            if (sharedFolderId != null)
            {
                if (!re.Regex.IsMatch(sharedFolderId, @"\A(?:[-_0-9a-zA-Z:]+)\z"))
                {
                    throw new sys.ArgumentOutOfRangeException("sharedFolderId", @"Value should match pattern '\A(?:[-_0-9a-zA-Z:]+)\z'");
                }
            }

            this.Id = id;
            this.SharedFolderId = sharedFolderId;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FolderMetadata" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public FolderMetadata()
        {
        }

        /// <summary>
        /// <para>A unique identifier for the folder.</para>
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// <para>Deprecated. Please use <see cref="SharingInfo" /> instead.</para>
        /// </summary>
        public string SharedFolderId { get; protected set; }
    }
}