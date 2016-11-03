using Kajabity.Tools.Java;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.TrainingDataGeneration
{
    public class VisualPartitionClassifier
    {
        private static VisualPartitionClassifier instance = new VisualPartitionClassifier();

		private int numberOfPartitions;
        private int[] partitions;
        private double[] l;
        private double g;
        private double fovAngle;
        private Dictionary<int, double[]> partitionAnglesForInceptionLevels;

        public static VisualPartitionClassifier GetInstance()
        {
            return instance;
        }

        public void InitializeFromProperties()
        {
            JavaProperties projectProperties = PropertiesReader.GetPropertyFile(PropertyFile.Project);

            SetViewport(double.Parse(projectProperties.GetProperty("topology.fov")));
            SetPartitions(parseCSV(projectProperties.GetProperty("topology.partitions")));
			SetNumberOfPartitions (partitions);
        }

        public void Initialize(double fovAngle, params int[] partitions)
        {
            SetViewport(fovAngle);
            SetPartitions(partitions);
			SetNumberOfPartitions (partitions);
        }

        private static int[] parseCSV(String CSV)
        {
            String[] split = CSV.Split(',');
            int[] result = new int[split.Length];
            for(int i = 0; i < split.Length; i++)
            {
                result[i] = int.Parse(split[i]);
            }
            return result;
        }

		private void SetNumberOfPartitions(params int[] partitions) {
			int result = 0;


			foreach(int partition in partitions) {
				result += (partition*partition) - 1;
			}
			numberOfPartitions = result + 1;
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
            g = 0.5 / Math.Tan(FromDegreesToRadians(fovAngle / 2));
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
                    splits[j] = GetPartitionAngle(j, l[i]);
                }
                
                partitionAnglesForInceptionLevels.Add(i, splits);
            }
        }

        private double GetPartitionAngle(int i, double l)
        {
            return FromRadiansToDegrees(Math.Tanh(( (0.5+i)*l) / g ));
        }

        private double FromRadiansToDegrees(double angleInRadians)
        {
            return angleInRadians * 180 / Math.PI;
        }

        private double FromDegreesToRadians(double angleInDegrees)
        {
            return angleInDegrees * Math.PI / 180;
        }

        public PartitionId GetVisualPartition(double horizontalAngle, double verticalAngle)
        {
            double nonNegativeHorizontalAngle = ToNonNegativeAngle(horizontalAngle);
            double nonNegativeVerticalAngle = ToNonNegativeAngle(verticalAngle);
            if(IsOutsideFov(nonNegativeHorizontalAngle) || IsOutsideFov(nonNegativeVerticalAngle))
            {
                return null;
            }
            return GetVisualPartition(0, nonNegativeHorizontalAngle, nonNegativeVerticalAngle, fovAngle);
        }

        private Boolean IsOutsideFov(double angle)
        {
            return angle < 0 || angle > fovAngle;
        }

        private double ToNonNegativeAngle(double angle)
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
                double inceptionFov = GetInceptionFov(inceptionLevel);
                return GetVisualPartition(inceptionLevel + 1, GetAngleRelativeToInceptionFov(horizontalAngle, inceptionFov, currentFov), GetAngleRelativeToInceptionFov(verticalAngle, inceptionFov, currentFov), inceptionFov);
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

        private double GetAngleRelativeToInceptionFov(double angle, double inceptionFov, double currentFov)
        {
            return angle - (currentFov / 2 - inceptionFov / 2);
        }

        private double GetInceptionFov(int inceptionLevel)
        {
            return GetPartitionAngle(0, l[inceptionLevel])*2;
        }

		public int GetNumberOfPartitions() {
			return numberOfPartitions;
		}

		public int[] GetPartitions() {
			return partitions;
		}
    }
}
