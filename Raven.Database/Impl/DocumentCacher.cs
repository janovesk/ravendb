﻿using System;
using System.Runtime.Caching;
using Raven.Json.Linq;

namespace Raven.Database.Impl
{
    public class DocumentCacher : IDocumentCacher
    {
        private readonly MemoryCache cachedSerializedDocuments = new MemoryCache(typeof(DocumentCacher).FullName + ".Cache");

        public CachedDocument GetCachedDocument(string key, Guid etag)
        {
            var cachedDocument = (CachedDocument)cachedSerializedDocuments.Get("Doc/" + key + "/" + etag);
            if (cachedDocument == null)
                return null;
            return new CachedDocument
            {
                Document = cachedDocument.Document.CreateSnapshot(),
                Metadata = cachedDocument.Metadata.CreateSnapshot()
            };
        }

        public void SetCachedDocument(string key, Guid etag, RavenJObject doc, RavenJObject metadata)
        {
            cachedSerializedDocuments["Doc/" + key + "/" + etag] = new CachedDocument
            {
                Document = new RavenJObject(doc).EnsureSnapshot(),
                Metadata = new RavenJObject(metadata).EnsureSnapshot()
            };
        }

        public void Dispose()
        {
            cachedSerializedDocuments.Dispose();
        }
    }
}