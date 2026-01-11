# Student Housing API - Test Examples

This file contains curl commands to test all API endpoints.

## 1. Authentication

### Login as Student
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "password123"
  }'
```

### Login as Staff
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "jane@example.com",
    "password": "password123"
  }'
```

### Login as Maintenance
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "mike@example.com",
    "password": "password123"
  }'
```

## 2. Issues (Student)

### Get My Issues
```bash
# Save student token
STUDENT_TOKEN=$(curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"john@example.com","password":"password123"}' -s | \
  python3 -c "import sys, json; print(json.load(sys.stdin)['token'])")

# Get issues
curl -X GET http://localhost:5000/api/issues \
  -H "Authorization: Bearer $STUDENT_TOKEN"
```

### Create New Issue
```bash
curl -X POST http://localhost:5000/api/issues \
  -H "Authorization: Bearer $STUDENT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "description": "Broken window in room 305",
    "sharedSpace": "Hallway",
    "photoProof": "base64encodedimage",
    "buildingId": 1
  }'
```

### Get Specific Issue
```bash
curl -X GET http://localhost:5000/api/issues/1 \
  -H "Authorization: Bearer $STUDENT_TOKEN"
```

## 3. Issues (Staff)

### Get All Issues
```bash
# Save staff token
STAFF_TOKEN=$(curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"jane@example.com","password":"password123"}' -s | \
  python3 -c "import sys, json; print(json.load(sys.stdin)['token'])")

# Get all issues
curl -X GET http://localhost:5000/api/issues \
  -H "Authorization: Bearer $STAFF_TOKEN"
```

### Filter Issues by Status
```bash
curl -X GET "http://localhost:5000/api/issues?status=Open" \
  -H "Authorization: Bearer $STAFF_TOKEN"
```

### Filter Issues by Building
```bash
curl -X GET "http://localhost:5000/api/issues?buildingId=1" \
  -H "Authorization: Bearer $STAFF_TOKEN"
```

### Assign Issue to Maintenance
```bash
curl -X PATCH http://localhost:5000/api/issues/1 \
  -H "Authorization: Bearer $STAFF_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "InProgress",
    "assignedToUserId": 3
  }'
```

### Update Issue Status
```bash
curl -X PATCH http://localhost:5000/api/issues/1 \
  -H "Authorization: Bearer $STAFF_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Resolved"
  }'
```

## 4. Tasks (Student)

### Get My Cleaning Tasks
```bash
curl -X GET http://localhost:5000/api/tasks/cleaning \
  -H "Authorization: Bearer $STUDENT_TOKEN"
```

### Get My Garbage Tasks
```bash
curl -X GET http://localhost:5000/api/tasks/garbage \
  -H "Authorization: Bearer $STUDENT_TOKEN"
```

### Complete Cleaning Task
```bash
curl -X POST http://localhost:5000/api/tasks/cleaning/1/complete \
  -H "Authorization: Bearer $STUDENT_TOKEN"
```

### Complete Garbage Task
```bash
curl -X POST http://localhost:5000/api/tasks/garbage/1/complete \
  -H "Authorization: Bearer $STUDENT_TOKEN"
```

## 5. Tasks (Staff)

### Get All Cleaning Tasks
```bash
curl -X GET http://localhost:5000/api/tasks/cleaning \
  -H "Authorization: Bearer $STAFF_TOKEN"
```

### Get All Garbage Tasks
```bash
curl -X GET http://localhost:5000/api/tasks/garbage \
  -H "Authorization: Bearer $STAFF_TOKEN"
```

### Create Cleaning Task
```bash
curl -X POST http://localhost:5000/api/tasks/cleaning \
  -H "Authorization: Bearer $STAFF_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "description": "Deep clean the lobby area",
    "locationOrSpace": "Main Lobby",
    "buildingId": 1,
    "assignedUserId": 1,
    "dueDate": "2026-01-15T10:00:00Z"
  }'
```

### Create Garbage Task
```bash
curl -X POST http://localhost:5000/api/tasks/garbage \
  -H "Authorization: Bearer $STAFF_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "description": "Take out recycling bins",
    "locationOrSpace": "Recycling area",
    "buildingId": 1,
    "assignedUserId": 1,
    "dueDate": "2026-01-13T08:00:00Z"
  }'
```

### Verify Cleaning Task
```bash
curl -X POST http://localhost:5000/api/tasks/cleaning/1/verify \
  -H "Authorization: Bearer $STAFF_TOKEN"
```

### Verify Garbage Task
```bash
curl -X POST http://localhost:5000/api/tasks/garbage/1/verify \
  -H "Authorization: Bearer $STAFF_TOKEN"
```

## 6. Dashboard (Staff)

### Get Dashboard Statistics
```bash
curl -X GET http://localhost:5000/api/dashboard \
  -H "Authorization: Bearer $STAFF_TOKEN"
```

## Seed Data

### Users
- **Student**: john@example.com / password123 (ID: 1)
- **Staff**: jane@example.com / password123 (ID: 2)
- **Maintenance**: mike@example.com / password123 (ID: 3)

### Building
- **Sunrise Residence** (ID: 1)
  - Address: 123 University Ave, College Town, ST 12345

### Sample Data Created
- 1 issue (Water leak)
- 1 cleaning task (Clean common kitchen)
- 1 garbage task (Take out garbage bins)

## Response Examples

### Successful Login Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "role": "Student",
  "userId": 1,
  "username": "john.student"
}
```

### Issue Response
```json
{
  "id": 1,
  "description": "Water leak in the common bathroom on floor 2",
  "sharedSpace": "Common Bathroom - Floor 2",
  "photoProof": null,
  "status": "Open",
  "createdByUserId": 1,
  "createdByUsername": "john.student",
  "buildingId": 1,
  "buildingName": "Sunrise Residence",
  "assignedToUserId": null,
  "assignedToUsername": null,
  "createdAt": "2026-01-11T18:32:12.057Z",
  "updatedAt": null
}
```

### Dashboard Response
```json
{
  "openIssues": 1,
  "inProgressIssues": 0,
  "resolvedIssues": 0,
  "closedIssues": 0,
  "overdueTasks": 0,
  "tasksDueToday": 0
}
```

## Error Responses

### 401 Unauthorized
```json
{
  "message": "Invalid email or password"
}
```

### 403 Forbidden
No access to resource (role-based authorization failed)

### 404 Not Found
Resource not found

### 400 Bad Request
```json
{
  "message": "Building not found"
}
```
