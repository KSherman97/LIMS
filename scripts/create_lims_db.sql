CREATE DATABASE  IF NOT EXISTS `lims` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `lims`;
-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: 3.21.92.43    Database: lims
-- ------------------------------------------------------
-- Server version 5.7.29-0ubuntu0.18.04.1

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `BookDetails`
--

DROP TABLE IF EXISTS `BookDetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BookDetails` (
  `bookId` int(11) NOT NULL AUTO_INCREMENT,
  `ISBN` char(13) NOT NULL,
  `bookCondition` varchar(256) NOT NULL,
  `location` varchar(256) NOT NULL,
  `availability` varchar(20) NOT NULL,
  PRIMARY KEY (`bookId`),
  UNIQUE KEY `bookId_UNIQUE` (`bookId`),
  KEY `ISBN` (`ISBN`),
  CONSTRAINT `BookDetails_ibfk_1` FOREIGN KEY (`ISBN`) REFERENCES `Books` (`ISBN`)
) ENGINE=InnoDB AUTO_INCREMENT=43 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BookDetails`
--

LOCK TABLES `BookDetails` WRITE;
/*!40000 ALTER TABLE `BookDetails` DISABLE KEYS */;
INSERT INTO `BookDetails` VALUES (15,'9780135957059','Good','P1.1.1','reserved'),(16,'9780135957059','Great','P1.1.2','checkedout'),(17,'9780135957059','Ok','P1.1.3','available'),(18,'9780135957059','Bad','P1.1.4','available'),(19,'9780290204890','Great','B1.1.1','available'),(20,'9780290204890','Ok','B1.1.2','checkedout'),(21,'9781401294052','Ok','B1.2.1','available'),(22,'9781401294052','Ok','B1.2.2','available'),(23,'9781401294052','Great','B1.2.3','available'),(24,'9781608867844','Great','D1.1.1','reserved'),(25,'9780596154578','Great','H1.1.1','available'),(26,'9780596154578','Good','H1.1.2','available'),(27,'9781679598067','Good','R1.1.1','available'),(28,'9781679598067','Good','R1.1.2','available'),(29,'9781679598067','Great','R1.1.3','available'),(30,'9781679598067','Bad','R1.1.4','available'),(31,'9781679598067','Bad','R1.1.4','available'),(32,'9781679598067','Bad','R1.1.4','available'),(33,'9780393089059','Good','O1.1.1','available'),(34,'9780393089059','Good','O1.1.2','available'),(35,'9780393089059','Bad','O1.1.3','available'),(36,'9781926606903','Bad','W1.1.1','available'),(37,'9781926606903','Great','W1.1.2','available'),(38,'9781926606903','Great','W1.1.3','available'),(39,'9781597499576','Great','V1.1.1','checkedout'),(40,'9781597499576','Bad','V1.1.2','available'),(41,'9781328869333','Good','G1.1.1','available'),(42,'9781328869333','Good','G1.1.2','available');
/*!40000 ALTER TABLE `BookDetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BookHistory`
--

DROP TABLE IF EXISTS `BookHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BookHistory` (
  `bookHistoryId` int(11) NOT NULL AUTO_INCREMENT,
  `userId` int(11) NOT NULL,
  `bookId` int(11) NOT NULL,
  `dateCheckout` date NOT NULL,
  `dateDue` date NOT NULL,
  `dateReturned` date DEFAULT NULL,
  PRIMARY KEY (`bookHistoryId`),
  KEY `userId` (`userId`),
  KEY `BookHistory_ibfk_2` (`bookId`),
  CONSTRAINT `BookHistory_ibfk_1` FOREIGN KEY (`userId`) REFERENCES `Users` (`userId`),
  CONSTRAINT `BookHistory_ibfk_2` FOREIGN KEY (`bookId`) REFERENCES `BookDetails` (`bookId`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BookHistory`
--

