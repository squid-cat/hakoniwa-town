import { BrowserRouter, Route, Routes } from "react-router-dom";
import { NotFound } from "./pages/NotFound";
import { UnityController } from "./pages/UnityController";
import { UnityPage } from "./pages/UnityPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<UnityPage />} />
        <Route path="/controller" element={<UnityController />} />
        <Route path="/notfound" element={<NotFound />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
