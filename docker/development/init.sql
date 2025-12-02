-- docker/development/init.sql
-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create schemas for organization
CREATE SCHEMA IF NOT EXISTS catalog;
CREATE SCHEMA IF NOT EXISTS inventory;
CREATE SCHEMA IF NOT EXISTS sales;
CREATE SCHEMA IF NOT EXISTS purchasing;
CREATE SCHEMA IF NOT EXISTS tax;
CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS audit;

-- Set search path for Egyptian Arabic support
SET search_path TO public, catalog, inventory, sales, purchasing, tax, auth, audit;

-- Enable case-insensitive Arabic/English collation
CREATE COLLATION IF NOT EXISTS arabic_ci (
    provider = icu,
    locale = 'ar-EG-u-ks-level1',
    deterministic = false
);
