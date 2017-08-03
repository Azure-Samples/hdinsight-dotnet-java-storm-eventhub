using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.SCP;
using Microsoft.SCP.Rpc.Generated;

namespace EventHubReader
{
    public class LogBolt : ISCPBolt
    {
        Context ctx;

        /// <summary>
        /// Initialize the LogBolt
        /// </summary>
        /// <param name="ctx">SCP Context instance</param>
        public LogBolt(Context ctx)
        {
            //Save the context
            this.ctx = ctx;

            //Define input schema
            Dictionary<string, List<Type>> inputSchema = new Dictionary<string, List<Type>>();
            //The stream contains string values
            inputSchema.Add(Constants.DEFAULT_STREAM_ID, new List<Type>() { typeof(string) });

            //Declare input and output schemas. Output is null
            this.ctx.DeclareComponentSchema(new ComponentStreamSchema(inputSchema, null));
            //Use a custom deserializer. This matches the one declared in EventHubsReaderTopology.cs
            this.ctx.DeclareCustomizedDeserializer(new CustomizedInteropJSONDeserializer());

        }

        /// <summary>
        /// Returns a new instance of the bolt
        /// </summary>
        /// <param name="ctx">SCP Context instance</param>
        /// <param name="parms">Parameters to initialize this spout/bolt</param>
        /// <returns></returns>
        public static LogBolt Get(Context ctx, Dictionary<string, Object> parms)
        {
            return new LogBolt(ctx);
        }

        /// <summary>
        /// The Execute() function is called, when a new tuple is available.
        /// </summary>
        /// <param name="tuple"></param>
        public void Execute(SCPTuple tuple)
        {
            //Get the string data from the tuple
            string eventValue = tuple.GetString(0);
            if (eventValue != null)
            {
                //Log the data
                Context.Logger.Info("Received data: " + eventValue);
                //ACK the tuple so the spout knows it was processed
                //If we don't ACK, the EventHubSpout can stop receiving; it expects ACKs
                this.ctx.Ack(tuple);
            }
        }
    }
}