// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.FunctionalTests;
using Xunit;

namespace Microsoft.EntityFrameworkCore.SqlServer.FunctionalTests
{
    public class InheritanceRelationshipsQuerySqlServerTest : InheritanceRelationshipsQueryTestBase<SqlServerTestStore, InheritanceRelationshipsQuerySqlServerFixture>
    {
        public InheritanceRelationshipsQuerySqlServerTest(InheritanceRelationshipsQuerySqlServerFixture fixture)
            : base(fixture)
        {
        }

        public override void Include_reference_with_inheritance1()
        {
            base.Include_reference_with_inheritance1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
) AS [b] ON [b].[BaseParentId] = [e].[Id]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')",
                Sql);
        }

        public override void Include_reference_with_inheritance2()
        {
            base.Include_reference_with_inheritance2();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Include_reference_with_inheritance_reverse()
        {
            base.Include_reference_with_inheritance_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[BaseParentId], [e].[Discriminator], [e].[Name], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [BaseReferenceOnBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b] ON [e].[BaseParentId] = [b].[Id]
WHERE [e].[Discriminator] IN ('DerivedReferenceOnBase', 'BaseReferenceOnBase')",
                Sql);
        }

        public override void Include_self_refence_with_inheritence()
        {
            base.Include_self_refence_with_inheritence();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE [b].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
) AS [b] ON [b].[BaseId] = [e].[Id]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')",
                Sql);
        }

        public override void Include_self_refence_with_inheritence_reverse()
        {
            base.Include_self_refence_with_inheritence_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b] ON [e].[BaseId] = [b].[Id]
WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'",
                Sql);
        }

        public override void Include_reference_with_inheritance_with_filter1()
        {
            base.Include_reference_with_inheritance_with_filter1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
) AS [b] ON [b].[BaseParentId] = [e].[Id]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)",
                Sql);
        }

        public override void Include_reference_with_inheritance_with_filter2()
        {
            base.Include_reference_with_inheritance_with_filter2();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Include_reference_with_inheritance_with_filter_reverse()
        {
            base.Include_reference_with_inheritance_with_filter_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[BaseParentId], [e].[Discriminator], [e].[Name], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [BaseReferenceOnBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b] ON [e].[BaseParentId] = [b].[Id]
WHERE [e].[Discriminator] IN ('DerivedReferenceOnBase', 'BaseReferenceOnBase') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)",
                Sql);
        }

        public override void Include_reference_without_inheritance()
        {
            base.Include_reference_without_inheritance();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [r].[Id], [r].[Name], [r].[ParentId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN [ReferenceOnBase] AS [r] ON [r].[ParentId] = [e].[Id]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')",
                Sql);
        }

        public override void Include_reference_without_inheritance_reverse()
        {
            base.Include_reference_without_inheritance_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Name], [e].[ParentId], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [ReferenceOnBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b] ON [e].[ParentId] = [b].[Id]",
                Sql);
        }

        public override void Include_reference_without_inheritance_with_filter()
        {
            base.Include_reference_without_inheritance_with_filter();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [r].[Id], [r].[Name], [r].[ParentId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN [ReferenceOnBase] AS [r] ON [r].[ParentId] = [e].[Id]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)",
                Sql);
        }

        public override void Include_reference_without_inheritance_with_filter_reverse()
        {
            base.Include_reference_without_inheritance_with_filter_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Name], [e].[ParentId], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [ReferenceOnBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b] ON [e].[ParentId] = [b].[Id]
