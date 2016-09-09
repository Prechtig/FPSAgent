SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `TrainingDB`
--

-- --------------------------------------------------------

--
-- Table structure for table `trainingDataTb`
--

CREATE TABLE IF NOT EXISTS `trainingData` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `pixeldata` mediumblob NOT NULL,
  `width` int NOT NULL,
  `height` int NOT NULL,
  
  `horizontalangle1` double,
  `verticalangle1` double,
  `distance1` double,
  `withinsight1` double,

  `horizontalangle2` double,
  `verticalangle2` double,
  `distance2` double,
  `withinsight2` double,

  `horizontalangle3` double,
  `verticalangle3` double,
  `distance3` double,
  `withinsight3` double,

  `horizontalangle4` double,
  `verticalangle4` double,
  `distance4` double,
  `withinsight4` double,

  `horizontalangle5` double,
  `verticalangle5` double,
  `distance5` double,
  `withinsight5` double,

  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;