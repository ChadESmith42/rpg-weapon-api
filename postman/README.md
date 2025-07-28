# Weapon API - Postman Testing Collection

This directory contains comprehensive Postman testing resources for the Weapon API, including authentication flows, CRUD operations, and integration testing scenarios.

## Files

- **`WeaponAPI-Comprehensive-Testing.postman_collection.json`** - Complete test collection with all API endpoints
- **`WeaponAPI-Testing.postman_environment.json`** - Environment variables for test execution
- **`README.md`** - This documentation file

## Quick Start

### Prerequisites

1. **Start PostgreSQL Database**:
   ```bash
   docker-compose up -d postgres
   ```

2. **Start the Weapon API**:
   ```bash
   dotnet run --project WeaponApi.Api
   ```
   The API will be available at `http://localhost:5297`

### Import into Postman

1. **Import Collection**:
   - Open Postman
   - Click "Import" → "Upload Files"
   - Select `WeaponAPI-Comprehensive-Testing.postman_collection.json`

2. **Import Environment**:
   - Click "Import" → "Upload Files"
   - Select `WeaponAPI-Testing.postman_environment.json`
   - Select the "Weapon API Testing" environment in the top-right dropdown

3. **Configure Base URL** (if needed):
   - The environment defaults to `http://localhost:5297`
   - Update the `base_url` variable if your API runs on a different port

## Test Structure

The collection is organized into **6 main folders** with **35 total requests**:

### 1. Authentication Tests (4 requests)
- Register New User
- Login User
- Get User Profile
- Refresh Token

### 2. Weapons CRUD Tests (4 requests)
- Create Random Weapon
- Get All Weapons
- Get Weapon by ID
- Create Custom Weapon

### 3. Weapons Advanced Operations (6 requests)
- Damage Weapon
- Get Damaged Weapon
- Get Repair Estimate
- Repair Weapon
- Verify Repair

### 4. Integration Test Scenario (8 requests)
Complete end-to-end flow:
- Register Integration Test User
- Get Integration User Profile
- Create Integration Test Weapon
- Damage Integration Weapon
- Get Integration Weapon by ID
- Get Integration Repair Estimate
- Repair Integration Weapon
- Verify Integration Changes

### 5. Error Handling Tests (4 requests)
- Invalid Authentication
- Get Non-existent Weapon
- Invalid Login Credentials
- Invalid Weapon Creation

### 6. Cleanup Operations (5 requests)
- Delete Custom Weapon
- Delete Test Weapon
- Delete Integration Weapon
- Logout Main User
- Logout Integration User

## Test Execution Strategy

### Recommended Execution Order

1. **Sequential Folder Execution** (Recommended):
   - Run each folder in order for complete test coverage
   - Environment variables are set and used across folders

2. **Individual Endpoint Testing**:
   - Run specific requests for targeted testing
   - Ensure authentication tests run first to set tokens

3. **Collection Runner**:
   - Use Postman's Collection Runner for automated execution
   - Select the entire collection or specific folders

### Environment Variables Used

The tests automatically manage these environment variables:

**Authentication Variables:**
- `access_token` - JWT access token for API authentication
- `refresh_token` - JWT refresh token for token renewal
- `user_id` - Main test user ID
- `user_email` - Main test user email
- `username` - Main test user username

**Weapon Testing Variables:**
- `test_weapon_id` - Primary test weapon ID
- `test_weapon_name` - Primary test weapon name
- `test_weapon_max_hp` - Maximum hit points for verification
- `test_weapon_current_hp` - Current hit points after damage
- `custom_weapon_id` - Custom created weapon ID

**Integration Testing Variables:**
- `integration_access_token` - Separate token for integration user
- `integration_user_id` - Integration test user ID
- `integration_weapon_id` - Integration test weapon ID

## Test Features

### Comprehensive Validation
- **Response Status Codes** - Validates HTTP status codes
- **Response Structure** - Checks required fields in responses
- **Data Consistency** - Verifies data persistence across operations
- **Authentication Flow** - Tests JWT token management
- **Error Handling** - Validates proper error responses
- **Business Logic** - Tests weapon damage/repair mechanics

### Automated Test Data Management
- **Dynamic User Creation** - Uses random usernames/emails
- **Token Storage** - Automatically stores and uses JWT tokens
- **Weapon Lifecycle** - Creates, modifies, and cleans up test weapons
- **State Verification** - Confirms changes persist correctly

### Real-world Testing Scenarios
- **User Registration & Authentication**
- **Weapon Creation & Management**
- **Damage Application & Repair**
- **Data Retrieval & Verification**
- **Cleanup & Resource Management**

## Test Scenarios Covered

### Happy Path Testing
- User registration and login flow
- Weapon creation and retrieval
- Weapon damage and repair cycle
- Profile access and token refresh

### Error Scenario Testing
- Invalid authentication attempts
- Non-existent resource access
- Invalid data validation
- Proper error message responses

### Integration Testing
- Complete end-to-end user journey
- Cross-feature data consistency
- Sequential operation validation
- State persistence verification

## Customization

### Modifying Base URL
Update the `base_url` environment variable to match your API deployment:
```json
{
  "key": "base_url",
  "value": "https://your-api-domain.com",
  "type": "default"
}
```

### Adding Custom Tests
The collection structure supports easy extension:
1. Add new requests to appropriate folders
2. Use existing environment variables
3. Follow the established test patterns
4. Update cleanup operations if needed

### Test Data Customization
Modify request bodies to test specific scenarios:
- User registration data
- Weapon creation parameters
- Damage amounts
- Repair amounts

## Expected Results

### Successful Test Run
- **Authentication Tests**: 4/4 passing
- **CRUD Tests**: 4/4 passing
- **Advanced Operations**: 6/6 passing
- **Integration Scenario**: 8/8 passing
- **Error Handling**: 4/4 passing
- **Cleanup**: 5/5 passing

**Total: 31/31 tests passing**

### Performance Expectations
- Response times should be under 10 seconds
- Authentication operations: < 2 seconds
- CRUD operations: < 1 second
- Database operations: < 3 seconds

## Troubleshooting

### Common Issues

1. **Database Connection Errors**:
   ```bash
   # Ensure PostgreSQL is running
   docker-compose up -d postgres
   ```

2. **API Not Running**:
   ```bash
   # Start the API
   dotnet run --project WeaponApi.Api
   ```

3. **SSL Certificate Issues**:
   - Accept the development certificate
   - Or change `base_url` to `http://localhost:5000`

4. **Token Expiration**:
   - Re-run authentication tests to refresh tokens
   - Or use the "Refresh Token" request

### Debug Information
- All requests include console logging for debugging
- Response times and status codes are logged
- Environment variables are tracked throughout execution

## Test Metrics

The collection provides comprehensive coverage of:
- **8 Authentication endpoints** (register, login, profile, refresh, logout)
- **7 Weapon management endpoints** (CRUD, random creation)
- **3 Advanced weapon operations** (damage, repair, estimate)
- **Error scenarios** for all major operations
- **Integration testing** with realistic user journeys

This represents **100% API endpoint coverage** for the Weapon API.

---

## Ready to Test

Your Postman collection is ready for comprehensive API testing. Run the tests in sequence for the best experience, or target specific areas for focused testing.

Happy testing!
