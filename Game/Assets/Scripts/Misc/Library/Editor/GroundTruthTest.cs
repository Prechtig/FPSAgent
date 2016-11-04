using Assets.Scripts.TrainingDataGeneration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    class GroundTruthTest
    {
        [Test]
        public void TestPartitionToIndex()
        {
            VisualPartitionClassifier.GetInstance().Initialize(60, 3, 3);
            double horizontalAngle1 = 0.0, verticalAngle1 = 0.0;
            PartitionId partitionId1 = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle1, verticalAngle1);
            Assert.AreEqual(1, partitionId1.X);
            Assert.AreEqual(1, partitionId1.Y);
            Assert.AreEqual(1, partitionId1.InceptionLevel);
            double[] features1 = GroundTruth.CalculateFeatures(horizontalAngle1, verticalAngle1);
            Assert.AreEqual(1.0, features1[12]);

            double horizontalAngle2 = -10.7, verticalAngle2 = -9.3;
            PartitionId partitionId2 = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle2, verticalAngle2);
            Assert.AreEqual(0, partitionId2.X);
            Assert.AreEqual(0, partitionId2.Y);
            Assert.AreEqual(1, partitionId2.InceptionLevel);
            double[] features2 = GroundTruth.CalculateFeatures(horizontalAngle2, verticalAngle2);
            Assert.AreEqual(1.0, features2[8]);

            double horizontalAngle3 = 10, verticalAngle3 = 10;
            PartitionId partitionId3 = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle3, verticalAngle3);
            Assert.AreEqual(2, partitionId3.X);
            Assert.AreEqual(2, partitionId3.Y);
            Assert.AreEqual(1, partitionId3.InceptionLevel);
            double[] features3 = GroundTruth.CalculateFeatures(horizontalAngle3, verticalAngle3);
            Assert.AreEqual(1.0, features3[16]);

            double horizontalAngle4 = -8.9, verticalAngle4 = 10;
            PartitionId partitionId4 = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle4, verticalAngle4);
            Assert.AreEqual(0, partitionId4.X);
            Assert.AreEqual(2, partitionId4.Y);
            Assert.AreEqual(1, partitionId4.InceptionLevel);
            double[] features4 = GroundTruth.CalculateFeatures(horizontalAngle4, verticalAngle4);
            Assert.AreEqual(1.0, features4[14]);

            double horizontalAngle5 = -15, verticalAngle5 = 29;
            PartitionId partitionId5 = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle5, verticalAngle5);
            Assert.AreEqual(0, partitionId5.X);
            Assert.AreEqual(2, partitionId5.Y);
            Assert.AreEqual(0, partitionId5.InceptionLevel);
            double[] features5 = GroundTruth.CalculateFeatures(horizontalAngle5, verticalAngle5);
            Assert.AreEqual(1.0, features5[5]);

            double horizontalAngle6 = 17, verticalAngle6 = 0.0;
            PartitionId partitionId6 = VisualPartitionClassifier.GetInstance().GetVisualPartition(horizontalAngle6, verticalAngle6);
            Assert.AreEqual(2, partitionId6.X);
            Assert.AreEqual(1, partitionId6.Y);
            Assert.AreEqual(0, partitionId6.InceptionLevel);
            double[] features6 = GroundTruth.CalculateFeatures(horizontalAngle6, verticalAngle6);
            Assert.AreEqual(1.0, features6[4]);
        }
    }
