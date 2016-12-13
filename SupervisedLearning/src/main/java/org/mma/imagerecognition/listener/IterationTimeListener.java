package org.mma.imagerecognition.listener;

import org.deeplearning4j.nn.api.Model;
import org.deeplearning4j.optimize.api.IterationListener;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class IterationTimeListener implements IterationListener {
    private final int printIterations;
    private static final Logger log = LoggerFactory.getLogger(IterationTimeListener.class);
    private boolean invoked = false;
    private long iterCount = 0;
    private long timeOfLastIteration;

    /**
     * @param printIterations    frequency with which to print scores (i.e., every printIterations parameter updates)
     */
    public IterationTimeListener(int printIterations) {
    	if(printIterations <= 0) {
            printIterations = 1;
        }
        this.printIterations = printIterations;
        timeOfLastIteration = System.currentTimeMillis();
    }

    /** Default constructor printing every 10 iterations */
    public IterationTimeListener() {
    	this(10);
    }

    @Override
    public boolean invoked(){ return invoked; }

    @Override
    public void invoke() { this.invoked = true; }

    @Override
    public void iterationDone(Model model, int iteration) {
    	long now = System.currentTimeMillis();
    	long timeOfLast = timeOfLastIteration;
    	timeOfLastIteration = now;
    	
        if(iterCount++ % printIterations == 0) {
            invoke();
            System.out.println("Time of iteration " + iterCount + " is " + (now-timeOfLast) + " ms");
        }
    }
}
