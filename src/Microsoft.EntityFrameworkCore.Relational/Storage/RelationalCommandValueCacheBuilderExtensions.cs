// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public static class RelationalCommandValueCacheBuilderExtensions
    {
        public static IRelationalCommandValueCacheBuilder Append(
            [NotNull] this IRelationalCommandValueCacheBuilder commandBuilder,
            [NotNull] object o)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));
            Check.NotNull(o, nameof(o));

            commandBuilder.Instance.Append(o);

            return commandBuilder;
        }

        public static IRelationalCommandValueCacheBuilder AppendLine([NotNull] this IRelationalCommandValueCacheBuilder commandBuilder)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.Instance.AppendLine();

            return commandBuilder;
        }

        public static IRelationalCommandValueCacheBuilder AppendLine(
            [NotNull] this IRelationalCommandValueCacheBuilder commandBuilder,
            [NotNull] object o)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));
            Check.NotNull(o, nameof(o));

            commandBuilder.Instance.AppendLine(o);

            return commandBuilder;
        }

        public static IRelationalCommandValueCacheBuilder AppendLines(
            [NotNull] this IRelationalCommandValueCacheBuilder commandBuilder,
            [NotNull] object o)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));
            Check.NotNull(o, nameof(o));

            commandBuilder.Instance.AppendLines(o);

            return commandBuilder;
        }

        public static IRelationalCommandValueCacheBuilder IncrementIndent([NotNull] this IRelationalCommandValueCacheBuilder commandBuilder)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.Instance.IncrementIndent();

            return commandBuilder;
        }

        public static IRelationalCommandValueCacheBuilder DecrementIndent([NotNull] this IRelationalCommandValueCacheBuilder commandBuilder)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.Instance.DecrementIndent();

            return commandBuilder;
        }

        public static IDisposable Indent([NotNull] this IRelationalCommandValueCacheBuilder commandBuilder)
            => Check.NotNull(commandBuilder, nameof(commandBuilder)).Instance.Indent();

        public static int GetLength([NotNull] this IRelationalCommandValueCacheBuilder commandBuilder)
            => Check.NotNull(commandBuilder, nameof(commandBuilder)).Instance.Length;

        public static IRelationalCommandValueCacheBuilder AddParameterByValue(
            [NotNull] this IRelationalCommandValueCacheBuilder commandBuilder,
            [NotNull] string invariantName,
            [NotNull] string name,
            [CanBeNull] object value)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.ParameterBuilder.AddParameterByValue(invariantName, name, value);

            return commandBuilder;
        }

        public static IRelationalCommandValueCacheBuilder AddParameterByType(
            [NotNull] this IRelationalCommandValueCacheBuilder commandBuilder,
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] Type type,
            [CanBeNull] object value)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.ParameterBuilder.AddParameterByType(invariantName, name, type, value);

            return commandBuilder;
        }

        public static IRelationalCommandValueCacheBuilder AddParameterByProperty(
            [NotNull] this IRelationalCommandValueCacheBuilder commandBuilder,
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] IProperty property,
            [CanBeNull] object value)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.ParameterBuilder.AddParameterByProperty(invariantName, name, property, value);

            return commandBuilder;
        }

        public static IRelationalCommandValueCacheBuilder AddCompositeParameter(
            [NotNull] this IRelationalCommandValueCacheBuilder commandBuilder,
            [NotNull] string invariantName,
            [NotNull] Action<IRelationalParameterValueCacheCollection> builderAction)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.ParameterBuilder.AddCompositeParameter(invariantName, builderAction);

            return commandBuilder;
        }
    }
}
