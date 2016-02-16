// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class DbParameterRelationalParameter : IRelationalParameter
    {
        public DbParameterRelationalParameter(
            [NotNull] string invariantName)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));

            InvariantName = invariantName;
        }

        public virtual string InvariantName { get; }

        public virtual void AddDbParameter(
            [NotNull] DbCommand command,
            [NotNull] object value)
        {
            Check.NotNull(command, nameof(command));
            Check.NotNull(value, nameof(value));

            var parameter = value as DbParameter;

            if (parameter != null)
            {
                command.Parameters.Add(parameter);
            }
            else
            {
                throw new InvalidOperationException(RelationalStrings.ParameterNotDbParameter(InvariantName));
            }
        }
    }
}
