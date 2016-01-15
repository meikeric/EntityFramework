// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.FunctionalTests;

namespace Microsoft.Data.Entity.InMemory.FunctionalTests
{
    public class ComplexNavigationsQueryInMemoryTest : ComplexNavigationsQueryTestBase<InMemoryTestStore, ComplexNavigationsQueryInMemoryFixture>
    {
        public ComplexNavigationsQueryInMemoryTest(ComplexNavigationsQueryInMemoryFixture fixture)
            : base(fixture)
        {
        }

        public override void Select_multiple_nav_prop_reference_optional_via_DefaultIfEmpty()
        {
            base.Select_multiple_nav_prop_reference_optional_via_DefaultIfEmpty();
        }
    }
}