LOCK TABLES `BookHistory` WRITE;
/*!40000 ALTER TABLE `BookHistory` DISABLE KEYS */;
INSERT INTO `BookHistory` VALUES (7,5,16,'2020-04-20','2020-05-04',NULL),(8,6,20,'2020-04-20','2020-05-04',NULL),(9,9,39,'2020-04-20','2020-05-04',NULL);
/*!40000 ALTER TABLE `BookHistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BookRequests`
--

DROP TABLE IF EXISTS `BookRequests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BookRequests` (
  `requestId` int(11) NOT NULL AUTO_INCREMENT,
  `userId` int(11) NOT NULL,
  `ISBN` char(13) NOT NULL,
  `dateRequested` date NOT NULL,
  PRIMARY KEY (`requestId`),
  UNIQUE KEY `requestId_UNIQUE` (`requestId`),
  KEY `userId` (`userId`),
  KEY `ISBN` (`ISBN`),
  CONSTRAINT `BookRequests_ibfk_1` FOREIGN KEY (`userId`) REFERENCES `Users` (`userId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BookRequests`
--

LOCK TABLES `BookRequests` WRITE;
/*!40000 ALTER TABLE `BookRequests` DISABLE KEYS */;
INSERT INTO `BookRequests` VALUES (2,7,'1231231231231','0001-01-01'),(3,7,'1231231231111','0001-01-01'),(4,6,'1231231231231','2020-04-14'),(5,8,'1231231231231','2020-04-14');
/*!40000 ALTER TABLE `BookRequests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Books`
--

DROP TABLE IF EXISTS `Books`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Books` (
  `ISBN` char(13) NOT NULL,
  `title` varchar(256) NOT NULL,
  `genre` varchar(256) DEFAULT NULL,
  `author` varchar(256) DEFAULT NULL,
  `summary` varchar(2048) DEFAULT NULL,
  `datePublished` date DEFAULT NULL,
  `imagePath` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`ISBN`),
  UNIQUE KEY `ISBN_UNIQUE` (`ISBN`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Books`
--

LOCK TABLES `Books` WRITE;
/*!40000 ALTER TABLE `Books` DISABLE KEYS */;
INSERT INTO `Books` VALUES ('9780135957059','The Pragmatic Programmer','Nonfiction','David Thomas, Andrew Hunt','The Pragmatic Programmer is one of those rare tech books you’ll read, re-read, and read again over the years. Whether you’re new to the field or an experienced practitioner, you’ll come away with fresh insights each and every time.\n\nDave Thomas and Andy Hunt wrote the first edition of this influential book in 1999 to help their clients create better software and rediscover the joy of coding. These lessons have helped a generation of programmers examine the very essence of software development, independent of any particular language, framework, or methodology, and the Pragmatic philosophy has spawned hundreds of books, screencasts, and audio books, as well as thousands of careers and success stories.\nWritten as a series of self-contained sections and filled with classic and fresh anecdotes, thoughtful examples, and interesting analogies, The Pragmatic Programmer illustrates the best approaches and major pitfalls of many different aspects of software development. Whether you’re a new coder, an experienced programmer, or a manager responsible for software projects, use these lessons daily, and you’ll quickly see improvements in personal productivity, accuracy, and job satisfaction. You’ll learn skills and develop habits and attitudes that form the foundation for long-term success in your career.',NULL,'the_pragmatic_programmer.jpg'),('9780290204890','Batman: Year One','Fantasy','Frank Miller','In 1986, Frank Miller and David Mazzucchelli produced this groundbreaking reinterpretation of the origin of Batman—who he is, and how he came to be. Sometimes careless and naive, this Dark Knight is far from the flawless vigilante he is today.\n\nIn his first year on the job, Batman feels his way around a Gotham City far darker than the one he left. His solemn vow to extinguish the town’s criminal element is only half the battle; along with Lieutenant James Gordon, the Dark Knight must also fight a police force more corrupt than the scum in the streets.\n\nBatman: Year One stands next to Batman: The Dark Knight Returns on the mantle of greatest Batman graphic novels of all time. Timeless in its appeal, Frank Miller and David Mazzucchelli’s masterpiece would stand apart from the crowded comics field even today.\n\nThis edition includes the complete graphic novel, a new introduction by writer Frank Miller and a new illustrated afterword by artist David Mazzucchelli. Completing this collection are over 40 pages of never-before-seen developmental material such as character and layout sketches, sample script pages, sketches, and more that pro-vide a glimpse into the making of this contemporary classic.\n\nThis volume collects Batman #404-407.',NULL,'batman_year_one.jpg'),('9780393089059','The Odyssey','Fantasy','Homer','A lean, fleet-footed translation that recaptures Homer’s “nimble gallop” and brings an ancient epic to new life.\n\nThe first great adventure story in the Western canon, The Odyssey is a poem about violence and the aftermath of war; about wealth, poverty, and power; about marriage and family; about travelers, hospitality, and the yearning for home.\n\nIn this fresh, authoritative version?the first English translation of The Odyssey by a woman?this stirring tale of shipwrecks, monsters, and magic comes alive in an entirely new way. Written in iambic pentameter verse and a vivid, contemporary idiom, this engrossing translation matches the number of lines in the Greek original, thus striding at Homer’s sprightly pace and singing with a voice that echoes Homer’s music.\n\nWilson’s Odyssey captures the beauty and enchantment of this ancient poem as well as the suspense and drama of its narrative. Its characters are unforgettable, from the cunning goddess Athena, whose interventions guide and protect the hero, to the awkward teenage son, Telemachus, who struggles to achieve adulthood and find his father; from the cautious, clever, and miserable Penelope, who somehow keeps clamoring suitors at bay during her husband’s long absence, to the “complicated” hero himself, a man of many disguises, many tricks, and many moods, who emerges in this translation as a more fully rounded human being than ever before.\n\nA fascinating introduction provides an informative overview of the Bronze Age milieu that produced the epic, the major themes of the poem, the controversies about its origins, and the unparalleled scope of its impact and influence. Maps drawn especially for this volume, a pronunciation glossary, and extensive notes and summaries of each book make this an Odyssey that will be treasured by a new generation of scholars, students, and general readers alike.\n3 maps',NULL,'the_odyssey.jpg'),('9780596154578','Hacking: The Next Generation','Nonfiction','Nitesh Dhanjani','With the advent of rich Internet applications, the explosion of social media, and the increased use of powerful cloud computing infrastructures, a new generation of attackers has added cunning new techniques to its arsenal. For anyone involved in defending an application or a network of systems, Hacking: The Next Generation is one of the few books to identify a variety of emerging attack vectors. You\'ll not only find valuable information on new hacks that attempt to exploit technical flaws, you\'ll also learn how attackers take advantage of individuals via social networking sites, and abuse vulnerabilities in wireless technologies and cloud infrastructures. Written by seasoned Internet security professionals, this book helps you understand the motives and psychology of hackers behind these attacks, enabling you to better prepare and defend against them. Learn how \"inside out\" techniques can poke holes into protected networks Understand the new wave of \"blended threats\" that take advantage of multiple application vulnerabilities to steal corporate data Recognize weaknesses in today\'s powerful cloud infrastructures and how they can be exploited Prevent attacks against the mobile workforce and their devices containing valuable data Be aware of attacks via social networking sites to obtain confidential information from executives and their assistants Get case studies that show how several layers of vulnerabilities can be used to compromise multinational corporations.',NULL,'hacking_the_next_generation.jpg'),('9781328869333','1984','Science Fiction','George Orwell','With extraordinary relevance and renewed popularity, George Orwell’s 1984 takes on new life in this hardcover edition.\n\n“Orwell saw, to his credit, that the act of falsifying reality is only secondarily a way of changing perceptions. It is, above all, a way of asserting power.”—The New Yorker\n \nIn 1984, London is a grim city in the totalitarian state of Oceania where Big Brother is always watching you and the Thought Police can practically read your mind. Winston Smith is a man in grave danger for the simple reason that his memory still functions. Drawn into a forbidden love affair, Winston finds the courage to join a secret revolutionary organization called The Brotherhood, dedicated to the destruction of the Party. Together with his beloved Julia, he hazards his life in a deadly match against the powers that be.\n\nLionel Trilling said of Orwell’s masterpiece “1984 is a profound, terrifying, and wholly fascinating book. It is a fantasy of the political future, and like any such fantasy, serves its author as a magnifying device for an examination of the present.” Though the year 1984 now exists in the past, Orwell’s novel remains an urgent call for the individual willing to speak truth to power.',NULL,'1984.jpg'),('9781401294052','Batman: The Killing Joke','Fantasy','Alan Moore','A NEW YORK TIMES Bestseller!\n\nPresented for the first time with stark, stunning new coloring by Brian Bolland, BATMAN: THE KILLING JOKE is Alan Moore\'s unforgettable meditation on the razor-thin line between sanity and insanity, heroism and villainy, comedy and tragedy.\n\nAccording to the grinning engine of madness and mayhem known as the Joker, that\'s all that separates the sane from the psychotic. Freed once again from the confines of Arkham Asylum, he\'s out to prove his deranged point. And he\'s going to use Gotham City\'s top cop, Commissioner Jim Gordon, and the Commissioner’s brilliant and beautiful daughter Barbara to do it.',NULL,'batman_the_killing_joke.jpg'),('9781597499576','Violent Python: A Cookbook for Hackers, Forensic Analysts, Penetration Testers and Security Engineers','Nonfiction','TJ O\'Connor','Violent Python shows you how to move from a theoretical understanding of offensive computing concepts to a practical implementation. Instead of relying on another attacker’s tools, this book will teach you to forge your own weapons using the Python programming language. This book demonstrates how to write Python scripts to automate large-scale network attacks, extract metadata, and investigate forensic artifacts. It also shows how to write code to intercept and analyze network traffic using Python, craft and spoof wireless frames to attack wireless and Bluetooth devices, and how to data-mine popular social media websites and evade modern anti-virus.',NULL,'violent_python.jpg'),('9781608867844','Do Androids Dream of Electric Sheep?','Science Fiction','Phillip K. Dick','What\'s to Love: If you loved the 1982 Ridley Scott film Blade Runner, chances are you know it\'s based on Philip K. Dick\'s novel Do Androids Dream of Electric Sheep? From 2009-2011, we published a 24-issue graphic interpretation of the novel as realized by artist Tony Parker. Now, for the first time, all 24 issues of the Eisner Award-nominated series are collected into one complete omnibus edition, featuring a brand-new cover by Mondo artist Jay Shaw and essays from Jonathan Lethem, Ed Brubaker, Warren Ellis, Matt Fraction, and more.\n\nWhat It Is: San Francisco lies under a cloud of radioactive dust. The World War has killed millions, driving entire species to extinction and sending mankind off-planet. Those who remain covet any living creature, and for people who can\'t afford one, companies build incredibly realistic fakes: horses, birds, cats, sheep...even humans. Rick Deckard is an officially sanctioned bounty hunter tasked to find six rogue androids. They\'re machines, but look, sound, and think like humans.',NULL,'do_androids_dream_of_electric_sheep.jpg'),('9781679598067','Romeo and Juliet','Romance','William Shakespeare','Romeo and Juliet is a tragedy written by William Shakespeare early in his career about two young star-crossed lovers whose deaths ultimately reconcile their feuding families. It was among Shakespeare\'s most popular plays during his lifetime and along with Hamlet, is one of his most frequently performed plays. Today, the title characters are regarded as archetypal young lovers.\nA True Classic that Belongs on Every Bookshelf!',NULL,'romeo_and_juliet.jpg'),('9781926606903','The War of the Worlds','Science Fiction','H.G. Wells','Shortly after astronomers observe explosions on the surface of Mars, meteor-like objects begin crashing into Earth. Martians emerge from their craters in large tripods, wiping out army units with heat-rays as they roam the English countryside. When the order is given to evacuate London, all seems lost. But there is one minor detail that the Martians did not plan for. H. G. Wells is credited with the popularisation of time travel in 1895 with The Time Machine, introducing the idea of time being the \"fourth dimension\" a decade before the publication of Einstein\'s first Relativity papers. In 1896, he imagined a mad scientist creating human-like beings from animals in The Island of Doctor Moreau, which created a growing interest in animal welfare throughout Europe. In 1897 with The Invisible Man, Wells shows how a formula could render one invisible, recognizing that an invisible eye would not be able to focus, thus rendering the invisible man blind. With The War of the Worlds in 1898, Wells established the idea that an advanced civilization could live on Mars, popularizing the term \'martian\' and the idea that aliens could invade Earth.',NULL,'war_of_the_worlds.jpg');
/*!40000 ALTER TABLE `Books` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Orders`
--

DROP TABLE IF EXISTS `Orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Orders` (
  `orderId` int(11) NOT NULL AUTO_INCREMENT,
  `ISBN` char(13) NOT NULL,
  `quantity` int(11) NOT NULL,
  `dateOrdered` date NOT NULL,
  `dateExpected` date DEFAULT NULL,
  `dateRecieved` date DEFAULT NULL,
  PRIMARY KEY (`orderId`),
  KEY `ISBN` (`ISBN`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Orders`
--

LOCK TABLES `Orders` WRITE;
/*!40000 ALTER TABLE `Orders` DISABLE KEYS */;
INSERT INTO `Orders` VALUES (3,'1231231231231',5,'2020-04-20','2020-04-30',NULL),(4,'1111111111111',3,'2020-04-08','2020-05-29',NULL),(5,'1451451451451',1,'2020-04-01','2020-04-08','2020-04-08');
/*!40000 ALTER TABLE `Orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Reservations`
--

DROP TABLE IF EXISTS `Reservations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Reservations` (
  `reservationId` int(11) NOT NULL AUTO_INCREMENT,
  `userId` int(11) NOT NULL,
  `bookId` int(11) NOT NULL,
  `dateReserved` date NOT NULL,
  PRIMARY KEY (`reservationId`),
  KEY `userId` (`userId`),
  KEY `Reservations_ibfk_2` (`bookId`),
  CONSTRAINT `Reservations_ibfk_1` FOREIGN KEY (`userId`) REFERENCES `Users` (`userId`),
  CONSTRAINT `Reservations_ibfk_2` FOREIGN KEY (`bookId`) REFERENCES `BookDetails` (`bookId`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Reservations`
--

LOCK TABLES `Reservations` WRITE;
/*!40000 ALTER TABLE `Reservations` DISABLE KEYS */;
INSERT INTO `Reservations` VALUES (11,8,15,'2020-04-20'),(12,7,24,'2020-04-20');
/*!40000 ALTER TABLE `Reservations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `UserReviews`
--

DROP TABLE IF EXISTS `UserReviews`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `UserReviews` (
  `reviewId` int(11) NOT NULL AUTO_INCREMENT,
  `userId` int(11) NOT NULL,
  `ISBN` char(13) NOT NULL,
  `rating` int(11) DEFAULT NULL,
  `reviewText` varchar(2048) NOT NULL,
  PRIMARY KEY (`reviewId`),
  KEY `userId` (`userId`),
  KEY `ISBN` (`ISBN`),
  CONSTRAINT `UserReviews_ibfk_1` FOREIGN KEY (`userId`) REFERENCES `Users` (`userId`),
  CONSTRAINT `UserReviews_ibfk_2` FOREIGN KEY (`ISBN`) REFERENCES `Books` (`ISBN`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `UserReviews`
--

LOCK TABLES `UserReviews` WRITE;
/*!40000 ALTER TABLE `UserReviews` DISABLE KEYS */;
INSERT INTO `UserReviews` VALUES (3,8,'9780393089059',0,'What a book!'),(4,8,'9780135957059',0,'Really helped me!'),(5,8,'9780290204890',0,'Great Read!'),(6,8,'9780596154578',0,'Super Cool!'),(7,8,'9781328869333',0,'Eerily familiar!'),(8,8,'9781401294052',0,'One of the greats!'),(9,8,'9781597499576',0,'Very Informative'),(10,8,'9781926606903',0,'Almost as good as the radio play!'),(11,8,'9781679598067',0,'Classic!'),(12,7,'9780135957059',0,'A must read!'),(13,7,'9780596154578',0,'Verrrrrrry Useful!'),(14,7,'9781597499576',0,'Mind opening!'),(15,7,'9781679598067',0,'Very sad!');
/*!40000 ALTER TABLE `UserReviews` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Users` (
  `userId` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(256) NOT NULL,
  `password` varchar(256) NOT NULL,
  `salt` varchar(256) NOT NULL,
  `accountType` varchar(256) NOT NULL,
  `firstName` varchar(256) NOT NULL,
  `lastName` varchar(256) NOT NULL,
  `address` varchar(256) NOT NULL,
  `city` varchar(256) NOT NULL,
  `state` char(2) NOT NULL,
  `zip` char(5) NOT NULL,
  `phone` char(10) NOT NULL,
  PRIMARY KEY (`userId`),
  UNIQUE KEY `username` (`username`),
  UNIQUE KEY `userId_UNIQUE` (`userId`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Users`
--

LOCK TABLES `Users` WRITE;
/*!40000 ALTER TABLE `Users` DISABLE KEYS */;
INSERT INTO `Users` VALUES (5,'chuck123','TB???z???Nl???????R??M?[????F?|','U9??N???l??.#?f?lx?????U??Q\r~P','user','chuck','example','123 Example Street','New Exampsterdam','AL','12345','1234567890'),(6,'terran1','o?w6\\S,?x????^jQ@!?F??TtF??','JJ?]~?????D?9?????kO?-?|u\0?J4?5','employee','jim','reynor','123 Main St','New york','MI','49001','2698235726'),(7,'residentEvil','@BDd &6?ql??UN????????? ??N?','\"?3?O??\'eSYwE????_LL??TG{?','user','Clancy','Javis','123 bayou','New Orleans','LA','23041','1234567890'),(8,'kingOfTheWorld','??/????z???D?NTYL?\\g<?t\'?r_1','??????!?Y???/?x.?DFG?8?','user','Duke','Nuke\'em','123','Kazoo','HI','12345','1234123123'),(9,'starNemesis','(m??&`@?%G ?y????p???\n????','?w`??\0??R?2????C`??DP?.???','manager','Jill','Valentine','123 Racoon Street','Racoon City','NY','12345','1231231231');
/*!40000 ALTER TABLE `Users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-04-20 11:42:31
