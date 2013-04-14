namespace PhluffyFotos.Data.WindowsAzure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class PhotoAlbumDataContext : TableServiceContext
    {
        public const string AlbumTable = "Albums";
        public const string PhotoTable = "Photos";
        public const string TagTable = "Tags";
        public const string PhotoTagTable = "PhotoTags";

        private static bool initialized;
        private static object initializationLock = new object();

        private Dictionary<string, Type> resolverTypes;

        public PhotoAlbumDataContext()
            : this(DataConnectionStringHotFix.GetAccount())
        {
        }

        public PhotoAlbumDataContext(Microsoft.WindowsAzure.CloudStorageAccount account)
            : base(account.TableEndpoint.ToString(), account.Credentials)
        {
            if (!initialized)
            {
                lock (initializationLock)
                {
                    if (!initialized)
                    {
                        this.CreateTables();
                        initialized = true;
                    }
                }
            }

            // we are setting up a dictionary of types to resolve in order
            // to workaround a performance bug during serialization
            this.resolverTypes = new Dictionary<string, Type>();
            this.resolverTypes.Add(AlbumTable, typeof(AlbumRow));
            this.resolverTypes.Add(PhotoTable, typeof(PhotoRow));
            this.resolverTypes.Add(TagTable, typeof(TagRow));
            this.resolverTypes.Add(PhotoTagTable, typeof(PhotoTagRow));

            this.ResolveType = (name) =>
            {
                var parts = name.Split('.');
                if (parts.Length == 2)
                {
                    var processedKey = UppercaseFirst(parts[1]);
                    return resolverTypes[processedKey];
                }

                return null;
            };
        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public IQueryable<AlbumRow> Albums
        {
            get
            {
                return this.CreateQuery<AlbumRow>(AlbumTable);
            }
        }

        public IQueryable<PhotoRow> Photos
        {
            get
            {
                return this.CreateQuery<PhotoRow>(PhotoTable);
            }
        }

        public IQueryable<TagRow> Tags
        {
            get
            {
                return this.CreateQuery<TagRow>(TagTable);
            }
        }

        public IQueryable<PhotoTagRow> PhotoTags
        {
            get
            {
                return this.CreateQuery<PhotoTagRow>(PhotoTagTable);
            }
        }

        public void CreateTables(Type serviceContextType, string baseAddress, StorageCredentials credentials)
        {
            TableStorageExtensionMethods.CreateTablesFromModel(serviceContextType, baseAddress, credentials);
        }

        public void CreateTables()
        {
            TableStorageExtensionMethods.CreateTablesFromModel(typeof(PhotoAlbumDataContext), this.BaseUri.AbsoluteUri, this.StorageCredentials);
        }
    }

    public static class DataConnectionStringHotFix
    {
        public static CloudStorageAccount GetAccount()
        {
            //CloudStorageAccount account;
            //if (!CloudStorageAccount.TryParse("DefaultEndpointsProtocol=https;AccountName=phluffyphotostorage;AccountKey=/uE8jZ26NgLwIWYURLLNHiQgaRWfDeehEeM2gTEpnS3AFiHOW8CVbh1g+BbGAdF9olI4+Fvve7cAO01dDveQMA==", out account))
            //    throw new ApplicationException("Cannot load storage account from config");
            //return account;
            var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            if (account == null)
                throw new ApplicationException("Cannot load storage account from config");
            return account;
        }
    }
}

