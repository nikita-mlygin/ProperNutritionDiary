DROP DATABASE IF EXISTS dev;
CREATE DATABASE IF NOT EXISTS dev;
CREATE USER IF NOT EXISTS 'user'@'%' IDENTIFIED BY 'secret';
GRANT ALL PRIVILEGES ON dev.* TO 'user'@'%' WITH GRANT OPTION;
USE dev;

CREATE TABLE `product`
(
    num int not null auto_increment,
    id binary(16) not null,
    name varchar(255) not null,
    calories decimal not null,
    proteins decimal not null,
    fats decimal not null,
    carbohydrates decimal not null,
    external_source_type int null,
    external_source varchar(255),
    owner binary(16) null,
    created_at datetime not null default now(),
    updated_at datetime null default null,
    view_count int default 0,
    add_count int default 0,
    primary key (id),
    unique (num)
);

CREATE TABLE `favorite_product`
(
    user_id binary(16) not null,
    product_id binary(16) not null,
    added_at datetime not null default now(),
    primary key(user_id, product_id),
    foreign key (product_id) references product (id)
);

CREATE TABLE `user_product_statistics`
(
    user_id binary(16) not null,
    product_id binary(16) not null,
    view_count int default 0,
    add_count int default 0,
    primary key(user_id, product_id),
    foreign key (product_id) references product (id)
);