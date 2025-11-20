import React, { useState } from 'react';
import {
  AppBar,
  Toolbar,
  Typography,
  Container,
  Box,
  Tabs,
  Tab,
  Paper,
  Chip,
} from '@mui/material';
import {
  School as SchoolIcon,
  Event as EventIcon,
  Grade as GradeIcon,
  MeetingRoom as MeetingRoomIcon,
  Person as PersonIcon,
  EventAvailable as ScheduleIcon,
  Notifications as NotificationIcon,
} from '@mui/icons-material';

import ExamsView from './components/ExamsView';
import StudentsView from './components/StudentsView';
import GradesView from './components/GradesView';
import RoomsView from './components/RoomsView';
import SchedulingView from './components/SchedulingView';
import NotificationsView from './components/NotificationsView';

function TabPanel({ children, value, index }) {
  return (
    <div role="tabpanel" hidden={value !== index}>
      {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
    </div>
  );
}

function App() {
  const [tabValue, setTabValue] = useState(0);

  const handleTabChange = (event, newValue) => {
    setTabValue(newValue);
  };

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <AppBar position="static" elevation={0}>
        <Toolbar>
          <SchoolIcon sx={{ mr: 2 }} />
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Gestionare Sesiune Examene
          </Typography>
          <Chip 
            label="2 APIs: 5001 + 5002" 
            color="secondary" 
            size="small" 
            sx={{ mr: 2 }}
          />
          <Typography variant="caption" sx={{ opacity: 0.8 }}>
            PSSC - Lab 4 - Polly Retry
          </Typography>
        </Toolbar>
      </AppBar>

      <Container maxWidth="lg" sx={{ mt: 4, mb: 4, flex: 1 }}>
        <Paper elevation={2} sx={{ borderRadius: 2 }}>
        <Tabs 
          value={tabValue} 
          onChange={handleTabChange} 
          variant="fullWidth"
          sx={{ borderBottom: 1, borderColor: 'divider' }}
        >
          <Tab icon={<EventIcon />} label="Examene" />
          <Tab icon={<PersonIcon />} label="Studenți" />
          <Tab icon={<GradeIcon />} label="Note" />
          <Tab icon={<MeetingRoomIcon />} label="Săli" />
          <Tab icon={<ScheduleIcon />} label="Programare" />
          <Tab icon={<NotificationIcon />} label="Notificări" />
        </Tabs>          <TabPanel value={tabValue} index={0}>
            <ExamsView />
          </TabPanel>
          <TabPanel value={tabValue} index={1}>
            <StudentsView />
          </TabPanel>
          <TabPanel value={tabValue} index={2}>
            <GradesView />
          </TabPanel>
          <TabPanel value={tabValue} index={3}>
            <RoomsView />
          </TabPanel>
          <TabPanel value={tabValue} index={4}>
            <SchedulingView />
          </TabPanel>
          <TabPanel value={tabValue} index={5}>
            <NotificationsView />
          </TabPanel>
        </Paper>
      </Container>

      <Box
        component="footer"
        sx={{
          py: 2,
          px: 2,
          mt: 'auto',
          backgroundColor: (theme) => theme.palette.grey[200],
        }}
      >
        <Container maxWidth="lg">
          <Typography variant="body2" color="text.secondary" align="center">
            © 2025 Universitatea Politehnica Timișoara - Sistem Gestionare Examene
          </Typography>
        </Container>
      </Box>
    </Box>
  );
}

export default App;
