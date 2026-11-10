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
-- Dummy data
-- ---------------------------------------------------------------

INSERT INTO `customers` (`first_name`, `last_name`, `mail`, `phone_number`) VALUES
('Anna',    'Andersson',  'anna.andersson@example.com',  '+46701234501'),
('Erik',    'Johansson',  'erik.johansson@example.com',  '+46701234502'),
('Maria',   'Karlsson',   'maria.karlsson@example.com',  '+46701234503'),
('Johan',   'Nilsson',    'johan.nilsson@example.com',   '+46701234504'),
('Sara',    'Eriksson',   'sara.eriksson@example.com',   '+46701234505'),
('Lars',    'Larsson',    'lars.larsson@example.com',    '+46701234506'),
('Emma',    'Olsson',     'emma.olsson@example.com',     '+46701234507'),
('Karl',    'Persson',    'karl.persson@example.com',    '+46701234508'),
('Linnea',  'Svensson',   'linnea.svensson@example.com', '+46701234509'),
('Oskar',   'Gustafsson', 'oskar.gustafsson@example.com','+46701234510');

INSERT INTO `accommodations` (`name`, `location`, `pool`, `restaurant`, `rating`, `distance_to_beach`, `distance_to_city_center`) VALUES
('Sunset Beach Resort',   'Alcúdia, Mallorca',     TRUE,  TRUE,  5,  100, 3500),
('Hotel Costa Azul',      'Málaga, Spain',         TRUE,  TRUE,  4,  400, 1200),
('Casa del Sol',          'Santorini, Greece',     FALSE, FALSE, 3, 1500,  800),
('Olive Grove Apartments','Chania, Crete',         TRUE,  FALSE, 4,  700, 2000),
('Blue Lagoon Hotel',     'Antalya, Turkey',       TRUE,  TRUE,  5,   50, 6000),
('City Budget Inn',       'Lisbon, Portugal',      FALSE, TRUE,  2, 5000,  300);

INSERT INTO `rooms` (`room_number`, `maximum_people`, `price_per_night`, `accommodation_id`) VALUES
-- Sunset Beach Resort (1)
('101', 2, 1450.00, 1),
('102', 4, 2100.00, 1),
('201', 6, 3200.00, 1),
-- Hotel Costa Azul (2)
('11',  2,  990.00, 2),
('12',  3, 1250.00, 2),
('21',  4, 1600.00, 2),
-- Casa del Sol (3)
('A',   2,  850.00, 3),
('B',   2,  850.00, 3),
-- Olive Grove Apartments (4)
('1',   4, 1100.00, 4),
('2',   5, 1350.00, 4),
-- Blue Lagoon Hotel (5)
('301', 2, 1800.00, 5),
('302', 2, 1800.00, 5),
('401', 4, 2600.00, 5),
-- City Budget Inn (6)
('1A',  1,  450.00, 6),
('1B',  2,  600.00, 6);

INSERT INTO `bookings` (`customer_id`, `room_id`, `check_in_date`, `check_out_date`, `number_of_people`) VALUES
(1,  2, '2026-06-20', '2026-06-27', 4),
(2,  4, '2026-07-01', '2026-07-08', 2),
(3,  7, '2026-07-05', '2026-07-12', 2),
(4,  9, '2026-07-10', '2026-07-24', 3),
(5, 11, '2026-08-01', '2026-08-08', 2),
(5, 14, '2026-09-15', '2026-09-18', 1),
(6, 13, '2026-08-10', '2026-08-17', 4),
(7,  1, '2026-06-27', '2026-07-04', 2),
(8, 15, '2026-10-02', '2026-10-05', 2),
(9,  5, '2026-07-15', '2026-07-22', 3),
(10, 3, '2026-12-20', '2027-01-03', 5);
