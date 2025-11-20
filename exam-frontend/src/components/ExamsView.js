import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Grid,
  Chip,
  CircularProgress,
  Alert,
} from '@mui/material';
import {
  Event as EventIcon,
  MeetingRoom as RoomIcon,
  People as PeopleIcon,
} from '@mui/icons-material';
import { examApi } from '../api';

function ExamsView() {
  const [exams, setExams] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadExams();
  }, []);

  const loadExams = async () => {
    try {
      setLoading(true);
      const response = await examApi.getAllExams();
      setExams(response.data.data || []);
      setError(null);
    } catch (err) {
      setError('Eroare la încărcarea examenelor: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return <Alert severity="error">{error}</Alert>;
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom sx={{ mb: 3, fontWeight: 500 }}>
        Examene Programate
      </Typography>

      {exams.length === 0 ? (
        <Alert severity="info">Nu există examene programate.</Alert>
      ) : (
        <Grid container spacing={2}>
          {exams.map((exam) => (
            <Grid item xs={12} sm={6} md={4} key={exam.id}>
              <Card elevation={1} sx={{ height: '100%', borderRadius: 2 }}>
                <CardContent>
                  <Box display="flex" alignItems="center" mb={2}>
                    <EventIcon color="primary" sx={{ mr: 1 }} />
                    <Typography variant="h6" component="div">
                      {exam.courseCode}
                    </Typography>
                  </Box>

                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    📅 {new Date(exam.examDate).toLocaleDateString('ro-RO', {
                      day: '2-digit',
                      month: 'long',
                      year: 'numeric',
                    })}
                  </Typography>

                  <Box mt={2} display="flex" gap={1} flexWrap="wrap">
                    <Chip
                      icon={<RoomIcon />}
                      label={`Sala ${exam.roomNumber}`}
                      size="small"
                      color="primary"
                      variant="outlined"
                    />
                    <Chip
                      icon={<PeopleIcon />}
                      label={`${exam.registeredStudentsCount}/${exam.roomCapacity}`}
                      size="small"
                      color={
                        exam.registeredStudentsCount >= exam.roomCapacity
                          ? 'error'
                          : 'success'
                      }
                    />
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
    </Box>
  );
}

export default ExamsView;
