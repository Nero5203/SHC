import { useState } from "react";
import RegisterUserPage from "./RegisterUserPage.jsx";
import LoginUserPage from "./LoginUserPage.jsx";
import UserHomePage from "./UserHomePage.jsx";

function App() {
  const [page, setPage] = useState("login");

  function navigate(value) {
    setPage(value);
  }

  if (page === "home") {
    return <UserHomePage onLogout={navigate} />;
  }

  if (page === "login") {
    return <LoginUserPage onLogout={navigate} />;
  }

  return <RegisterUserPage onLogout={navigate} />;
}

export default App;
