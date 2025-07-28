-- PostgreSQL initialization script for Weapon API
-- This script runs when the PostgreSQL container is first created

-- Create the application user first
CREATE USER weaponapi_user WITH PASSWORD 'weaponapi_dev_password';

-- Ensure the weaponapi database exists
SELECT 'CREATE DATABASE weaponapi'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'weaponapi')\gexec

-- Connect to the weaponapi database
\c weaponapi;

-- Create extensions that might be useful for the application
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Set timezone
SET timezone = 'UTC';

-- Create a custom schema for the application (optional)
-- CREATE SCHEMA IF NOT EXISTS weapon_api;

-- Grant permissions to the application user
GRANT ALL PRIVILEGES ON DATABASE weaponapi TO weaponapi_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO weaponapi_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO weaponapi_user;
GRANT ALL PRIVILEGES ON ALL FUNCTIONS IN SCHEMA public TO weaponapi_user;

-- Set default privileges for future objects
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO weaponapi_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO weaponapi_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON FUNCTIONS TO weaponapi_user;

-- Log successful initialization
\echo 'PostgreSQL database weaponapi initialized successfully'
