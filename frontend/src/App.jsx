import { useState } from "react";
import RegisterUserPage from "./RegisterUserPage.jsx";
import LoginUserPage from "./LoginUserPage.jsx";

function App() {
  const [page, setPage] = useState("login");

  function navigate(value) {
    setPage(value);
    localStorage.setItem("shc.page", value);
  }

  if (page === "login") {
    return <LoginUserPage onLogout={navigate} />;
  }

  return <RegisterUserPage onLogout={navigate} />;
}

export default App;
