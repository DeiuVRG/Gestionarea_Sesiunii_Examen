import axios from 'axios';

const API_BASE_URL = 'http://localhost:5001/api';
const NOTIFICATIONS_API_URL = 'http://localhost:5002/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

const notificationsApiClient = axios.create({
  baseURL: NOTIFICATIONS_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const examApi = {
  // Exams
  getAllExams: () => api.get('/exams'),
  getExamById: (id) => api.get(`/exams/${id}`),
  getRooms: () => api.get('/exams/rooms'),

  // Students
  getAllRegistrations: () => api.get('/students/registrations'),
  getStudentRegistrations: (studentNumber) => 
    api.get(`/students/${studentNumber}/registrations`),
  registerStudent: (data) => api.post('/students/register', data),

  // Grades
  getAllGrades: () => api.get('/grades'),
  getStudentGrades: (studentNumber) => 
    api.get(`/grades/student/${studentNumber}`),
  getExamGrades: (courseCode, examDate) => 
    api.get(`/grades/exam/${courseCode}/${examDate}`),
  publishGrades: (data) => api.post('/grades', data),

  // Scheduling (NEW - triggers notification with Polly retry)
  scheduleExam: (data) => api.post('/scheduling/schedule-exam', data),
};

export const notificationsApi = {
  // Notifications API (port 5002)
  getAllNotifications: () => notificationsApiClient.get('/notifications'),
  getHealth: () => notificationsApiClient.get('/notifications/health'),
  clearNotifications: () => notificationsApiClient.delete('/notifications'),
};

export default api;
