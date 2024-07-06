CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `vacancies` (
    `id` bigint NOT NULL AUTO_INCREMENT,
    `name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `city` longtext CHARACTER SET utf8mb4 NULL,
    `address_raw` longtext CHARACTER SET utf8mb4 NULL,
    `employer_name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `employer_url` longtext CHARACTER SET utf8mb4 NOT NULL,
    `employer_logo` longtext CHARACTER SET utf8mb4 NULL,
    `salary_currency` longtext CHARACTER SET utf8mb4 NULL,
    `salary_from` bigint NULL,
    `salary_to` bigint NULL,
    `snippet_requirement` longtext CHARACTER SET utf8mb4 NULL,
    `snippet_responsibility` longtext CHARACTER SET utf8mb4 NULL,
    `experience` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_vacancies` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240705142015_InitialMigration', '8.0.6');

COMMIT;

START TRANSACTION;

ALTER TABLE `vacancies` MODIFY COLUMN `employer_url` longtext CHARACTER SET utf8mb4 NULL;

ALTER TABLE `vacancies` MODIFY COLUMN `employer_name` longtext CHARACTER SET utf8mb4 NULL;

ALTER TABLE `vacancies` MODIFY COLUMN `id` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240705143417_FixedIdString', '8.0.6');

COMMIT;

START TRANSACTION;

ALTER TABLE `vacancies` ADD `timestamp` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240705203222_TimestampsAdded', '8.0.6');

COMMIT;

START TRANSACTION;

ALTER TABLE `vacancies` MODIFY COLUMN `timestamp` bigint NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240705203459_TimestampsChangedToLong', '8.0.6');

COMMIT;

