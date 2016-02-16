﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public class RelationalCommandValueCacheBuilderTest
    {
        [Fact]
        public void Builds_simple_command()
        {
            var commandBuilder = new RelationalCommandValueCacheBuilder(
                new FakeSensitiveDataLogger<RelationalCommandValueCache>(),
                new DiagnosticListener("Fake"),
                new FakeRelationalTypeMapper());

            var command = commandBuilder.Build();

            Assert.Equal("", command.CommandText);
            Assert.Equal(0, command.Parameters.Count);
        }

        [Fact]
        public void Build_command_with_parameter()
        {
            var commandBuilder = new RelationalCommandValueCacheBuilder(
                new FakeSensitiveDataLogger<RelationalCommandValueCache>(),
                new DiagnosticListener("Fake"),
                new FakeRelationalTypeMapper());

            commandBuilder.ParameterBuilder.AddParameterByValue(
                "InvariantName",
                "Name",
                "Value");

            var command = commandBuilder.Build();

            Assert.Equal("", command.CommandText);
            Assert.Equal(1, command.Parameters.Count);
            Assert.Equal("InvariantName", command.Parameters[0].InvariantName);
            Assert.Equal("Value", command.ParameterValues["InvariantName"]);
        }
    }
}
