# KumoDictionary
KumoDictionary provides `IDictionary<TKey, TValue>` interface for access to a behind Managed NoSQL/KVS (e.g. Azure Storage Table, DynamoDB).

When you set a value to KumoDictionary, it serializes the value by [MessagePack](https://github.com/neuecc/MessagePack-CSharp/) and writes to NoSQL/KVS transparently. Also you can read it from the dictionary.

> **Extra:** "Kumo(雲)" means "cloud" in Japanese.

![](docs/images/SampleCodeImage.png)

## 💡 Motivation
Serverless apps (Azure Functions, AWS Lambda ...) need to store a data sometimes. However, setting up and accessing a datastore is boring. KumoDictionary helps that.

When you need performance or scalability, you should design an app-specific table and data access patterns. KumoDictionary isn't aiming to do that.

## ⚙ Requirement
.NET Standard 2.0

## ✅ Supported functions and capabilities
- Supports Key-Value-Store backends
    - Microsoft Azure Storage Table
    - Amazon DynamoDB
- Supports serialization for complex types
    - MessagePack (Default) 
    - JSON (KumoDictionary.Serialization.Utf8Json)
- `IDictionary<TKey, TValue>` interface implementation
    - Indexer access (getter/setter)
    - `GetEnumerator` (`foreach` support)
    - `Add`, `Remove`, `TryGetValue`, `Clear`, `ContainsKey`, `Contains`
- Additional `*Async` methods

## ❌ Limitations / Not supported features
- High-scalability KVS table design and access
- Transaction
- Bulk insert/update
- `IDictionary<TKey, TValue>` interface implementation
    - `Count` property 
    - `ICollection<KeyValuePair<TKey, TValue>>.CopyTo` method

## ⚡ Quick Start
### Install NuGet Package

- KumoDictionary.AzureStorageTable
- KumoDictionary.AmazonDynamoDB

```sh
# Use Microsoft Azure Storage Table
$ dotnet add package KumoDictionary.AzureStorageTable

# Use Amazon DynamoDB
$ dotnet add package KumoDictionary.AmazonDynamoDB
```

### Create a DynamoDB table for KumoDictionary (Amazon DynamoDB only)

![](docs/images/DynamoDB-CreateTable.png)

|Primary key|Key name|Key type|
| --- | --- | --- |
|Partition key|Dictionary|String|
|Sort key|Key|String|

> NOTE: If using KumoDictionary backend provider for Azure Storage Table, the backend creates a table when if not the table exists.

### Configure KumoDictionary backend provider as default provider.

Set KumoDictionary backend provider as a default provider at the entry point.

```csharp
using KumoDictionary;
using KumoDictionary.Provider;

// Set backend provider for Microsoft Azure Storage Table
var tableName = "MyTestTable";
var connectionString = "DefaultEndpointsProtocol=https;AccountName=...";

KumoDictionaryAzureStorageTableProvider.UseAsDefault(connectionString, tableName);


// Set backend provider for  Amazon DynamoDB
var tableName = "MyTestTable";
var dynamoDBClient = new AmazonDynamoDBClient();

KumoDictionaryAmazonDynamoDBProvider.UseAsDefault(tableName, dynamoDBClient);
```

### Create a instance of `KumoDictionary<TValue>` and set or get values.

Create an instace of `KumoDictionary<TValue>` class with favorite name and you can use like a `IDictionary<string, TValue>` class.

```csharp
var dict = new KumoDictionary<MyClass>("dictionaryName1");
dict["key1"] = new MyClass { ValueA = 1234 };
dict["key2"] = new MyClass { ValueA = 5678 };

Console.WriteLine(dict["key1"].ValueA); // => 1234
```

## 📝 Best practice / Important note
### Using complex typed key carefully.
You can use complex type as a dictionary key(`TKey`). However, once a complex typed key is serialized, to add fields or properties to the type breaks compatibility.

For example:
```csharp
// Version 1
public class MyKey
{
    public int ValueA { get; set; }
}

var dict = new KumoDictionary<MyKey, int>("dictionaryName1");

dict[new MyKey { ValueA = 1 }] = 123;

Console.WriteLine(dict[new MyKey { ValueA = 1 }]); // => 123
```

```csharp
// Version 2
public class MyKey
{
    public int ValueA { get; set; }
    public int ValueB { get; set; } // add a new property!
}

var dict = new KumoDictionary<MyKey, int>("dictionaryName1");

Console.WriteLine(dict[new MyKey { ValueA = 1 }]); // => `KeyNotFoundException`
```

**We recommend using a primitive type (string, int, enum ...) for a dictionary key.**

### Get a value once, use it twice or more.
When you get a value through a dictionary indexer, KumoDictionary access silently backend via a network I/O.  If you use the value multiple time in a statement, you should store the value and reuse it.

```csharp
var dict = new KumoDictionary<int>("dictionaryName1");

// This statement generates network requests 2 times.
Console.WriteLine("{0}: {1}", dict["KeyA"].GetType(), dict["KeyA"].ToString());

// Recommend: You should store a value.
var value = dict["KeyA"];
Console.WriteLine("{0}: {1}", value.GetType(), value.ToString());
```

### KumoDictionary will not support Redis as backend.
If you want to use Redis as store, please consider to use [CloudStructures](https://github.com/neuecc/CloudStructures).

## License
MIT License