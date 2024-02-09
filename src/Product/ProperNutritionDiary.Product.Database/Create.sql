DROP DATABASE IF EXISTS dev;
CREATE DATABASE IF NOT EXISTS dev;
CREATE USER IF NOT EXISTS 'user'@'%' IDENTIFIED BY 'secret';
GRANT ALL PRIVILEGES ON dev.* TO 'user'@'%' WITH GRANT OPTION;
USE dev;

CREATE TABLE `product`
(
    id binary(16) not null,
    name varchar(255) not null,
    calories decimal not null,
    proteins decimal not null,
    fats decimal not null,
    carbohydrates decimal not null,
    owner binary(16) null,
    primary key (id)
);