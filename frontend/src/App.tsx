import { BrowserRouter, Route, Routes } from "react-router-dom";
import { NotFound } from "./pages/NotFound";
import { TrainController } from "./pages/TrainController";
import { UnityPage } from "./pages/UnityPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<UnityPage />} />
        <Route path="/controller/:roomId" element={<TrainController />} />
        <Route path="/notfound" element={<NotFound />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
