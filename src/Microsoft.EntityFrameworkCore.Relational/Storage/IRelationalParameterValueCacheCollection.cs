// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public interface IRelationalParameterValueCacheCollection
    {
        IReadOnlyList<IRelationalParameter> Parameters { get; }

        IReadOnlyDictionary<string, object> CachedParameterValues { get; }

        void AddParameterByValue(
            [NotNull] string invariantName,
            [NotNull] string name,
            [CanBeNull] object value);

        void AddParameterByType(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] Type type,
            [CanBeNull] object value);

        void AddParameterByProperty(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] IProperty property,
            [CanBeNull] object value);

        void AddCompositeParameter(
            [NotNull] string invariantName,
            [NotNull] Action<IRelationalParameterValueCacheCollection> collectionAction);
    }
}
