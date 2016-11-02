using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.TrainingDataGeneration;


    class VisualPartitionClassifierTest
    {
        [Test]
        public void TestSimplePartitioning()
        {
            VisualPartitionClassifier.GetInstance().Initialize(90, 3);
            PartitionId partitionId = VisualPartitionClassifier.GetInstance().GetVisualPartition(-45.0, -45.0);
            Assert.AreEqual(0, partitionId.X);
            Assert.AreEqual(0, partitionId.Y);
            Assert.AreEqual(0, partitionId.InceptionLevel);
        }

        [Test]
        public void TestPartitioning1()
        {
            VisualPartitionClassifier.GetInstance().Initialize(90, 5);
            PartitionId partitionId1 = VisualPartitionClassifier.GetInstance().GetVisualPartition(43.0, -45.0);
            Assert.AreEqual(4, partitionId1.X);
            Assert.AreEqual(0, partitionId1.Y);
            Assert.AreEqual(0, partitionId1.InceptionLevel);

            PartitionId partitionId2 = VisualPartitionClassifier.GetInstance().GetVisualPartition(0.0, 0.0);
            Assert.AreEqual(2, partitionId2.X);
            Assert.AreEqual(2, partitionId2.Y);
            Assert.AreEqual(0, partitionId2.InceptionLevel);

            PartitionId partitionId3 = VisualPartitionClassifier.GetInstance().GetVisualPartition(11.29, 0.0);
            Assert.AreEqual(2, partitionId3.X);
            Assert.AreEqual(2, partitionId3.Y);
            Assert.AreEqual(0, partitionId3.InceptionLevel);

            PartitionId partitionId4 = VisualPartitionClassifier.GetInstance().GetVisualPartition(11.31, 0.0);
            Assert.AreEqual(3, partitionId4.X);
            Assert.AreEqual(2, partitionId4.Y);
            Assert.AreEqual(0, partitionId4.InceptionLevel);

            PartitionId partitionId5 = VisualPartitionClassifier.GetInstance().GetVisualPartition(-11.56, 11.5);
            Assert.AreEqual(1, partitionId5.X);
            Assert.AreEqual(3, partitionId5.Y);
            Assert.AreEqual(0, partitionId5.InceptionLevel);

            PartitionId partitionId6 = VisualPartitionClassifier.GetInstance().GetVisualPartition(43.0, 0.0);
            Assert.AreEqual(4, partitionId6.X);
            Assert.AreEqual(2, partitionId6.Y);
            Assert.AreEqual(0, partitionId6.InceptionLevel);
    }

    }
