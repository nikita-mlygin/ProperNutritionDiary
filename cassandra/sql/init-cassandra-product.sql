CREATE ROLE IF NOT EXISTS user WITH SUPERUSER = true AND LOGIN = true AND PASSWORD = 'secret';

CREATE KEYSPACE IF NOT EXISTS product WITH REPLICATION = {'class': 'SimpleStrategy', 'replication_factor': 1};

USE product;

CREATE TABLE IF NOT EXISTS product_view
(
    user_id UUID,
    product_id UUID,
    id UUID,
    view_at TIMESTAMP,
    PRIMARY KEY ((user_id, product_id), id)
);

CREATE TABLE IF NOT EXISTS product_add
(
    user_id UUID,
    product_id UUID,
    id UUID,
    add_at TIMESTAMP,
    PRIMARY KEY ((user_id, product_id), id)
);