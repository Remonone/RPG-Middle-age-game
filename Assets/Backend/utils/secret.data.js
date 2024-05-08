import dotenv from 'dotenv';

dotenv.config();

export const MONGO_DB_URL = process.env["MONGO_DB_URL"];
export const ENCRYPTION_KEY = process.env["ENCRYPTION_KEY"];
export const CRYPT_SALT = process.env["CRYPT_SALT"];
export const DEFAULT_SCENE = process.env["DEFAULT_SCENE"];