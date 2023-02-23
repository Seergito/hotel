-- phpMyAdmin SQL Dump
-- version 4.7.0
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Server version: 5.6.34-log
-- PHP Version: 7.1.5

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `hotel`
--
CREATE DATABASE `hotel` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_spanish_ci;
USE `hotel`;


-- --------------------------------------------------------

--
-- Table structure for table `facturas`
--

DROP TABLE IF EXISTS `facturas`;
CREATE TABLE `facturas` (
  `id` int(11) NOT NULL,
  `dni` char(9) NOT NULL,
  `nombre` varchar(35) NOT NULL,
  `apellidos` varchar(80) NOT NULL,
  `llegada` date NOT NULL,
  `salida` date NOT NULL,
  `habitacion` enum('Estándar','Superior') NOT NULL,
  `buffet` tinyint(1) NOT NULL,
  `wifi` tinyint(1) NOT NULL,
  `spa` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


--
-- Indexes for dumped tables
--

--
-- Indexes for table `facturas`
--
ALTER TABLE `facturas`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `cliente_llegada` (`dni`,`llegada`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `facturas`
--
ALTER TABLE `facturas`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;COMMIT;


DELIMITER $$
--
-- Procedures
--
DROP PROCEDURE IF EXISTS `facturas_con_importes`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `facturas_con_importes` ()  SELECT *,
@total_noches:=DATEDIFF(`salida`,`llegada`) as total_noches, 
@precio_habitacion:=if(habitacion = 'Estándar',60,120) as precio_habitacion,
@importe_hospedaje:=@total_noches*@precio_habitacion as importe_hospedaje,
@precio_servicios:=if(buffet=1,15,0)+if(wifi=1,5,0)+if(spa=1,30,0) as precio_servicios,
@importe_servicios:=@precio_servicios*@total_noches as importe_servicios,
@importe_total := @importe_hospedaje + @importe_servicios as importe_total,
@importe_iva := round(@importe_total*0.21,2) as importe_iva,
@total_pagar := @importe_total + @importe_iva as total_pagar
FROM `facturas` order by id desc$$

DELIMITER ;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