WHERE ([e].[Name] <> 'Bar') OR [e].[Name] IS NULL",
                Sql);
        }

        public override void Include_collection_with_inheritance1()
        {
            base.Include_collection_with_inheritance1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
ORDER BY [e].[Id]

SELECT [b0].[Id], [b0].[BaseParentId], [b0].[Discriminator], [b0].[Name]
FROM [BaseCollectionOnBase] AS [b0]
INNER JOIN (
    SELECT DISTINCT [e].[Id]
    FROM [BaseInheritanceRelationshipEntity] AS [e]
    WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
) AS [e0] ON [b0].[BaseParentId] = [e0].[Id]
WHERE ([b0].[Discriminator] = 'DerivedCollectionOnBase') OR ([b0].[Discriminator] = 'BaseCollectionOnBase')
ORDER BY [e0].[Id]",
                Sql);
        }

        public override void Include_collection_with_inheritance2()
        {
            base.Include_collection_with_inheritance2();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Include_collection_with_inheritance_reverse()
        {
            base.Include_collection_with_inheritance_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[BaseParentId], [e].[Discriminator], [e].[Name], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [BaseCollectionOnBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b] ON [e].[BaseParentId] = [b].[Id]
WHERE [e].[Discriminator] IN ('DerivedCollectionOnBase', 'BaseCollectionOnBase')",
                Sql);
        }

        public override void Include_collection_with_inheritance_with_filter1()
        {
            base.Include_collection_with_inheritance_with_filter1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)
ORDER BY [e].[Id]

SELECT [b0].[Id], [b0].[BaseParentId], [b0].[Discriminator], [b0].[Name]
FROM [BaseCollectionOnBase] AS [b0]
INNER JOIN (
    SELECT DISTINCT [e].[Id]
    FROM [BaseInheritanceRelationshipEntity] AS [e]
    WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)
) AS [e0] ON [b0].[BaseParentId] = [e0].[Id]
WHERE ([b0].[Discriminator] = 'DerivedCollectionOnBase') OR ([b0].[Discriminator] = 'BaseCollectionOnBase')
ORDER BY [e0].[Id]",
                Sql);
        }

        public override void Include_collection_with_inheritance_with_filter2()
        {
            base.Include_collection_with_inheritance_with_filter2();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Include_collection_with_inheritance_with_filter_reverse()
        {
            base.Include_collection_with_inheritance_with_filter_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[BaseParentId], [e].[Discriminator], [e].[Name], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [BaseCollectionOnBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b] ON [e].[BaseParentId] = [b].[Id]
WHERE [e].[Discriminator] IN ('DerivedCollectionOnBase', 'BaseCollectionOnBase') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)",
                Sql);
        }

        public override void Include_collection_without_inheritance()
        {
            base.Include_collection_without_inheritance();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
ORDER BY [e].[Id]

SELECT [c0].[Id], [c0].[Name], [c0].[ParentId]
FROM [CollectionOnBase] AS [c0]
INNER JOIN (
    SELECT DISTINCT [e].[Id]
    FROM [BaseInheritanceRelationshipEntity] AS [e]
    WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
) AS [e0] ON [c0].[ParentId] = [e0].[Id]
ORDER BY [e0].[Id]",
                Sql);
        }

        public override void Include_collection_without_inheritance_reverse()
        {
            base.Include_collection_without_inheritance_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Name], [e].[ParentId], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [CollectionOnBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b] ON [e].[ParentId] = [b].[Id]",
                Sql);
        }

        public override void Include_collection_without_inheritance_with_filter()
        {
            base.Include_collection_without_inheritance_with_filter();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)
ORDER BY [e].[Id]

SELECT [c0].[Id], [c0].[Name], [c0].[ParentId]
FROM [CollectionOnBase] AS [c0]
INNER JOIN (
    SELECT DISTINCT [e].[Id]
    FROM [BaseInheritanceRelationshipEntity] AS [e]
    WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)
) AS [e0] ON [c0].[ParentId] = [e0].[Id]
ORDER BY [e0].[Id]",
                Sql);
        }

        public override void Include_collection_without_inheritance_with_filter_reverse()
        {
            base.Include_collection_without_inheritance_with_filter_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Name], [e].[ParentId], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [CollectionOnBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b] ON [e].[ParentId] = [b].[Id]
