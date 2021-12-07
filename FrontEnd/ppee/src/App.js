import './App.css';
import FileUpload from './Components/FileUpload';
import Navigation from './Components/Navigation';
import Report from './Components/Report';
import Training from './Components/Training';
import { BrowserRouter, Route, Routes } from "react-router-dom";

function App() {
  return (
    <BrowserRouter>
      <div className="App">
        <Navigation />
        <Routes>
          <Route path='/' element={<FileUpload />} />
          <Route path="/file-upload" element={<FileUpload />} />
          <Route path="/training" element={<Training />} />
          <Route path="/report" element={<Report />} />
        </Routes>

        
      </div>
    </BrowserRouter>
  );
}

export default App;
