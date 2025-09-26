CREATE DATABASE `budgettrackerdb`
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE `budgettrackerdb`;

CREATE TABLE `categories` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `subcategories` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `CategoryId` INT NOT NULL,
    `Name` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_Subcategories_CategoryId` (`CategoryId`),
    CONSTRAINT `FK_Subcategories_Categories_CategoryId` 
        FOREIGN KEY (`CategoryId`) 
        REFERENCES `categories` (`Id`) 
        ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `records` (
    `Id` CHAR(36) NOT NULL,
    `UserId` VARCHAR(450) NOT NULL,
    `Amount` DECIMAL(18,2) NOT NULL,
    `Currency` VARCHAR(3) NOT NULL DEFAULT 'USD',
    `CategoryId` INT NOT NULL,
    `SubcategoryId` INT NOT NULL,
    `Time` DATETIME NOT NULL,
    `Description` VARCHAR(500) NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_Records_UserId` (`UserId`),
    INDEX `IX_Records_UserId_Time` (`UserId`, `Time`),
    INDEX `IX_Records_CategoryId` (`CategoryId`),
    INDEX `IX_Records_SubcategoryId` (`SubcategoryId`),
    CONSTRAINT `FK_Records_Categories_CategoryId` 
        FOREIGN KEY (`CategoryId`) 
        REFERENCES `categories` (`Id`) 
        ON DELETE RESTRICT,
    CONSTRAINT `FK_Records_Subcategories_SubcategoryId` 
        FOREIGN KEY (`SubcategoryId`) 
        REFERENCES `subcategories` (`Id`) 
        ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `budgets` (
    `Id` CHAR(36) NOT NULL,
    `UserId` VARCHAR(450) NOT NULL,
    `Year` INT NOT NULL,
    `Month` INT NOT NULL,
    `SubcategoryId` INT NOT NULL,
    `PlannedAmount` DECIMAL(18,2) NOT NULL,
    `Currency` VARCHAR(3) NOT NULL,
    `ActualSpent` DECIMAL(18,2) NOT NULL,
    `ActualSpentCurrency` VARCHAR(3) NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_Budgets_UserId` (`UserId`),
    INDEX `IX_Budgets_SubcategoryId` (`SubcategoryId`),
    CONSTRAINT `FK_Budgets_Subcategories_SubcategoryId` 
        FOREIGN KEY (`SubcategoryId`) 
        REFERENCES `subcategories` (`Id`) 
        ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `categories` (`Id`, `Name`) VALUES
(1, 'Income'),
(2, 'Expenses');

INSERT INTO `subcategories` (`Id`, `CategoryId`, `Name`) VALUES
(1, 1, 'Salary'),
(2, 1, 'Freelance'),
(3, 1, 'Investments'),
(4, 1, 'Other'),
(5, 2, 'Food'),
(6, 2, 'Transportation'),
(7, 2, 'Shopping'),
(8, 2, 'Entertainment'),
(9, 2, 'Bills'),
(10, 2, 'Healthcare'),
(11, 2, 'Education'),
(12, 2, 'Travel'),
(13, 2, 'Other');