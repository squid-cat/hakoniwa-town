import { BrowserRouter, Route, Routes } from "react-router-dom";
import { TrainController } from "./pages/TrainController";
import { UnityPage } from "./pages/UnityPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<UnityPage />} />
        <Route path="/controller" element={<TrainController />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
