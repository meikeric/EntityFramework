// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;
using Remotion.Linq.Clauses;

namespace Microsoft.Data.Entity.Query.ExpressionVisitors.Internal
{
    public abstract class EntityShaper : Shaper
    {
        protected EntityShaper(
            [NotNull] IQuerySource querySource,
            [NotNull] string entityType,
            bool trackingQuery,
            [NotNull] IKey key,
            [NotNull] Func<ValueBuffer, object> materializer,
            [NotNull] string materializerString)
            : base(querySource)
        {
            IsTrackingQuery = trackingQuery;
            EntityType = entityType;
            Key = key;
            Materializer = materializer;
            MaterializerString = materializerString;
        }

        protected virtual string EntityType { get; }
        protected virtual bool IsTrackingQuery { get; }
        protected virtual IKey Key { get; }
        protected virtual Func<ValueBuffer, object> Materializer { get; }
        protected virtual string MaterializerString { get; }
        protected virtual bool AllowNullResult { get; private set; }
        protected virtual int ValueBufferOffset { get; private set; }

        public abstract IShaper<TDerived> Cast<TDerived>() where TDerived : class;
        
        public abstract EntityShaper WithOffset(int offset);

        protected virtual EntityShaper SetOffset(int offset)
        {
            ValueBufferOffset += offset;
            AllowNullResult = true;

            return this;
        }
    }
}
