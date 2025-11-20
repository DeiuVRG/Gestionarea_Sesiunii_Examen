import React, { useState, useEffect } from 'react';
import {
  Box,
  TextField,
  Button,
  Typography,
  Alert,
  CircularProgress,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Snackbar,
} from '@mui/material';
import { PersonAdd as PersonAddIcon } from '@mui/icons-material';
import { examApi } from '../api';

function StudentsView() {
  const [registrations, setRegistrations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });

  const [formData, setFormData] = useState({
    studentRegistrationNumber: '',
    courseCode: '',
    examDate: '',
  });

  useEffect(() => {
    loadRegistrations();
  }, []);

  const loadRegistrations = async () => {
    try {
      setLoading(true);
      const response = await examApi.getAllRegistrations();
      setRegistrations(response.data.data || []);
      setError(null);
    } catch (err) {
      setError('Eroare la încărcarea înregistrărilor: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await examApi.registerStudent(formData);
      setSnackbar({
        open: true,
        message: 'Student înregistrat cu succes!',
        severity: 'success',
      });
      setFormData({ studentRegistrationNumber: '', courseCode: '', examDate: '' });
      loadRegistrations();
    } catch (err) {
      setSnackbar({
        open: true,
        message: 'Eroare la înregistrare: ' + (err.response?.data?.errors?.[0] || err.message),
        severity: 'error',
      });
    }
  };

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom sx={{ mb: 3, fontWeight: 500 }}>
        Înregistrări Studenți
      </Typography>

      {/* Formular înregistrare */}
      <Paper elevation={1} sx={{ p: 3, mb: 3, borderRadius: 2 }}>
        <Typography variant="h6" gutterBottom>
          <PersonAddIcon sx={{ verticalAlign: 'middle', mr: 1 }} />
          Înregistrare Nouă
        </Typography>

        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 2 }}>
          <Box display="flex" gap={2} flexWrap="wrap">
            <TextField
              required
              label="Nr. Matricol"
              name="studentRegistrationNumber"
              value={formData.studentRegistrationNumber}
              onChange={handleChange}
              size="small"
              placeholder="LM12345"
              sx={{ flex: 1, minWidth: 200 }}
            />
            <TextField
              required
              label="Cod Curs"
              name="courseCode"
              value={formData.courseCode}
              onChange={handleChange}
              size="small"
              placeholder="PSSC"
              sx={{ flex: 1, minWidth: 150 }}
            />
            <TextField
              required
              label="Data Examen"
              name="examDate"
              type="date"
              value={formData.examDate}
              onChange={handleChange}
              size="small"
              InputLabelProps={{ shrink: true }}
              sx={{ flex: 1, minWidth: 180 }}
            />
            <Button
              type="submit"
              variant="contained"
              startIcon={<PersonAddIcon />}
              sx={{ minWidth: 150 }}
            >
              Înregistrează
            </Button>
          </Box>
        </Box>
      </Paper>

      {/* Tabel înregistrări */}
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <TableContainer component={Paper} elevation={1} sx={{ borderRadius: 2 }}>
        <Table>
          <TableHead>
            <TableRow sx={{ backgroundColor: 'primary.main' }}>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Nr. Matricol</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Curs</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Data Examen</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Sala</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Înregistrat la</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {registrations.length === 0 ? (
              <TableRow>
                <TableCell colSpan={5} align="center">
                  <Typography color="text.secondary">Nu există înregistrări</Typography>
                </TableCell>
              </TableRow>
            ) : (
              registrations.map((reg) => (
                <TableRow key={reg.id} hover>
                  <TableCell>{reg.studentRegistrationNumber}</TableCell>
                  <TableCell>{reg.courseCode}</TableCell>
                  <TableCell>
                    {new Date(reg.examDate).toLocaleDateString('ro-RO')}
                  </TableCell>
                  <TableCell>{reg.roomNumber}</TableCell>
                  <TableCell>
                    {new Date(reg.registeredAt).toLocaleDateString('ro-RO', {
                      day: '2-digit',
                      month: 'short',
                      hour: '2-digit',
                      minute: '2-digit',
                    })}
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert
          onClose={() => setSnackbar({ ...snackbar, open: false })}
          severity={snackbar.severity}
          sx={{ width: '100%' }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}

export default StudentsView;
