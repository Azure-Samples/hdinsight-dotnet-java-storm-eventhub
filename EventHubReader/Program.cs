using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SCP;
using Microsoft.SCP.Topology;
using System.Configuration;

namespace EventHubReader
{
    [Active(true)]
    class Program : TopologyDescriptor
    {
        static void Main(string[] args)
        {
        }

        /// <summary>
        /// Get a Storm topology builder
        /// </summary>
        /// <returns>A topology builder</returns>
        public ITopologyBuilder GetTopologyBuilder()
        {
            TopologyBuilder topologyBuilder = new TopologyBuilder("EventHubReader" + DateTime.Now.ToString("yyyyMMddHHmmss"));

            // Get the number of partitions in EventHub
            var eventHubPartitions = int.Parse(ConfigurationManager.AppSettings["EventHubPartitions"]);

            // Add the EvetnHubSpout to the topology using the SetEventHubSpout and EventHubSpoutConfig helper methods.
            // NOTE: These methods set the spout to read data in a String encoding.
            topologyBuilder.SetEventHubSpout(
                "EventHubSpout",
                new EventHubSpoutConfig(
                    ConfigurationManager.AppSettings["EventHubSharedAccessKeyName"],
                    ConfigurationManager.AppSettings["EventHubSharedAccessKey"],
                    ConfigurationManager.AppSettings["EventHubNamespace"],
                    ConfigurationManager.AppSettings["EventHubEntityPath"],
                    eventHubPartitions),
                eventHubPartitions);

            // Set a customized JSON Serializer to serialize a Java object (emitted by Java Spout) into JSON string
            // Here, full name of the Java JSON Serializer class is required
            List<string> javaSerializerInfo = new List<string>() { "microsoft.scp.storm.multilang.CustomizedInteropJSONSerializer" };

            // Add the logbolt to the topology
            // Use a serializer to understand data from the Java component
            topologyBuilder.SetBolt(
                typeof(LogBolt).Name,
                LogBolt.Get,
                new Dictionary<string, List<string>>(),
                eventHubPartitions,
                true
                ).
                DeclareCustomizedJavaSerializer(javaSerializerInfo).
                shuffleGrouping("EventHubSpout");

            return topologyBuilder;
        }
    }
}

