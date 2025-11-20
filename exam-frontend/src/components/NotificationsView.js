import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Button,
  Alert,
  CircularProgress,
  Card,
  CardContent,
  Chip,
  IconButton,
  Divider,
  Stack,
} from '@mui/material';
import {
  Notifications as NotificationIcon,
  Refresh as RefreshIcon,
  Delete as DeleteIcon,
  CheckCircle as CheckIcon,
  Room as RoomIcon,
  Event as EventIcon,
  School as SchoolIcon,
} from '@mui/icons-material';
import { notificationsApi } from '../api';

function NotificationsView() {
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [health, setHealth] = useState(null);
  const [autoRefresh, setAutoRefresh] = useState(false);

  const fetchNotifications = async () => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await notificationsApi.getAllNotifications();
      console.log('Notifications response:', response.data);
      
      if (response.data.success) {
        setNotifications(response.data.notifications || []);
      } else {
        setError('Failed to fetch notifications');
      }
    } catch (err) {
      console.error('Error fetching notifications:', err);
      setError(err.response?.data?.error || err.message || 'Network error connecting to port 5002');
    } finally {
      setLoading(false);
    }
  };

  const fetchHealth = async () => {
    try {
      const response = await notificationsApi.getHealth();
      setHealth(response.data);
    } catch (err) {
      setHealth({ status: 'unhealthy', error: err.message });
    }
  };

  const handleClearNotifications = async () => {
    if (!window.confirm('Ștergi toate notificările?')) return;
    
    try {
      await notificationsApi.clearNotifications();
      setNotifications([]);
      alert('Notificările au fost șterse cu succes!');
    } catch (err) {
      alert('Eroare la ștergerea notificărilor: ' + err.message);
    }
  };

  useEffect(() => {
    fetchNotifications();
    fetchHealth();
  }, []);

  useEffect(() => {
    if (autoRefresh) {
      const interval = setInterval(() => {
        fetchNotifications();
        fetchHealth();
      }, 5000); // Refresh every 5 seconds
      
      return () => clearInterval(interval);
    }
  }, [autoRefresh]);

  return (
    <Box sx={{ p: 3 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <NotificationIcon fontSize="large" />
          Notificări Primite
        </Typography>
        
        <Box sx={{ display: 'flex', gap: 2 }}>
          {health && (
            <Chip 
              label={health.status === 'healthy' ? '✅ API Online (5002)' : '❌ API Offline'}
              color={health.status === 'healthy' ? 'success' : 'error'}
              size="small"
            />
          )}
          
          <Button
            variant="outlined"
            size="small"
            onClick={() => setAutoRefresh(!autoRefresh)}
            color={autoRefresh ? 'primary' : 'default'}
          >
            {autoRefresh ? '⏸ Oprește Auto-Refresh' : '▶ Auto-Refresh (5s)'}
          </Button>
          
          <IconButton onClick={fetchNotifications} disabled={loading} color="primary">
            <RefreshIcon />
          </IconButton>
          
          <IconButton 
            onClick={handleClearNotifications} 
            color="error"
            disabled={notifications.length === 0}
          >
            <DeleteIcon />
          </IconButton>
        </Box>
      </Box>

      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Notificări primite de la API-ul principal (port 5001) prin Notifications API (port 5002).
        Acestea sunt trimise automat când programezi un examen, folosind <strong>Polly Retry Policy</strong> cu 3 reîncercări exponențiale.
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          <strong>Eroare:</strong> {error}
          <Typography variant="body2" sx={{ mt: 1 }}>
            Asigură-te că Notifications API rulează pe <strong>http://localhost:5002</strong>
          </Typography>
        </Alert>
      )}

      {loading && !notifications.length && (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
          <CircularProgress />
        </Box>
      )}

      {!loading && !error && notifications.length === 0 && (
        <Alert severity="info">
          <Typography variant="body1">
            📭 Nu există notificări încă.
          </Typography>
          <Typography variant="body2" sx={{ mt: 1 }}>
            Mergi la tab-ul <strong>Programare</strong> și programează un examen pentru a genera o notificare.
          </Typography>
        </Alert>
      )}

      {notifications.length > 0 && (
        <Box>
          <Typography variant="h6" gutterBottom>
            Total: {notifications.length} notificar{notifications.length === 1 ? 'e' : 'i'}
          </Typography>
          
          <Stack spacing={2}>
            {notifications.map((notification, index) => (
              <Card key={index} variant="outlined" sx={{ position: 'relative' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <Box sx={{ flex: 1 }}>
                      <Typography variant="h6" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <SchoolIcon color="primary" />
                        {notification.courseCode}
                        <Chip 
                          icon={<CheckIcon />} 
                          label="Sală Asignată" 
                          color="success" 
                          size="small" 
                        />
                      </Typography>
                      
                      <Divider sx={{ my: 2 }} />
                      
                      <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 2 }}>
                        <Box>
                          <Typography variant="body2" color="text.secondary" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <RoomIcon fontSize="small" />
                            <strong>Sală:</strong>
                          </Typography>
                          <Typography variant="body1">
                            {notification.roomNumber} (Capacitate: {notification.roomCapacity} locuri)
                          </Typography>
                        </Box>
                        
                        <Box>
                          <Typography variant="body2" color="text.secondary" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <EventIcon fontSize="small" />
                            <strong>Data Examen:</strong>
                          </Typography>
                          <Typography variant="body1">
                            {new Date(notification.examDate).toLocaleDateString('ro-RO', {
                              year: 'numeric',
                              month: 'long',
                              day: 'numeric',
                            })}
                          </Typography>
                        </Box>
                      </Box>
                      
                      <Box sx={{ mt: 2 }}>
                        <Typography variant="caption" color="text.secondary">
                          📬 Primită la: {new Date(notification.assignedAt).toLocaleString('ro-RO')}
                        </Typography>
                      </Box>
                    </Box>
                  </Box>
                </CardContent>
              </Card>
            ))}
          </Stack>
        </Box>
      )}

      <Box sx={{ mt: 3 }}>
        <Alert severity="info">
          <Typography variant="body2">
            <strong>💡 Cum funcționează:</strong>
          </Typography>
          <Typography variant="body2" component="div" sx={{ mt: 1 }}>
            <ol style={{ margin: 0, paddingLeft: 20 }}>
              <li>Programezi un examen în tab-ul <strong>Programare</strong></li>
              <li>Main API (port 5001) procesează cererea și alocă o sală</li>
              <li>Main API trimite notificare către Notifications API (port 5002) cu <strong>Polly Retry</strong></li>
              <li>Dacă trimiterea eșuează, Polly încearcă automat de 3 ori (2s, 4s, 8s)</li>
              <li>Notificarea apare aici în timp real (cu auto-refresh activat)</li>
            </ol>
          </Typography>
        </Alert>
      </Box>
    </Box>
  );
}

export default NotificationsView;
