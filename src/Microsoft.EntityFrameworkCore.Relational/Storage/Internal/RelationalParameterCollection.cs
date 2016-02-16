// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class RelationalParameterCollection : RelationalParameterCollectionBase, IRelationalParameterCollection
    {
        public RelationalParameterCollection([NotNull] IRelationalTypeMapper typeMapper)
            : base(Check.NotNull(typeMapper, nameof(typeMapper)))
        {
        }

        public virtual void AddParameterByValue(
            [NotNull] string invariantName,
            [NotNull] string name,
            [CanBeNull] object value)
            => AddByValue(
                Check.NotEmpty(invariantName, nameof(invariantName)),
                Check.NotEmpty(name, nameof(name)),
                value);

        public virtual void AddParameterByType(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] Type type)
            => AddByType(
                Check.NotEmpty(invariantName, nameof(invariantName)),
                Check.NotEmpty(name, nameof(name)),
                Check.NotNull(type, nameof(type)));

        public virtual void AddParameterByProperty(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] IProperty property)
            => AddByProperty(
                Check.NotEmpty(invariantName, nameof(invariantName)),
                Check.NotEmpty(name, nameof(name)),
                Check.NotNull(property, nameof(property)));

        public virtual void AddCompositeParameter(
            [NotNull] string invariantName,
            [NotNull] Action<IRelationalParameterCollection> collectionAction)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotNull(collectionAction, nameof(collectionAction));

            var innerCollection = new RelationalParameterCollection(TypeMapper);

            collectionAction(innerCollection);

            if (innerCollection.Parameters.Count > 0)
            {
                AddComposite(invariantName, innerCollection.Parameters);
            }
        }
    }
}
