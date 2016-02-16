// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public abstract class RelationalCommandBase
    {
        protected RelationalCommandBase(
            [NotNull] ISensitiveDataLogger logger,
            [NotNull] DiagnosticSource diagnosticSource,
            [NotNull] string commandText,
            [NotNull] IReadOnlyList<IRelationalParameter> parameters)
        {
            Check.NotNull(logger, nameof(logger));
            Check.NotNull(diagnosticSource, nameof(diagnosticSource));
            Check.NotNull(commandText, nameof(commandText));
            Check.NotNull(parameters, nameof(parameters));

            Logger = logger;
            DiagnosticSource = diagnosticSource;
            CommandText = commandText;
            Parameters = parameters;
        }

        public virtual string CommandText { get; }

        public virtual IReadOnlyList<IRelationalParameter> Parameters { get; }

        protected virtual ISensitiveDataLogger Logger { get; }

        protected virtual DiagnosticSource DiagnosticSource { get; }

        protected virtual object Execute(
            [NotNull] IRelationalConnection connection,
            [NotNull] string executeMethod,
            [NotNull] IReadOnlyDictionary<string, object> parameterValues,
            bool openConnection,
            bool closeConnection)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotEmpty(executeMethod, nameof(executeMethod));
            Check.NotNull(parameterValues, nameof(parameterValues));

            var dbCommand = CreateCommand(connection, parameterValues);

            WriteDiagnostic(
                RelationalDiagnostics.BeforeExecuteCommand,
                dbCommand,
                executeMethod);

            object result;

            if (openConnection)
            {
                connection.Open();
            }

            Stopwatch stopwatch = null;

            try
            {
                if (Logger.IsEnabled(LogLevel.Information))
                {
                    stopwatch = Stopwatch.StartNew();
                }

                switch (executeMethod)
                {
                    case nameof(DbCommand.ExecuteNonQuery):
                        {
                            using (dbCommand)
                            {
                                result = dbCommand.ExecuteNonQuery();
                            }

                            break;
                        }
                    case nameof(DbCommand.ExecuteScalar):
                        {
                            using (dbCommand)
                            {
                                result = dbCommand.ExecuteScalar();
                            }

                            break;
                        }
                    case nameof(DbCommand.ExecuteReader):
                        {
                            try
                            {
                                result
                                    = new RelationalDataReader(
                                        openConnection ? connection : null,
                                        dbCommand,
                                        dbCommand.ExecuteReader());
                            }
                            catch
                            {
                                dbCommand.Dispose();

                                throw;
                            }

                            break;
                        }
                    default:
                        {
                            throw new NotSupportedException();
                        }
                }

                stopwatch?.Stop();

                Logger.LogCommandExecuted(dbCommand, stopwatch?.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                stopwatch?.Stop();

                Logger.LogCommandExecuted(dbCommand, stopwatch?.ElapsedMilliseconds);

                DiagnosticSource
                    .WriteCommandError(
                        dbCommand,
                        executeMethod,
                        async: false,
                        exception: exception);

                if (openConnection && !closeConnection)
                {
                    connection.Close();
                }

                throw;
            }
            finally
            {
                if (closeConnection)
                {
                    connection.Close();
                }
            }

            WriteDiagnostic(
                RelationalDiagnostics.AfterExecuteCommand,
                dbCommand,
                executeMethod);

            return result;
        }

        protected virtual async Task<object> ExecuteAsync(
            [NotNull] IRelationalConnection connection,
            [NotNull] string executeMethod,
            [CanBeNull] IReadOnlyDictionary<string, object> parameterValues,
            bool openConnection,
            bool closeConnection,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotEmpty(executeMethod, nameof(executeMethod));
            Check.NotNull(parameterValues, nameof(parameterValues));

            var dbCommand = CreateCommand(connection, parameterValues);

            WriteDiagnostic(
                RelationalDiagnostics.BeforeExecuteCommand,
                dbCommand,
                executeMethod,
                async: true);

            object result;

            if (openConnection)
            {
                await connection.OpenAsync(cancellationToken);
            }

            Stopwatch stopwatch = null;

            try
            {
                if (Logger.IsEnabled(LogLevel.Information))
                {
                    stopwatch = Stopwatch.StartNew();
                }

                switch (executeMethod)
                {
                    case nameof(DbCommand.ExecuteNonQuery):
                        {
                            using (dbCommand)
                            {
                                result = await dbCommand.ExecuteNonQueryAsync(cancellationToken);
                            }

                            break;
                        }
                    case nameof(DbCommand.ExecuteScalar):
                        {
                            using (dbCommand)
                            {
                                result = await dbCommand.ExecuteScalarAsync(cancellationToken);
                            }

                            break;
                        }
                    case nameof(DbCommand.ExecuteReader):
                        {
                            try
                            {
                                result
                                    = new RelationalDataReader(
                                        openConnection ? connection : null,
                                        dbCommand,
                                        await dbCommand.ExecuteReaderAsync(cancellationToken));
                            }
                            catch
                            {
                                dbCommand.Dispose();

                                throw;
                            }

                            break;
                        }
                    default:
                        {
                            throw new NotSupportedException();
                        }
                }

                stopwatch?.Stop();

                Logger.LogCommandExecuted(dbCommand, stopwatch?.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                stopwatch?.Stop();

                Logger.LogCommandExecuted(dbCommand, stopwatch?.ElapsedMilliseconds);

                DiagnosticSource
                    .WriteCommandError(
                        dbCommand,
                        executeMethod,
                        async: true,
                        exception: exception);

                if (openConnection && !closeConnection)
                {
                    connection.Close();
                }

                throw;
            }
            finally
            {
                if (closeConnection)
                {
                    connection.Close();
                }
            }

            WriteDiagnostic(
                RelationalDiagnostics.AfterExecuteCommand,
                dbCommand,
                executeMethod,
                async: true);

            return result;
        }

        private void WriteDiagnostic(
            string name,
            DbCommand command,
            string executeMethod,
            bool async = false)
            => DiagnosticSource.WriteCommand(
                name,
                command,
                executeMethod,
                async: async);

        private DbCommand CreateCommand(
            IRelationalConnection connection,
            IReadOnlyDictionary<string, object> parameterValues)
        {
            var command = connection.DbConnection.CreateCommand();

            command.CommandText = CommandText;

            if (connection.CurrentTransaction != null)
            {
                command.Transaction = connection.CurrentTransaction.GetDbTransaction();
            }

            if (connection.CommandTimeout != null)
            {
                command.CommandTimeout = (int)connection.CommandTimeout;
            }

            if (Parameters.Count > 0)
            {
                foreach (var parameter in Parameters)
                {
                    object parameterValue;

                    if (parameterValues.TryGetValue(parameter.InvariantName, out parameterValue))
                    {
                        parameter.AddDbParameter(command, parameterValue);
                    }
                    else
                    {
                        throw new InvalidOperationException(RelationalStrings.MissingParameterValue(parameter.InvariantName));
                    }
                }
            }

            return command;
        }

    }
}
