namespace Dropbox.Api.Files {

    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;
    public class FileMetadata : Metadata {

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FileMetadata" /> class.</para>
        /// </summary>
        /// <param name="name">The last component of the path (including extension). This never
        /// contains a slash.</param>
        /// <param name="id">A unique identifier for the file.</param>
        /// <param name="clientModified">For files, this is the modification time set by the
        /// desktop client when the file was added to Dropbox. Since this time is not verified
        /// (the Dropbox server stores whatever the desktop client sends up), this should only
        /// be used for display purposes (such as sorting) and not, for example, to determine
        /// if a file has changed or not.</param>
        /// <param name="serverModified">The last time the file was modified on
        /// Dropbox.</param>
        /// <param name="rev">A unique identifier for the current revision of a file. This
        /// field is the same rev as elsewhere in the API and can be used to detect changes and
        /// avoid conflicts.</param>
        /// <param name="size">The file size in bytes.</param>
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
        /// <param name="hasExplicitSharedMembers">This flag will only be present if
        /// include_has_explicit_shared_members  is true in <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderAsync" /> or <see
        /// cref="Dropbox.Api.Files.Routes.FilesUserRoutes.GetMetadataAsync" />. If this  flag
        /// is present, it will be true if this file has any explicit shared  members. This is
        /// different from sharing_info in that this could be true  in the case where a file
        /// has explicit members but is not contained within  a shared folder.</param>
        /// <param name="contentHash">A hash of the file content. This field can be used to
        /// verify data integrity. For more information see our <a
        /// href="/developers/reference/content-hash">Content hash</a> page.</param>
        public FileMetadata(string name,
                            string id,
                            sys.DateTime clientModified,
                            sys.DateTime serverModified,
                            string rev,
                            ulong size,
                            string pathLower = null,
                            string pathDisplay = null,
                            string parentSharedFolderId = null,
                            bool? hasExplicitSharedMembers = null,
                            string contentHash = null)
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

            if (rev == null)
            {
                throw new sys.ArgumentNullException("rev");
            }
            if (rev.Length < 9)
            {
                throw new sys.ArgumentOutOfRangeException("rev", "Length should be at least 9");
            }
            if (!re.Regex.IsMatch(rev, @"\A(?:[0-9a-f]+)\z"))
            {
                throw new sys.ArgumentOutOfRangeException("rev", @"Value should match pattern '\A(?:[0-9a-f]+)\z'");
            }

            if (contentHash != null)
            {
                if (contentHash.Length < 64)
                {
                    throw new sys.ArgumentOutOfRangeException("contentHash", "Length should be at least 64");
                }
                if (contentHash.Length > 64)
                {
                    throw new sys.ArgumentOutOfRangeException("contentHash", "Length should be at most 64");
                }
            }

            this.Id = id;
            this.ClientModified = clientModified;
            this.ServerModified = serverModified;
            this.Rev = rev;
            this.Size = size;
            this.HasExplicitSharedMembers = hasExplicitSharedMembers;
            this.ContentHash = contentHash;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="FileMetadata" /> class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public FileMetadata()
        {
        }

        /// <summary>
        /// <para>A unique identifier for the file.</para>
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// <para>For files, this is the modification time set by the desktop client when the
        /// file was added to Dropbox. Since this time is not verified (the Dropbox server
        /// stores whatever the desktop client sends up), this should only be used for display
        /// purposes (such as sorting) and not, for example, to determine if a file has changed
        /// or not.</para>
        /// </summary>
        public sys.DateTime ClientModified { get; protected set; }

        /// <summary>
        /// <para>The last time the file was modified on Dropbox.</para>
        /// </summary>
        public sys.DateTime ServerModified { get; protected set; }

        /// <summary>
        /// <para>A unique identifier for the current revision of a file. This field is the
        /// same rev as elsewhere in the API and can be used to detect changes and avoid
        /// conflicts.</para>
        /// </summary>
        public string Rev { get; protected set; }

        /// <summary>
        /// <para>The file size in bytes.</para>
        /// </summary>
        public ulong Size { get; protected set; }

        /// <summary>
        /// <para>This flag will only be present if include_has_explicit_shared_members  is
        /// true in <see cref="Dropbox.Api.Files.Routes.FilesUserRoutes.ListFolderAsync" /> or
        /// <see cref="Dropbox.Api.Files.Routes.FilesUserRoutes.GetMetadataAsync" />. If this
        /// flag is present, it will be true if this file has any explicit shared  members.
        /// This is different from sharing_info in that this could be true  in the case where a
        /// file has explicit members but is not contained within  a shared folder.</para>
        /// </summary>
        public bool? HasExplicitSharedMembers { get; protected set; }

        /// <summary>
        /// <para>A hash of the file content. This field can be used to verify data integrity.
        /// For more information see our <a href="/developers/reference/content-hash">Content
        /// hash</a> page.</para>
        /// </summary>
        public string ContentHash { get; protected set; }
    }
}