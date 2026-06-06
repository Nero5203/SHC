import { useState } from "react";
import { defaultApiUrl, postJson } from "./apiClient.js";

const initialForm = {
  email: "",
  password: ""
};

function LoginUserPage({ onLogout }) {
  const [apiUrl, setApiUrl] = useState(() => localStorage.getItem("shc.apiUrl") || defaultApiUrl);
  const [form, setForm] = useState(initialForm);
  const [status, setStatus] = useState({ type: "idle", message: "Ready to log in." });
  const [isSubmitting, setIsSubmitting] = useState(false);

  function updateApiUrl(value) {
    setApiUrl(value);
    localStorage.setItem("shc.apiUrl", value);
  }

  function updateField(field, value) {
    setForm((current) => ({ ...current, [field]: value }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setIsSubmitting(true);
    setStatus({ type: "loading", message: "Logging in..." });

    try {
      const result = await postJson(apiUrl, "/api/auth/login", form);
      setStatus({
        type: "success",
        message: result?.token ? "Logged in successfully." : "Logged in successfully."
      });
    } catch (error) {
      setStatus({ type: "error", message: error.message });
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-mark">S</div>
          <div>
            <strong>SHC</strong>
            <span>Cloud Console</span>
          </div>
        </div>

        <nav className="module-list" aria-label="Frontend modules">
          <button className="module-item" onClick={() => onLogout("register")} type="button">
            <span>Register User</span>
            <small>Auth</small>
          </button>
          <button className="module-item active" type="button">
            <span>Login</span>
            <small>Auth</small>
          </button>
        </nav>
      </aside>

      <section className="page">
        <header className="topbar">
          <div>
            <p className="eyebrow">Auth Module</p>
            <h1>Login</h1>
          </div>
          <label className="api-field">
            API URL
            <input value={apiUrl} onChange={(event) => updateApiUrl(event.target.value)} />
          </label>
        </header>

        <section className="content-grid">
          <form className="panel register-form" onSubmit={handleSubmit}>
            <div className="form-header">
              <div>
                <h2>Sign In</h2>
                <p>Log in using your account credentials.</p>
              </div>
            </div>

            <div className="form-grid">
              <label>
                Email
                <input
                  value={form.email}
                  onChange={(event) => updateField("email", event.target.value)}
                  required
                  type="email"
                />
              </label>

              <label>
                Password
                <input
                  value={form.password}
                  onChange={(event) => updateField("password", event.target.value)}
                  required
                  type="password"
                />
              </label>
            </div>

            <div className="form-footer">
              <button className="primary-button" disabled={isSubmitting} type="submit">
                {isSubmitting ? "Logging in..." : "Login"}
              </button>
              <button className="secondary-button" disabled={isSubmitting} type="button" onClick={() => setForm(initialForm)}>
                Clear
              </button>
              <span className="form-footer-alt">
                Don't have an account?{" "}
                <button type="button" onClick={() => onLogout("register")}>
                  Register
                </button>
              </span>
            </div>
          </form>

          <aside className={`panel status-panel ${status.type}`}>
            <h2>Status</h2>
            <p>{status.message}</p>
            <dl>
              <div>
                <dt>Endpoint</dt>
                <dd>POST /api/auth/login</dd>
              </div>
              <div>
                <dt>Backend</dt>
                <dd>{apiUrl}</dd>
              </div>
            </dl>
          </aside>
        </section>
      </section>
    </main>
  );
}

export default LoginUserPage;
