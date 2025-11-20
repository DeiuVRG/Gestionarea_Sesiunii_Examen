#!/bin/bash
# Test API Endpoints

echo "======================================"
echo "📡 TEST EXAM SCHEDULING API"
echo "======================================"
echo ""

BASE_URL="http://localhost:5001/api"

echo "1️⃣ GET /api/exams/rooms - Toate sălile"
echo "--------------------------------------"
curl -s "$BASE_URL/exams/rooms" | python3 -m json.tool
echo ""
echo ""

echo "2️⃣ GET /api/exams - Toate examenele"
echo "--------------------------------------"
curl -s "$BASE_URL/exams" | python3 -m json.tool
echo ""
echo ""

echo "3️⃣ GET /api/students/registrations - Toate înregistrările"
echo "--------------------------------------"
curl -s "$BASE_URL/students/registrations" | python3 -m json.tool
echo ""
echo ""

echo "4️⃣ POST /api/students/register - Înregistrare student nou"
echo "--------------------------------------"
curl -s -X POST "$BASE_URL/students/register" \
  -H "Content-Type: application/json" \
  -d '{"studentRegistrationNumber":"LM77777","courseCode":"PSSC","examDate":"2026-06-15"}' | python3 -m json.tool
echo ""
echo ""

echo "5️⃣ GET /api/grades - Toate notele"
echo "--------------------------------------"
curl -s "$BASE_URL/grades" | python3 -m json.tool
echo ""
echo ""

echo "6️⃣ POST /api/grades - Publicare note"
echo "--------------------------------------"
curl -s -X POST "$BASE_URL/grades" \
  -H "Content-Type: application/json" \
  -d '{"courseCode":"PSSC","examDate":"2026-06-15","grades":[{"studentRegistrationNumber":"LM77777","grade":9.50}]}' | python3 -m json.tool
echo ""
echo ""

echo "7️⃣ GET /api/grades/student/LM77777 - Notele studentului"
echo "--------------------------------------"
curl -s "$BASE_URL/grades/student/LM77777" | python3 -m json.tool
echo ""
echo ""

echo "======================================"
echo "✅ Testare completă!"
echo "======================================"
