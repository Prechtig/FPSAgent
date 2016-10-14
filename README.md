# Mastering the Titans

## Setup
To empower the Titans one must have a configuration file in `~/.ssh/config`.
The contents of said file should have the following form
```
Host hostAlias
  HostName hostIpAddress
  User userName
  Port portNumber
  IdentityFile path/to/privateKey
```
An example of such a file is given
```
Host titan
  HostName 163.172.128.195
  User andreas
  Port 2222
  IdentityFile ~/.ssh/titan_rsa
```
## Getting the Titans to work
To log on the server simply type `ssh titan` or whatever host-alias you chose.

### Files and Folders of interest
- `~/config/` contains the two property files
- `~/nn` contains the runnable jar `cnn.jar`, the logs, models and trainingdata
- `~/repo` contains our own repo, the repos needed to build DL4J and a `build.sh` file
- `build.sh` builds the entire stack, i.e. javacpp -> libnd4j -> nd4j -> datavec -> dl4j -> SupervisedLearning. Everything except SupervisedLearning is build from the latest master commit of each branch.
- `FPSAgent` contains a `build.sh` file and the `pom.xml`
- `build.sh` builds SupervisedLearning and copies the compiled jar file to `~/nn/cnn.jar` as well as the `project.properties` to `~/config/project.properties`.
- `pom.xml` needs alteration if pulled from master, e.g. enable CUDA. This is described in a later section.

### Using CUDA as backend
#### Altering the pom.xml
The pom file needs to be changed in order to run with CUDA and the latest DL4J. We need to run with the latest DL4J as there is a bug in version 0.6.0 that affects us.

The following is a subsection of the pom file that needs to be changed.
```
<properties>
  <nd4j.backend>nd4j-native-platform</nd4j.backend>
  <nd4j.version>0.6.0</nd4j.version>
  <dl4j.version>0.6.0</dl4j.version>
  <datavec.version>0.6.0</datavec.version>
</properties>
```
The above should be changed to
```
<properties>
  <nd4j.backend>nd4j-cuda-7.5</nd4j.backend>
  <nd4j.version>0.6.1-SNAPSHOT</nd4j.version>
  <dl4j.version>0.6.1-SNAPSHOT</dl4j.version>
  <datavec.version>0.6.1-SNAPSHOT</datavec.version>
</properties>
```

#### Configuring the CUDA environment from Java
An import needs to be added to ``Trainer.java``.
Using your favorite terminal editor, e.g. vim or nano, do
```sh
$ nano ~/repo/FPSAgent/SupervisedLearning/src/main/java/org/mma/imagerecognition/executables/Trainer.java
```
and add 

```
import ... more imports
import org.nd4j.jita.conf.CudaEnvironment;

public static void main(String[] args) throws IOException {
  DataTypeUtil.setDTypeForContext(DataBuffer.Type.FLOAT);

  boolean sliEnabled = PropertiesReader.getProjectProperties().getProperty("training.sli").equals("true");

  CudaEnvironment.getInstance().getConfiguration()
    .setMaximumDeviceCacheableLength(GIGABYTE * 1)
    .setMaximumDeviceCache			(GIGABYTE * 12)
    .setMaximumHostCacheableLength	(GIGABYTE * 1)
    .setMaximumHostCache			(GIGABYTE * 16)
    .allowMultiGPU(sliEnabled);
  ... more code
```

### To SLI or not to SLI
If others are putting the Titans to work it might be better not to use SLI as to not run into memory limitations.
Fiddle with the batch size if memory consumption is too large or to small. The batch size should be divisible by 32.

#### To enable SLI
Open ``project.properties`` and set ``training.sli = true``. A batch size of 64 seems to work well with SLI enabled, but this can be fiddled with. Set the desired batch size in ``project.properties`` with ``training.persistence.batchSize = 64``.

#### To disable SLI
Open ``project.properties`` and set ``training.sli = false``. A batch size of 256 seems to work well with SLI disabled, but this can be fiddled with. Set the desired batch size in ``project.properties`` with ``training.persistence.batchSize = 256``. 

When not running in SLI mode ND4J will default to using the first graphics card. If someone else are running something one the first GPU but not the second, it will be better to only use the second GPU.
Do the following
```sh
$ export CUDA_VISIBLE_DEVICES=1
```
``BEWARE that this setting is specific to the running session, so the launch of training should be done in the same session!``

### Building the jar file
To build only SupervisedLearning do

```sh
$ cd ~/repo/FPSAgent
$ ./build.sh
```

To build the DL4J dependencies and then SupervisedLearning do

```sh
$ cd ~/repo
$ ./build.sh
```

### Useful commands on the server
To inspect the load on the two GPU's run
```sh
$ watch -n 0.1 nvidia-smi
```

To see how many epochs the training have completed run
```sh
$ grep -rnw ~/nn/out -e "epoch"
```

To see the 10 latest scores of the training run
```sh
$ tail ~/nn/out
```

To browse the entire training score file run
```sh
$ cat ~/nn/out | less
```

### Useful commands outside the server
To copy a trained model from the server to your local machine run
```sh
$ scp titan:nn/models/continuous/model19.bin path/to/your/desired/destination
```













