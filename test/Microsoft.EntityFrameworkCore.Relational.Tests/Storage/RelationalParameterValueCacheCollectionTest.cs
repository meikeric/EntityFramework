// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public class RelationalParameterValueCacheCollectionTest
    {
        [Fact]
        public void Can_add_type_mapped_parameter_by_value()
        {
            var typeMapper = new FakeRelationalTypeMapper();

            var parameterCollection = new RelationalParameterValueCacheCollection(typeMapper);

            parameterCollection.AddParameterByValue(
                "InvariantName",
                "Name",
                17);

            Assert.Equal(1, parameterCollection.Parameters.Count);
            Assert.Equal(1, parameterCollection.CachedParameterValues.Count);

            var parameter = parameterCollection.Parameters[0] as TypeMappedRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("InvariantName", parameter.InvariantName);
            Assert.Equal(typeMapper.GetMappingForValue(17), parameter.RelationalTypeMapping);
            Assert.Equal(false, parameter.Nullable);

            Assert.Equal(17, parameterCollection.CachedParameterValues["InvariantName"]);
        }

        [Fact]
        public void Can_add_type_mapped_parameter_by_nullable_value()
        {
            var typeMapper = new FakeRelationalTypeMapper();

            var parameterCollection = new RelationalParameterValueCacheCollection(typeMapper);

            parameterCollection.AddParameterByValue(
                "InvariantName",
                "Name",
                "value");

            Assert.Equal(1, parameterCollection.Parameters.Count);
            Assert.Equal(1, parameterCollection.CachedParameterValues.Count);

            var parameter = parameterCollection.Parameters[0] as TypeMappedRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("InvariantName", parameter.InvariantName);
            Assert.Equal(typeMapper.GetMappingForValue("value"), parameter.RelationalTypeMapping);
            Assert.Equal(true, parameter.Nullable);

            Assert.Equal("value", parameterCollection.CachedParameterValues["InvariantName"]);
        }

        [Fact]
        public void Can_add_type_mapped_parameter_by_null_value()
        {
            var typeMapper = new FakeRelationalTypeMapper();

            var parameterCollection = new RelationalParameterValueCacheCollection(typeMapper);

            parameterCollection.AddParameterByValue(
                "InvariantName",
                "Name",
                null);

            Assert.Equal(1, parameterCollection.Parameters.Count);
            Assert.Equal(1, parameterCollection.CachedParameterValues.Count);

            var parameter = parameterCollection.Parameters[0] as TypeMappedRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("InvariantName", parameter.InvariantName);
            Assert.Equal(typeMapper.GetMappingForValue(null), parameter.RelationalTypeMapping);
            Assert.Null(parameter.Nullable);

            Assert.Equal(null, parameterCollection.CachedParameterValues["InvariantName"]);
        }

        [Fact]
        public void Can_add_dp_parameter_definition_by_value()
        {
            var typeMapper = new FakeRelationalTypeMapper();

            var parameterCollection = new RelationalParameterValueCacheCollection(typeMapper);

            var dbParameter = new FakeDbParameter { ParameterName = "FirstParameter", Value = 17, DbType = DbType.Int32 };

            parameterCollection.AddParameterByValue(
                "InvariantName",
                "Name",
                dbParameter);

            Assert.Equal(1, parameterCollection.Parameters.Count);
            Assert.Equal(1, parameterCollection.CachedParameterValues.Count);

            var parameter = parameterCollection.Parameters[0] as DbParameterRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("InvariantName", parameter.InvariantName);

            Assert.Equal(dbParameter, parameterCollection.CachedParameterValues["InvariantName"]);
        }

        [Fact]
        public void Can_add_type_mapped_parameter_by_type()
        {
            var typeMapper = new FakeRelationalTypeMapper();

            var parameterCollection = new RelationalParameterValueCacheCollection(typeMapper);

            parameterCollection.AddParameterByType(
                "InvariantName",
                "Name",
                typeof(int),
                17);

            Assert.Equal(1, parameterCollection.Parameters.Count);
            Assert.Equal(1, parameterCollection.CachedParameterValues.Count);

            var parameter = parameterCollection.Parameters[0] as TypeMappedRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("InvariantName", parameter.InvariantName);
            Assert.Equal(typeMapper.GetMapping(typeof(int)), parameter.RelationalTypeMapping);
            Assert.Equal(false, parameter.Nullable);

            Assert.Equal(17, parameterCollection.CachedParameterValues["InvariantName"]);
        }

        [Fact]
        public void Can_add_type_mapped_parameter_by_nullable_type()
        {
            var typeMapper = new FakeRelationalTypeMapper();

            var parameterCollection = new RelationalParameterValueCacheCollection(typeMapper);

            parameterCollection.AddParameterByType(
                "InvariantName",
                "Name",
                typeof(int?),
                17);

            Assert.Equal(1, parameterCollection.Parameters.Count);
            Assert.Equal(1, parameterCollection.CachedParameterValues.Count);

            var parameter = parameterCollection.Parameters[0] as TypeMappedRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("InvariantName", parameter.InvariantName);
            Assert.Equal(typeMapper.GetMapping(typeof(int?)), parameter.RelationalTypeMapping);
            Assert.Equal(true, parameter.Nullable);

            Assert.Equal(17, parameterCollection.CachedParameterValues["InvariantName"]);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Can_add_type_mapped_parameter_by_property(bool nullable)
        {
            var typeMapper = new FakeRelationalTypeMapper();

            var property = new Model().AddEntityType("MyType").AddProperty("MyProp", typeof(string));
            property.IsNullable = nullable;

            var parameterCollection = new RelationalParameterValueCacheCollection(typeMapper);

            parameterCollection.AddParameterByProperty(
                "InvariantName",
                "Name",
                property,
                "value");

            Assert.Equal(1, parameterCollection.Parameters.Count);
            Assert.Equal(1, parameterCollection.CachedParameterValues.Count);

            var parameter = parameterCollection.Parameters[0] as TypeMappedRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("InvariantName", parameter.InvariantName);
            Assert.Equal(typeMapper.GetMapping(property), parameter.RelationalTypeMapping);
            Assert.Equal(nullable, parameter.Nullable);

            Assert.Equal("value", parameterCollection.CachedParameterValues["InvariantName"]);
        }

        [Fact]
        public void Can_add_composite_parameter()
        {
            var typeMapper = new FakeRelationalTypeMapper();

            var parameterCollection = new RelationalParameterValueCacheCollection(typeMapper);

            parameterCollection.AddCompositeParameter(
                "CompositeInvariant",
                collection =>
                {
                    collection.AddParameterByValue(
                        "FirstInvariant",
                        "FirstName",
                        17);

                    collection.AddParameterByValue(
                        "SecondInvariant",
                        "SecondName",
                        "value");
                });

            Assert.Equal(1, parameterCollection.Parameters.Count);
            Assert.Equal(1, parameterCollection.CachedParameterValues.Count);

            var parameter = parameterCollection.Parameters[0] as CompositeRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("CompositeInvariant", parameter.InvariantName);
            Assert.Equal(2, parameter.RelationalParameters.Count);

            var compositeValue = parameterCollection.CachedParameterValues["CompositeInvariant"] as object[];

            Assert.NotNull(compositeValue);
            Assert.Equal(2, compositeValue.Length);
            Assert.Equal(17, compositeValue[0]);
            Assert.Equal("value", compositeValue[1]);
        }

        [Fact]
        public void Does_not_add_empty_composite_parameter()
        {
            var typeMapper = new FakeRelationalTypeMapper();

            var parameterCollection = new RelationalParameterValueCacheCollection(typeMapper);

            parameterCollection.AddCompositeParameter(
                "CompositeInvariant",
                collection => { });

            Assert.Equal(0, parameterCollection.Parameters.Count);
            Assert.Equal(0, parameterCollection.CachedParameterValues.Count);
        }
    }
}
