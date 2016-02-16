// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public abstract class RelationalParameterCollectionBase
    {
        private readonly List<IRelationalParameter> _parameters = new List<IRelationalParameter>();

        protected RelationalParameterCollectionBase(IRelationalTypeMapper typeMapper)
        {
            Check.NotNull(typeMapper, nameof(typeMapper));

            TypeMapper = typeMapper;
        }

        public virtual IReadOnlyList<IRelationalParameter> Parameters => _parameters;

        protected virtual IRelationalTypeMapper TypeMapper { get; }

        protected virtual void AddByValue(
            [NotNull] string invariantName,
            [NotNull] string name,
            [CanBeNull] object value)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotEmpty(name, nameof(name));

            var type = value?.GetType();

            if (typeof(DbParameter).GetTypeInfo().IsAssignableFrom(type?.GetTypeInfo()))
            {
                _parameters.Add(
                    new DbParameterRelationalParameter(invariantName));
            }
            else
            {
                _parameters.Add(
                    new TypeMappedRelationalParameter(
                        Check.NotEmpty(invariantName, nameof(invariantName)),
                        Check.NotEmpty(name, nameof(name)),
                        TypeMapper.GetMappingForValue(value),
                        type?.IsNullableType()));
            }
        }

        protected virtual void AddByType(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] Type type)
            => _parameters.Add(
                new TypeMappedRelationalParameter(
                    invariantName,
                    name,
                    TypeMapper.GetMapping(type),
                    type.IsNullableType()));


        protected virtual void AddByProperty(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] IProperty property)
            => _parameters.Add(
                new TypeMappedRelationalParameter(
                    invariantName,
                    name,
                    TypeMapper.GetMapping(property),
                    property.IsNullable));

        protected virtual void AddComposite(
            [NotNull] string invariantName,
            [NotNull] IReadOnlyList<IRelationalParameter> parameters)
            => _parameters.Add(
                new CompositeRelationalParameter(
                    Check.NotNull(invariantName, nameof(invariantName)),
                    Check.NotNull(parameters, nameof(parameters))));
    }
}
