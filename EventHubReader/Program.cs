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
    /// <summary>
    /// A hybrid C#/Java topology
    ///     The Java-based EventHubSpout reads from event hub
    ///     Data is then parsed by C# and written to Table Storage
    /// </summary>
    [Active(true)]
    class Program : TopologyDescriptor
    {
        static void Main(string[] args)
        {
        }

        /// <summary>
        /// Builds a topology that can be submitted to Storm on HDInsight
        /// </summary>
        /// <returns>A topology builder</returns>
        public ITopologyBuilder GetTopologyBuilder()
        {
            //The friendly name of this topology is 'EventHubReader'
            TopologyBuilder topologyBuilder = new TopologyBuilder("EventHubReader" + DateTime.Now.ToString("yyyyMMddHHmmss"));

            //Set the partition count
            var partitionCount = int.Parse(ConfigurationManager.AppSettings["EventHubPartitions"]);
            //Set the Java spout component
            topologyBuilder.SetEventHubSpout(
                "com.microsoft.eventhubs.spout.EventHubSpout",
                new EventHubSpoutConfig(
                    ConfigurationManager.AppSettings["EventHubSharedAccessKeyName"],
                    ConfigurationManager.AppSettings["EventHubSharedAccessKey"],
                    ConfigurationManager.AppSettings["EventHubNamespace"],
                    ConfigurationManager.AppSettings["EventHubEntityPath"],
                    partitionCount),
                partitionCount);


            // Use a JSON Serializer to serialize data from the Java Spout into a JSON string
            List<string> javaSerializerInfo = new List<string>() { "microsoft.scp.storm.multilang.CustomizedInteropJSONSerializer" };

            //Set the C# bolt that consumes data from the spout
            //NOTE: The EventHubSpout component requires ACK's to be returned
            //by downstream components. If not, it will stop receiveing messages
            //after the configured MaxPendingMsgsPerPartition value (default 1024).
            topologyBuilder.SetBolt(
                "Bolt",                                              //Friendly name of this component
                Bolt.Get,
                new Dictionary<string, List<string>>(),
                partitionCount,                                      //Parallelisim hint - partition count
                true).                                               //Enable ACK's, needed for the spout    
                DeclareCustomizedJavaSerializer(javaSerializerInfo). //Use the serializer when sending to the bolt
                shuffleGrouping("com.microsoft.eventhubs.spout.EventHubSpout");                    //Consume data from the spout component

            //Create a new configuration for the topology
            StormConfig config = new StormConfig();
            config.setNumWorkers(partitionCount); //Set the number of workers to partition count

            //Set the configuration for the topology
            topologyBuilder.SetTopologyConfig(config);

            return topologyBuilder;
        }
    }
}

