import { useState } from "react";
import { ArrowLeft, Lock, LogIn, Mail, ShieldCheck } from "./icons.jsx";
import {
  defaultApiUrl,
  extractTokens,
  getDashboardPageFromToken,
  postJson,
  setAuthToken,
  setRefreshToken
} from "./apiClient.js";

const initialForm = {
  email: "",
  password: ""
};

function LoginUserPage({ onLogout }) {
  const apiUrl = localStorage.getItem("shc.apiUrl") || defaultApiUrl;
  const [form, setForm] = useState(initialForm);
  const [status, setStatus] = useState({ type: "idle", message: "Ready to log in." });
  const [isSubmitting, setIsSubmitting] = useState(false);

  function updateField(field, value) {
    setForm((current) => ({ ...current, [field]: value }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setIsSubmitting(true);
    setStatus({ type: "loading", message: "Logging in..." });

    try {
      const result = await postJson(apiUrl, "/api/auth/login", form);
      const { accessToken, refreshToken } = extractTokens(result);

      if (!accessToken) {
        throw new Error("No access token received.");
      }

      setAuthToken(accessToken);
      if (refreshToken) {
        setRefreshToken(refreshToken);
      }

      const nextPage = getDashboardPageFromToken(accessToken);

      setStatus({
        type: "success",
        message: nextPage === "admin"
          ? "Logged in successfully. Opening admin dashboard..."
          : "Logged in successfully. Opening your dashboard..."
      });

      setTimeout(() => onLogout(nextPage), 800);
    } catch (error) {
      setStatus({ type: "error", message: error.message });
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-showcase">
        <header className="auth-showcase-nav">
          <button className="landing-back-button" type="button" onClick={() => onLogout("landing")}>
            <ArrowLeft size={16} />
            Back
          </button>
          <div className="landing-brand">
            <div className="landing-brand-mark" aria-hidden="true" />
            <div>
              <strong>SHC DRIVE</strong>
              <span>Secure cloud workspace</span>
            </div>
          </div>
        </header>

        <div className="auth-showcase-copy">
          <p className="landing-kicker">
            <ShieldCheck size={14} />
            Secure access
          </p>
          <h1>Welcome back to SHC DRIVE.</h1>
          <p>
            Sign in to open your real dashboard, file workspace, shared links, subscriptions,
            notifications, and admin tools based on your role.
          </p>
        </div>

        <div className="auth-preview-grid" aria-hidden="true">
          <article className="auth-preview-card">
            <div className="auth-preview-icon">
              <Lock size={18} />
            </div>
            <strong>Role-based entry</strong>
            <span>Users and admins land in the correct dashboard automatically.</span>
          </article>
          <article className="auth-preview-card">
            <div className="auth-preview-icon">
              <Mail size={18} />
            </div>
            <strong>Real authentication</strong>
            <span>Your login already connects to the backend token flow.</span>
          </article>
        </div>
      </section>

      <section className="auth-form-shell">
        <form className="auth-form-card" onSubmit={handleSubmit}>
          <div className="auth-form-icon">
            <LogIn size={28} />
          </div>

          <div className="auth-form-header">
            <h2>Login</h2>
            <p>Use your account credentials to enter SHC DRIVE.</p>
          </div>

          <div className="auth-form-grid">
            <label>
              Email
              <div className="auth-input-wrap">
                <Mail size={18} />
                <input
                  value={form.email}
                  onChange={(event) => updateField("email", event.target.value)}
                  required
                  type="email"
                  placeholder="you@example.com"
                />
              </div>
            </label>

            <label>
              Password
              <div className="auth-input-wrap">
                <Lock size={18} />
                <input
                  value={form.password}
                  onChange={(event) => updateField("password", event.target.value)}
                  required
                  type="password"
                  placeholder="Enter your password"
                />
              </div>
            </label>
          </div>

          <button className="primary-button auth-submit-button" disabled={isSubmitting} type="submit">
            {isSubmitting ? "Logging in..." : "Login"}
          </button>

          <div className={`auth-status auth-status-${status.type}`}>
            <strong>Status</strong>
            <span>{status.message}</span>
          </div>

          <div className="auth-form-footer">
            <button className="secondary-button" disabled={isSubmitting} type="button" onClick={() => setForm(initialForm)}>
              Clear
            </button>
            <span>
              Don&apos;t have an account?
              <button type="button" onClick={() => onLogout("register")}>
                Register
              </button>
            </span>
          </div>
        </form>
      </section>
    </main>
  );
}

export default LoginUserPage;
