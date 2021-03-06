﻿/* Copyright 2010-2011 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents the result of GetIndexes.
    /// </summary>
    public class GetIndexesResult : IEnumerable<IndexInfo>
    {
        // private fields
        private BsonDocument[] documents;
        private IndexInfo[] indexes;

        // constructors
        /// <summary>
        /// Initializes a new instance of the GetIndexesResult class.
        /// </summary>
        /// <param name="documents">The raw documents containing the information about the indexes.</param>
        public GetIndexesResult(BsonDocument[] documents)
        {
            this.documents = documents;
            this.indexes = this.documents.Select(d => new IndexInfo(d)).ToArray();
        }

        // public operators
        /// <summary>
        /// Gets the IndexInfo at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the IndexInfo to get.</param>
        /// <returns>An IndexInfo.</returns>
        public IndexInfo this[int index]
        {
            get { return indexes[index]; }
        }

        // public properties
        /// <summary>
        /// Gets the count of indexes.
        /// </summary>
        public int Count
        {
            get { return indexes.Length; }
        }

        /// <summary>
        /// Gets the raw BSON documents containing the information about the indexes.
        /// </summary>
        public IEnumerable<BsonDocument> RawDocuments
        {
            get { return documents; }
        }

        // public methods

        // explicit interface implementations
        IEnumerator<IndexInfo> IEnumerable<IndexInfo>.GetEnumerator()
        {
            return ((IEnumerable<IndexInfo>)indexes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return indexes.GetEnumerator();
        }
    }

    /// <summary>
    /// Represents information about an index.
    /// </summary>
    public class IndexInfo
    {
        // private fields
        private BsonDocument document;

        // constructors
        /// <summary>
        /// Creates a new instance of the IndexInfo class.
        /// </summary>
        /// <param name="document">The BSON document that contains information about the index.</param>
        public IndexInfo(BsonDocument document)
        {
            this.document = document;
        }

        // public properties
        /// <summary>
        /// Gets whether the dups were dropped when the index was created.
        /// </summary>
        public bool DroppedDups
        {
            get
            {
                BsonValue value;
                if (document.TryGetValue("dropDups", out value))
                {
                    return value.ToBoolean();
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets whether the index was created in the background.
        /// </summary>
        public bool IsBackground
        {
            get
            {
                BsonValue value;
                if (document.TryGetValue("background", out value))
                {
                    return value.ToBoolean();
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets whether the index is sparse.
        /// </summary>
        public bool IsSparse
        {
            get
            {
                BsonValue value;
                if (document.TryGetValue("sparse", out value))
                {
                    return value.ToBoolean();
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets whether the index is unique.
        /// </summary>
        public bool IsUnique
        {
            get
            {
                BsonValue value;
                if (document.TryGetValue("unique", out value))
                {
                    return value.ToBoolean();
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the key of the index.
        /// </summary>
        public IndexKeysDocument Key
        {
            get
            {
                return new IndexKeysDocument(document["key"].AsBsonDocument.Elements);
            }
        }

        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        public string Name
        {
            get
            {
                return document["name"].AsString;
            }
        }

        /// <summary>
        /// Gets the namespace of the collection that the index is for.
        /// </summary>
        public string Namespace
        {
            get
            {
                return document["ns"].AsString;
            }
        }

        /// <summary>
        /// Gets the raw BSON document containing the index information.
        /// </summary>
        public BsonDocument RawDocument
        {
            get { return document; }
        }

        /// <summary>
        /// Gets the version of the index.
        /// </summary>
        public int Version
        {
            get
            {
                BsonValue value;
                if (document.TryGetValue("v", out value))
                {
                    return value.ToInt32();
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
