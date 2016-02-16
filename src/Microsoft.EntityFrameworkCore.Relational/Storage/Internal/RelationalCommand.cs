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
    public class RelationalCommand : RelationalCommandBase, IRelationalCommand
    {
        public RelationalCommand(
            [NotNull] ISensitiveDataLogger logger,
            [NotNull] DiagnosticSource diagnosticSource,
            [NotNull] string commandText,
            [NotNull] IReadOnlyList<IRelationalParameter> parameters)
            : base(
                 Check.NotNull(logger, nameof(logger)),
                 Check.NotNull(diagnosticSource, nameof(diagnosticSource)),
                 Check.NotNull(commandText, nameof(commandText)),
                 Check.NotNull(parameters, nameof(parameters)))
        {
        }

        public virtual int ExecuteNonQuery(
            IRelationalConnection connection,
            IReadOnlyDictionary<string, object> parameterValues,
            bool manageConnection = true)
            => (int)Execute(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteNonQuery),
                Check.NotNull(parameterValues, nameof(parameterValues)),
                openConnection: manageConnection,
                closeConnection: manageConnection);

        public virtual Task<int> ExecuteNonQueryAsync(
            IRelationalConnection connection,
            IReadOnlyDictionary<string, object> parameterValues,
            bool manageConnection = true,
            CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteNonQuery),
                Check.NotNull(parameterValues, nameof(parameterValues)),
                openConnection: manageConnection,
                closeConnection: manageConnection,
                cancellationToken: cancellationToken).Cast<object, int>();

        public virtual object ExecuteScalar(
            IRelationalConnection connection,
            IReadOnlyDictionary<string, object> parameterValues,
            bool manageConnection = true)
            => Execute(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteScalar),
                Check.NotNull(parameterValues, nameof(parameterValues)),
                openConnection: manageConnection,
                closeConnection: manageConnection);

        public virtual Task<object> ExecuteScalarAsync(
            IRelationalConnection connection,
            IReadOnlyDictionary<string, object> parameterValues,
            bool manageConnection = true,
            CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteScalar),
                Check.NotNull(parameterValues, nameof(parameterValues)),
                openConnection: manageConnection,
                closeConnection: manageConnection,
                cancellationToken: cancellationToken);

        public virtual RelationalDataReader ExecuteReader(
            IRelationalConnection connection,
            IReadOnlyDictionary<string, object> parameterValues,
            bool manageConnection = true)
            => (RelationalDataReader)Execute(
                Check.NotNull(connection, nameof(connection)),
                nameof(ExecuteReader),
                Check.NotNull(parameterValues, nameof(parameterValues)),
                openConnection: manageConnection,
                closeConnection: false);

        public virtual Task<RelationalDataReader> ExecuteReaderAsync(
            IRelationalConnection connection,
            IReadOnlyDictionary<string, object> parameterValues,
            bool manageConnection = true,
            CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync(
                    Check.NotNull(connection, nameof(connection)),
                    nameof(ExecuteReader),
                    Check.NotNull(parameterValues, nameof(parameterValues)),
                    openConnection: manageConnection,
                    closeConnection: false,
                    cancellationToken: cancellationToken).Cast<object, RelationalDataReader>();
    }
}
