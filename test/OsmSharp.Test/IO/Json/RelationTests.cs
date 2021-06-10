using System;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;

namespace OsmSharp.Test.IO.Json
{
    [TestFixture]
    public class RelationTests
    {
        [Test]
        public void Relation_FromJson_Empty_ShouldReturnEmptyRelation()
        {
            var r = JsonSerializer.Deserialize<Relation>("{\"type\":\"relation\"}");
            
            Assert.AreEqual(null, r.Id);
        }

        [Test]
        public void Relation_FromJson_Complete_ShouldReturnComplete()
        {
            var r = JsonSerializer.Deserialize<Relation>("{\"type\":\"relation\",\"id\":2361924,\"timestamp\":\"2021-05-03T15:06:29Z\"," +
                                                         "\"version\":67,\"changeset\":104065346,\"user\":\"pointseven\",\"uid\":4355310," +
                                                         "\"members\":[" +
                                                             "{\"type\":\"node\",\"ref\":4442684,\"role\":\"node-role\"}," +
                                                             "{\"type\":\"way\",\"ref\":211334447,\"role\":\"way-role\"}," +
                                                             "{\"type\":\"relation\",\"ref\":211334453,\"role\":\"relation-role\"}]," +
                                                         "\"tags\":" +
                                                         "  {\"distance\":\"126\"," +
                                                         "  \"network\":\"nwn\"," +
                                                         "  \"ref\":\"GR 565\"," +
                                                         "  \"route\":\"hiking\"," +
                                                         "  \"type\":\"route\"}}");
            
            Assert.AreEqual(2361924, r.Id);
            Assert.AreEqual(new DateTime(2021,05,03,15,06,29, DateTimeKind.Utc), r.TimeStamp);
            Assert.AreEqual(67, r.Version);
            Assert.AreEqual(104065346, r.ChangeSetId);
            Assert.AreEqual("pointseven", r.UserName);
            Assert.AreEqual(4355310, r.UserId);
            Assert.NotNull(r.Tags);
            Assert.AreEqual(5, r.Tags.Count);
            Assert.AreEqual("distance", r.Tags.ToArray()[0].Key);
            Assert.AreEqual("126", r.Tags.ToArray()[0].Value);
            Assert.AreEqual("network", r.Tags.ToArray()[1].Key);
            Assert.AreEqual("nwn", r.Tags.ToArray()[1].Value);
            Assert.AreEqual("ref", r.Tags.ToArray()[2].Key);
            Assert.AreEqual("GR 565", r.Tags.ToArray()[2].Value);
            Assert.AreEqual("route", r.Tags.ToArray()[3].Key);
            Assert.AreEqual("hiking", r.Tags.ToArray()[3].Value);
            Assert.AreEqual("type", r.Tags.ToArray()[4].Key);
            Assert.AreEqual("route", r.Tags.ToArray()[4].Value);
            Assert.NotNull(r.Members);
            Assert.AreEqual(3, r.Members.Length);
            Assert.AreEqual(OsmGeoType.Node, r.Members[0].Type);
            Assert.AreEqual("node-role", r.Members[0].Role);
            Assert.AreEqual(4442684, r.Members[0].Id);
            Assert.AreEqual(OsmGeoType.Way, r.Members[1].Type);
            Assert.AreEqual("way-role", r.Members[1].Role);
            Assert.AreEqual(211334447, r.Members[1].Id);
            Assert.AreEqual(OsmGeoType.Relation, r.Members[2].Type);
            Assert.AreEqual("relation-role", r.Members[2].Role);
            Assert.AreEqual(211334453, r.Members[2].Id);
        }
    }
}