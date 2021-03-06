﻿using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class ShipRateTierSqlClausesTests : BaseUsingSqlServerSyntax<IShipRateTier>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="ShipRateTierDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_ShipRateTier_Base_Sql_Clause()
        {
            //// Arrange
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchShipRateTier]")
                .Where("[merchShipRateTier].[pk] = '" + key.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<ShipRateTierDto>()
                .Where<ShipRateTierDto>(x => x.Key == key);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

    }
}