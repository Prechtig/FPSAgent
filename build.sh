#!/bin/sh

cd SupervisedLearning
mvn clean install
cd ..
cp SupervisedLearning/target/SupervisedLearning-0.0.1-SNAPSHOT-jar-with-dependencies.jar ~/nn/cnn.jar
echo Copied .jar to ~/nn/cnn.jar
cp config/project.properties ~/config/project.properties
echo Copied project.properties to ~/config/project.properties

