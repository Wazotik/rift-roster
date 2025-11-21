import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { BrowserRouter } from 'react-router-dom'
import { MantineProvider } from '@mantine/core';
import { Notifications } from '@mantine/notifications'
import '@mantine/core/styles.css';
import '@mantine/notifications/styles.css';
import { QueryClientProvider } from '@tanstack/react-query'
import { queryClient } from './lib/queryClient.ts'



createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <MantineProvider defaultColorScheme='dark' theme={{ 
            defaultRadius: 'md',
            fontFamily: 'Karla, sans-serif',
            headings: { fontFamily: 'Karla, sans-serif' }
        }}>
            <Notifications />
            <QueryClientProvider client={queryClient}>
                <BrowserRouter>
                    <App />
                </BrowserRouter>
            </QueryClientProvider>
        </MantineProvider>
    </StrictMode>,
)
