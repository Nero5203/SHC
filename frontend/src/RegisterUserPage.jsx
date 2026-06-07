import { useState } from "react";
import { ArrowLeft, Lock, Mail, Phone, ShieldCheck, UserPlus } from "./icons.jsx";
import { defaultApiUrl, postJson } from "./apiClient.js";

const initialForm = {
  firstName: "",
  lastName: "",
  username: "",
  email: "",
  phoneNumber: "",
  password: ""
};

function RegisterUserPage({ onLogout }) {
  const apiUrl = localStorage.getItem("shc.apiUrl") || defaultApiUrl;
  const [form, setForm] = useState(initialForm);
  const [status, setStatus] = useState({ type: "idle", message: "Ready to register a user." });
  const [isSubmitting, setIsSubmitting] = useState(false);

  function updateField(field, value) {
    setForm((current) => ({ ...current, [field]: value }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setIsSubmitting(true);
    setStatus({ type: "loading", message: "Registering user..." });

    try {
      const result = await postJson(apiUrl, "/api/auth/register", form);
      setForm(initialForm);
      setStatus({
        type: "success",
        message: typeof result === "string" && result ? result : "User registered successfully."
      });
      onLogout("login");
    } catch (error) {
      setStatus({ type: "error", message: error.message });
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="auth-page auth-page-register">
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
            New workspace access
          </p>
          <h1>Create a new SHC DRIVE account.</h1>
          <p>
            Register a teammate through the real auth endpoint, then move directly into
            login and role-aware dashboard access.
          </p>
        </div>

        <div className="auth-preview-grid" aria-hidden="true">
          <article className="auth-preview-card">
            <div className="auth-preview-icon">
              <UserPlus size={18} />
            </div>
            <strong>Backend registration</strong>
            <span>The form submits to your actual register endpoint.</span>
          </article>
          <article className="auth-preview-card">
            <div className="auth-preview-icon">
              <ShieldCheck size={18} />
            </div>
            <strong>Ready for login</strong>
            <span>After sign-up, the flow sends the user into the login experience.</span>
          </article>
        </div>
      </section>

      <section className="auth-form-shell">
        <form className="auth-form-card auth-form-card-wide" onSubmit={handleSubmit}>
          <div className="auth-form-icon">
            <UserPlus size={28} />
          </div>

          <div className="auth-form-header">
            <h2>Register User</h2>
            <p>Create a new account for SHC DRIVE.</p>
          </div>

          <div className="auth-form-grid auth-form-grid-two">
            <label>
              First Name
              <input
                value={form.firstName}
                onChange={(event) => updateField("firstName", event.target.value)}
                required
                placeholder="First name"
              />
            </label>

            <label>
              Last Name
              <input
                value={form.lastName}
                onChange={(event) => updateField("lastName", event.target.value)}
                required
                placeholder="Last name"
              />
            </label>

            <label>
              Username
              <input
                value={form.username}
                onChange={(event) => updateField("username", event.target.value)}
                required
                placeholder="Username"
              />
            </label>

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
              Phone Number
              <div className="auth-input-wrap">
                <Phone size={18} />
                <input
                  value={form.phoneNumber}
                  onChange={(event) => updateField("phoneNumber", event.target.value)}
                  required
                  placeholder="+00 000 000 000"
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
                  placeholder="Create a password"
                />
              </div>
            </label>
          </div>

          <button className="primary-button auth-submit-button" disabled={isSubmitting} type="submit">
            {isSubmitting ? "Registering..." : "Create account"}
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
              Already have an account?
              <button type="button" onClick={() => onLogout("login")}>
                Login
              </button>
            </span>
          </div>
        </form>
      </section>
    </main>
  );
}

export default RegisterUserPage;
