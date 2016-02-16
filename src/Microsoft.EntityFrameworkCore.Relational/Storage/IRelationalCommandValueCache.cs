// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public interface IRelationalCommandValueCache
    {
        string CommandText { get; }

        IReadOnlyList<IRelationalParameter> Parameters { get; }

        IReadOnlyDictionary<string, object> ParameterValues { get; }

        int ExecuteNonQuery(
            [NotNull] IRelationalConnection connection,
            bool manageConnection = true);

        Task<int> ExecuteNonQueryAsync(
            [NotNull] IRelationalConnection connection,
            bool manageConnection = true,
            CancellationToken cancellationToken = default(CancellationToken));

        object ExecuteScalar(
            [NotNull] IRelationalConnection connection,
            bool manageConnection = true);

        Task<object> ExecuteScalarAsync(
            [NotNull] IRelationalConnection connection,
            bool manageConnection = true,
            CancellationToken cancellationToken = default(CancellationToken));

        RelationalDataReader ExecuteReader(
            [NotNull] IRelationalConnection connection,
            bool manageConnection = true);

        Task<RelationalDataReader> ExecuteReaderAsync(
            [NotNull] IRelationalConnection connection,
            bool manageConnection = true,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
