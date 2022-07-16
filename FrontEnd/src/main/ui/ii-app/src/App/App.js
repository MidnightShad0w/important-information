import React from 'react';
// import './App.css';
import Register from './Register/Register';
import Main from './MainPage/Main'
import { BrowserRouter, Route, Routes} from 'react-router-dom';


function App() {
  return (
    <BrowserRouter>
        <Routes>
          <Route path ="/" element = {<Register />}/>
          <Route path ="/main" element = {<Main />}/>
        </Routes>
    </BrowserRouter>
  );
}

export default App;