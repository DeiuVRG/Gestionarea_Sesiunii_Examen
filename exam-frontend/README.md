# 🎓 Exam Scheduling Frontend

Frontend React minimalist cu Material-UI pentru sistemul de gestionare a sesiunii de examene.

## 🚀 Quick Start

```bash
# Instalare dependențe
npm install

# Pornire development server
npm start
```

Aplicația va rula pe **http://localhost:3000**

## ⚙️ Configurare

**Important:** Asigură-te că API-ul backend rulează pe `http://localhost:5001`

```bash
# În alt terminal, pornește backend-ul
cd ../Laborator4-AI
dotnet run --urls="http://localhost:5001"
```

## 🎨 Funcționalități

### 📋 Examene
- Vizualizare examene programate
- Informații despre săli și capacități
- Status înregistrări (câți studenți înregistrați)

### 👨‍🎓 Studenți
- **Formular înregistrare** - Înregistrare student la examen
- **Tabel înregistrări** - Toate înregistrările cu detalii complete
- Validare automată și feedback instant

### 📊 Note
- **Publicare note** - Formular pentru introducere și publicare note
- **Căutare** - Căutare note după număr matricol
- **Vizualizare** - Tabel cu toate notele și status promovare
- **Color coding** - Verde pentru promovat, roșu pentru nepromovat

### 🏫 Săli
- Grid cu toate sălile disponibile
- Capacitate fiecare sală
- Design card-uri hover effect
- Capacitate totală

## 🛠️ Tehnologii

- **React 18.2** - Framework UI
- **Material-UI 5.14** - Component library
- **Axios** - HTTP client pentru API calls
- **Emotion** - CSS-in-JS styling

## 📁 Structură Proiect

```
exam-frontend/
├── public/
│   └── index.html
├── src/
│   ├── components/
│   │   ├── ExamsView.js      # View examene
│   │   ├── StudentsView.js    # View înregistrări + form
│   │   ├── GradesView.js      # View note + publicare
│   │   └── RoomsView.js       # View săli
│   ├── api.js                 # API client (axios)
│   ├── App.js                 # Main app cu tabs
│   └── index.js              # Entry point
└── package.json
```

## 🎯 API Endpoints Utilizate

- `GET /api/exams` - Lista examene
- `GET /api/exams/rooms` - Săli disponibile
- `GET /api/students/registrations` - Înregistrări studenți
- `POST /api/students/register` - Înregistrare student nou
- `GET /api/grades` - Toate notele
- `GET /api/grades/student/{studentNumber}` - Note student
- `POST /api/grades` - Publicare note

## 🎨 Design Principles

- **Minimalist** - Design curat, fără clutter
- **Intuitive** - Navigare simplă prin tabs
- **Responsive** - Funcționează pe toate screen sizes
- **Material Design** - Urmează guidelines-urile Google
- **Color coded** - Feedback vizual pentru actions

## 📱 Features

✅ **Real-time updates** - Refresh automat după operații  
✅ **Error handling** - Mesaje de eroare friendly  
✅ **Success notifications** - Snackbar pentru confirmări  
✅ **Form validation** - Validare client-side  
✅ **Loading states** - Spinners pentru loading  
✅ **Empty states** - Mesaje când nu există date  

## 🔧 Development

```bash
# Install dependencies
npm install

# Start dev server (port 3000)
npm start

# Build for production
npm run build
```

## 🌐 CORS

Backend-ul are CORS activat pentru `http://localhost:3000`, deci nu sunt probleme de cross-origin.

## 📸 Preview

**Tabs disponibile:**
1. 📅 **Examene** - Card-uri cu examene programate
2. 👥 **Studenți** - Formular + tabel înregistrări
3. 📊 **Note** - Publicare + vizualizare note
4. 🏫 **Săli** - Grid cu săli disponibile

## 🎓 Universitatea Politehnica Timișoara
PSSC - Laboratorul 4 - Sistem Gestionare Examene
