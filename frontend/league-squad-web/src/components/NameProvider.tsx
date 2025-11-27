import React, { useState } from 'react'

const Name = React.createContext({
    name: "string",
    setName: () => {},
});

const NameProvider = ({ children }) => {
    const [name, setName] = useState("Not wahaj");

    return (
        <Name.Provider value={{ name, setName: () => setName }}>
            {children}
        </Name.Provider>
    )
}

export default NameProvider