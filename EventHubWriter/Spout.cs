using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.SCP;
using Microsoft.SCP.Rpc.Generated;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace EventHubWriter
{
    /// <summary>
    /// A spout that generates random data
    /// and emits it as a JSON formatted string
    /// </summary>
    public class Spout : ISCPSpout
    {
        private Context ctx;
        private Random r = new Random();

        /// <summary>
        /// Constructor for the spout
        /// </summary>
        /// <param name="ctx">Topology context</param>
        public Spout(Context ctx)
        {
            // Store the context
            this.ctx = ctx;

            // Define the schema for the output stream
            Dictionary<string, List<Type>> outputSchema = new Dictionary<string, List<Type>>();
            // Output is a tuple containing a single string field
            outputSchema.Add("default", new List<Type>() { typeof(string) });
            // Declare the stream
            this.ctx.DeclareComponentSchema(new ComponentStreamSchema(null, outputSchema));
            // Use the interop JSON serializer
            this.ctx.DeclareCustomizedSerializer(new CustomizedInteropJSONSerializer());
        }

        /// <summary>
        /// Gets a new instance of this component
        /// </summary>
        /// <param name="ctx">Topology context</param>
        /// <param name="parms">Parameters</param>
        /// <returns></returns>
        public static Spout Get(Context ctx, Dictionary<string, Object> parms)
        {
            return new Spout(ctx);
        }

        /// <summary>
        /// Emit data to the stream
        /// </summary>
        /// <param name="parms"></param>
        public void NextTuple(Dictionary<string, Object> parms)
        {
            // Create a JSON object
            JObject eventData = new JObject();
            // Add some properties
            eventData.Add("deviceId", r.Next(10));
            eventData.Add("deviceValue", r.Next(100));
            // Emit it
            this.ctx.Emit(new Values(eventData.ToString(Formatting.None)));
            // Log data
            Context.Logger.Info("Spout emitting {0}", eventData.ToString(Formatting.None));
        }

        public void Ack(long seqId, Dictionary<string, Object> parms)
        {
            // not used
        }

        public void Fail(long seqId, Dictionary<string, Object> parms)
        {
            // not used
        }
    }
}