WHERE ([e].[Name] <> 'Bar') OR [e].[Name] IS NULL",
                Sql);
        }

        public override void Include_reference_with_inheritance_on_derived1()
        {
            base.Include_reference_with_inheritance_on_derived1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
) AS [b] ON [b].[BaseParentId] = [e].[Id]
WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'",
                Sql);
        }

        public override void Include_reference_with_inheritance_on_derived2()
        {
            base.Include_reference_with_inheritance_on_derived2();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name], [b].[DerivedInheritanceRelationshipEntityId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnDerived] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnDerived') OR ([b].[Discriminator] = 'BaseReferenceOnDerived')
) AS [b] ON [b].[BaseParentId] = [e].[Id]
WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'",
                Sql);
        }

        public override void Include_reference_with_inheritance_on_derived3()
        {
            base.Include_reference_with_inheritance_on_derived3();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Include_reference_with_inheritance_on_derived4()
        {
            base.Include_reference_with_inheritance_on_derived4();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name], [b].[DerivedInheritanceRelationshipEntityId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnDerived] AS [b]
    WHERE [b].[Discriminator] = 'DerivedReferenceOnDerived'
) AS [b] ON [b].[DerivedInheritanceRelationshipEntityId] = [e].[Id]
WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'",
                Sql);
        }

        public override void Include_reference_with_inheritance_on_derived_reverse()
        {
            base.Include_reference_with_inheritance_on_derived_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[BaseParentId], [e].[Discriminator], [e].[Name], [e].[DerivedInheritanceRelationshipEntityId], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [BaseReferenceOnDerived] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE [b].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
) AS [b] ON [e].[BaseParentId] = [b].[Id]
WHERE [e].[Discriminator] IN ('DerivedReferenceOnDerived', 'BaseReferenceOnDerived')",
                Sql);
        }

        public override void Include_reference_with_inheritance_on_derived_with_filter1()
        {
            base.Include_reference_with_inheritance_on_derived_with_filter1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
) AS [b] ON [b].[BaseParentId] = [e].[Id]
WHERE ([e].[Discriminator] = 'DerivedInheritanceRelationshipEntity') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)",
                Sql);
        }

        public override void Include_reference_with_inheritance_on_derived_with_filter2()
        {
            base.Include_reference_with_inheritance_on_derived_with_filter2();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name], [b].[DerivedInheritanceRelationshipEntityId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnDerived] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnDerived') OR ([b].[Discriminator] = 'BaseReferenceOnDerived')
) AS [b] ON [b].[BaseParentId] = [e].[Id]
WHERE ([e].[Discriminator] = 'DerivedInheritanceRelationshipEntity') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)",
                Sql);
        }

        public override void Include_reference_with_inheritance_on_derived_with_filter3()
        {
            base.Include_reference_with_inheritance_on_derived_with_filter3();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Include_reference_with_inheritance_on_derived_with_filter4()
        {
            base.Include_reference_with_inheritance_on_derived_with_filter4();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name], [b].[DerivedInheritanceRelationshipEntityId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnDerived] AS [b]
    WHERE [b].[Discriminator] = 'DerivedReferenceOnDerived'
) AS [b] ON [b].[DerivedInheritanceRelationshipEntityId] = [e].[Id]
WHERE ([e].[Discriminator] = 'DerivedInheritanceRelationshipEntity') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)",
                Sql);
        }

        public override void Include_reference_with_inheritance_on_derived_with_filter_reverse()
        {
            base.Include_reference_with_inheritance_on_derived_with_filter_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[BaseParentId], [e].[Discriminator], [e].[Name], [e].[DerivedInheritanceRelationshipEntityId], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [BaseReferenceOnDerived] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE [b].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
) AS [b] ON [e].[BaseParentId] = [b].[Id]
WHERE [e].[Discriminator] IN ('DerivedReferenceOnDerived', 'BaseReferenceOnDerived') AND (([e].[Name] <> 'Bar') OR [e].[Name] IS NULL)",
                Sql);
        }

        public override void Include_reference_without_inheritance_on_derived1()
        {
            base.Include_reference_without_inheritance_on_derived1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [r].[Id], [r].[Name], [r].[ParentId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN [ReferenceOnBase] AS [r] ON [r].[ParentId] = [e].[Id]
WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'",
                Sql);
        }

        public override void Include_reference_without_inheritance_on_derived2()
        {
            base.Include_reference_without_inheritance_on_derived2();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [r].[Id], [r].[Name], [r].[ParentId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN [ReferenceOnDerived] AS [r] ON [r].[ParentId] = [e].[Id]
WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'",
                Sql);
        }

        public override void Include_reference_without_inheritance_on_derived_reverse()
        {
            base.Include_reference_without_inheritance_on_derived_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Name], [e].[ParentId], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [ReferenceOnDerived] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE [b].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
) AS [b] ON [e].[ParentId] = [b].[Id]",
                Sql);
        }

        public override void Include_collection_with_inheritance_on_derived1()
        {
            base.Include_collection_with_inheritance_on_derived1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
ORDER BY [e].[Id]

SELECT [b0].[Id], [b0].[BaseParentId], [b0].[Discriminator], [b0].[Name]
FROM [BaseCollectionOnBase] AS [b0]
INNER JOIN (
    SELECT DISTINCT [e].[Id]
    FROM [BaseInheritanceRelationshipEntity] AS [e]
    WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
) AS [e0] ON [b0].[BaseParentId] = [e0].[Id]
WHERE ([b0].[Discriminator] = 'DerivedCollectionOnBase') OR ([b0].[Discriminator] = 'BaseCollectionOnBase')
ORDER BY [e0].[Id]",
                Sql);
        }

        public override void Include_collection_with_inheritance_on_derived2()
        {
            base.Include_collection_with_inheritance_on_derived2();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
ORDER BY [e].[Id]

SELECT [b0].[Id], [b0].[Discriminator], [b0].[Name], [b0].[ParentId], [b0].[DerivedInheritanceRelationshipEntityId]
FROM [BaseCollectionOnDerived] AS [b0]
INNER JOIN (
    SELECT DISTINCT [e].[Id]
    FROM [BaseInheritanceRelationshipEntity] AS [e]
    WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
) AS [e0] ON [b0].[ParentId] = [e0].[Id]
WHERE ([b0].[Discriminator] = 'DerivedCollectionOnDerived') OR ([b0].[Discriminator] = 'BaseCollectionOnDerived')
ORDER BY [e0].[Id]",
                Sql);
        }

        public override void Include_collection_with_inheritance_on_derived3()
        {
            base.Include_collection_with_inheritance_on_derived3();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Include_collection_with_inheritance_on_derived4()
        {
            base.Include_collection_with_inheritance_on_derived4();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Include_collection_with_inheritance_on_derived_reverse()
        {
            base.Include_collection_with_inheritance_on_derived_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[ParentId], [e].[DerivedInheritanceRelationshipEntityId], [b].[Id], [b].[Discriminator], [b].[Name], [b].[BaseId]
FROM [BaseCollectionOnDerived] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseInheritanceRelationshipEntity] AS [b]
    WHERE [b].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
) AS [b] ON [e].[ParentId] = [b].[Id]
WHERE [e].[Discriminator] IN ('DerivedCollectionOnDerived', 'BaseCollectionOnDerived')",
                Sql);
        }

        public override void Nested_include_with_inheritance_reference_reference1()
        {
            base.Nested_include_with_inheritance_reference_reference1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name], [n].[Id], [n].[Discriminator], [n].[Name], [n].[ParentCollectionId], [n].[ParentReferenceId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
) AS [b] ON [b].[BaseParentId] = [e].[Id]
LEFT JOIN (
    SELECT [n].*
    FROM [NestedReferenceBase] AS [n]
    WHERE ([n].[Discriminator] = 'NestedReferenceDerived') OR ([n].[Discriminator] = 'NestedReferenceBase')
) AS [n] ON [n].[ParentReferenceId] = [b].[Id]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')",
                Sql);
        }

        public override void Nested_include_with_inheritance_reference_reference2()
        {
            base.Nested_include_with_inheritance_reference_reference2();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Nested_include_with_inheritance_reference_reference3()
        {
            base.Nested_include_with_inheritance_reference_reference3();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name], [n].[Id], [n].[Discriminator], [n].[Name], [n].[ParentCollectionId], [n].[ParentReferenceId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
) AS [b] ON [b].[BaseParentId] = [e].[Id]
LEFT JOIN (
    SELECT [n].*
    FROM [NestedReferenceBase] AS [n]
    WHERE ([n].[Discriminator] = 'NestedReferenceDerived') OR ([n].[Discriminator] = 'NestedReferenceBase')
) AS [n] ON [n].[ParentReferenceId] = [b].[Id]
WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'",
                Sql);
        }

        public override void Nested_include_with_inheritance_reference_reference4()
        {
            base.Nested_include_with_inheritance_reference_reference4();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Nested_include_with_inheritance_reference_reference_reverse()
        {
            base.Nested_include_with_inheritance_reference_reference_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[ParentCollectionId], [e].[ParentReferenceId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name], [b0].[Id], [b0].[Discriminator], [b0].[Name], [b0].[BaseId]
FROM [NestedReferenceBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
) AS [b] ON [e].[ParentReferenceId] = [b].[Id]
LEFT JOIN (
    SELECT [b0].*
    FROM [BaseInheritanceRelationshipEntity] AS [b0]
    WHERE ([b0].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b0].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b0] ON [b].[BaseParentId] = [b0].[Id]
WHERE [e].[Discriminator] IN ('NestedReferenceDerived', 'NestedReferenceBase')",
                Sql);
        }

        public override void Nested_include_with_inheritance_reference_collection1()
        {
            base.Nested_include_with_inheritance_reference_collection1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
) AS [b] ON [b].[BaseParentId] = [e].[Id]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
ORDER BY [b].[Id]

