import { useCallback, useEffect, useMemo, useState } from "react";
import {
  clearAuth,
  defaultApiUrl,
  deleteJson,
  getJson,
  getPermissionsFromToken,
  getRolesFromToken,
  getUserNameFromToken,
  isAdminToken,
  isAuthenticated,
  postJson,
  putJson
} from "./apiClient.js";

const nodeStatusOptions = [
  { value: "0", label: "Online" },
  { value: "1", label: "Offline" },
  { value: "2", label: "Maintenance" },
  { value: "3", label: "Degraded" },
  { value: "4", label: "Full" }
];

const emptyStorageNodeForm = {
  storageNodeId: "",
  name: "",
  hostname: "",
  ipAddress: "",
  port: "",
  basePath: "",
  totalCapacityBytes: ""
};

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
  const [adminUsers, setAdminUsers] = useState([]);
  const [availableRoles, setAvailableRoles] = useState([]);
  const [userRoleMap, setUserRoleMap] = useState({});
  const [selectedRoleByUser, setSelectedRoleByUser] = useState({});
  const [loadingUsers, setLoadingUsers] = useState(false);
  const [usersError, setUsersError] = useState("");
  const [roleActionKey, setRoleActionKey] = useState("");
  const [storageNodes, setStorageNodes] = useState([]);
  const [storageNodeForm, setStorageNodeForm] = useState(emptyStorageNodeForm);
  const [heartbeatDrafts, setHeartbeatDrafts] = useState({});
  const [loadingStorageNodes, setLoadingStorageNodes] = useState(false);
  const [storageNodesError, setStorageNodesError] = useState("");
  const [storageActionKey, setStorageActionKey] = useState("");

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

  const loadUsersWithRoles = useCallback(async () => {
    setLoadingUsers(true);
    setUsersError("");

    try {
      const [users, roleList] = await Promise.all([
        getJson(apiUrl, "/api/users"),
        getJson(apiUrl, "/api/roles")
      ]);

      const safeUsers = Array.isArray(users) ? users : [];
      const safeRoles = Array.isArray(roleList) ? roleList : [];

      setAdminUsers(safeUsers);
      setAvailableRoles(safeRoles);

      const userRoleEntries = await Promise.all(
        safeUsers.map(async (user) => {
          const userId = getValue(user, "userId", "UserId");
          if (!userId) return ["", []];

          try {
            const userRoles = await getJson(apiUrl, `/api/roles/user/${userId}`);
            return [userId, Array.isArray(userRoles) ? userRoles : []];
          } catch (error) {
            return [userId, []];
          }
        })
      );

      setUserRoleMap(Object.fromEntries(userRoleEntries.filter(([userId]) => userId)));
    } catch (error) {
      setUsersError(error.message);
    } finally {
      setLoadingUsers(false);
    }
  }, [apiUrl]);

  async function handleAssignRole(userId) {
    const roleId = selectedRoleByUser[userId];
    if (!roleId) return;

    const actionKey = `${userId}:${roleId}:assign`;
    setRoleActionKey(actionKey);

    try {
      await postJson(apiUrl, `/api/roles/${roleId}/users/${userId}`, {});
      setSelectedRoleByUser((current) => ({ ...current, [userId]: "" }));
      await loadUsersWithRoles();
    } catch (error) {
      setUsersError(error.message);
    } finally {
      setRoleActionKey("");
    }
  }

  async function handleRemoveRole(userId, roleId) {
    const actionKey = `${userId}:${roleId}:remove`;
    setRoleActionKey(actionKey);

    try {
      await deleteJson(apiUrl, `/api/roles/${roleId}/users/${userId}`);
      await loadUsersWithRoles();
    } catch (error) {
      setUsersError(error.message);
    } finally {
      setRoleActionKey("");
    }
  }

  const loadStorageNodes = useCallback(async () => {
    setLoadingStorageNodes(true);
    setStorageNodesError("");

    try {
      const data = await getJson(apiUrl, "/api/storage-nodes");
      const safeNodes = Array.isArray(data) ? data : [];

      setStorageNodes(safeNodes);
      setHeartbeatDrafts(
        Object.fromEntries(
          safeNodes.map((node) => {
            const nodeId = getValue(node, "storageNodeId", "StorageNodeId");
            return [
              nodeId,
              {
                totalCapacityBytes: getValue(node, "totalCapacityBytes", "TotalCapacityBytes"),
                usedCapacityBytes: getValue(node, "usedCapacityBytes", "UsedCapacityBytes"),
                status: getNodeStatusValue(node)
              }
            ];
          })
        )
      );
    } catch (error) {
      setStorageNodesError(error.message);
    } finally {
      setLoadingStorageNodes(false);
    }
  }, [apiUrl]);

  async function handleSaveStorageNode(event) {
    event.preventDefault();
    const isEditing = !!storageNodeForm.storageNodeId;
    const actionKey = isEditing ? `${storageNodeForm.storageNodeId}:edit` : "create";

    setStorageActionKey(actionKey);
    setStorageNodesError("");

    const body = {
      Name: storageNodeForm.name.trim(),
      Hostname: storageNodeForm.hostname.trim(),
      IpAddress: storageNodeForm.ipAddress.trim(),
      Port: Number(storageNodeForm.port),
      BasePath: storageNodeForm.basePath.trim(),
      TotalCapacityBytes: Number(storageNodeForm.totalCapacityBytes)
    };

    try {
      if (isEditing) {
        await putJson(apiUrl, `/api/storage-nodes/${storageNodeForm.storageNodeId}`, body);
      } else {
        await postJson(apiUrl, "/api/storage-nodes", body);
      }

      setStorageNodeForm(emptyStorageNodeForm);
      await loadStorageNodes();
    } catch (error) {
      setStorageNodesError(error.message);
    } finally {
      setStorageActionKey("");
    }
  }

  function handleEditStorageNode(node) {
    setStorageNodeForm({
      storageNodeId: getValue(node, "storageNodeId", "StorageNodeId"),
      name: getValue(node, "name", "Name"),
      hostname: getValue(node, "hostname", "Hostname"),
      ipAddress: getValue(node, "ipAddress", "IpAddress"),
      port: getValue(node, "port", "Port"),
      basePath: getValue(node, "basePath", "BasePath"),
      totalCapacityBytes: getValue(node, "totalCapacityBytes", "TotalCapacityBytes")
    });
  }

  async function handleUpdateStorageNodeStatus(nodeId, status) {
    const actionKey = `${nodeId}:status`;
    setStorageActionKey(actionKey);
    setStorageNodesError("");

    try {
      await putJson(apiUrl, `/api/storage-nodes/${nodeId}/status`, {
        Status: Number(status)
      });
      await loadStorageNodes();
    } catch (error) {
      setStorageNodesError(error.message);
    } finally {
      setStorageActionKey("");
    }
  }

  async function handleStorageNodeHeartbeat(nodeId) {
    const draft = heartbeatDrafts[nodeId];
    if (!draft) return;

    const actionKey = `${nodeId}:heartbeat`;
    setStorageActionKey(actionKey);
    setStorageNodesError("");

    try {
      await putJson(apiUrl, `/api/storage-nodes/${nodeId}/heartbeat`, {
        TotalCapacityBytes: Number(draft.totalCapacityBytes),
        UsedCapacityBytes: Number(draft.usedCapacityBytes),
        Status: Number(draft.status)
      });
      await loadStorageNodes();
    } catch (error) {
      setStorageNodesError(error.message);
    } finally {
      setStorageActionKey("");
    }
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

  useEffect(() => {
    if (activeModuleKey === "users") {
      loadUsersWithRoles();
    }
  }, [activeModuleKey, loadUsersWithRoles]);

  useEffect(() => {
    if (activeModuleKey === "storage") {
      loadStorageNodes();
    }
  }, [activeModuleKey, loadStorageNodes]);

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
            {!["users", "storage"].includes(activeModule?.key) && (
              <>
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
              </>
            )}

            {activeModule?.key === "users" ? (
              <section className="panel users-admin-panel">
                <div className="users-admin-header">
                  <div>
                    <p className="eyebrow">Accounts</p>
                    <h2>Users</h2>
                    <p>View registered users, see their current roles, and assign or remove roles.</p>
                  </div>
                  <button className="secondary-button" disabled={loadingUsers} onClick={loadUsersWithRoles} type="button">
                    {loadingUsers ? "Refreshing..." : "Refresh"}
                  </button>
                </div>

                {usersError && <p className="inline-error">{usersError}</p>}

                {loadingUsers ? (
                  <p className="loading-text">Loading users...</p>
                ) : adminUsers.length === 0 ? (
                  <p className="empty-text">No users found.</p>
                ) : (
                  <div className="users-table-wrap">
                    <table className="users-table">
                      <thead>
                        <tr>
                          <th>User</th>
                          <th>Email</th>
                          <th>Roles</th>
                          <th>Assign Role</th>
                        </tr>
                      </thead>
                      <tbody>
                        {adminUsers.map((user) => {
                          const userId = getValue(user, "userId", "UserId");
                          const userRoles = userRoleMap[userId] ?? [];
                          const assignedRoleIds = new Set(
                            userRoles.map((role) => getValue(role, "roleId", "RoleId"))
                          );
                          const assignableRoles = availableRoles.filter((role) =>
                            !assignedRoleIds.has(getValue(role, "roleId", "RoleId"))
                          );
                          const selectedRoleId = selectedRoleByUser[userId] ?? "";

                          return (
                            <tr key={userId}>
                              <td>
                                <strong>{getUserDisplayName(user)}</strong>
                                <span>{getValue(user, "username", "Username")}</span>
                              </td>
                              <td>{getValue(user, "email", "Email")}</td>
                              <td>
                                <div className="role-chip-list">
                                  {userRoles.length > 0 ? (
                                    userRoles.map((role) => {
                                      const roleId = getValue(role, "roleId", "RoleId");
                                      const roleName = getValue(role, "roleName", "RoleName");
                                      const isRemoving = roleActionKey === `${userId}:${roleId}:remove`;

                                      return (
                                        <span className="role-chip" key={roleId}>
                                          {roleName}
                                          <button
                                            aria-label={`Remove ${roleName}`}
                                            disabled={isRemoving}
                                            onClick={() => handleRemoveRole(userId, roleId)}
                                            type="button"
                                          >
                                            {isRemoving ? "..." : "x"}
                                          </button>
                                        </span>
                                      );
                                    })
                                  ) : (
                                    <span className="muted-text">No role</span>
                                  )}
                                </div>
                              </td>
                              <td>
                                <div className="role-assign-control">
                                  <select
                                    value={selectedRoleId}
                                    onChange={(event) =>
                                      setSelectedRoleByUser((current) => ({
                                        ...current,
                                        [userId]: event.target.value
                                      }))
                                    }
                                  >
                                    <option value="">
                                      {assignableRoles.length > 0 ? "Choose role" : "All roles assigned"}
                                    </option>
                                    {assignableRoles.map((role) => {
                                      const roleId = getValue(role, "roleId", "RoleId");
                                      const roleName = getValue(role, "name", "Name");

                                      return (
                                        <option key={roleId} value={roleId}>
                                          {roleName}
                                        </option>
                                      );
                                    })}
                                  </select>
                                  <button
                                    className="primary-button"
                                    disabled={!selectedRoleId || roleActionKey === `${userId}:${selectedRoleId}:assign`}
                                    onClick={() => handleAssignRole(userId)}
                                    type="button"
                                  >
                                    {roleActionKey === `${userId}:${selectedRoleId}:assign` ? "Saving..." : "Assign"}
                                  </button>
                                </div>
                              </td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  </div>
                )}
              </section>
            ) : activeModule?.key === "storage" ? (
              <section className="panel storage-admin-panel">
                <div className="storage-admin-header">
                  <div>
                    <p className="eyebrow">Infrastructure</p>
                    <h2>Storage Nodes</h2>
                    <p>Register laptop nodes, edit connection details, update status, and send heartbeat capacity data.</p>
                  </div>
                  <button className="secondary-button" disabled={loadingStorageNodes} onClick={loadStorageNodes} type="button">
                    {loadingStorageNodes ? "Refreshing..." : "Refresh"}
                  </button>
                </div>

                {storageNodesError && <p className="inline-error">{storageNodesError}</p>}

                <form className="storage-node-form" onSubmit={handleSaveStorageNode}>
                  <div className="storage-node-form-header">
                    <div>
                      <h3>{storageNodeForm.storageNodeId ? "Edit Storage Node" : "Add Storage Node"}</h3>
                      <p>{storageNodeForm.storageNodeId ? "Update the selected node details." : "Register one laptop/server as a storage node."}</p>
                    </div>
                    {storageNodeForm.storageNodeId && (
                      <button className="secondary-button" onClick={() => setStorageNodeForm(emptyStorageNodeForm)} type="button">
                        Cancel Edit
                      </button>
                    )}
                  </div>

                  <div className="storage-node-form-grid">
                    <label>
                      Name
                      <input
                        required
                        value={storageNodeForm.name}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, name: event.target.value }))}
                      />
                    </label>
                    <label>
                      Hostname
                      <input
                        required
                        value={storageNodeForm.hostname}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, hostname: event.target.value }))}
                      />
                    </label>
                    <label>
                      IP Address
                      <input
                        required
                        value={storageNodeForm.ipAddress}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, ipAddress: event.target.value }))}
                      />
                    </label>
                    <label>
                      Port
                      <input
                        min="1"
                        required
                        type="number"
                        value={storageNodeForm.port}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, port: event.target.value }))}
                      />
                    </label>
                    <label>
                      Base Path
                      <input
                        required
                        value={storageNodeForm.basePath}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, basePath: event.target.value }))}
                      />
                    </label>
                    <label>
                      Total Capacity Bytes
                      <input
                        min="1"
                        required
                        type="number"
                        value={storageNodeForm.totalCapacityBytes}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, totalCapacityBytes: event.target.value }))}
                      />
                    </label>
                  </div>

                  <button className="primary-button" disabled={!!storageActionKey} type="submit">
                    {storageNodeForm.storageNodeId
                      ? storageActionKey ? "Saving..." : "Save Changes"
                      : storageActionKey === "create" ? "Adding..." : "Add Storage Node"}
                  </button>
                </form>

                {loadingStorageNodes ? (
                  <p className="loading-text">Loading storage nodes...</p>
                ) : storageNodes.length === 0 ? (
                  <p className="empty-text">No storage nodes found.</p>
                ) : (
                  <div className="storage-node-list">
                    {storageNodes.map((node) => {
                      const nodeId = getValue(node, "storageNodeId", "StorageNodeId");
                      const totalBytes = Number(getValue(node, "totalCapacityBytes", "TotalCapacityBytes")) || 0;
                      const usedBytes = Number(getValue(node, "usedCapacityBytes", "UsedCapacityBytes")) || 0;
                      const usedPercent = totalBytes > 0 ? Math.min((usedBytes / totalBytes) * 100, 100) : 0;
                      const heartbeatDraft = heartbeatDrafts[nodeId] ?? {
                        totalCapacityBytes: String(totalBytes),
                        usedCapacityBytes: String(usedBytes),
                        status: getNodeStatusValue(node)
                      };

                      return (
                        <article className="storage-node-card" key={nodeId}>
                          <div className="storage-node-card-header">
                            <div>
                              <h3>{getValue(node, "name", "Name")}</h3>
                              <p>{getValue(node, "hostname", "Hostname")} - {getValue(node, "ipAddress", "IpAddress")}:{getValue(node, "port", "Port")}</p>
                            </div>
                            <span className={`node-status status-${getNodeStatusLabel(node).toLowerCase()}`}>
                              {getNodeStatusLabel(node)}
                            </span>
                          </div>

                          <div className="node-capacity">
                            <div className="node-capacity-meta">
                              <span>{formatBytes(usedBytes)} used</span>
                              <span>{formatBytes(totalBytes)} total</span>
                            </div>
                            <div className="storage-bar">
                              <div className="storage-fill" style={{ width: `${usedPercent}%` }}></div>
                            </div>
                          </div>

                          <dl className="node-detail-grid">
                            <div>
                              <dt>Base Path</dt>
                              <dd>{getValue(node, "basePath", "BasePath")}</dd>
                            </div>
                            <div>
                              <dt>Files</dt>
                              <dd>{getValue(node, "fileCount", "FileCount") || "0"}</dd>
                            </div>
                            <div>
                              <dt>Last Heartbeat</dt>
                              <dd>{formatDate(getValue(node, "lastHeartbeatAt", "LastHeartbeatAt"))}</dd>
                            </div>
                            <div>
                              <dt>Updated</dt>
                              <dd>{formatDate(getValue(node, "updatedAt", "UpdatedAt"))}</dd>
                            </div>
                          </dl>

                          <div className="node-actions">
                            <button className="secondary-button" onClick={() => handleEditStorageNode(node)} type="button">
                              Edit
                            </button>
                            <label>
                              Status
                              <select
                                value={getNodeStatusValue(node)}
                                onChange={(event) => handleUpdateStorageNodeStatus(nodeId, event.target.value)}
                                disabled={storageActionKey === `${nodeId}:status`}
                              >
                                {nodeStatusOptions.map((option) => (
                                  <option key={option.value} value={option.value}>{option.label}</option>
                                ))}
                              </select>
                            </label>
                          </div>

                          <div className="heartbeat-form">
                            <label>
                              Used Bytes
                              <input
                                min="0"
                                type="number"
                                value={heartbeatDraft.usedCapacityBytes}
                                onChange={(event) =>
                                  setHeartbeatDrafts((current) => ({
                                    ...current,
                                    [nodeId]: { ...heartbeatDraft, usedCapacityBytes: event.target.value }
                                  }))
                                }
                              />
                            </label>
                            <label>
                              Total Bytes
                              <input
                                min="1"
                                type="number"
                                value={heartbeatDraft.totalCapacityBytes}
                                onChange={(event) =>
                                  setHeartbeatDrafts((current) => ({
                                    ...current,
                                    [nodeId]: { ...heartbeatDraft, totalCapacityBytes: event.target.value }
                                  }))
                                }
                              />
                            </label>
                            <label>
                              Heartbeat Status
                              <select
                                value={heartbeatDraft.status}
                                onChange={(event) =>
                                  setHeartbeatDrafts((current) => ({
                                    ...current,
                                    [nodeId]: { ...heartbeatDraft, status: event.target.value }
                                  }))
                                }
                              >
                                {nodeStatusOptions.map((option) => (
                                  <option key={option.value} value={option.value}>{option.label}</option>
                                ))}
                              </select>
                            </label>
                            <button
                              className="primary-button"
                              disabled={storageActionKey === `${nodeId}:heartbeat`}
                              onClick={() => handleStorageNodeHeartbeat(nodeId)}
                              type="button"
                            >
                              {storageActionKey === `${nodeId}:heartbeat` ? "Sending..." : "Send Heartbeat"}
                            </button>
                          </div>
                        </article>
                      );
                    })}
                  </div>
                )}
              </section>
            ) : activeModule && (
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

function getValue(source, ...keys) {
  for (const key of keys) {
    const value = source?.[key];
    if (value !== undefined && value !== null && String(value).trim()) {
      return String(value).trim();
    }
  }

  return "";
}

function getUserDisplayName(user) {
  const firstName = getValue(user, "firstName", "FirstName");
  const lastName = getValue(user, "lastName", "LastName");
  const fullName = [firstName, lastName].filter(Boolean).join(" ");
  return fullName || getValue(user, "username", "Username") || getValue(user, "email", "Email") || "User";
}

function getNodeStatusValue(node) {
  const status = getValue(node, "status", "Status");
  const matchingOption = nodeStatusOptions.find((option) =>
    option.value === status || option.label.toLowerCase() === status.toLowerCase()
  );

  return matchingOption?.value ?? "1";
}

function getNodeStatusLabel(node) {
  const status = getNodeStatusValue(node);
  return nodeStatusOptions.find((option) => option.value === status)?.label ?? "Offline";
}

function formatBytes(bytes) {
  if (!bytes) return "0 B";

  const units = ["B", "KB", "MB", "GB", "TB"];
  const index = Math.min(Math.floor(Math.log(bytes) / Math.log(1024)), units.length - 1);
  const value = bytes / Math.pow(1024, index);

  return `${value.toFixed(value >= 10 || index === 0 ? 0 : 1)} ${units[index]}`;
}

function formatDate(value) {
  if (!value) return "Not set";

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;

  return date.toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric"
  });
}

export default AdminDashboardPage;
