using CromulentBisgetti.ContainerPacking;
using CromulentBisgetti.ContainerPacking.Algorithms;
using CromulentBisgetti.ContainerPacking.Entities;
using ContainerPacking2;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using PackingService = CromulentBisgetti.ContainerPacking.PackingService;
using PackingService2 = ContainerPacking2.PackingService;

using Container = CromulentBisgetti.ContainerPacking.Entities.Container;
using Container2 = ContainerPacking2.Entities.Container;

using Item = CromulentBisgetti.ContainerPacking.Entities.Item;
using Item2 = ContainerPacking2.Entities.Item;
namespace TestConsole
{
    internal class Program
    {
        private readonly static ILogger _log;
        static Program()
        {
            _log = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .CreateLogger();
        }
        private static Object _lockObject = new object();
        static void Main(string[] args)
        {
            var max = 100;
            var current = 1;
            var current2 = 1;
            var times = new List<double>();
            var times2 = new List<double>();
            Parallel.ForEach(LoadTestCases().Take(max), testCase =>
            {
                var sw = new Stopwatch();
                sw.Start();
                var results = PackingService.Pack(testCase.Containers, testCase.Items, new List<int> { (int)AlgorithmType.EB_AFIT });
                var result = results[0].AlgorithmPackingResults[0];
                sw.Stop();
                lock(_lockObject)
                {
                    times.Add(sw.Elapsed.TotalMilliseconds);
                    if(current % 20 == 0)
                        _log.Information("Test {id}: {boxes} boxes, {ms}ms, avg: {avg}", current, testCase.Items.Sum(i => i.Quantity), sw.Elapsed.TotalMilliseconds, times.Average());
                    current++;
                }
                Assert.AreEqual(result.PackedItems.Count + result.UnpackedItems.Count, testCase.Results.ItemCount);
                Assert.AreEqual(result.PackedItems.Count, testCase.Results.PackedCount);
                Assert.IsTrue(Math.Abs(result.PercentContainerVolumePacked - testCase.Results.ContainerVolumePacked) < 0.02m);
                Assert.AreEqual(result.PercentItemVolumePacked, testCase.Results.ItemVolumePacked);

                //XXXXXXXXXXXXXXXX

                var sw2 = new Stopwatch();
                sw2.Start();
                var results2 = PackingService2.Pack(testCase.Containers2, testCase.Items2, new List<int> { (int)AlgorithmType.EB_AFIT });
                var result2 = results[0].AlgorithmPackingResults[0];
                sw2.Stop();
                lock (_lockObject)
                {
                    times2.Add(sw2.Elapsed.TotalMilliseconds);
                    if (current % 20 == 0)
                        _log.Information("Test2 {id}: {boxes} boxes, {ms}ms, avg: {avg}", current2, testCase.Items.Sum(i => i.Quantity), sw2.Elapsed.TotalMilliseconds, times2.Average());
                    current2++;
                }
                Assert.AreEqual(result2.PackedItems.Count + result2.UnpackedItems.Count, testCase.Results.ItemCount);
                Assert.AreEqual(result2.PackedItems.Count, testCase.Results.PackedCount);
                Assert.IsTrue(Math.Abs(result2.PercentContainerVolumePacked - testCase.Results.ContainerVolumePacked) < 0.02m);
                Assert.AreEqual(result2.PercentItemVolumePacked, testCase.Results.ItemVolumePacked);
            });

            _log.Information("Finished {count} tests, average1 {ms}ms, average2 {ms2}ms", times.Count, times.Average(), times2.Average());
        }

        private static IEnumerable<TestCase> LoadTestCases()
        {
            string resourceName = "TestConsole.DataFiles.ORLibrary.txt";

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            string idLine;
            while ((idLine = reader.ReadLine()) != null)
            {
                // First line in each test case is an ID. Skip it.
                var id = Convert.ToInt32(idLine.Split(' ')[0]);
                // Second line states the results of the test, as reported in the EB-AFIT master's thesis, appendix E.
                var testResults = reader.ReadLine().Split(' ');
                var results = new Results(
                    Convert.ToInt64(testResults[1]),
                    Convert.ToInt64(testResults[2]),
                    Convert.ToDecimal(testResults[3]),
                    Convert.ToDecimal(testResults[4])
                    );

                // Third line defines the container dimensions.
                var containerDims = reader.ReadLine().Split(' ');
                var container = new Container(
                    0,
                    Convert.ToInt64(containerDims[0]),
                    Convert.ToInt64(containerDims[1]),
                    Convert.ToInt64(containerDims[2])
                    );
                var containers = new List<Container>
                {
                    container
                };

                var container2 = new Container2(
                    0,
                    Convert.ToInt64(containerDims[0]),
                    Convert.ToInt64(containerDims[1]),
                    Convert.ToInt64(containerDims[2])
                    );

                var containers2 = new List<Container2>
                {
                    container2
                };

                // Fourth line states how many distinct item types we are packing.
                var itemTypeCount = Convert.ToInt32(reader.ReadLine());
                var itemsToPack = new List<Item>();
                var itemsToPack2 = new List<Item2>();

                for (var i = 0; i < itemTypeCount; i++)
                {
                    var itemArray = reader.ReadLine().Split(' ');

                    var item = new Item(0, Convert.ToDecimal(itemArray[1]), Convert.ToDecimal(itemArray[3]), Convert.ToDecimal(itemArray[5]), Convert.ToInt32(itemArray[7]));
                    itemsToPack.Add(item);

                    var item2 = new Item2(0, Convert.ToInt64(itemArray[1]), Convert.ToInt64(itemArray[3]), Convert.ToInt64(itemArray[5]), Convert.ToInt32(itemArray[7]));
                    itemsToPack2.Add(item2);
                }

                yield return new TestCase(id, results, containers, containers2, itemsToPack, itemsToPack2);
            }
        }

        record Results(long ItemCount, long PackedCount, decimal ContainerVolumePacked, decimal ItemVolumePacked);
        record TestCase(int Id, Results Results, List<Container> Containers, List<Container2> Containers2, List<Item> Items, List<Item2> Items2);

    }
}