import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  Alert,
  CircularProgress,
  Card,
  CardContent,
  Chip,
  Grid,
} from '@mui/material';
import {
  EventAvailable as ScheduleIcon,
  Send as SendIcon,
  Notifications as NotificationIcon,
} from '@mui/icons-material';
import { examApi } from '../api';

function SchedulingView() {
  const [formData, setFormData] = useState({
    courseCode: 'PSSC',
    proposedDate1: '2025-12-20',
    proposedDate2: '2025-12-21',
    proposedDate3: '2025-12-22',
    durationMinutes: 120,
    expectedStudents: 30,
  });
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: name === 'durationMinutes' || name === 'expectedStudents' 
        ? parseInt(value) || 0 
        : value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);

    try {
      const response = await examApi.scheduleExam(formData);
      console.log('Schedule response:', response.data);
      
      if (response.data.success) {
        setResult(response.data);
      } else {
        setError(response.data.errors?.join(', ') || 'Failed to schedule exam');
      }
    } catch (err) {
      console.error('Error scheduling exam:', err);
      setError(err.response?.data?.errors?.join(', ') || err.message || 'Network error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <ScheduleIcon fontSize="large" />
        Programare Examene
      </Typography>
      
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Programează un examen folosind workflow-ul de domain. 
        <Chip 
          icon={<NotificationIcon />} 
          label="Trimite notificare automată cu Polly Retry (3 reîncercări)" 
          color="primary" 
          size="small" 
          sx={{ ml: 1 }}
        />
      </Typography>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Formular Programare
            </Typography>
            
            <Box component="form" onSubmit={handleSubmit} sx={{ mt: 2 }}>
              <TextField
                fullWidth
                label="Cod Curs"
                name="courseCode"
                value={formData.courseCode}
                onChange={handleChange}
                margin="normal"
                required
                helperText="Ex: PSSC, ML, AI"
              />
              
              <TextField
                fullWidth
                label="Dată Propusă 1"
                name="proposedDate1"
                type="date"
                value={formData.proposedDate1}
                onChange={handleChange}
                margin="normal"
                required
                InputLabelProps={{ shrink: true }}
              />
              
              <TextField
                fullWidth
                label="Dată Propusă 2"
                name="proposedDate2"
                type="date"
                value={formData.proposedDate2}
                onChange={handleChange}
                margin="normal"
                required
                InputLabelProps={{ shrink: true }}
              />
              
              <TextField
                fullWidth
                label="Dată Propusă 3"
                name="proposedDate3"
                type="date"
                value={formData.proposedDate3}
                onChange={handleChange}
                margin="normal"
                required
                InputLabelProps={{ shrink: true }}
              />
              
              <TextField
                fullWidth
                label="Durată (minute)"
                name="durationMinutes"
                type="number"
                value={formData.durationMinutes}
                onChange={handleChange}
                margin="normal"
                required
                helperText="Ex: 120 pentru 2 ore"
              />
              
              <TextField
                fullWidth
                label="Studenți Așteptați"
                name="expectedStudents"
                type="number"
                value={formData.expectedStudents}
                onChange={handleChange}
                margin="normal"
                required
                helperText="Capacitatea minimă a sălii"
              />

              <Button
                type="submit"
                variant="contained"
                color="primary"
                fullWidth
                disabled={loading}
                startIcon={loading ? <CircularProgress size={20} /> : <SendIcon />}
                sx={{ mt: 3 }}
              >
                {loading ? 'Se programează...' : 'Programează Examen'}
              </Button>
            </Box>
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Rezultat
            </Typography>

            {error && (
              <Alert severity="error" sx={{ mt: 2 }}>
                <strong>Eroare:</strong> {error}
              </Alert>
            )}

            {result && (
              <Box sx={{ mt: 2 }}>
                <Alert severity="success" sx={{ mb: 2 }}>
                  <strong>{result.message}</strong>
                </Alert>

                <Card variant="outlined">
                  <CardContent>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                      Detalii Examen Programat
                    </Typography>
                    
                    <Box sx={{ mt: 2 }}>
                      <Typography variant="body2">
                        <strong>Curs:</strong> {result.data?.courseCode}
                      </Typography>
                      <Typography variant="body2">
                        <strong>Data:</strong> {new Date(result.data?.scheduledDate).toLocaleDateString('ro-RO')}
                      </Typography>
                      <Typography variant="body2">
                        <strong>Sală:</strong> {result.data?.allocatedRoom}
                      </Typography>
                      <Typography variant="body2">
                        <strong>Capacitate:</strong> {result.data?.roomCapacity} locuri
                      </Typography>
                      <Typography variant="body2">
                        <strong>Publicat la:</strong> {new Date(result.data?.publishedAt).toLocaleString('ro-RO')}
                      </Typography>
                      
                      {result.data?.notificationSent && (
                        <Chip 
                          icon={<NotificationIcon />}
                          label="✅ Notificare trimisă cu succes" 
                          color="success" 
                          size="small" 
                          sx={{ mt: 2 }}
                        />
                      )}
                    </Box>
                  </CardContent>
                </Card>

                <Alert severity="info" sx={{ mt: 2 }}>
                  <Typography variant="body2">
                    📬 Verifică tab-ul <strong>Notificări</strong> pentru a vedea notificarea primită pe API-ul separat (port 5002)
                  </Typography>
                </Alert>
              </Box>
            )}

            {!error && !result && !loading && (
              <Alert severity="info" sx={{ mt: 2 }}>
                Completează formularul și apasă "Programează Examen" pentru a testa workflow-ul cu notificare Polly.
              </Alert>
            )}
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}

export default SchedulingView;
