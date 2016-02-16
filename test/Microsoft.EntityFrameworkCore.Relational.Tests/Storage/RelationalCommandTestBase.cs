// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.FunctionalTests.TestUtilities.Xunit;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public abstract class RelationalCommandTestBase
    {
        [Fact]
        public void Configures_DbCommand()
        {
            var fakeConnection = CreateConnection();

            var context = CreateTestContext(
                commandText: "CommandText",
                connection: fakeConnection);

            ExecuteNonQuery(context);

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);

            var command = fakeConnection.DbConnections[0].DbCommands[0];

            Assert.Equal("CommandText", command.CommandText);
            Assert.Null(command.Transaction);
            Assert.Equal(FakeDbCommand.DefaultCommandTimeout, command.CommandTimeout);
        }

        [Fact]
        public void Configures_DbCommand_with_transaction()
        {
            var fakeConnection = CreateConnection();

            var relationalTransaction = fakeConnection.BeginTransaction();

            var context = CreateTestContext(connection: fakeConnection);

            ExecuteNonQuery(context);

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);

            var command = fakeConnection.DbConnections[0].DbCommands[0];

            Assert.Same(relationalTransaction.GetDbTransaction(), command.Transaction);
        }

        [Fact]
        public void Configures_DbCommand_with_timeout()
        {
            var optionsExtension = new FakeRelationalOptionsExtension
            {
                ConnectionString = ConnectionString,
                CommandTimeout = 42
            };

            var fakeConnection = CreateConnection(CreateOptions(optionsExtension));

            var context = CreateTestContext(connection: fakeConnection);

            ExecuteNonQuery(context);

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);

            var command = fakeConnection.DbConnections[0].DbCommands[0];

            Assert.Equal(42, command.CommandTimeout);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Can_ExecuteNonQuery(bool manageConnection)
        {
            var executeNonQueryCount = 0;
            var disposeCount = -1;

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeNonQuery: c =>
                    {
                        executeNonQueryCount++;
                        disposeCount = c.DisposeCount;
                        return 1;
                    }));

            var optionsExtension = new FakeRelationalOptionsExtension { Connection = fakeDbConnection };

            var options = CreateOptions(optionsExtension);

            var context = CreateTestContext(
                connection: new FakeRelationalConnection(options),
                manageConnection: manageConnection);

            var result = ExecuteNonQuery(context);

            Assert.Equal(1, result);

            var expectedCount = manageConnection ? 1 : 0;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);

            // Durring command execution
            Assert.Equal(1, executeNonQueryCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Can_ExecuteNonQueryAsync(bool manageConnection)
        {
            var executeNonQueryCount = 0;
            var disposeCount = -1;

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeNonQueryAsync: (c, ct) =>
                    {
                        executeNonQueryCount++;
                        disposeCount = c.DisposeCount;
                        return Task.FromResult(1);
                    }));

            var optionsExtension = new FakeRelationalOptionsExtension { Connection = fakeDbConnection };

            var options = CreateOptions(optionsExtension);

            var context = CreateTestContext(
                connection: new FakeRelationalConnection(options),
                manageConnection: manageConnection);

            var result = await ExecuteNonQueryAsync(context);

            Assert.Equal(1, result);

            var expectedCount = manageConnection ? 1 : 0;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);

            // Durring command execution
            Assert.Equal(1, executeNonQueryCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Can_ExecuteScalar(bool manageConnection)
        {
            var executeScalarCount = 0;
            var disposeCount = -1;

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeScalar: c =>
                    {
                        executeScalarCount++;
                        disposeCount = c.DisposeCount;
                        return "ExecuteScalar Result";
                    }));

            var optionsExtension = new FakeRelationalOptionsExtension { Connection = fakeDbConnection };

            var options = CreateOptions(optionsExtension);

            var context = CreateTestContext(
                connection: new FakeRelationalConnection(options),
                manageConnection: manageConnection);

            var result = (string)ExecuteScalar(context);

            Assert.Equal("ExecuteScalar Result", result);

            var expectedCount = manageConnection ? 1 : 0;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);

            // Durring command execution
            Assert.Equal(1, executeScalarCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Can_ExecuteScalarAsync(bool manageConnection)
        {
            var executeScalarCount = 0;
            var disposeCount = -1;

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeScalarAsync: (c, ct) =>
                    {
                        executeScalarCount++;
                        disposeCount = c.DisposeCount;
                        return Task.FromResult<object>("ExecuteScalar Result");
                    }));

            var optionsExtension = new FakeRelationalOptionsExtension { Connection = fakeDbConnection };

            var options = CreateOptions(optionsExtension);

            var context = CreateTestContext(
                connection: new FakeRelationalConnection(options),
                manageConnection: manageConnection);

            var result = (string)await ExecuteScalarAsync(context);

            Assert.Equal("ExecuteScalar Result", result);

            var expectedCount = manageConnection ? 1 : 0;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);

            // Durring command execution
            Assert.Equal(1, executeScalarCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Can_ExecuteReader(bool manageConnection)
        {
            var executeReaderCount = 0;
            var disposeCount = -1;

            var dbDataReader = new FakeDbDataReader();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeReader: (c, b) =>
                    {
                        executeReaderCount++;
                        disposeCount = c.DisposeCount;
                        return dbDataReader;
                    }));

            var optionsExtension = new FakeRelationalOptionsExtension { Connection = fakeDbConnection };

            var options = CreateOptions(optionsExtension);

            var context = CreateTestContext(
                connection: new FakeRelationalConnection(options),
                manageConnection: manageConnection);

            var result = ExecuteReader(context);

            Assert.Same(dbDataReader, result.DbDataReader);
            Assert.Equal(0, fakeDbConnection.CloseCount);

            var expectedCount = manageConnection ? 1 : 0;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);

            // Durring command execution
            Assert.Equal(1, executeReaderCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(0, dbDataReader.DisposeCount);
            Assert.Equal(0, fakeDbConnection.DbCommands[0].DisposeCount);

            // After reader dispose
            result.Dispose();
            Assert.Equal(1, dbDataReader.DisposeCount);
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);
        }


        [ConditionalTheory]
        [MonoVersionCondition(Min = "4.2.0", SkipReason = "ExecuteReaderAsync is not implemented in Mono < 4.2.0")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Can_ExecuteReaderAsync(bool manageConnection)
        {
            var executeReaderCount = 0;
            var disposeCount = -1;

            var dbDataReader = new FakeDbDataReader();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeReaderAsync: (c, b, ct) =>
                    {
                        executeReaderCount++;
                        disposeCount = c.DisposeCount;
                        return Task.FromResult<DbDataReader>(dbDataReader);
                    }));

            var optionsExtension = new FakeRelationalOptionsExtension { Connection = fakeDbConnection };

            var options = CreateOptions(optionsExtension);

            var context = CreateTestContext(
                connection: new FakeRelationalConnection(options),
                manageConnection: manageConnection);

            var result = await ExecuteReaderAsync(context);

            Assert.Same(dbDataReader, result.DbDataReader);
            Assert.Equal(0, fakeDbConnection.CloseCount);

            var expectedCount = manageConnection ? 1 : 0;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);

            // Durring command execution
            Assert.Equal(1, executeReaderCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(0, dbDataReader.DisposeCount);
            Assert.Equal(0, fakeDbConnection.DbCommands[0].DisposeCount);

            // After reader dispose
            result.Dispose();
            Assert.Equal(1, dbDataReader.DisposeCount);
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Throws_when_parameters_are_configured_and_value_is_missing(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var context = CreateTestContext(
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new RelationalTypeMapping("int", typeof(int), DbType.Int32), false),
                    new TypeMappedRelationalParameter("SecondInvariant", "SecondParameter",new RelationalTypeMapping("long", typeof(long), DbType.Int64), true),
                    new TypeMappedRelationalParameter("ThirdInvariant", "ThirdParameter", RelationalTypeMapping.NullMapping, null)
                },
                parameterValues: new Dictionary<string, object>
                {
                    { "FirstInvariant", 17 },
                    { "SecondInvariant", 18L }
                },
                connection: fakeConnection);

            if (async)
            {
                Assert.Equal(
                    RelationalStrings.MissingParameterValue("ThirdInvariant"),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async ()
                        => await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context))).Message);
            }
            else
            {
                Assert.Equal(
                    RelationalStrings.MissingParameterValue("ThirdInvariant"),
                    Assert.Throws<InvalidOperationException>(()
                        => ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context)).Message);
            }
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Configures_DbCommand_with_type_mapped_parameters(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var context = CreateTestContext(
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new RelationalTypeMapping("int", typeof(int), DbType.Int32), false),
                    new TypeMappedRelationalParameter("SecondInvariant", "SecondParameter",new RelationalTypeMapping("long", typeof(long), DbType.Int64), true),
                    new TypeMappedRelationalParameter("ThirdInvariant", "ThirdParameter", RelationalTypeMapping.NullMapping, null)
                },
                parameterValues: new Dictionary<string, object>
                {
                    { "FirstInvariant", 17 },
                    { "SecondInvariant", 18L },
                    { "ThirdInvariant", null }
                },
                connection: fakeConnection);

            if (async)
            {
                await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context);
            }
            else
            {
                ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context);
            }

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);
            Assert.Equal(3, fakeConnection.DbConnections[0].DbCommands[0].Parameters.Count);

            var parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[0];

            Assert.Equal("FirstParameter", parameter.ParameterName);
            Assert.Equal(17, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.Equal(false, parameter.IsNullable);
            Assert.Equal(DbType.Int32, parameter.DbType);

            parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[1];

            Assert.Equal("SecondParameter", parameter.ParameterName);
            Assert.Equal(18L, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.Equal(true, parameter.IsNullable);
            Assert.Equal(DbType.Int64, parameter.DbType);

            parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[2];

            Assert.Equal("ThirdParameter", parameter.ParameterName);
            Assert.Equal(DBNull.Value, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.Equal(FakeDbParameter.DefaultDbType, parameter.DbType);
        }


        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Configures_DbCommand_with_DbParameter_parameters(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var firstParameter = new FakeDbParameter { ParameterName = "FirstParameter", Value = 17, DbType = DbType.Int32 };
            var secondParameter = new FakeDbParameter { ParameterName = "SecondParameter", Value = 18L, DbType = DbType.Int64 };
            var thirdParameter = new FakeDbParameter { ParameterName = "ThirdParameter", Value = DBNull.Value};

            var context = CreateTestContext(
                parameters: new[]
                {
                    new DbParameterRelationalParameter("FirstInvariant"),
                    new DbParameterRelationalParameter("SecondInvariant"),
                    new DbParameterRelationalParameter("ThirdInvariant")
                },
                parameterValues: new Dictionary<string, object>
                {
                    { "FirstInvariant", firstParameter },
                    { "SecondInvariant", secondParameter },
                    { "ThirdInvariant", thirdParameter }
                },
                connection: fakeConnection);

            if (async)
            {
                await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context);
            }
            else
            {
                ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context);
            }

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);
            Assert.Equal(3, fakeConnection.DbConnections[0].DbCommands[0].Parameters.Count);

            Assert.Equal(firstParameter, fakeConnection.DbConnections[0].DbCommands[0].Parameters[0]);
            Assert.Equal(secondParameter, fakeConnection.DbConnections[0].DbCommands[0].Parameters[1]);
            Assert.Equal(thirdParameter, fakeConnection.DbConnections[0].DbCommands[0].Parameters[2]);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Throws_when_db_parameter_parameters_are_configured_and_value_is_not_db_parameter(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var context = CreateTestContext(
                parameters: new[]
                {
                    new DbParameterRelationalParameter("FirstInvariant")
                },
                parameterValues: new Dictionary<string, object>
                {
                    { "FirstInvariant", 17 }
                },
                connection: fakeConnection);

            if (async)
            {
                Assert.Equal(
                    RelationalStrings.ParameterNotDbParameter("FirstInvariant"),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async ()
                        => await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context))).Message);
            }
            else
            {
                Assert.Equal(
                    RelationalStrings.ParameterNotDbParameter("FirstInvariant"),
                    Assert.Throws<InvalidOperationException>(()
                        => ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context)).Message);
            }
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Configures_DbCommand_with_composite_parameters(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var context = CreateTestContext(
                parameters: new[]
                {
                    new CompositeRelationalParameter(
                        "CompositeInvariant",
                        new[]
                        {
                            new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new RelationalTypeMapping("int", typeof(int), DbType.Int32), false),
                            new TypeMappedRelationalParameter("SecondInvariant", "SecondParameter",new RelationalTypeMapping("long", typeof(long), DbType.Int64), true),
                            new TypeMappedRelationalParameter("ThirdInvariant", "ThirdParameter", RelationalTypeMapping.NullMapping, null)
                        })
                },
                parameterValues: new Dictionary<string, object>
                {
                    { "CompositeInvariant", new object[] { 17, 18L, null } }
                },
                connection: fakeConnection);

            if (async)
            {
                await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context);
            }
            else
            {
                ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context);
            }

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);
            Assert.Equal(3, fakeConnection.DbConnections[0].DbCommands[0].Parameters.Count);

            var parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[0];

            Assert.Equal("FirstParameter", parameter.ParameterName);
            Assert.Equal(17, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.Equal(false, parameter.IsNullable);
            Assert.Equal(DbType.Int32, parameter.DbType);

            parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[1];

            Assert.Equal("SecondParameter", parameter.ParameterName);
            Assert.Equal(18L, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.Equal(true, parameter.IsNullable);
            Assert.Equal(DbType.Int64, parameter.DbType);

            parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[2];

            Assert.Equal("ThirdParameter", parameter.ParameterName);
            Assert.Equal(DBNull.Value, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.Equal(FakeDbParameter.DefaultDbType, parameter.DbType);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Throws_when_composite_parameters_are_configured_and_value_is_missing(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var context = CreateTestContext(
                parameters: new[]
                {
                    new CompositeRelationalParameter(
                        "CompositeInvariant",
                        new[]
                        {
                            new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new RelationalTypeMapping("int", typeof(int), DbType.Int32), false),
                            new TypeMappedRelationalParameter("SecondInvariant", "SecondParameter",new RelationalTypeMapping("long", typeof(long), DbType.Int64), true),
                            new TypeMappedRelationalParameter("ThirdInvariant", "ThirdParameter", RelationalTypeMapping.NullMapping, null)
                        })
                },
                parameterValues: new Dictionary<string, object>
                {
                    { "CompositeInvariant", new object[] { 17, 18L} }
                },
                connection: fakeConnection);

            if (async)
            {
                Assert.Equal(
                    RelationalStrings.MissingParameterValue("ThirdInvariant"),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async ()
                        => await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context))).Message);
            }
            else
            {
                Assert.Equal(
                    RelationalStrings.MissingParameterValue("ThirdInvariant"),
                    Assert.Throws<InvalidOperationException>(()
                        => ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context)).Message);
            }
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Throws_when_composite_parameters_are_configured_and_value_is_not_object_array(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var context = CreateTestContext(
                parameters: new[]
                {
                    new CompositeRelationalParameter(
                        "CompositeInvariant",
                        new[]
                        {
                            new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new RelationalTypeMapping("int", typeof(int), DbType.Int32), false),
                        })
                },
                parameterValues: new Dictionary<string, object>
                {
                    { "CompositeInvariant", 17 }
                },
                connection: fakeConnection);

            if (async)
            {
                Assert.Equal(
                    RelationalStrings.ParameterNotObjectArray("CompositeInvariant"),
                    (await Assert.ThrowsAsync<InvalidOperationException>(async ()
                        => await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context))).Message);
            }
            else
            {
                Assert.Equal(
                    RelationalStrings.ParameterNotObjectArray("CompositeInvariant"),
                    Assert.Throws<InvalidOperationException>(()
                        => ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context)).Message);
            }
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Disposes_command_on_exception(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var exception = new InvalidOperationException();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    c => { throw exception; },
                    c => { throw exception; },
                    (c, cb) => { throw exception; },
                    (c, ct) => { throw exception; },
                    (c, ct) => { throw exception; },
                    (c, cb, ct) => { throw exception; }));

            var optionsExtension = new FakeRelationalOptionsExtension { Connection = fakeDbConnection };

            var options = CreateOptions(optionsExtension);

            var context = CreateTestContext(connection: new FakeRelationalConnection(options));

            if (async)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async ()
                        => await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context));
            }
            else
            {
                Assert.Throws<InvalidOperationException>(()
                    => ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context));
            }

            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Closes_managed_connections_on_exception(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var exception = new InvalidOperationException();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    c => { throw exception; },
                    c => { throw exception; },
                    (c, cb) => { throw exception; },
                    (c, ct) => { throw exception; },
                    (c, ct) => { throw exception; },
                    (c, cb, ct) => { throw exception; }));

            var optionsExtension = new FakeRelationalOptionsExtension { Connection = fakeDbConnection };

            var options = CreateOptions(optionsExtension);

            var context = CreateTestContext(
                connection: new FakeRelationalConnection(options),
                manageConnection: true);

            if (async)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async ()
                        => await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context));

                Assert.Equal(1, fakeDbConnection.OpenAsyncCount);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(()
                    => ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context));

                Assert.Equal(1, fakeDbConnection.OpenCount);
            }

            Assert.Equal(1, fakeDbConnection.CloseCount);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Does_not_close_unmanaged_connections_on_exception(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var exception = new InvalidOperationException();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    c => { throw exception; },
                    c => { throw exception; },
                    (c, cb) => { throw exception; },
                    (c, ct) => { throw exception; },
                    (c, ct) => { throw exception; },
                    (c, cb, ct) => { throw exception; }));

            var optionsExtension = new FakeRelationalOptionsExtension { Connection = fakeDbConnection };

            var options = CreateOptions(optionsExtension);

            var context = CreateTestContext(
                connection: new FakeRelationalConnection(options),
                manageConnection: false);

            if (async)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async ()
                        => await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context));

                Assert.Equal(0, fakeDbConnection.OpenAsyncCount);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(()
                    => ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context));

                Assert.Equal(0, fakeDbConnection.OpenCount);
            }

            Assert.Equal(0, fakeDbConnection.CloseCount);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Logs_commands_without_parameter_values(
            Delegate commandDelegate,
            string diagnosticName,
            bool async)
        {
            var options = CreateOptions();

            var log = new List<Tuple<LogLevel, string>>();

            var context = CreateTestContext(
                logger: new SensitiveDataLogger<RelationalCommandValueCache>(
                    new ListLogger<RelationalCommandValueCache>(log),
                    options),
                commandText: "Logged Command",
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new RelationalTypeMapping("int", typeof(int), DbType.Int32), false)
                },
                parameterValues: new Dictionary<string, object> { { "FirstInvariant", 17 } },
                connection: new FakeRelationalConnection(options));

            if (async)
            {
                await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context);
            }
            else
            {
                ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context);
            }

            Assert.Equal(1, log.Count);
            Assert.Equal(LogLevel.Information, log[0].Item1);
            Assert.EndsWith(
                @"[Parameters=[FirstParameter='?'], CommandType='0', CommandTimeout='30']
Logged Command",
                log[0].Item2);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Logs_commands_parameter_values(
            Delegate commandDelegate,
            string diagnosticName,
            bool async)
        {
            var optionsExtension = new FakeRelationalOptionsExtension { ConnectionString = ConnectionString };

            var options = CreateOptions(optionsExtension, logParameters: true);

            var log = new List<Tuple<LogLevel, string>>();

            var context = CreateTestContext(
                logger: new SensitiveDataLogger<RelationalCommandValueCache>(
                    new ListLogger<RelationalCommandValueCache>(log),
                    options),
                commandText: "Logged Command",
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new RelationalTypeMapping("int", typeof(int), DbType.Int32), false)
                },
                parameterValues: new Dictionary<string, object> { { "FirstInvariant", 17 } },
                connection: new FakeRelationalConnection(options));

            if (async)
            {
                await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context);
            }
            else
            {
                ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context);
            }

            Assert.Equal(2, log.Count);
            Assert.Equal(LogLevel.Warning, log[0].Item1);
            Assert.Equal(CoreStrings.SensitiveDataLoggingEnabled, log[0].Item2);

            Assert.Equal(LogLevel.Information, log[1].Item1);
            Assert.EndsWith(
                @"ms) [Parameters=[FirstParameter='17'], CommandType='0', CommandTimeout='30']
Logged Command",
                log[1].Item2);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Reports_command_diagnostic(
            Delegate commandDelegate,
            string diagnosticName,
            bool async)
        {
            var options = CreateOptions();

            var fakeConnection = new FakeRelationalConnection(options);

            var diagnostic = new List<Tuple<string, object>>();

            var context = CreateTestContext(
                diagnosticSource: new ListDiagnosticSource(diagnostic),
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new RelationalTypeMapping("int", typeof(int), DbType.Int32), false)
                },
                parameterValues: new Dictionary<string, object> { { "FirstInvariant", 17 } },
                connection: fakeConnection);

            if (async)
            {
                await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context);
            }
            else
            {
                ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context);
            }

            Assert.Equal(2, diagnostic.Count);
            Assert.Equal(RelationalDiagnostics.BeforeExecuteCommand, diagnostic[0].Item1);
            Assert.Equal(RelationalDiagnostics.AfterExecuteCommand, diagnostic[1].Item1);

            var beforeData = (RelationalDiagnosticSourceMessage)diagnostic[0].Item2;
            var afterData = (RelationalDiagnosticSourceMessage)diagnostic[1].Item2;

            Assert.Equal(fakeConnection.DbConnections[0].DbCommands[0], beforeData.Command);
            Assert.Equal(fakeConnection.DbConnections[0].DbCommands[0], afterData.Command);

            Assert.Equal(diagnosticName, beforeData.ExecuteMethod);
            Assert.Equal(diagnosticName, afterData.ExecuteMethod);

            Assert.Equal(async, beforeData.IsAsync);
            Assert.Equal(async, afterData.IsAsync);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Reports_command_diagnostic_on_exception(
            Delegate commandDelegate,
            string diagnosticName,
            bool async)
        {
            var exception = new InvalidOperationException();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    c => { throw exception; },
                    c => { throw exception; },
                    (c, cb) => { throw exception; },
                    (c, ct) => { throw exception; },
                    (c, ct) => { throw exception; },
                    (c, cb, ct) => { throw exception; }));

            var optionsExtension = new FakeRelationalOptionsExtension { Connection = fakeDbConnection };

            var options = CreateOptions(optionsExtension);

            var diagnostic = new List<Tuple<string, object>>();

            var context = CreateTestContext(
                diagnosticSource: new ListDiagnosticSource(diagnostic),
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new RelationalTypeMapping("int", typeof(int), DbType.Int32), false)
                },
                parameterValues: new Dictionary<string, object> { { "FirstInvariant", 17 } },
                connection: new FakeRelationalConnection(options));

            if (async)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async ()
                        => await ((Func<RelationalCommandTestBase, TestContext, Task>)commandDelegate)(this, context));
            }
            else
            {
                Assert.Throws<InvalidOperationException>(()
                    => ((Action<RelationalCommandTestBase, TestContext>)commandDelegate)(this, context));
            }

            Assert.Equal(2, diagnostic.Count);
            Assert.Equal(RelationalDiagnostics.BeforeExecuteCommand, diagnostic[0].Item1);
            Assert.Equal(RelationalDiagnostics.CommandExecutionError, diagnostic[1].Item1);

            var beforeData = (RelationalDiagnosticSourceMessage)diagnostic[0].Item2;
            var afterData = (RelationalDiagnosticSourceMessage)diagnostic[1].Item2;

            Assert.Equal(fakeDbConnection.DbCommands[0], beforeData.Command);
            Assert.Equal(fakeDbConnection.DbCommands[0], afterData.Command);

            Assert.Equal(diagnosticName, beforeData.ExecuteMethod);
            Assert.Equal(diagnosticName, afterData.ExecuteMethod);

            Assert.Equal(async, beforeData.IsAsync);
            Assert.Equal(async, afterData.IsAsync);

            Assert.Equal(exception, afterData.Exception);
        }

        protected struct TestContext
        {
            public ISensitiveDataLogger Logger;
            public DiagnosticSource DiagnosticSource;
            public string CommandText;
            public IReadOnlyList<IRelationalParameter> Parameters;
            public IReadOnlyDictionary<string, object> ParameterValues;
            public IRelationalConnection Connection;
            public bool ManageConnection;
        }

        public static TheoryData CommandActions
            => new TheoryData<Delegate, string, bool>
                {
                    {
                        new Action<RelationalCommandTestBase, TestContext>((test, context) => test.ExecuteNonQuery(context)),
                        nameof(DbCommand.ExecuteNonQuery),
                        false
                    },
                    {
                        new Action<RelationalCommandTestBase, TestContext>((test, context) => test.ExecuteScalar(context)),
                        nameof(DbCommand.ExecuteScalar),
                        false
                    },
                    {
                        new Action<RelationalCommandTestBase, TestContext>((test, context) => test.ExecuteReader(context)),
                        nameof(DbCommand.ExecuteReader),
                        false
                    },
                    {
                        new Func<RelationalCommandTestBase, TestContext, Task>((test, context) => test.ExecuteNonQueryAsync(context)),
                        nameof(DbCommand.ExecuteNonQuery),
                        true
                    },
                    {
                        new Func<RelationalCommandTestBase, TestContext, Task>((test, context) => test.ExecuteScalarAsync(context)),
                        nameof(DbCommand.ExecuteScalar),
                        true
                    },
                    {
                        new Func<RelationalCommandTestBase, TestContext, Task>((test, context) => test.ExecuteReaderAsync(context)),
                        nameof(DbCommand.ExecuteReader),
                        true
                    }
                };

        protected abstract int ExecuteNonQuery(TestContext context);

        protected abstract object ExecuteScalar(TestContext context);

        protected abstract RelationalDataReader ExecuteReader(TestContext context);

        protected abstract Task<int> ExecuteNonQueryAsync(TestContext context);

        protected abstract Task<object> ExecuteScalarAsync(TestContext context);

        protected abstract Task<RelationalDataReader> ExecuteReaderAsync(TestContext context);

        private TestContext CreateTestContext(
            ISensitiveDataLogger logger = null,
            DiagnosticSource diagnosticSource = null,
            string commandText = "Command Text",
            IReadOnlyList<IRelationalParameter> parameters = null,
            IReadOnlyDictionary<string, object> parameterValues = null,
            IRelationalConnection connection = null,
            bool manageConnection = true)
            => new TestContext
            {
                Logger = logger ?? new FakeSensitiveDataLogger<RelationalCommandValueCache>(),
                DiagnosticSource = diagnosticSource ?? new DiagnosticListener("Fake"),
                CommandText = commandText,
                Parameters = parameters ?? new IRelationalParameter[0],
                ParameterValues = parameterValues ?? new Dictionary<string, object>(),
                Connection = connection ?? new FakeRelationalConnection(CreateOptions()),
                ManageConnection = manageConnection
            };

        private const string ConnectionString = "Fake Connection String";

        private static FakeRelationalConnection CreateConnection(IDbContextOptions options = null)
            => new FakeRelationalConnection(options ?? CreateOptions());

        private static IDbContextOptions CreateOptions(
            FakeRelationalOptionsExtension optionsExtension = null, bool logParameters = false)
        {
            var optionsBuilder = new DbContextOptionsBuilder();

            if (logParameters)
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(optionsExtension ?? new FakeRelationalOptionsExtension { ConnectionString = ConnectionString });

            return optionsBuilder.Options;
        }
    }
}
