import React, { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  CircularProgress,
  Alert,
  Grid,
  Card,
  CardContent,
  Chip,
} from '@mui/material';
import { MeetingRoom as RoomIcon, People as PeopleIcon } from '@mui/icons-material';
import { examApi } from '../api';

function RoomsView() {
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadRooms();
  }, []);

  const loadRooms = async () => {
    try {
      setLoading(true);
      const response = await examApi.getRooms();
      setRooms(response.data.data || []);
      setError(null);
    } catch (err) {
      setError('Eroare la încărcarea sălilor: ' + err.message);
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

  const totalCapacity = rooms.reduce((sum, room) => sum + room.capacity, 0);

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5" sx={{ fontWeight: 500 }}>
          Săli Disponibile
        </Typography>
        <Chip
          icon={<PeopleIcon />}
          label={`Capacitate totală: ${totalCapacity} locuri`}
          color="primary"
          variant="outlined"
        />
      </Box>

      {rooms.length === 0 ? (
        <Alert severity="info">Nu există săli disponibile.</Alert>
      ) : (
        <Grid container spacing={3}>
          {rooms.map((room) => (
            <Grid item xs={12} sm={6} md={4} lg={3} key={room.number}>
              <Card
                elevation={2}
                sx={{
                  height: '100%',
                  borderRadius: 2,
                  transition: 'transform 0.2s, box-shadow 0.2s',
                  '&:hover': {
                    transform: 'translateY(-4px)',
                    boxShadow: 4,
                  },
                }}
              >
                <CardContent>
                  <Box display="flex" alignItems="center" mb={2}>
                    <RoomIcon color="primary" sx={{ fontSize: 40, mr: 2 }} />
                    <Typography variant="h4" component="div" fontWeight="bold">
                      {room.number}
                    </Typography>
                  </Box>

                  <Box
                    display="flex"
                    alignItems="center"
                    justifyContent="center"
                    sx={{
                      backgroundColor: 'primary.light',
                      borderRadius: 2,
                      py: 2,
                      mt: 2,
                    }}
                  >
                    <PeopleIcon sx={{ mr: 1, color: 'primary.dark' }} />
                    <Typography
                      variant="h5"
                      component="span"
                      fontWeight="bold"
                      color="primary.dark"
                    >
                      {room.capacity}
                    </Typography>
                    <Typography
                      variant="body2"
                      component="span"
                      color="primary.dark"
                      sx={{ ml: 0.5 }}
                    >
                      locuri
                    </Typography>
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

export default RoomsView;
