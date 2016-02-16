// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class RelationalCommandValueCacheBuilder : IRelationalCommandValueCacheBuilder
    {
        private readonly ISensitiveDataLogger _logger;
        private readonly DiagnosticSource _diagnosticSource;

        private readonly IndentedStringBuilder _commandTextBuilder = new IndentedStringBuilder();

        public RelationalCommandValueCacheBuilder(
            [NotNull] ISensitiveDataLogger logger,
            [NotNull] DiagnosticSource diagnosticSource,
            [NotNull] IRelationalTypeMapper typeMapper)
        {
            Check.NotNull(logger, nameof(logger));
            Check.NotNull(diagnosticSource, nameof(diagnosticSource));
            Check.NotNull(typeMapper, nameof(typeMapper));

            _logger = logger;
            _diagnosticSource = diagnosticSource;
            ParameterBuilder = new RelationalParameterValueCacheCollection(typeMapper);
        }

        IndentedStringBuilder IInfrastructure<IndentedStringBuilder>.Instance
            => _commandTextBuilder;

        public virtual IRelationalParameterValueCacheCollection ParameterBuilder { get; }

        public virtual IRelationalCommandValueCache Build()
            => new RelationalCommandValueCache(
               _logger,
               _diagnosticSource,
               _commandTextBuilder.ToString(),
               ParameterBuilder.Parameters,
               ParameterBuilder.CachedParameterValues);

        public override string ToString() => _commandTextBuilder.ToString();
    }
}
