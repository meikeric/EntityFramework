// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class RelationalCommandValueCache : RelationalCommandBase, IRelationalCommandValueCache
    {
        public RelationalCommandValueCache(
            [NotNull] ISensitiveDataLogger logger,
            [NotNull] DiagnosticSource diagnosticSource,
            [NotNull] string commandText,
            [NotNull] IReadOnlyList<IRelationalParameter> parameters,
            [NotNull] IReadOnlyDictionary<string, object> parameterValues)
            : base(
                 Check.NotNull(logger, nameof(logger)),
                 Check.NotNull(diagnosticSource, nameof(diagnosticSource)),
                 Check.NotNull(commandText, nameof(commandText)),
                 Check.NotNull(parameters, nameof(parameters)))
        {
            Check.NotNull(parameterValues, nameof(parameterValues));

            ParameterValues = parameterValues;
        }

        public virtual IReadOnlyDictionary<string, object> ParameterValues { get; }

        public virtual int ExecuteNonQuery(
            IRelationalConnection connection,
            bool manageConnection = true)
            => (int)Execute(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteNonQuery),
                ParameterValues,
                openConnection: manageConnection,
                closeConnection: manageConnection);

        public virtual Task<int> ExecuteNonQueryAsync(
            IRelationalConnection connection,
            bool manageConnection = true,
            CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteNonQuery),
                ParameterValues,
                openConnection: manageConnection,
                closeConnection: manageConnection,
                cancellationToken: cancellationToken).Cast<object, int>();

        public virtual object ExecuteScalar(
            IRelationalConnection connection,
            bool manageConnection = true)
            => Execute(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteScalar),
                ParameterValues,
                openConnection: manageConnection,
                closeConnection: manageConnection);

        public virtual Task<object> ExecuteScalarAsync(
            IRelationalConnection connection,
            bool manageConnection = true,
            CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteScalar),
                ParameterValues,
                openConnection: manageConnection,
                closeConnection: manageConnection,
                cancellationToken: cancellationToken);

        public virtual RelationalDataReader ExecuteReader(
            IRelationalConnection connection,
            bool manageConnection = true)
            => (RelationalDataReader)Execute(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteReader),
                ParameterValues,
                openConnection: manageConnection,
                closeConnection: false);

        public virtual Task<RelationalDataReader> ExecuteReaderAsync(
            IRelationalConnection connection,
            bool manageConnection = true,
            CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteReader),
                ParameterValues,
                openConnection: manageConnection,
                closeConnection: false,
                cancellationToken: cancellationToken).Cast<object, RelationalDataReader>();
    }
}
