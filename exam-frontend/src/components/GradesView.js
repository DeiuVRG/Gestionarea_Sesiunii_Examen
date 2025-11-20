import React, { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  CircularProgress,
  Alert,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  TextField,
  Button,
  Grid,
  Snackbar,
} from '@mui/material';
import { Publish as PublishIcon, Search as SearchIcon } from '@mui/icons-material';
import { examApi } from '../api';

function GradesView() {
  const [grades, setGrades] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchStudent, setSearchStudent] = useState('');
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });

  const [publishForm, setPublishForm] = useState({
    courseCode: '',
    examDate: '',
    studentRegistrationNumber: '',
    grade: '',
  });

  useEffect(() => {
    loadGrades();
  }, []);

  const loadGrades = async () => {
    try {
      setLoading(true);
      const response = await examApi.getAllGrades();
      setGrades(response.data.data || []);
      setError(null);
    } catch (err) {
      setError('Eroare la încărcarea notelor: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handlePublishGrade = async (e) => {
    e.preventDefault();
    try {
      const payload = {
        courseCode: publishForm.courseCode,
        examDate: publishForm.examDate,
        grades: [
          {
            studentRegistrationNumber: publishForm.studentRegistrationNumber,
            grade: parseFloat(publishForm.grade),
          },
        ],
      };

      await examApi.publishGrades(payload);
      setSnackbar({
        open: true,
        message: 'Notă publicată cu succes!',
        severity: 'success',
      });
      setPublishForm({ courseCode: '', examDate: '', studentRegistrationNumber: '', grade: '' });
      loadGrades();
    } catch (err) {
      setSnackbar({
        open: true,
        message: 'Eroare la publicare: ' + (err.response?.data?.errors?.[0] || err.message),
        severity: 'error',
      });
    }
  };

  const handleSearchStudent = async () => {
    if (!searchStudent) {
      loadGrades();
      return;
    }

    try {
      setLoading(true);
      const response = await examApi.getStudentGrades(searchStudent);
      setGrades(response.data.data || []);
      setError(null);
    } catch (err) {
      setError('Eroare la căutare: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  if (loading && grades.length === 0) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom sx={{ mb: 3, fontWeight: 500 }}>
        Note Publicate
      </Typography>

      {/* Formular publicare notă */}
      <Paper elevation={1} sx={{ p: 3, mb: 3, borderRadius: 2 }}>
        <Typography variant="h6" gutterBottom>
          <PublishIcon sx={{ verticalAlign: 'middle', mr: 1 }} />
          Publicare Notă
        </Typography>

        <Box component="form" onSubmit={handlePublishGrade} sx={{ mt: 2 }}>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6} md={3}>
              <TextField
                required
                fullWidth
                label="Cod Curs"
                value={publishForm.courseCode}
                onChange={(e) =>
                  setPublishForm({ ...publishForm, courseCode: e.target.value })
                }
                size="small"
                placeholder="PSSC"
              />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <TextField
                required
                fullWidth
                label="Data Examen"
                type="date"
                value={publishForm.examDate}
                onChange={(e) =>
                  setPublishForm({ ...publishForm, examDate: e.target.value })
                }
                size="small"
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <TextField
                required
                fullWidth
                label="Nr. Matricol"
                value={publishForm.studentRegistrationNumber}
                onChange={(e) =>
                  setPublishForm({
                    ...publishForm,
                    studentRegistrationNumber: e.target.value,
                  })
                }
                size="small"
                placeholder="LM12345"
              />
            </Grid>
            <Grid item xs={12} sm={6} md={2}>
              <TextField
                required
                fullWidth
                label="Notă"
                type="number"
                value={publishForm.grade}
                onChange={(e) =>
                  setPublishForm({ ...publishForm, grade: e.target.value })
                }
                size="small"
                inputProps={{ min: 1, max: 10, step: 0.25 }}
              />
            </Grid>
            <Grid item xs={12} md={1}>
              <Button
                fullWidth
                type="submit"
                variant="contained"
                sx={{ height: '40px' }}
              >
                Publică
              </Button>
            </Grid>
          </Grid>
        </Box>
      </Paper>

      {/* Căutare student */}
      <Box display="flex" gap={2} mb={3}>
        <TextField
          label="Caută după nr. matricol"
          value={searchStudent}
          onChange={(e) => setSearchStudent(e.target.value)}
          size="small"
          placeholder="LM12345"
          sx={{ flex: 1, maxWidth: 300 }}
        />
        <Button
          variant="outlined"
          startIcon={<SearchIcon />}
          onClick={handleSearchStudent}
        >
          Caută
        </Button>
        {searchStudent && (
          <Button
            variant="text"
            onClick={() => {
              setSearchStudent('');
              loadGrades();
            }}
          >
            Resetează
          </Button>
        )}
      </Box>

      {/* Tabel note */}
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <TableContainer component={Paper} elevation={1} sx={{ borderRadius: 2 }}>
        <Table>
          <TableHead>
            <TableRow sx={{ backgroundColor: 'primary.main' }}>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Student</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Curs</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Data Examen</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Notă</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Status</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 600 }}>Publicat la</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {grades.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} align="center">
                  <Typography color="text.secondary">Nu există note</Typography>
                </TableCell>
              </TableRow>
            ) : (
              grades.map((grade) => (
                <TableRow key={grade.id} hover>
                  <TableCell>{grade.studentRegistrationNumber}</TableCell>
                  <TableCell>{grade.courseCode}</TableCell>
                  <TableCell>
                    {new Date(grade.examDate).toLocaleDateString('ro-RO')}
                  </TableCell>
                  <TableCell>
                    <Typography
                      variant="h6"
                      component="span"
                      color={grade.isPassing ? 'success.main' : 'error.main'}
                      fontWeight="bold"
                    >
                      {grade.grade.toFixed(2)}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={grade.isPassing ? 'Promovat' : 'Nepromovat'}
                      color={grade.isPassing ? 'success' : 'error'}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>
                    {new Date(grade.publishedAt).toLocaleDateString('ro-RO', {
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

export default GradesView;
