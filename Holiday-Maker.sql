CREATE DATABASE IF NOT EXISTS `holiday_maker`;
USE `holiday_maker`;

-- Drop in reverse dependency order so the script can be re-run
DROP TABLE IF EXISTS `bookings`;
DROP TABLE IF EXISTS `rooms`;
DROP TABLE IF EXISTS `accommodations`;
DROP TABLE IF EXISTS `customers`;

CREATE TABLE `customers`(
    `id` INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `first_name` VARCHAR(255) NOT NULL,
    `last_name` VARCHAR(255) NOT NULL,
    `mail` VARCHAR(255) NOT NULL,
    `phone_number` VARCHAR(20) NOT NULL,
    CONSTRAINT `customers_mail_unique` UNIQUE (`mail`)
);

CREATE TABLE `accommodations`(
    `id` INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL,
    `location` VARCHAR(255) NOT NULL,
    `pool` BOOLEAN NOT NULL DEFAULT FALSE,
    `restaurant` BOOLEAN NOT NULL DEFAULT FALSE,
    `rating` TINYINT UNSIGNED NOT NULL,              -- 1-5 stars
    `distance_to_beach` INT UNSIGNED NOT NULL,       -- meters
    `distance_to_city_center` INT UNSIGNED NOT NULL, -- meters
    CONSTRAINT `accommodations_rating_check` CHECK (`rating` BETWEEN 1 AND 5)
);

CREATE TABLE `rooms`(
    `id` INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `room_number` VARCHAR(20) NOT NULL,
    `maximum_people` INT UNSIGNED NOT NULL,
    `price_per_night` DECIMAL(8, 2) NOT NULL,
    `accommodation_id` INT UNSIGNED NOT NULL,
    CONSTRAINT `rooms_accommodation_room_unique` UNIQUE (`accommodation_id`, `room_number`),
    CONSTRAINT `rooms_maximum_people_check` CHECK (`maximum_people` > 0),
    CONSTRAINT `rooms_price_check` CHECK (`price_per_night` >= 0),
    CONSTRAINT `rooms_accommodation_id_foreign`
        FOREIGN KEY (`accommodation_id`) REFERENCES `accommodations`(`id`) ON DELETE CASCADE
);

CREATE TABLE `bookings`(
    `id` INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `customer_id` INT UNSIGNED NOT NULL,
    `room_id` INT UNSIGNED NOT NULL,
    `check_in_date` DATE NOT NULL,
    `check_out_date` DATE NOT NULL,
    `number_of_people` INT UNSIGNED NOT NULL,
    CONSTRAINT `bookings_dates_check` CHECK (`check_out_date` > `check_in_date`),
    CONSTRAINT `bookings_people_check` CHECK (`number_of_people` > 0),
    CONSTRAINT `bookings_customer_id_foreign`
        FOREIGN KEY (`customer_id`) REFERENCES `customers`(`id`) ON DELETE CASCADE,
    CONSTRAINT `bookings_room_id_foreign`
        FOREIGN KEY (`room_id`) REFERENCES `rooms`(`id`) ON DELETE CASCADE
);

-- ---------------------------------------------------------------
-- Testdata
-- 3 orter, 7 boenden, 24 rum (kapacitet 1-6), 6 kunder, 29 bokningar
-- De flesta bokningar ligger i december 2026.
--
-- Förberedda testfall:
--  * Samma dag:     Rum 1 (Hotel Costa Azul 101) är bokat 10-13 dec och 16-20 dec.
--                   Sök Malaga 13-16 dec, 2 pers -> rum 1 ska visas (exakt lucka).
--  * Upptaget:      Sök Malaga 11-12 dec, 2 pers -> rum 1 ska INTE visas.
--  * Omslutande:    Rum 8 (Playa Grande 301) är bokat 5-8 dec.
--                   Boka rum 8 3-12 dec -> ska nekas.
--                   Rum 9 (Playa Grande 302) är bokat 1-20 dec.
--                   Boka rum 9 5-8 dec -> ska nekas.
--  * Kapacitet:     Rum 4 (Hotel Costa Azul 202) tar 1 person. Boka för 2 -> nekas.
--                   Sök Chania med 6 pers -> inga rum (max 5 i Chania).
--  * Fullbokat:     Alla rum i Alcudia är bokade 24-27 dec.
--                   Sök Alcudia 24-27 dec -> inget resultat.
--  * Hela december: Rum 5 (Malaga City Hostel A1) är bokat 1 dec - 1 jan.
--  * Ändra antal:   Bokning 3 (rum 3, max 4 pers, bokad för 2).
--                   Ändra till 4 -> ok, ändra till 5 -> nekas.
--  * Filter:        Malaga har boenden med/utan pool och restaurang, olika
--                   avstånd och omdömen (2, 4, 5) att filtrera och sortera på.
-- ---------------------------------------------------------------

INSERT INTO `customers` (`first_name`, `last_name`, `mail`, `phone_number`) VALUES
('Anna',  'Andersson', 'anna.andersson@example.com', '+46701234501'),  -- 1
('Erik',  'Johansson', 'erik.johansson@example.com', '+46701234502'),  -- 2
('Maria', 'Karlsson',  'maria.karlsson@example.com', '+46701234503'),  -- 3
('Johan', 'Nilsson',   'johan.nilsson@example.com',  '+46701234504'),  -- 4
('Sara',  'Eriksson',  'sara.eriksson@example.com',  '+46701234505'),  -- 5
('Lars',  'Larsson',   'lars.larsson@example.com',   '+46701234506');  -- 6

