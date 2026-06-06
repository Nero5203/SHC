import { useState } from "react";
import { getDashboardPageFromToken, isAuthenticated } from "./apiClient.js";
import AdminDashboardPage from "./AdminDashboardPage.jsx";
import RegisterUserPage from "./RegisterUserPage.jsx";
import LoginUserPage from "./LoginUserPage.jsx";
import UserHomePage from "./UserHomePage.jsx";

function App() {
  const [page, setPage] = useState(() => (
    isAuthenticated() ? getDashboardPageFromToken() : "login"
  ));

  function navigate(value) {
    setPage(value);
  }

  if (page === "home") {
    return <UserHomePage onLogout={navigate} />;
  }

  if (page === "admin") {
    return <AdminDashboardPage onLogout={navigate} />;
  }

  if (page === "login") {
    return <LoginUserPage onLogout={navigate} />;
  }

  return <RegisterUserPage onLogout={navigate} />;
}

export default App;
