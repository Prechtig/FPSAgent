using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.TrainingDataGeneration;


class VisualPartitionClassifierTest
{
    [Test]
    public void TestInitializeFromProperties()
    {
        VisualPartitionClassifier.GetInstance().InitializeFromProperties();
    }

    [Test]
    public void TestOutsideFovInput()
    {
        VisualPartitionClassifier.GetInstance().Initialize(90, 3);
        PartitionId partitionId = VisualPartitionClassifier.GetInstance().GetVisualPartition(-45.1, -45.0);
        Assert.IsTrue(partitionId == null);
    }

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

    [Test]
    public void TestPartitioning2()
    {
        VisualPartitionClassifier.GetInstance().Initialize(60, 3, 3);
        PartitionId partitionId1 = VisualPartitionClassifier.GetInstance().GetVisualPartition(0.0, 0.0);
        Assert.AreEqual(1, partitionId1.X);
        Assert.AreEqual(1, partitionId1.Y);
        Assert.AreEqual(1, partitionId1.InceptionLevel);

        PartitionId partitionId2 = VisualPartitionClassifier.GetInstance().GetVisualPartition(-10.7, -9.3);
        Assert.AreEqual(0, partitionId2.X);
        Assert.AreEqual(0, partitionId2.Y);
        Assert.AreEqual(1, partitionId2.InceptionLevel);

        PartitionId partitionId3 = VisualPartitionClassifier.GetInstance().GetVisualPartition(10, 10);
        Assert.AreEqual(2, partitionId3.X);
        Assert.AreEqual(2, partitionId3.Y);
        Assert.AreEqual(1, partitionId3.InceptionLevel);

        PartitionId partitionId4 = VisualPartitionClassifier.GetInstance().GetVisualPartition(-8.9, 10);
        Assert.AreEqual(0, partitionId4.X);
        Assert.AreEqual(2, partitionId4.Y);
        Assert.AreEqual(1, partitionId4.InceptionLevel);

        PartitionId partitionId5 = VisualPartitionClassifier.GetInstance().GetVisualPartition(-15, 29);
        Assert.AreEqual(0, partitionId5.X);
        Assert.AreEqual(2, partitionId5.Y);
        Assert.AreEqual(0, partitionId5.InceptionLevel);

        PartitionId partitionId6 = VisualPartitionClassifier.GetInstance().GetVisualPartition(17, 0.0);
        Assert.AreEqual(2, partitionId6.X);
        Assert.AreEqual(1, partitionId6.Y);
        Assert.AreEqual(0, partitionId6.InceptionLevel);
    }

    [Test]
    public void TestPartitioning3()
    {
        VisualPartitionClassifier.GetInstance().Initialize(110, 5, 3, 3);
        PartitionId partitionId1 = VisualPartitionClassifier.GetInstance().GetVisualPartition(0.0, 0.0);
        Assert.AreEqual(1, partitionId1.X);
        Assert.AreEqual(1, partitionId1.Y);
        Assert.AreEqual(2, partitionId1.InceptionLevel);

        PartitionId partitionId2 = VisualPartitionClassifier.GetInstance().GetVisualPartition(-1.80, 1.810);
        Assert.AreEqual(1, partitionId2.X);
        Assert.AreEqual(1, partitionId2.Y);
        Assert.AreEqual(2, partitionId2.InceptionLevel);

        PartitionId partitionId3 = VisualPartitionClassifier.GetInstance().GetVisualPartition(-2.1, -2.1);
        Assert.AreEqual(0, partitionId3.X);
        Assert.AreEqual(0, partitionId3.Y);
        Assert.AreEqual(2, partitionId3.InceptionLevel);

        PartitionId partitionId4 = VisualPartitionClassifier.GetInstance().GetVisualPartition(3.12, 3.05);
        Assert.AreEqual(2, partitionId4.X);
        Assert.AreEqual(2, partitionId4.Y);
        Assert.AreEqual(2, partitionId4.InceptionLevel);

        PartitionId partitionId5 = VisualPartitionClassifier.GetInstance().GetVisualPartition(5.6, 0.2);
        Assert.AreEqual(2, partitionId5.X);
        Assert.AreEqual(1, partitionId5.Y);
        Assert.AreEqual(1, partitionId5.InceptionLevel);

        PartitionId partitionId6 = VisualPartitionClassifier.GetInstance().GetVisualPartition(-15, -15);
        Assert.AreEqual(0, partitionId6.X);
        Assert.AreEqual(0, partitionId6.Y);
        Assert.AreEqual(1, partitionId6.InceptionLevel);

        PartitionId partitionId7 = VisualPartitionClassifier.GetInstance().GetVisualPartition(17.7, 23.3);
        Assert.AreEqual(3, partitionId7.X);
        Assert.AreEqual(3, partitionId7.Y);
        Assert.AreEqual(0, partitionId7.InceptionLevel);

        PartitionId partitionId8 = VisualPartitionClassifier.GetInstance().GetVisualPartition(52.5, 52.5);
        Assert.AreEqual(4, partitionId8.X);
        Assert.AreEqual(4, partitionId8.Y);
        Assert.AreEqual(0, partitionId8.InceptionLevel);

        PartitionId partitionId9 = VisualPartitionClassifier.GetInstance().GetVisualPartition(-52.5, -52.5);
        Assert.AreEqual(0, partitionId9.X);
        Assert.AreEqual(0, partitionId9.Y);
        Assert.AreEqual(0, partitionId9.InceptionLevel);

        PartitionId partitionId10 = VisualPartitionClassifier.GetInstance().GetVisualPartition(-52.5, 52.5);
        Assert.AreEqual(0, partitionId10.X);
        Assert.AreEqual(4, partitionId10.Y);
        Assert.AreEqual(0, partitionId10.InceptionLevel);
    }
}