INSERT INTO `accommodations` (`name`, `location`, `pool`, `restaurant`, `rating`, `distance_to_beach`, `distance_to_city_center`) VALUES
('Hotel Costa Azul',       'Malaga',  TRUE,  TRUE,  4,  300, 1500),  -- 1
('Malaga City Hostel',     'Malaga',  FALSE, FALSE, 2, 2500,  200),  -- 2
('Playa Grande Resort',    'Malaga',  TRUE,  TRUE,  5,   50, 4000),  -- 3
('Sunset Beach Hotel',     'Alcudia', TRUE,  TRUE,  5,  100, 3000),  -- 4
('Pension Can Pere',       'Alcudia', FALSE, TRUE,  3,  800,  400),  -- 5
('Olive Grove Apartments', 'Chania',  TRUE,  FALSE, 4,  600, 1800),  -- 6
('Old Town Inn',           'Chania',  FALSE, FALSE, 3, 1200,  150);  -- 7

INSERT INTO `rooms` (`room_number`, `maximum_people`, `price_per_night`, `accommodation_id`) VALUES
-- Hotel Costa Azul (1)
('101', 2, 1200.00, 1),  -- rum 1
('102', 2, 1250.00, 1),  -- rum 2
('201', 4, 1900.00, 1),  -- rum 3
('202', 1,  850.00, 1),  -- rum 4
-- Malaga City Hostel (2)
('A1',  1,  450.00, 2),  -- rum 5
('A2',  2,  600.00, 2),  -- rum 6
('B1',  4,  900.00, 2),  -- rum 7
-- Playa Grande Resort (3)
('301', 2, 2400.00, 3),  -- rum 8
('302', 2, 2400.00, 3),  -- rum 9
('401', 4, 3200.00, 3),  -- rum 10
('501', 6, 4500.00, 3),  -- rum 11
-- Sunset Beach Hotel (4)
('11',  2, 1800.00, 4),  -- rum 12
('12',  2, 1800.00, 4),  -- rum 13
('21',  4, 2600.00, 4),  -- rum 14
-- Pension Can Pere (5)
('1',   1,  550.00, 5),  -- rum 15
('2',   2,  750.00, 5),  -- rum 16
('3',   3,  950.00, 5),  -- rum 17
-- Olive Grove Apartments (6)
('1',   2, 1000.00, 6),  -- rum 18
('2',   4, 1400.00, 6),  -- rum 19
('3',   5, 1650.00, 6),  -- rum 20
-- Old Town Inn (7)
('10',  1,  500.00, 7),  -- rum 21
('11',  2,  700.00, 7),  -- rum 22
('12',  2,  700.00, 7),  -- rum 23
('20',  3,  950.00, 7);  -- rum 24

INSERT INTO `bookings` (`customer_id`, `room_id`, `check_in_date`, `check_out_date`, `number_of_people`) VALUES
-- Malaga
(1,  1, '2026-12-10', '2026-12-13', 2),  -- 1  samma-dag-test (före luckan)
(2,  1, '2026-12-16', '2026-12-20', 2),  -- 2  samma-dag-test (efter luckan)
(3,  3, '2026-12-05', '2026-12-12', 2),  -- 3  ändra-antal-test (max 4)
(4,  2, '2026-12-01', '2026-12-04', 1),  -- 4
(4,  2, '2026-12-20', '2026-12-27', 2),  -- 5
(5,  5, '2026-12-01', '2027-01-01', 1),  -- 6  hela december
(6,  6, '2026-12-12', '2026-12-14', 2),  -- 7
(1,  6, '2026-11-20', '2026-11-23', 2),  -- 8  före december
(1,  7, '2026-12-18', '2026-12-26', 4),  -- 9
(2,  8, '2026-12-05', '2026-12-08', 2),  -- 10 omslutande-test
(3,  9, '2026-12-01', '2026-12-20', 2),  -- 11 lång bokning, omsluter
(5, 10, '2026-12-22', '2026-12-29', 4),  -- 12
(6, 11, '2026-12-27', '2027-01-02', 6),  -- 13 över nyår
-- Alcudia: alla rum upptagna 24-27 dec
(1, 12, '2026-12-24', '2026-12-27', 2),  -- 14
(2, 13, '2026-12-24', '2026-12-27', 2),  -- 15
(3, 14, '2026-12-24', '2026-12-27', 3),  -- 16
(4, 15, '2026-12-24', '2026-12-27', 1),  -- 17
(5, 16, '2026-12-24', '2026-12-27', 2),  -- 18
(6, 17, '2026-12-24', '2026-12-27', 3),  -- 19
(2, 12, '2026-12-02', '2026-12-09', 2),  -- 20
(3, 16, '2026-12-10', '2026-12-11', 1),  -- 21 en natt
(5, 14, '2026-12-11', '2026-12-15', 4),  -- 22
-- Chania
(4, 18, '2026-12-14', '2026-12-21', 2),  -- 23
(5, 19, '2026-12-03', '2026-12-06', 3),  -- 24
(6, 20, '2026-12-08', '2026-12-18', 5),  -- 25
(1, 21, '2026-12-01', '2026-12-03', 1),  -- 26
(2, 22, '2026-12-19', '2026-12-23', 2),  -- 27
(3, 23, '2026-12-06', '2026-12-09', 2),  -- 28
(4, 24, '2026-12-28', '2027-01-04', 3);  -- 29
