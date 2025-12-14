import { BrowserRouter, Route, Routes } from "react-router-dom";
import { UnityPage } from "./pages/UnityPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<UnityPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
