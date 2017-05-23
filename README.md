(---
services: hdinsight
platforms: dotnet,java
author: blackmist
---

# Read and write from EventHubs using a hybrid .NET/Java Storm topology

An example of using a hybrid dotnet/Java-based Apache Storm topology to work with Azure Event Hubs.

NOTE: This example requires a Storm on HDInsight cluster version 3.5. There are breaking changes 

##Prerequisites

* **Visual Studio 2013 or 2015**

* **Azure SDK for Visual Studio** - at least version 2.5. This gives you the Azure entries in Server Explorer and the ability to view things like table storage

* [Java](https://java.com). Java is used to package the topology when it is submitted to the HDInsight cluster.

    * The **JAVA_HOME** environment variable must point to the directory that contains Java.
    * The **%JAVA_HOME%/bin** directory must be in the path

* **HDInsight Tools for Visual Studio** - [http://azure.microsoft.com/en-us/documentation/articles/hdinsight-hadoop-visual-studio-tools-get-started/](http://azure.microsoft.com/en-us/documentation/articles/hdinsight-hadoop-visual-studio-tools-get-started/) has the steps to install and configure. This provides C# Storm topology templates and some utilities for working with HDInsight

* **eventhubs-storm-spout** jar file - this contains the Event Hub Spout and Bolt. Download the version at [https://github.com/hdinsight/hdinsight-storm-examples/tree/master/HDI3.5/lib](https://github.com/hdinsight/hdinsight-storm-examples/tree/master/HDI3.5/lib) for use with this example.

* **Azure Event Hub** - you need to create an Event Hub, with two policies defined - one that can send, and one that can listen. You will need the following information from the Event Hub configuration.

    * **Event Hub Name** - the name of the Event Hub you created

    * **Event Hub Namespace** - the namespace of the Azure ServiceBus that contains the Event Hub

    * **Listen policy** - the name and key of the policy that can listen to events for the Event Hub

    * **Send policy** - the name and key of the policy that can send events to the Event Hub
	
	For information on creating an Event Hub and policies, see [Get Started with Event Hubs](https://azure.microsoft.com/en-us/documentation/articles/event-hubs-csharp-ephcs-getstarted/).

* **Storm on HDInsight cluster** - the Azure HDInsight cluster that you will submit the topologies to. This example _requires_ Storm on HDInsight 3.5.

    Storm on HDInsight 3.5 includes Storm 1.0.1. There are breaking changes in the class namespace for Storm components, which cause problems if you try to use this example against earlier versions of Storm.

## SCP.NET package version

The SCP.NET package version that you use for this project depends on the version of Storm installed on your HDInsight cluster. Use SCP 1.0.#.# with this example, since it is for Storm 1.0.#, which is on HDInsight 3.5.

##Build and deploy

The two topologies in this project work together - EventHubWriter writes events, while EventHubReader reads them. You can deploy them in any order, but when both are deployed, events should flow from the Writer, to the Reader, then to Table Storage.

1. The configuration for Event Hub and Table Storage is stored in the __App.config__ for each project. Fill in the values needed to connect to Azure Event Hub and (for the reader,) Azure Table Storage. For the **TableName** entry, enter the name of the table you want events to be stored in.

2. In **Server Explorer**, right-click the solution (**EventHubExample**,) and select **Build**. This will restore any missing packages and build the project.

3. In **Server Explorer**, right-click the **Azure** entry and select **Connect to Microsoft Azure Subscription** to make sure you are connected.

4. In **Server Explorer**, right-click the project name (**EventHubWriter** or **EventHubReader**,) and select **Submit to Storm on HDInsight**.

    * Use the drop-down to select the Storm on HDInsight server

    * Expand **Additional Configurations**, select **Java File Paths**, and browse or enter the path to the directory that contains the **storm-eventhubs-1.0.2-jar-with-dependencies.jar** file.

    * Select **Submit** to submit the topology to the server

Once the topology has been submitted, the **Storm Topologies Viewer** should appear, with a list of running topologies. If not, you can open it from **Server Explorer** by expanding **Azure**, **HDInsight**, right-click the cluster name and select **View Storm Topologies**.

##View output

1. In **Storm Topologies Viewer**, select the  **EventHubWriter** topology.

2. In the graph view, double-click the __LogBolt__ component. This will open the __Component Summary__ page for the bolt.

3. In the __Executors__ section, select one of the links in the __Port__ column. This will display information logged by the component. The logged information is similar to the following:

        2016-10-20 13:26:44.186 m.s.s.b.ScpNetBolt [INFO] Processing tuple: source: com.microsoft.eventhubs.spout.EventHubSpout:7, stream: default, id: {5769732396213255808=520853934697489134}, [{"deviceId":3,"deviceValue":1379915540}]
        2016-10-20 13:26:44.234 m.s.s.b.ScpNetBolt [INFO] Processing tuple: source: com.microsoft.eventhubs.spout.EventHubSpout:7, stream: default, id: {7154038361491319965=4543766486572976404}, [{"deviceId":3,"deviceValue":459399321}]
        2016-10-20 13:26:44.335 m.s.s.b.ScpNetBolt [INFO] Processing tuple: source: com.microsoft.eventhubs.spout.EventHubSpout:6, stream: default, id: {513308780877039680=-7571211415704099042}, [{"deviceId":5,"deviceValue":845561159}]
        2016-10-20 13:26:44.445 m.s.s.b.ScpNetBolt [INFO] Processing tuple: source: com.microsoft.eventhubs.spout.EventHubSpout:7, stream: default, id: {-2409895457033895206=5479027861202203517}, [{"deviceId":8,"deviceValue":2105860655}]

##Kill the topologies

In the **Storm Topologies Viewer**, select each topology and then select the **Kill** button at the top of the Topology Viewer.

## Project code of conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
