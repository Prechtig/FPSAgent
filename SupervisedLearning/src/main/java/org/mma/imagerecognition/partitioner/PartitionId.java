package org.mma.imagerecognition.partitioner;

public class PartitionId
{		
    public int InceptionLevel;
    public int X;
    public int Y;

    public PartitionId(int inceptionLevel, int x, int y)
    {
        this.InceptionLevel = inceptionLevel;
        this.X = x;
        this.Y = y;
    }
}
