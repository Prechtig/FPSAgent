package org.mma.imagerecognition.partitioner;

import java.util.HashMap;
import java.util.Map;
import java.util.stream.Stream;

import org.mma.imagerecognition.tools.PropertiesReader;

public class VisualPartitionClassifier
{
    private static VisualPartitionClassifier instance = new VisualPartitionClassifier();

    private int numberOfPartitions;
    private int[] partitions;
    private Double[] l;
    private double g;
    private double fovAngle;
    private Map<Integer, Double[]> partitionAnglesForInceptionLevels;
	
	static {
		GetInstance().Initialize(Double.parseDouble(PropertiesReader.getProjectProperties().getProperty("topology.fov")), parseCSV(PropertiesReader.getProjectProperties().getProperty("topology.partitions")));
	}
	
	private static int[] parseCSV(String CSV) {
		String[] split = CSV.split(",");
		return Stream.of(split).mapToInt(s -> Integer.parseInt(s)).toArray();
	}
    
    public static VisualPartitionClassifier GetInstance()
    {
        return instance;
    }

    public void Initialize(double fovAngle, int... partitions)
    {
        SetViewport(fovAngle);
        SetPartitions(partitions);
        setNumberOfPartitions(partitions);
    }
    
    private void setNumberOfPartitions(int... partitions) {
    	int result = 0;
    	for(int partition : partitions) {
    		result += (partition*partition) - 1;
    	}
    	numberOfPartitions = result + 1;
    }

    private void SetViewport(double fovAngle)
    {
        this.fovAngle = fovAngle;
    }

    private void SetPartitions(int... partitions)
    {
        // Validate input
        for(int i = 0; i < partitions.length; i++)
        {
            if(partitions[i] % 2 == 0)
            {
                throw new IllegalArgumentException("Partitions have to be uneven");
            }
        }

        this.partitions = partitions;
        g = 0.5 / Math.tan(fromDegreesToRadians(fovAngle / 2));
        l = new Double[partitions.length];
        //Sets length of l for each inceptionLevel
        for (int i = 0; i < partitions.length; i++)
        {
            if (i == 0)
            {
                l[i] = 1.0 / (double) partitions[i];
            } else {
                l[i] = l[i - 1] / (double) partitions[i];
            }
        }
        //Sets the angling of partition splits for each inception level
        partitionAnglesForInceptionLevels = new HashMap<Integer, Double[]>();
        for (int i = 0; i < partitions.length; i++)
        {
            int numberOfSplits = (partitions[i] - 1)/2;
            Double[] splits = new Double[numberOfSplits];
            for (int j = 0; j < numberOfSplits; j++)
            {
                splits[j] = getPartitionAngle(j, l[i]);
            }
            partitionAnglesForInceptionLevels.put(new Integer(i), splits);
        }
    }

    private double getPartitionAngle(int i, double l)
    {
        return fromRadiansToDegrees(Math.tanh(( (0.5+i)*l) / g ));
    }

    private double fromRadiansToDegrees(double angleInRadians)
    {
        return angleInRadians * 180 / Math.PI;
    }

    private double fromDegreesToRadians(double angleInDegrees)
    {
        return angleInDegrees * Math.PI / 180;
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
        Double[] splitAngles = partitionAnglesForInceptionLevels.get(inceptionLevel);
        if(splitAngles == null)
        {
            throw new IllegalStateException("Should not happen");
        }

        int x = GetPartitionCoordinate(horizontalAngle, splitAngles, currentFov, inceptionLevel);
        int y = GetPartitionCoordinate(verticalAngle, splitAngles, currentFov, inceptionLevel);

        //If it is in the middle and it is possible to go deeper in the inception
        if(x == splitAngles.length && y == splitAngles.length && inceptionLevel < partitions.length - 1)
        {
            double inceptionFov = GetInceptionFov(inceptionLevel);
            return GetVisualPartition(inceptionLevel + 1, GetAngleRelativeToInceptionFov(horizontalAngle, inceptionFov, currentFov), GetAngleRelativeToInceptionFov(verticalAngle, inceptionFov, currentFov), inceptionFov);
        } else
        {
            return new PartitionId(inceptionLevel, x, y);
        }
    }

    private int GetPartitionCoordinate(double angle, Double[] splitAngles, double currentFovAngle, int inceptionLevel)
    {
        double fromAngle = 0.0;
        double toAngle = GetToAngle(0, partitions[inceptionLevel], splitAngles, currentFovAngle);
        int numberOfPartitions = splitAngles.length * 2 + 1;
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
        throw new IllegalStateException("Should not happen");
    }

    private double GetToAngle(int partition, int numberOfPartitions, Double[] splitAngles, double currentFovAngle)
    {
        if(partition < splitAngles.length)
        {
            return currentFovAngle / 2 - splitAngles[splitAngles.length - 1 - partition];
        } else if(partition == splitAngles.length)
        {
            return currentFovAngle / 2 + splitAngles[0];
        } else if(numberOfPartitions-1 != partition)
        {
            return currentFovAngle / 2 + splitAngles[partition - splitAngles.length];
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
        return getPartitionAngle(0, l[inceptionLevel])*2;
    }
    
    public int[] getPartitions() {
    	return partitions;
    }
    
    public int getNumberOfPartitions() {
    	return numberOfPartitions;
    }
    
	public int calculateFeatureIndexFromPartitionId(PartitionId partitionId) {
		int inceptionLevel = partitionId.InceptionLevel;
		int index = 0;
		
		// Move index according to inception level
		for(int i = 0; i < inceptionLevel; i++) {
			index += (partitions[i]*partitions[i]) -1;
		}
		
		// Move index according to y
		index += partitionId.Y * partitions[inceptionLevel];
		
		// Move index according to x
		index += partitionId.X;
		
		// If we're past the midpoint in the current inception level, subtract 1
		if((partitions[inceptionLevel]*partitions[inceptionLevel]-1) / 2 + previous(inceptionLevel) <= index
				&& inceptionLevel < (partitions.length-1) // Do not subtract one if we're in the innermost
				) {
			index -= 1;
		}
		return index;
	}
	
	private int previous(int i) {
		if(i == 0) {
			return 0;
		} else {
			return partitions[i]*partitions[i] - 1 + previous(i-1);
		}
	}
}
