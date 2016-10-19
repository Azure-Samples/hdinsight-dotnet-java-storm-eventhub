---
services: hdinsight
platforms: dotnet,java
author: blackmist
---

# Read and write from EventHubs using a hybrid .NET/Java Storm topology

An example of using a hybrid dotnet/Java-based Apache Storm topology to work with Azure Event Hubs

##Prerequisites

* **Visual Studio 2013 or 2015**

* **Azure SDK for Visual Studio** - at least version 2.5. This gives you the Azure entries in Server Explorer and the ability to view things like table storage

* **HDInsight Tools for Visual Studio** - [http://azure.microsoft.com/en-us/documentation/articles/hdinsight-hadoop-visual-studio-tools-get-started/](http://azure.microsoft.com/en-us/documentation/articles/hdinsight-hadoop-visual-studio-tools-get-started/) has the steps to install and configure. This provides C# Storm topology templates and some utilities for working with HDInsight

* **eventhubs-storm-spout** jar file - this contains the Event Hub Spout and Bolt. You an find this in the **lib/eventhubs** folder of the [https://github.com/hdinsight/hdinsight-storm-examples](https://github.com/hdinsight/hdinsight-storm-examples) repository

* **Azure Event Hub** - you need to create an Event Hub, with two policies defined - one that can send, and one that can listen. You will need the following information from the Event Hub configuration.

    * **Event Hub Name** - the name of the Event Hub you created

    * **Event Hub Namespace** - the namespace of the Azure ServiceBus that contains the Event Hub

    * **Listen policy** - the name and key of the policy that can listen to events for the Event Hub

    * **Send policy** - the name and key of the policy that can send events to the Event Hub
	
	For information on creating an Event Hub and policies, see [Get Started with Event Hubs](https://azure.microsoft.com/en-us/documentation/articles/event-hubs-csharp-ephcs-getstarted/).

* **Azure Table Storage** - you need to create a Storage Account

    * **Storage account name** - the name of the storage account

    * **Storage account key** - the key of the storage account

* **Storm on HDInsight cluster** - the Azure HDInsight cluster that you will submit the topologies to

## SCP.NET package version

The SCP.NET package version that you use for this project depends on the version of Storm installed on your HDInsight cluster. Use the following table to determine what SCP.NET version you must use in your project:

| HDInsight version | Apache Storm version | SCP.NET version |
|:-----------------:|:--------------------:|:---------------:|
| 3.3 | 0.10.# | 0.10.#.# |
| 3.4 | 0.10.# | 0.10.#.# |
| 3.5 | 1.0.# | 1.0.#.# |

##Build and deploy

The two topologies in this project work together - EventHubWriter writes events, while EventHubReader reads them. You can deploy them in any order, but when both are deployed, events should flow from the Writer, to the Reader, then to Table Storage.

1. The configuration for Event Hub and Table Storage is stored in the __App.config__ for each project. Fill in the values needed to connect to Azure Event Hub and (for the reader,) Azure Table Storage. For the **TableName** entry, enter the name of the table you want events to be stored in.

2. In **Server Explorer**, right-click the solution (**EventHubExample**,) and select **Build**. This will restore any missing packages and build the project.

3. In **Server Explorer**, right-click the **Azure** entry and select **Connect to Microsoft Azure Subscription** to make sure you are connected.

4. In **Server Explorer**, right-click the project name (**EventHubWriter** or **EventHubReader**,) and select **Submit to Storm on HDInsight**.

    * Use the drop-down to select the Storm on HDInsight server

    * Expand **Additional Configurations**, select **Java File Paths**, and browse or enter the path to the directory that contains the **eventhubs-storm-spout-0.9-jar-with-dependencies.jar** file.

    * Select **Submit** to submit the topology to the server

Once the topology has been submitted, the **Storm Topologies Viewer** should appear, with a list of running topologies. If not, you can open it from **Server Explorer** by expanding **Azure**, **HDInsight**, right-click the cluster name and select **View Storm Topologies**.

##View output

1. In **Server Explorer**, expand **Azure**, **Storage**, and the **Storage Account** you created earlier.

2. Expand **Tables**, then double click on the table you used as the output for the **EventHubReader**.

3. Once the table editor appears, you should see that the table has been populated with the random data created by the EventHubWriter, which is read by the EventHubReader, and then stored into Table Storage.

##Kill the topologies

In the **Storm Topologies Viewer**, select each topology and then select the **Kill** button at the top of the Topology Viewer.

## Project code of conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.