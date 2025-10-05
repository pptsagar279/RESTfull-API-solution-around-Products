# Authentication Guide - Product API

## How Authentication Works

Our Product API uses JWT tokens for authentication. Here's how it works in simple terms:

### 1. Login Process
When a user wants to access the API, they first need to log in:

POST /api/v1/auth/login
{
    "username": "admin",
    "password": "password123"
}

If credentials are correct, you get back:
json
{
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "550e8400-e29b-41d4-a716-446655440000",
    "expiresIn": 3600,
    "tokenType": "Bearer"
}

### 2. Using the Token
Include the access token in all API requests:

Authorization: Bearer <your_access_token>

### 3. Token Refresh
When your access token expires (after 1 hour), use the refresh token to get a new one:

POST /api/v1/auth/refreshtoken
{
    "accessToken": "expired_token_here",
    "refreshToken": "your_refresh_token_here"
}

## User Roles

We have 4 different user types with different permissions:

| Role | What they can do |
|------|------------------|
| **Admin** | Everything - read, create, update, delete |
| **Manager** | Read and write (create/update) |
| **User** | Read and limited write |
| **ReadOnly** | Only read data |

## Test Users

For testing, you can use these accounts:

| Username | Password | Role |
|----------|----------|------|
| admin | password123 | Admin |
| manager | password123 | Manager |
| user | password123 | User |
| readonly | password123 | ReadOnly |

## Quick Examples

### Login with cURL
```bash
curl -X POST "https://localhost:7000/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password123"}'
```

### Get products (requires authentication)
```bash
curl -X GET "https://localhost:7000/api/v1/products" \
  -H "Authorization: Bearer <your_token_here>"
```

### Refresh your token
```bash
curl -X POST "https://localhost:7000/api/v1/auth/refreshtoken" \
  -H "Content-Type: application/json" \
  -d '{"accessToken":"<expired_token>","refreshToken":"<refresh_token>"}'
```

## Important Notes

- **Token expires in 1 hour** - use refresh token to get new one
- **All API endpoints except login/refresh require authentication**
- **Different roles have different permissions** - check what your role can do
- **Use HTTPS in production** - current setup is for development

## Swagger Documentation

You can test the API directly in your browser at `https://localhost:7000` (Swagger UI). Click "Authorize" and enter your Bearer token to test protected endpoints.

## Security Notes

**Current setup is for development only:**
- Users are hardcoded (not from database)
- Passwords are not hashed
- No rate limiting on login attempts

**For production, you'd want:**
- Database user storage
- Password hashing (bcrypt)
- Rate limiting
- Token blacklisting
- HTTPS enforcement
