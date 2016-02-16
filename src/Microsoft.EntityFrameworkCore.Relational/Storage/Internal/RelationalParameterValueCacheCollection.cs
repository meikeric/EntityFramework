// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class RelationalParameterValueCacheCollection : RelationalParameterCollectionBase, IRelationalParameterValueCacheCollection
    {
        private readonly Dictionary<string, object> _parameterValues = new Dictionary<string, object>();

        public RelationalParameterValueCacheCollection([NotNull] IRelationalTypeMapper typeMapper)
            : base(Check.NotNull(typeMapper, nameof(typeMapper)))
        {
        }

        public virtual IReadOnlyDictionary<string, object> CachedParameterValues => _parameterValues;

        public virtual void AddParameterByValue(
            [NotNull] string invariantName,
            [NotNull] string name,
            [CanBeNull] object value)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotEmpty(name, nameof(name));

            base.AddByValue(invariantName, name, value);

            _parameterValues.Add(invariantName, value);
        }

        public virtual void AddParameterByType(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] Type type,
            [CanBeNull] object value)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(type, nameof(type));

            AddByType(invariantName, name, type);

            _parameterValues.Add(invariantName, value);
        }

        public virtual void AddParameterByProperty(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] IProperty property,
            [CanBeNull] object value)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(property, nameof(property));

            AddByProperty(invariantName, name, property);

            _parameterValues.Add(invariantName, value);
        }

        public virtual void AddCompositeParameter(
            [NotNull] string invariantName,
            [NotNull] Action<IRelationalParameterValueCacheCollection> collectionAction)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotNull(collectionAction, nameof(collectionAction));

            var innerCollection = new RelationalParameterValueCacheCollection(TypeMapper);

            collectionAction(innerCollection);

            if (innerCollection.Parameters.Count > 0)
            {
                AddComposite(invariantName, innerCollection.Parameters);

                var innerValues = new object[innerCollection.Parameters.Count];

                for (var index = 0; index < innerCollection.Parameters.Count; index++)
                {
                    innerValues[index] = innerCollection.CachedParameterValues[innerCollection.Parameters[index].InvariantName];
                }

                _parameterValues.Add(invariantName, innerValues);
            }
        }
    }
}
