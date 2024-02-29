using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using CromulentBisgetti.ContainerPacking;
using CromulentBisgetti.ContainerPacking.Entities;
using CromulentBisgetti.ContainerPacking.Algorithms;
using System.Linq;
using System.Text;

namespace CromulentBisgetti.ContainerPackingTests
{
	[TestClass]
	public class ContainerPackingTests
	{
		[TestMethod]
		public void EB_AFIT_Passes_700_Standard_Reference_Tests()
		{
            var max = 10;

            foreach (var testCase in LoadTestCases().Take(max))
            {
                Console.WriteLine("Case " + testCase.Id);


                var results = PackingService.Pack(testCase.Containers, testCase.Items, new List<int> { (int)AlgorithmType.EB_AFIT });
                var result = results[0].AlgorithmPackingResults[0];
             
                Assert.AreEqual(result.PackedItems.Count + result.UnpackedItems.Count, testCase.Results.ItemCount);
                Assert.AreEqual(result.PackedItems.Count, testCase.Results.PackedCount);
                Assert.IsTrue(Math.Abs(result.PercentContainerVolumePacked - testCase.Results.ContainerVolumePacked) < 0.02m);
                Assert.AreEqual(result.PercentItemVolumePacked, testCase.Results.ItemVolumePacked);
            }
        }

        private IEnumerable<TestCase> LoadTestCases()
        {
            string resourceName = "CromulentBisgetti.ContainerPackingTests.DataFiles.ORLibrary.txt";

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

                // Fourth line states how many distinct item types we are packing.
                var itemTypeCount = Convert.ToInt32(reader.ReadLine());
                var itemsToPack = new List<Item>();
                
                for (var i = 0; i < itemTypeCount; i++)
                {
                    var itemArray = reader.ReadLine().Split(' ');

                    var item = new Item(0, Convert.ToDecimal(itemArray[1]), Convert.ToDecimal(itemArray[3]), Convert.ToDecimal(itemArray[5]), Convert.ToInt32(itemArray[7]));
                    itemsToPack.Add(item);
                }

                yield return new TestCase(id, results, containers, itemsToPack);
            }

        }

        record Results(long ItemCount, long PackedCount, decimal ContainerVolumePacked, decimal ItemVolumePacked);
        record TestCase(int Id, Results Results, List<Container> Containers, List<Item> Items);
    }
}
