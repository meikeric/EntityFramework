// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public class RelationalCommandValueCacheTest : RelationalCommandTestBase
    {
        protected override int ExecuteNonQuery(TestContext context)
            => CreateCommand(context).ExecuteNonQuery(context.Connection, context.ManageConnection);

        protected override object ExecuteScalar(TestContext context)
            => CreateCommand(context).ExecuteScalar(context.Connection, context.ManageConnection);

        protected override RelationalDataReader ExecuteReader(TestContext context)
            => CreateCommand(context).ExecuteReader(context.Connection, context.ManageConnection);

        protected override Task<int> ExecuteNonQueryAsync(TestContext context)
            => CreateCommand(context).ExecuteNonQueryAsync(context.Connection, context.ManageConnection);

        protected override Task<object> ExecuteScalarAsync(TestContext context)
            => CreateCommand(context).ExecuteScalarAsync(context.Connection, context.ManageConnection);

        protected override Task<RelationalDataReader> ExecuteReaderAsync(TestContext context)
            => CreateCommand(context).ExecuteReaderAsync(context.Connection, context.ManageConnection);

        private IRelationalCommandValueCache CreateCommand(TestContext context)
            => new RelationalCommandValueCache(
                context.Logger,
                context.DiagnosticSource,
                context.CommandText,
                context.Parameters,
                context.ParameterValues);
    }
}
