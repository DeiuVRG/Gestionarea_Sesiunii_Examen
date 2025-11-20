#!/bin/bash

# Script pentru pornirea ambelor API-uri

echo "╔═══════════════════════════════════════════════════════════════════╗"
echo "║   Starting Exam Scheduling System - Two APIs with Polly Retry    ║"
echo "╚═══════════════════════════════════════════════════════════════════╝"
echo ""

# Culori pentru output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

WORKSPACE="/Users/deiuvrg/Library/CloudStorage/OneDrive-UniversitateaPolitehnicaTimisoara/An4IS/PSSC/Gestionarea_Sesiunii_examen"

# Verifică dacă porturile sunt libere
echo -e "${YELLOW}Checking ports...${NC}"
if lsof -Pi :5001 -sTCP:LISTEN -t >/dev/null ; then
    echo -e "${YELLOW}⚠️  Port 5001 is in use. Killing process...${NC}"
    lsof -ti:5001 | xargs kill -9 2>/dev/null
fi

if lsof -Pi :5002 -sTCP:LISTEN -t >/dev/null ; then
    echo -e "${YELLOW}⚠️  Port 5002 is in use. Killing process...${NC}"
    lsof -ti:5002 | xargs kill -9 2>/dev/null
fi

echo -e "${GREEN}✅ Ports 5001 and 5002 are available${NC}"
echo ""

# Pornește Notifications API (port 5002) în background
echo -e "${BLUE}Starting Notifications API on port 5002...${NC}"
cd "$WORKSPACE"
osascript -e 'tell application "Terminal" to do script "cd \"'$WORKSPACE'\" && dotnet run --project \"./NotificationsAPI/NotificationsAPI.csproj\" --urls \"http://localhost:5002\""'

sleep 3

# Pornește Main API (port 5001) în background
echo -e "${BLUE}Starting Main API (Laborator4-AI) on port 5001...${NC}"
osascript -e 'tell application "Terminal" to do script "cd \"'$WORKSPACE'\" && dotnet run --project \"./Laborator4-AI/Laborator4-AI.csproj\" --urls \"http://localhost:5001\""'

sleep 3

echo ""
echo -e "${GREEN}═══════════════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}✅ Both APIs are starting in separate Terminal windows!${NC}"
echo -e "${GREEN}═══════════════════════════════════════════════════════════════════${NC}"
echo ""
echo -e "📬 ${BLUE}Notifications API:${NC} http://localhost:5002"
echo -e "   - Swagger UI: http://localhost:5002/"
echo -e "   - Health Check: http://localhost:5002/api/notifications/health"
echo ""
echo -e "🚀 ${BLUE}Main API (Laborator4-AI):${NC} http://localhost:5001"
echo -e "   - Swagger UI: http://localhost:5001/"
echo -e "   - Schedule Exam: POST http://localhost:5001/api/Scheduling/schedule-exam"
echo ""
echo -e "${YELLOW}Wait ~5 seconds for both APIs to fully start, then test:${NC}"
echo ""
echo -e "${GREEN}# Test Notifications API health:${NC}"
echo "curl http://localhost:5002/api/notifications/health"
echo ""
echo -e "${GREEN}# Schedule an exam (triggers notification with Polly retry):${NC}"
echo 'curl -X POST http://localhost:5001/api/Scheduling/schedule-exam \'
echo '  -H "Content-Type: application/json" \'
echo '  -d '"'"'{'
echo '    "courseCode": "PSSC",'
echo '    "proposedDate1": "2025-12-20",'
echo '    "proposedDate2": "2025-12-21",'
echo '    "proposedDate3": "2025-12-22",'
echo '    "durationMinutes": 120,'
echo '    "expectedStudents": 30'
echo '  }'"'"
echo ""
echo -e "${GREEN}# Check received notifications:${NC}"
echo "curl http://localhost:5002/api/notifications"
echo ""
echo -e "${YELLOW}To stop:${NC} Close the Terminal windows or run: lsof -ti:5001,5002 | xargs kill -9"
echo ""
