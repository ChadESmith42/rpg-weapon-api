# 🧪 API Testing with Postman

This document explains how to use the comprehensive Postman testing collection for the Weapon API.

## 📁 Testing Resources

All Postman testing resources are located in the `/postman` directory:

```
postman/
├── WeaponAPI-Comprehensive-Testing.postman_collection.json  # Main test collection
├── WeaponAPI-Testing.postman_environment.json             # Environment variables
├── README.md                                               # Detailed documentation
└── setup.sh                                               # Automated setup script
```

## 🚀 Quick Start

### Option 1: Automated Setup (Recommended)

```bash
# Run the automated setup script
./postman/setup.sh
```

This script will:
- ✅ Check prerequisites (Docker, .NET)
- ✅ Start PostgreSQL database
- ✅ Build and start the Weapon API
- ✅ Provide Postman import instructions

### Option 2: Manual Setup

1. **Start Infrastructure**:
   ```bash
   docker-compose up -d postgres
   dotnet run --project WeaponApi.Api
   ```

2. **Import into Postman**:
   - Import `postman/WeaponAPI-Comprehensive-Testing.postman_collection.json`
   - Import `postman/WeaponAPI-Testing.postman_environment.json`
   - Select "Weapon API Testing" environment

## 📋 Test Collection Overview

The collection includes **35 requests** across **6 folders**:

### 🔐 Authentication Tests (4 requests)
- User registration and login
- Profile access and JWT token management
- Token refresh functionality

### ⚔️ Weapons CRUD Tests (4 requests)
- Create random and custom weapons
- Retrieve all weapons and individual weapons
- Basic CRUD operation validation

### 🛠️ Weapons Advanced Operations (6 requests)
- Weapon damage application
- Repair cost estimation
- Weapon repair functionality
- State persistence verification

### 🔄 Integration Test Scenario (8 requests)
Complete end-to-end user journey:
- Register user → Get profile → Create weapon → Damage → Repair → Verify

### ❌ Error Handling Tests (4 requests)
- Invalid authentication scenarios
- Non-existent resource handling
- Input validation testing
- Proper error response verification

### 🧹 Cleanup Operations (5 requests)
- Remove test weapons
- User logout operations
- Environment cleanup

## 🎯 Key Testing Features

### Automated Test Data Management
- **Dynamic User Creation**: Random usernames and emails
- **JWT Token Storage**: Automatic token capture and reuse
- **Weapon Lifecycle**: Create, modify, verify, cleanup
- **Environment Variables**: 19 variables managed automatically

### Comprehensive Validation
- HTTP status code verification
- Response structure validation
- Data consistency checking
- Business logic testing
- Performance monitoring (< 10s response times)

### Real-world Scenarios
- Complete authentication flows
- Weapon damage and repair cycles
- Cross-request data persistence
- Error condition handling

## 📊 Expected Test Results

**Successful Test Run**: 31/31 tests passing ✅

- Authentication: 4/4 ✅
- CRUD Operations: 4/4 ✅
- Advanced Operations: 6/6 ✅
- Integration Scenario: 8/8 ✅
- Error Handling: 4/4 ✅
- Cleanup: 5/5 ✅

## 🔧 Configuration

### Environment Variables

The testing environment automatically manages:

```json
{
  "base_url": "https://localhost:5001",
  "access_token": "(set by tests)",
  "test_weapon_id": "(set by tests)",
  "user_id": "(set by tests)"
  // ... and 15 more variables
}
```

### Base URL Configuration

Update `base_url` for different environments:
- Development: `https://localhost:5001`
- Custom Port: `https://localhost:YOUR_PORT`
- HTTP: `http://localhost:5000`

## 🚦 Execution Strategies

### 1. Sequential Folder Execution (Recommended)
Run folders in order for complete test coverage:
1. Authentication Tests
2. Weapons CRUD Tests
3. Weapons Advanced Operations
4. Integration Test Scenario
5. Error Handling Tests
6. Cleanup Operations

### 2. Collection Runner
Use Postman's Collection Runner for automated execution:
- Select entire collection or specific folders
- Configure iterations and delays as needed
- Review detailed test results

### 3. Individual Request Testing
Target specific endpoints for focused testing:
- Ensure authentication tests run first
- Use environment variables from previous requests

## 🐛 Troubleshooting

### Common Issues

**Database Connection Failed**:
```bash
# Ensure PostgreSQL is running
docker-compose up -d postgres
```

**API Not Responding**:
```bash
# Check if API is running
curl -k https://localhost:5001/health

# Restart if needed
dotnet run --project WeaponApi.Api
```

**SSL Certificate Issues**:
- Accept development certificates
- Or switch to HTTP: change `base_url` to `http://localhost:5000`

**Token Expiration**:
- Re-run "Authentication Tests" folder
- Or use "Refresh Token" request

### Debug Information

All requests include comprehensive logging:
- Request/response timing
- Status codes and response validation
- Environment variable tracking
- Console output for debugging

## 📈 Test Coverage

The collection provides **100% API endpoint coverage**:

**Authentication Endpoints (5)**:
- ✅ POST `/api/auth/register`
- ✅ POST `/api/auth/login`
- ✅ GET `/api/auth/profile`
- ✅ POST `/api/auth/refresh`
- ✅ POST `/api/auth/logout`

**Weapon Management Endpoints (8)**:
- ✅ GET `/api/weapons`
- ✅ POST `/api/weapons`
- ✅ GET `/api/weapons/{id}`
- ✅ DELETE `/api/weapons/{id}`
- ✅ GET `/api/weapons/create-random`
- ✅ POST `/api/weapons/{id}/damage`
- ✅ POST `/api/weapons/{id}/repair`
- ✅ POST `/api/weapons/estimate-repair`

## 🎉 Benefits

### For Developers
- **Rapid Testing**: Complete API validation in minutes
- **Regression Testing**: Catch breaking changes quickly
- **Documentation**: Living examples of API usage
- **Debugging**: Detailed request/response logging

### For QA Teams
- **Automated Testing**: Consistent test execution
- **Error Scenarios**: Comprehensive edge case coverage
- **Data Validation**: Verify business logic compliance
- **Performance Monitoring**: Response time tracking

### For DevOps
- **CI/CD Integration**: Automated API testing in pipelines
- **Environment Validation**: Verify deployment health
- **Monitoring**: Ongoing API health checks
- **Documentation**: Self-documenting API behavior

## 📚 Additional Resources

- **Detailed Documentation**: See `postman/README.md`
- **Setup Script**: Use `postman/setup.sh` for automation
- **API Documentation**: Available at `/swagger` when API is running
- **Troubleshooting**: Check `api.log` for API issues

---

## 🚀 Ready to Test!

Your comprehensive Postman testing environment is ready. Choose your preferred execution method and start validating your Weapon API!

For detailed instructions and advanced usage, see the complete documentation in `postman/README.md`.