SELECT [n0].[Id], [n0].[Discriminator], [n0].[Name], [n0].[ParentCollectionId], [n0].[ParentReferenceId]
FROM [NestedCollectionBase] AS [n0]
INNER JOIN (
    SELECT DISTINCT [b].[Id]
    FROM [BaseInheritanceRelationshipEntity] AS [e]
    LEFT JOIN (
        SELECT [b].*
        FROM [BaseReferenceOnBase] AS [b]
        WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
    ) AS [b] ON [b].[BaseParentId] = [e].[Id]
    WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
) AS [b0] ON [n0].[ParentReferenceId] = [b0].[Id]
WHERE ([n0].[Discriminator] = 'NestedCollectionDerived') OR ([n0].[Discriminator] = 'NestedCollectionBase')
ORDER BY [b0].[Id]",
                Sql);
        }

        public override void Nested_include_with_inheritance_reference_collection2()
        {
            base.Nested_include_with_inheritance_reference_collection2();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Nested_include_with_inheritance_reference_collection3()
        {
            base.Nested_include_with_inheritance_reference_collection3();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name]
FROM [BaseInheritanceRelationshipEntity] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
) AS [b] ON [b].[BaseParentId] = [e].[Id]
WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
ORDER BY [b].[Id]

SELECT [n0].[Id], [n0].[Discriminator], [n0].[Name], [n0].[ParentCollectionId], [n0].[ParentReferenceId]
FROM [NestedCollectionBase] AS [n0]
INNER JOIN (
    SELECT DISTINCT [b].[Id]
    FROM [BaseInheritanceRelationshipEntity] AS [e]
    LEFT JOIN (
        SELECT [b].*
        FROM [BaseReferenceOnBase] AS [b]
        WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
    ) AS [b] ON [b].[BaseParentId] = [e].[Id]
    WHERE [e].[Discriminator] = 'DerivedInheritanceRelationshipEntity'
) AS [b0] ON [n0].[ParentReferenceId] = [b0].[Id]
WHERE ([n0].[Discriminator] = 'NestedCollectionDerived') OR ([n0].[Discriminator] = 'NestedCollectionBase')
ORDER BY [b0].[Id]",
                Sql);
        }

        public override void Nested_include_with_inheritance_reference_collection4()
        {
            base.Nested_include_with_inheritance_reference_collection4();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Nested_include_with_inheritance_reference_collection_reverse()
        {
            base.Nested_include_with_inheritance_reference_collection_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[ParentCollectionId], [e].[ParentReferenceId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name], [b0].[Id], [b0].[Discriminator], [b0].[Name], [b0].[BaseId]
FROM [NestedCollectionBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseReferenceOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedReferenceOnBase') OR ([b].[Discriminator] = 'BaseReferenceOnBase')
) AS [b] ON [e].[ParentReferenceId] = [b].[Id]
LEFT JOIN (
    SELECT [b0].*
    FROM [BaseInheritanceRelationshipEntity] AS [b0]
    WHERE ([b0].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b0].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b0] ON [b].[BaseParentId] = [b0].[Id]
WHERE [e].[Discriminator] IN ('NestedCollectionDerived', 'NestedCollectionBase')",
                Sql);
        }

        public override void Nested_include_with_inheritance_collection_reference1()
        {
            base.Nested_include_with_inheritance_collection_reference1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
ORDER BY [e].[Id]

SELECT [b0].[Id], [b0].[BaseParentId], [b0].[Discriminator], [b0].[Name], [n].[Id], [n].[Discriminator], [n].[Name], [n].[ParentCollectionId], [n].[ParentReferenceId]
FROM [BaseCollectionOnBase] AS [b0]
INNER JOIN (
    SELECT DISTINCT [e].[Id]
    FROM [BaseInheritanceRelationshipEntity] AS [e]
    WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
) AS [e0] ON [b0].[BaseParentId] = [e0].[Id]
LEFT JOIN (
    SELECT [n].*
    FROM [NestedReferenceBase] AS [n]
    WHERE ([n].[Discriminator] = 'NestedReferenceDerived') OR ([n].[Discriminator] = 'NestedReferenceBase')
) AS [n] ON [n].[ParentCollectionId] = [b0].[Id]
WHERE ([b0].[Discriminator] = 'DerivedCollectionOnBase') OR ([b0].[Discriminator] = 'BaseCollectionOnBase')
ORDER BY [e0].[Id]",
                Sql);
        }

        public override void Nested_include_with_inheritance_collection_reference2()
        {
            base.Nested_include_with_inheritance_collection_reference2();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Nested_include_with_inheritance_collection_reference3()
        {
            base.Nested_include_with_inheritance_collection_reference3();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Nested_include_with_inheritance_collection_reference4()
        {
            base.Nested_include_with_inheritance_collection_reference4();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Nested_include_with_inheritance_collection_reference_reverse()
        {
            base.Nested_include_with_inheritance_collection_reference_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[ParentCollectionId], [e].[ParentReferenceId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name], [b0].[Id], [b0].[Discriminator], [b0].[Name], [b0].[BaseId]
FROM [NestedReferenceBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseCollectionOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedCollectionOnBase') OR ([b].[Discriminator] = 'BaseCollectionOnBase')
) AS [b] ON [e].[ParentCollectionId] = [b].[Id]
LEFT JOIN (
    SELECT [b0].*
    FROM [BaseInheritanceRelationshipEntity] AS [b0]
    WHERE ([b0].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b0].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b0] ON [b].[BaseParentId] = [b0].[Id]
WHERE [e].[Discriminator] IN ('NestedReferenceDerived', 'NestedReferenceBase')",
                Sql);
        }

        public override void Nested_include_with_inheritance_collection_collection1()
        {
            base.Nested_include_with_inheritance_collection_collection1();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[BaseId]
FROM [BaseInheritanceRelationshipEntity] AS [e]
WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
ORDER BY [e].[Id]

SELECT [b0].[Id], [b0].[BaseParentId], [b0].[Discriminator], [b0].[Name]
FROM [BaseCollectionOnBase] AS [b0]
INNER JOIN (
    SELECT DISTINCT [e].[Id]
    FROM [BaseInheritanceRelationshipEntity] AS [e]
    WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
) AS [e0] ON [b0].[BaseParentId] = [e0].[Id]
WHERE ([b0].[Discriminator] = 'DerivedCollectionOnBase') OR ([b0].[Discriminator] = 'BaseCollectionOnBase')
ORDER BY [e0].[Id], [b0].[Id]

SELECT [n0].[Id], [n0].[Discriminator], [n0].[Name], [n0].[ParentCollectionId], [n0].[ParentReferenceId]
FROM [NestedCollectionBase] AS [n0]
INNER JOIN (
    SELECT DISTINCT [e0].[Id], [b0].[Id] AS [Id0]
    FROM [BaseCollectionOnBase] AS [b0]
    INNER JOIN (
        SELECT DISTINCT [e].[Id]
        FROM [BaseInheritanceRelationshipEntity] AS [e]
        WHERE [e].[Discriminator] IN ('DerivedInheritanceRelationshipEntity', 'BaseInheritanceRelationshipEntity')
    ) AS [e0] ON [b0].[BaseParentId] = [e0].[Id]
    WHERE ([b0].[Discriminator] = 'DerivedCollectionOnBase') OR ([b0].[Discriminator] = 'BaseCollectionOnBase')
) AS [b00] ON [n0].[ParentCollectionId] = [b00].[Id0]
WHERE ([n0].[Discriminator] = 'NestedCollectionDerived') OR ([n0].[Discriminator] = 'NestedCollectionBase')
ORDER BY [b00].[Id], [b00].[Id0]",
                Sql);
        }

        public override void Nested_include_with_inheritance_collection_collection2()
        {
            base.Nested_include_with_inheritance_collection_collection2();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Nested_include_with_inheritance_collection_collection3()
        {
            base.Nested_include_with_inheritance_collection_collection3();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Nested_include_with_inheritance_collection_collection4()
        {
            base.Nested_include_with_inheritance_collection_collection4();

            Assert.Equal(
                @"",
                Sql);
        }

        public override void Nested_include_with_inheritance_collection_collection_reverse()
        {
            base.Nested_include_with_inheritance_collection_collection_reverse();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Discriminator], [e].[Name], [e].[ParentCollectionId], [e].[ParentReferenceId], [b].[Id], [b].[BaseParentId], [b].[Discriminator], [b].[Name], [b0].[Id], [b0].[Discriminator], [b0].[Name], [b0].[BaseId]
FROM [NestedCollectionBase] AS [e]
LEFT JOIN (
    SELECT [b].*
    FROM [BaseCollectionOnBase] AS [b]
    WHERE ([b].[Discriminator] = 'DerivedCollectionOnBase') OR ([b].[Discriminator] = 'BaseCollectionOnBase')
) AS [b] ON [e].[ParentCollectionId] = [b].[Id]
LEFT JOIN (
    SELECT [b0].*
    FROM [BaseInheritanceRelationshipEntity] AS [b0]
    WHERE ([b0].[Discriminator] = 'DerivedInheritanceRelationshipEntity') OR ([b0].[Discriminator] = 'BaseInheritanceRelationshipEntity')
) AS [b0] ON [b].[BaseParentId] = [b0].[Id]
WHERE [e].[Discriminator] IN ('NestedCollectionDerived', 'NestedCollectionBase')",
                Sql);
        }

        public override void Nested_include_collection_reference_on_non_entity_base()
        {
            base.Nested_include_collection_reference_on_non_entity_base();

            Assert.Equal(
                @"SELECT [e].[Id], [e].[Name]
FROM [ReferencedEntity] AS [e]
ORDER BY [e].[Id]

SELECT [p0].[Id], [p0].[Name], [p0].[ReferenceId], [p0].[ReferencedEntityId], [r].[Id], [r].[Name]
FROM [PrincipalEntity] AS [p0]
INNER JOIN (
    SELECT DISTINCT [e].[Id]
    FROM [ReferencedEntity] AS [e]
) AS [e0] ON [p0].[ReferencedEntityId] = [e0].[Id]
LEFT JOIN [ReferencedEntity] AS [r] ON [p0].[ReferenceId] = [r].[Id]
ORDER BY [e0].[Id]",
                Sql);
        }

        protected override void ClearLog()
        {
        }

        private static string Sql => TestSqlLoggerFactory.Sql;
    }
}
