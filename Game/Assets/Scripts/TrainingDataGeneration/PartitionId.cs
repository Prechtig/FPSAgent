using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.TrainingDataGeneration
{
    public class PartitionId
    {
        public int InceptionLevel
        {
            get;
            private set;
        }

        public int X
        {
            get;
            private set;
        }

        public int Y
        {
            get;
            private set;
        }

        public PartitionId(int inceptionLevel, int x, int y)
        {
            this.InceptionLevel = inceptionLevel;
            this.X = x;
            this.Y = y;
        }
    }
}
