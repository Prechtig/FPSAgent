using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.TrainingDataGeneration
{
    public class VisualPartitionClassifier
    {
        private static VisualPartitionClassifier instance = new VisualPartitionClassifier();

        private int[] partitions;
        private double[] l;
        private double g;
        private double fovAngle;
        private Dictionary<int, double[]> partitionAnglesForInceptionLevels;

        public static VisualPartitionClassifier GetInstance()
        {
            return instance;
        }

        public void Initialize(double fovAngle, params int[] partitions)
        {
            SetViewport(fovAngle);
            SetPartitions(partitions);
        }

        private void SetViewport(double fovAngle)
        {
            this.fovAngle = fovAngle;
        }

        private void SetPartitions(params int[] partitions)
        {
            // Validate input
            for(int i = 0; i < partitions.Length; i++)
            {
                if(partitions[i] % 2 == 0)
                {
                    throw new ArgumentException("Partitions have to be uneven");
                }
            }

            this.partitions = partitions;
            g = 0.5 / Math.Tanh(fovAngle / 2);
            l = new double[partitions.Length];
            //Sets length of l for each inceptionLevel
            for (int i = 0; i < partitions.Length; i++)
            {
                if (i == 0)
                {
                    l[i] = 1.0 / (double) partitions[i];
                } else {
                    l[i] = l[i - 1] / (double) partitions[i];
                }
            }
            //Sets the angling of partition splits for each inception level
            partitionAnglesForInceptionLevels = new Dictionary<int, double[]>();
            for (int i = 0; i < partitions.Length; i++)
            {
                int numberOfSplits = (partitions[i] - 1)/2;
                double[] splits = new double[numberOfSplits];
                for (int j = 0; j < numberOfSplits; j++)
                {
                    splits[j] = getPartitionAngle(j, l[i]);
                }
                
                partitionAnglesForInceptionLevels.Add(i, splits);
            }
        }

        private double getPartitionAngle(int i, double l)
        {
            return fromRadiansToDegrees(Math.Tanh(( (0.5+i)*l) / g ));
        }

        private double fromRadiansToDegrees(double angleInRadians)
        {
            return angleInRadians * 180 / Math.PI;
        }

        public PartitionId GetVisualPartition(double horizontalAngle, double verticalAngle)
        {
            return GetVisualPartition(0, toNonNegativeAngle(horizontalAngle), toNonNegativeAngle(verticalAngle), fovAngle);
        }

        private double toNonNegativeAngle(double angle)
        {
            return fovAngle / 2 + angle;
        }

        private PartitionId GetVisualPartition(int inceptionLevel, double horizontalAngle, double verticalAngle, double currentFov)
        {
            double[] splitAngles;
            if(!partitionAnglesForInceptionLevels.TryGetValue(inceptionLevel, out splitAngles))
            {
                throw new InvalidOperationException("Should not happen");
            }

            int x = GetPartitionCoordinate(horizontalAngle, splitAngles, currentFov, inceptionLevel);
            int y = GetPartitionCoordinate(verticalAngle, splitAngles, currentFov, inceptionLevel);

            //If it is in the middle and it is possible to go deeper in the inception
            if(x == splitAngles.Length && y == splitAngles.Length && inceptionLevel < partitions.Length - 1)
            {
                return GetVisualPartition(inceptionLevel + 1, horizontalAngle, verticalAngle, GetInceptionFov(inceptionLevel));
            } else
            {
                return new PartitionId(inceptionLevel, x, y);
            }
            
        }

        private int GetPartitionCoordinate(double angle, double[] splitAngles, double currentFovAngle, int inceptionLevel)
        {
            double fromAngle = 0.0;
            double toAngle = GetToAngle(0, partitions[inceptionLevel], splitAngles, currentFovAngle);
            int numberOfPartitions = splitAngles.Length * 2 + 1;
            for(int i = 0; i < numberOfPartitions; i++)
            {
                if(angle <= toAngle && fromAngle <= angle)
                {
                    return i;
                } else {
                    fromAngle = toAngle;
                    toAngle = GetToAngle(i+1, partitions[inceptionLevel], splitAngles, currentFovAngle);
                }
            }
            throw new InvalidOperationException("Should not happen");
        }

        private double GetToAngle(int partition, int numberOfPartitions, double[] splitAngles, double currentFovAngle)
        {
            if(partition < splitAngles.Length)
            {
                return currentFovAngle / 2 - splitAngles[splitAngles.Length - 1 - partition];
            } else if(partition == splitAngles.Length)
            {
                return currentFovAngle / 2 + splitAngles[0];
            } else if(numberOfPartitions-1 != partition)
            {
                return currentFovAngle / 2 + splitAngles[partition - splitAngles.Length];
            } else
            {
                return currentFovAngle;
            }
        }

        private double GetInceptionFov(int inceptionLevel)
        {
            return getPartitionAngle(0, l[inceptionLevel])*2;
        }
    }
}
