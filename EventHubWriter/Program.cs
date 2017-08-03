using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SCP;
using Microsoft.SCP.Topology;
using System.Configuration;

namespace EventHubWriter
{
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
            // Friendly name of the topology
            TopologyBuilder topologyBuilder = new TopologyBuilder("EventHubWriter" + DateTime.Now.ToString("yyyyMMddHHmmss"));

            // Number of partitions in Event Hub. Used for parallelization.
            int partitionCount = int.Parse(ConfigurationManager.AppSettings["EventHubPartitionCount"]);

            // Deserializer used to deserialize JSON data from C# components to java.lang.string
            List<string> javaDeserializerInfo = new List<string>()
                { "microsoft.scp.storm.multilang.CustomizedInteropJSONDeserializer", "java.lang.String" };

            // Spout that emits randomly generated JSON data
            topologyBuilder.SetSpout(
                "Spout",
                Spout.Get,
                new Dictionary<string, List<string>>()
                {
                    {Constants.DEFAULT_STREAM_ID, new List<string>(){"event"}}
                },
                partitionCount)
                    .DeclareCustomizedJavaDeserializer(javaDeserializerInfo);

            // Java construcvtor for the Event Hub Bolt
            JavaComponentConstructor constructor = JavaComponentConstructor.CreateFromClojureExpr(
                String.Format(@"(org.apache.storm.eventhubs.bolt.EventHubBolt. (org.apache.storm.eventhubs.bolt.EventHubBoltConfig. " +
                    @"""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" {5}))",
                    ConfigurationManager.AppSettings["EventHubPolicyName"],
                    ConfigurationManager.AppSettings["EventHubPolicyKey"],
                    ConfigurationManager.AppSettings["EventHubNamespace"],
                    "servicebus.windows.net",
                    ConfigurationManager.AppSettings["EventHubName"],
                    "true"));

            // Set the bolt to subscribe to data from the spout
            topologyBuilder.SetJavaBolt(
                "eventhubbolt",
                constructor,
                partitionCount)
                    .shuffleGrouping("Spout");

            // Return the topology builder
            return topologyBuilder;
        }
    }
}

