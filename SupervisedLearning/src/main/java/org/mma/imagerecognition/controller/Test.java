package org.mma.imagerecognition.controller;

import org.deeplearning4j.nn.api.OptimizationAlgorithm;
import org.deeplearning4j.nn.conf.MultiLayerConfiguration;
import org.deeplearning4j.nn.conf.NeuralNetConfiguration;
import org.deeplearning4j.nn.conf.Updater;
import org.deeplearning4j.nn.conf.inputs.InputType;
import org.deeplearning4j.nn.conf.layers.OutputLayer;
import org.deeplearning4j.nn.weights.WeightInit;
import org.nd4j.linalg.lossfunctions.LossFunctions;

public class Test {
	
	public static void  main(String[] args){
        int height = 28;
        int width = 28;
        int featureCount = 10;
        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder()
            .seed(98)
            .iterations(1)
            .regularization(true)
            .l2(0.0005)
            .learningRate(0.0005)
            .weightInit(WeightInit.XAVIER)
            .activation("relu")
            .optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
            .updater(Updater.NESTEROVS).momentum(0.9)
            .list()
            .layer(0, new OutputLayer.Builder(LossFunctions.LossFunction.SQUARED_LOSS)
                .name("layer5")
                .nOut(featureCount)
                .activation("identity")
                .build())
            .backprop(true).pretrain(false)
            .setInputType(InputType.convolutionalFlat(height, width, 3)).build();

        String json = conf.toJson();
        MultiLayerConfiguration fromJson = MultiLayerConfiguration.fromJson(json);
    }

}
