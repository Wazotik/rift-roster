import React from 'react'
import { Text, Button } from '@radix-ui/themes'

const Landing = () => {
    return (
        <>
            <div>Landing</div>
            <div style={{ padding: 24 }}>
                <Text size="7" weight="bold">League Squad Tracker</Text>
                <div style={{ height: 12 }} />
                <Button>Get Started</Button>
            </div>
        </>
    )
}

export default Landing