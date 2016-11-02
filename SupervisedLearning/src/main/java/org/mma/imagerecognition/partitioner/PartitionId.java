package org.mma.imagerecognition.partitioner;

public class PartitionId
{		
    public final int InceptionLevel;
    public final int X;
    public final int Y;

    public PartitionId(int inceptionLevel, int x, int y)
    {
        this.InceptionLevel = inceptionLevel;
        this.X = x;
        this.Y = y;
    }
}
