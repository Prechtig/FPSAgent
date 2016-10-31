using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.TrainingDataGeneration
{
    class VisualPartitionClassifier
    {
        private static VisualPartitionClassifier instance = new VisualPartitionClassifier();

        private int[] partitions;
        private double[] l;
        private double g;
        private double fovAngle;

        public static VisualPartitionClassifier getInstance()
        {
            return instance;
        }

        private void setViewport(double fovAngle)
        {
            this.fovAngle = fovAngle;
        }

        private void setPartitions(params int[] partitions)
        {
            this.partitions = partitions;
            g = 0.5 / Math.Tanh(fovAngle/2);
            for(int i = 0; i < partitions.Length; i++)
            {
                if(i == 0)
                {
                    l[i] = 1 / partitions[i];
                } else
                {
                    l[i] = l[i-1] / partitions[i];
                }
            }
        }

        public int getVisualPartition(double horizontalAngle, double verticalAngle)
        {
            /** implement that **/
            return 0;
        }
    }
}
