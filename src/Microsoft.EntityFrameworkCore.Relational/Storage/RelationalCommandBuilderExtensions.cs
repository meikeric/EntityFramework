// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public static class RelationalCommandBuilderExtensions
    {
        public static IRelationalCommandBuilder Append(
            [NotNull] this IRelationalCommandBuilder commandBuilder,
            [NotNull] object o)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));
            Check.NotNull(o, nameof(o));

            commandBuilder.Instance.Append(o);

            return commandBuilder;
        }

        public static IRelationalCommandBuilder AppendLine([NotNull] this IRelationalCommandBuilder commandBuilder)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.Instance.AppendLine();

            return commandBuilder;
        }

        public static IRelationalCommandBuilder AppendLine(
            [NotNull] this IRelationalCommandBuilder commandBuilder,
            [NotNull] object o)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));
            Check.NotNull(o, nameof(o));

            commandBuilder.Instance.AppendLine(o);

            return commandBuilder;
        }

        public static IRelationalCommandBuilder AppendLines(
            [NotNull] this IRelationalCommandBuilder commandBuilder,
            [NotNull] object o)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));
            Check.NotNull(o, nameof(o));

            commandBuilder.Instance.AppendLines(o);

            return commandBuilder;
        }

        public static IRelationalCommandBuilder IncrementIndent([NotNull] this IRelationalCommandBuilder commandBuilder)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.Instance.IncrementIndent();

            return commandBuilder;
        }

        public static IRelationalCommandBuilder DecrementIndent([NotNull] this IRelationalCommandBuilder commandBuilder)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.Instance.DecrementIndent();

            return commandBuilder;
        }

        public static IDisposable Indent([NotNull] this IRelationalCommandBuilder commandBuilder)
            => Check.NotNull(commandBuilder, nameof(commandBuilder)).Instance.Indent();

        public static int GetLength([NotNull] this IRelationalCommandBuilder commandBuilder)
            => Check.NotNull(commandBuilder, nameof(commandBuilder)).Instance.Length;

        public static IRelationalCommandBuilder AddParameterByValue(
            [NotNull] this IRelationalCommandBuilder commandBuilder,
            [NotNull] string invariantName,
            [NotNull] string name,
            [CanBeNull] object value)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.ParameterBuilder.AddParameterByValue(invariantName, name, value);

            return commandBuilder;
        }

        public static IRelationalCommandBuilder AddParameterByType(
            [NotNull] this IRelationalCommandBuilder commandBuilder,
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] Type type)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.ParameterBuilder.AddParameterByType(invariantName, name, type);

            return commandBuilder;
        }

        public static IRelationalCommandBuilder AddParameterByProperty(
            [NotNull] this IRelationalCommandBuilder commandBuilder,
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] IProperty property)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.ParameterBuilder.AddParameterByProperty(invariantName, name, property);

            return commandBuilder;
        }

        public static IRelationalCommandBuilder AddCompositeParameter(
            [NotNull] this IRelationalCommandBuilder commandBuilder,
            [NotNull] string invariantName,
            [NotNull] Action<IRelationalParameterCollection> builderAction)
        {
            Check.NotNull(commandBuilder, nameof(commandBuilder));

            commandBuilder.ParameterBuilder.AddCompositeParameter(invariantName, builderAction);

            return commandBuilder;
        }
    }
}
