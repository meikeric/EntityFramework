﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class TypeMappedRelationalParameter : IRelationalParameter
    {
        public TypeMappedRelationalParameter(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] RelationalTypeMapping relationalTypeMapping,
            [CanBeNull] bool? nullable)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(relationalTypeMapping, nameof(relationalTypeMapping));

            InvariantName = invariantName;
            Name = name;
            RelationalTypeMapping = relationalTypeMapping;
            Nullable = nullable;
        }

        public virtual string InvariantName { get; }

        public virtual string Name { get; }

        public virtual RelationalTypeMapping RelationalTypeMapping { get; }

        public virtual bool? Nullable { get; }

        public virtual void AddDbParameter(
            [NotNull] DbCommand command,
            [CanBeNull] object value)
        {
            Check.NotNull(command, nameof(command));

            command.Parameters
                .Add(RelationalTypeMapping
                    .CreateParameter(command, Name, value, Nullable));
        }

    }
}
