import { useEffect, useMemo, useState } from "react";
import {
  clearAuth,
  defaultApiUrl,
  getJson,
  getPermissionsFromToken,
  getRolesFromToken,
  getUserNameFromToken,
  isAdminToken,
  isAuthenticated,
  postJson
} from "./apiClient.js";

const adminModules = [
  {
    key: "users",
    title: "Users",
    area: "Accounts",
    permissions: ["User.Manage", "System.Admin"],
    route: "/api/users",
    countPath: "/api/users",
    description: "Manage profiles, settings, deletion, and account ownership.",
    note: "The admin list endpoint is available now."
  },
  {
    key: "roles",
    title: "Roles",
    area: "Authorization",
    permissions: ["System.Admin"],
    route: "/api/roles",
    countPath: "/api/roles",
    description: "Create roles, edit role names, and assign roles to users."
  },
  {
    key: "permissions",
    title: "Permissions",
    area: "Authorization",
    permissions: ["System.Admin"],
    route: "/api/permissions",
    description: "Grant, update, revoke, and check resource permissions."
  },
  {
    key: "storage",
    title: "Storage Nodes",
    area: "Infrastructure",
    permissions: ["Node.Manage", "System.Admin"],
    route: "/api/storage-nodes",
    countPath: "/api/storage-nodes",
    description: "Register laptop nodes, update heartbeat, and manage node capacity."
  },
  {
    key: "settings",
    title: "System Settings",
    area: "Platform",
    permissions: ["System.Admin"],
    route: "/api/system-settings",
    countPath: "/api/system-settings/all",
    description: "Control file limits, registration rules, link defaults, and trash retention."
  },
  {
    key: "subscriptions",
    title: "Subscriptions",
    area: "Billing",
    permissions: ["System.Admin"],
    route: "/api/subscriptions",
    countPath: "/api/subscriptions/plans",
    description: "Manage subscription plans and user subscription records."
  },
  {
    key: "audit",
    title: "Audit Logs",
    area: "Security",
    permissions: ["System.Admin"],
    route: "/api/audit-logs",
    countPath: "/api/audit-logs",
    description: "Review important permission and system actions."
  }
];

function AdminDashboardPage({ onLogout }) {
  const [apiUrl, setApiUrl] = useState(() => localStorage.getItem("shc.apiUrl") || defaultApiUrl);
  const [activeModuleKey, setActiveModuleKey] = useState("users");
  const [counts, setCounts] = useState({});
  const [loadingCounts, setLoadingCounts] = useState(false);

  const roles = getRolesFromToken();
  const permissions = getPermissionsFromToken();
  const isAdmin = isAdminToken();
  const displayName = cleanDisplayName(getUserNameFromToken());

  const visibleModules = useMemo(() => {
    if (isAdmin) return adminModules;

    const permissionSet = new Set(permissions.map((permission) => permission.toLowerCase()));
    return adminModules.filter((module) =>
      module.permissions.some((permission) => permissionSet.has(permission.toLowerCase()))
    );
  }, [isAdmin, permissions]);

  const activeModule = visibleModules.find((module) => module.key === activeModuleKey) ?? visibleModules[0];

  function updateApiUrl(value) {
    setApiUrl(value);
    localStorage.setItem("shc.apiUrl", value);
  }

  async function handleLogout() {
    try {
      const refreshToken = localStorage.getItem("shc.refreshToken");
      if (refreshToken) {
        await postJson(apiUrl, "/api/auth/logout", { RefreshToken: refreshToken }).catch(() => {});
      }
    } finally {
      clearAuth();
      onLogout("login");
    }
  }

  useEffect(() => {
    if (!isAuthenticated()) {
      onLogout("login");
      return;
    }

    if (!isAdminToken()) {
      onLogout("home");
    }
  }, [onLogout]);

  useEffect(() => {
    if (!activeModule && visibleModules.length > 0) {
      setActiveModuleKey(visibleModules[0].key);
    }
  }, [activeModule, visibleModules]);

  useEffect(() => {
    const countableModules = visibleModules.filter((module) => module.countPath);
    if (countableModules.length === 0) return;

    let cancelled = false;
    setLoadingCounts(true);

    Promise.all(
      countableModules.map(async (module) => {
        try {
          const data = await getJson(apiUrl, module.countPath);
          const value = Array.isArray(data) ? data.length : data ? 1 : 0;
          return [module.key, { status: "ok", value }];
        } catch (error) {
          return [module.key, { status: "error", value: error.message }];
        }
      })
    ).then((entries) => {
      if (!cancelled) {
        setCounts(Object.fromEntries(entries));
        setLoadingCounts(false);
      }
    });

    return () => {
      cancelled = true;
    };
  }, [apiUrl, visibleModules]);

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-mark">S</div>
          <div>
            <strong>SHC</strong>
            <span>Admin Console</span>
          </div>
        </div>

        <nav className="module-list" aria-label="Admin modules">
          {visibleModules.map((module) => (
            <button
              className={`module-item ${activeModule?.key === module.key ? "active" : ""}`}
              key={module.key}
              onClick={() => setActiveModuleKey(module.key)}
              type="button"
            >
              <span>{module.title}</span>
              <small>{module.area}</small>
            </button>
          ))}
        </nav>

        <div className="sidebar-footer">
          <button className="module-item" onClick={() => onLogout("home")} type="button">
            <span>User Dashboard</span>
            <small>Switch</small>
          </button>
          <button className="module-item" onClick={handleLogout} type="button">
            <span>Logout</span>
            <small>Exit</small>
          </button>
        </div>
      </aside>

      <section className="page">
        <header className="topbar">
          <div>
            <p className="eyebrow">Admin Dashboard</p>
            <h1>{displayName ? `Welcome admin, ${displayName}` : "Welcome admin"}</h1>
          </div>
          <label className="api-field">
            API URL
            <input value={apiUrl} onChange={(event) => updateApiUrl(event.target.value)} />
          </label>
        </header>

        <section className="content-grid admin-dashboard-grid">
          <section className="admin-main">
            <div className="section-heading">
              <h2>Authorized Modules</h2>
              <p>These are the backend areas your token allows you to use.</p>
            </div>

            <div className="admin-module-grid">
              {visibleModules.map((module) => (
                <article
                  className={`panel module-card ${activeModule?.key === module.key ? "selected" : ""}`}
                  key={module.key}
                >
                  <div>
                    <span className="module-area">{module.area}</span>
                    <h2>{module.title}</h2>
                    <p>{module.description}</p>
                  </div>
                  <div className="module-meta">
                    <span>{module.route}</span>
                    <strong>{formatCount(counts[module.key], loadingCounts)}</strong>
                  </div>
                </article>
              ))}
            </div>

            {activeModule && (
              <section className="panel module-detail">
                <div>
                  <p className="eyebrow">{activeModule.area}</p>
                  <h2>{activeModule.title}</h2>
                </div>
                <p>{activeModule.description}</p>
                <dl>
                  <div>
                    <dt>Required permission</dt>
                    <dd>{activeModule.permissions.join(" or ")}</dd>
                  </div>
                  <div>
                    <dt>Main API route</dt>
                    <dd>{activeModule.route}</dd>
                  </div>
                  {activeModule.note && (
                    <div>
                      <dt>Backend note</dt>
                      <dd>{activeModule.note}</dd>
                    </div>
                  )}
                </dl>
              </section>
            )}
          </section>

          <aside className="side-panels">
            <section className="panel">
              <h2>Roles</h2>
              <div className="pill-list">
                {roles.length > 0 ? roles.map((role) => <span key={role}>{role}</span>) : <span>No roles in token</span>}
              </div>
            </section>

            <section className="panel">
              <h2>Permissions</h2>
              <div className="permission-list">
                {permissions.length > 0
                  ? permissions.map((permission) => <span key={permission}>{permission}</span>)
                  : <span>No permissions in token</span>}
              </div>
            </section>
          </aside>
        </section>
      </section>
    </main>
  );
}

function formatCount(countInfo, isLoading) {
  if (!countInfo && isLoading) return "Loading";
  if (!countInfo) return "Ready";
  if (countInfo.status === "error") return "Not available";
  return String(countInfo.value);
}

function cleanDisplayName(value) {
  const text = String(value ?? "").trim();
  if (!text || text.toLowerCase() === "undefined" || text.toLowerCase() === "null") {
    return "";
  }

  return text;
}

export default AdminDashboardPage;
