import { useState, useEffect, useCallback, useRef } from "react";
import {
  getJson,
  postJson,
  putJson,
  deleteJson,
  uploadFile,
  downloadFileBlob,
  defaultApiUrl,
  getUserIdFromToken,
  getUserNameFromToken,
  getPermissionsFromToken,
  getRolesFromToken,
  clearAuth,
  isAuthenticated
} from "./apiClient.js";

const userDashboardModules = [
  { title: "My Files", area: "Storage", permissions: ["File.Read"], fallbackUser: true },
  { title: "Upload Files", area: "Storage", permissions: ["File.Upload"] },
  { title: "Folders", area: "Storage", permissions: ["Folder.Create"] },
  { title: "Link Sharing", area: "Sharing", permissions: ["File.Share", "Folder.Share"] },
  { title: "Notifications", area: "Activity", permissions: [], fallbackUser: true },
  { title: "Subscription", area: "Billing", permissions: [], fallbackUser: true }
];

function formatBytes(bytes) {
  if (bytes === 0) return "0 B";
  const k = 1024;
  const sizes = ["B", "KB", "MB", "GB", "TB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(2))} ${sizes[i]}`;
}

function formatDate(dateString) {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric"
  });
}

function getProfileValue(profile, ...keys) {
  if (!profile) return "";

  for (const key of keys) {
    const value = profile[key];
    if (value !== undefined && value !== null && String(value).trim()) {
      return String(value).trim();
    }
  }

  return "";
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

function getArrayValue(source, ...keys) {
  for (const key of keys) {
    const value = source?.[key];
    if (Array.isArray(value)) {
      return value;
    }
  }

  return [];
}

function normalizeFolderContents(data) {
  return {
    folders: getArrayValue(data, "folders", "Folders"),
    files: getArrayValue(data, "files", "Files")
  };
}

function getFileId(file) {
  return getValue(file, "fileItemId", "FileItemId");
}

function getFileName(file) {
  return getValue(file, "fileName", "FileName") || "Unnamed file";
}

function getFolderId(folder) {
  return getValue(folder, "folderId", "FolderId");
}

function getFolderName(folder) {
  return getValue(folder, "name", "Name") || "Unnamed folder";
}

function getProfileDisplayName(profile, fallbackName) {
  const firstName = getProfileValue(profile, "firstName", "FirstName");
  const lastName = getProfileValue(profile, "lastName", "LastName");
  const fullName = [firstName, lastName].filter(Boolean).join(" ");
  const username = getProfileValue(profile, "username", "Username");
  const email = getProfileValue(profile, "email", "Email");

  return fullName || username || email || fallbackName;
}

function UserHomePage({ onLogout }) {
  const [apiUrl, setApiUrl] = useState(() => localStorage.getItem("shc.apiUrl") || defaultApiUrl);
  const [activeTab, setActiveTab] = useState("files");

  const [profile, setProfile] = useState(null);
  const [loadingProfile, setLoadingProfile] = useState(true);

  const [subscription, setSubscription] = useState(null);
  const [entitlements, setEntitlements] = useState(null);
  const [loadingSubscription, setLoadingSubscription] = useState(true);

  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [loadingNotifications, setLoadingNotifications] = useState(true);

  const [folderContents, setFolderContents] = useState({ folders: [], files: [] });
  const [currentFolder, setCurrentFolder] = useState(null);
  const [loadingFiles, setLoadingFiles] = useState(true);

  const [newFolderName, setNewFolderName] = useState("");
  const [creatingFolder, setCreatingFolder] = useState(false);

  const [uploading, setUploading] = useState(false);
  const [uploadError, setUploadError] = useState("");

  const [selectedFile, setSelectedFile] = useState(null);
  const [downloadProgress, setDownloadProgress] = useState(null);

  const [deletingId, setDeletingId] = useState(null);
  const [renamingId, setRenamingId] = useState(null);
  const [renameValue, setRenameValue] = useState("");
  const [moveTargetByFile, setMoveTargetByFile] = useState({});
  const [movingId, setMovingId] = useState(null);

  const fileInputRef = useRef(null);

  const userId = getUserIdFromToken();
  const tokenUserName = getUserNameFromToken() || "User";
  const userDisplayName = getProfileDisplayName(profile, tokenUserName);
  const userEmail = getProfileValue(profile, "email", "Email");
  const roles = getRolesFromToken();
  const permissions = getPermissionsFromToken();
  const permissionSet = new Set(permissions.map((permission) => permission.toLowerCase()));
  const hasUserRole = roles.some((role) => role.toLowerCase() === "user" || role.toLowerCase() === "admin");
  const authorizedModules = userDashboardModules.filter((module) =>
    module.permissions.some((permission) => permissionSet.has(permission.toLowerCase()))
    || (module.fallbackUser && hasUserRole)
  );

  function updateApiUrl(value) {
    setApiUrl(value);
    localStorage.setItem("shc.apiUrl", value);
  }

  const fetchProfile = useCallback(async () => {
    if (!userId) return;
    try {
      const user = await getJson(apiUrl, `/api/users/${userId}`);
      setProfile(user);
    } catch (error) {
      console.error("Failed to fetch profile:", error);
    } finally {
      setLoadingProfile(false);
    }
  }, [apiUrl, userId]);

  const fetchSubscriptionData = useCallback(async () => {
    if (!userId) return;
    try {
      const [activeSub, userEntitlements] = await Promise.all([
        getJson(apiUrl, `/api/subscriptions/user/${userId}/active`).catch(() => null),
        getJson(apiUrl, `/api/subscriptions/user/${userId}/entitlements`).catch(() => null)
      ]);
      setSubscription(activeSub);
      setEntitlements(userEntitlements);
    } catch (error) {
      console.error("Failed to fetch subscription:", error);
    } finally {
      setLoadingSubscription(false);
    }
  }, [apiUrl, userId]);

  const fetchNotifications = useCallback(async () => {
    if (!userId) return;
    try {
      const unread = await getJson(apiUrl, `/api/notifications/user/${userId}/unread`);
      setNotifications(unread);
      setUnreadCount(unread.length);
    } catch (error) {
      console.error("Failed to fetch notifications:", error);
    } finally {
      setLoadingNotifications(false);
    }
  }, [apiUrl, userId]);

  const fetchFolderContents = useCallback(async (folderId = null) => {
    if (!userId) return;
    setLoadingFiles(true);
    try {
      const url = folderId
        ? `/api/folders/user/${userId}/contents?folderId=${folderId}`
        : `/api/folders/user/${userId}/contents`;
      const data = await getJson(apiUrl, url);
      setFolderContents(normalizeFolderContents(data));
      setCurrentFolder(folderId);
    } catch (error) {
      console.error("Failed to fetch folder contents:", error);
    } finally {
      setLoadingFiles(false);
    }
  }, [apiUrl, userId]);

  useEffect(() => {
    if (!isAuthenticated()) {
      onLogout("login");
      return;
    }
    fetchProfile();
    fetchSubscriptionData();
    fetchNotifications();
    fetchFolderContents();
  }, [onLogout, fetchProfile, fetchSubscriptionData, fetchNotifications, fetchFolderContents]);

  const handleCreateFolder = async (event) => {
    event.preventDefault();
    if (!newFolderName.trim() || !userId) return;

    setCreatingFolder(true);
    try {
      await postJson(apiUrl, "/api/folders", {
        UserId: userId,
        Name: newFolderName.trim(),
        ParentFolderId: currentFolder
      });
      setNewFolderName("");
      fetchFolderContents(currentFolder);
    } catch (error) {
      alert(`Failed to create folder: ${error.message}`);
    } finally {
      setCreatingFolder(false);
    }
  };

  const handleUploadClick = () => {
    fileInputRef.current?.click();
  };

  const handleFileChange = async (event) => {
    const file = event.target.files?.[0];
    if (!file || !userId) return;

    setSelectedFile(file);
    setUploadError("");
    setUploading(true);

    try {
      await uploadFile(apiUrl, userId, currentFolder, file);
      await fetchFolderContents(currentFolder);
      setSelectedFile(null);
    } catch (error) {
      setUploadError(error.message);
    } finally {
      setUploading(false);
      event.target.value = "";
    }
  };

  const handleDownload = async (fileItem) => {
    const fileItemId = getFileId(fileItem);
    const fileName = getFileName(fileItem);

    if (!fileItemId) {
      alert("File id was not found.");
      return;
    }

    setDownloadProgress(fileItemId);
    try {
      const blob = await downloadFileBlob(apiUrl, fileItemId);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      alert(`Failed to download file: ${error.message}`);
    } finally {
      setDownloadProgress(null);
    }
  };

  const handleDeleteFile = async (fileItem) => {
    const fileItemId = getFileId(fileItem);
    const fileName = getFileName(fileItem);

    if (!fileItemId) {
      alert("File id was not found.");
      return;
    }

    if (!window.confirm(`Delete "${fileName}"?`)) return;
    setDeletingId(fileItemId);
    try {
      await deleteJson(apiUrl, `/api/files/${fileItemId}`);
      await fetchFolderContents(currentFolder);
    } catch (error) {
      alert(`Failed to delete file: ${error.message}`);
    } finally {
      setDeletingId(null);
    }
  };

  const handleDeleteFolder = async (folder) => {
    const folderId = getFolderId(folder);
    const folderName = getFolderName(folder);

    if (!folderId) {
      alert("Folder id was not found.");
      return;
    }

    if (!window.confirm(`Delete folder "${folderName}" and all its contents?`)) return;
    setDeletingId(folderId);
    try {
      await deleteJson(apiUrl, `/api/folders/${folderId}`);
      await fetchFolderContents(currentFolder);
    } catch (error) {
      alert(`Failed to delete folder: ${error.message}`);
    } finally {
      setDeletingId(null);
    }
  };

  const startRename = (item) => {
    const itemId = getFileId(item) || getFolderId(item);
    const itemName = getFileId(item) ? getFileName(item) : getFolderName(item);

    setRenamingId(itemId);
    setRenameValue(itemName);
  };

  const cancelRename = () => {
    setRenamingId(null);
    setRenameValue("");
  };

  const submitRename = async (item) => {
    const trimmed = renameValue.trim();
    if (!trimmed) {
      cancelRename();
      return;
    }

    const isFile = !!getFileId(item);
    const id = isFile ? getFileId(item) : getFolderId(item);
    const endpoint = isFile
      ? `/api/files/${id}/rename`
      : `/api/folders/${id}/rename`;

    try {
      const body = isFile
        ? { FileItemId: id, NewName: trimmed }
        : { FolderId: id, NewName: trimmed };

      await putJson(apiUrl, endpoint, body);

      await fetchFolderContents(currentFolder);
    } catch (error) {
      alert(`Failed to rename: ${error.message}`);
    } finally {
      setRenamingId(null);
      setRenameValue("");
    }
  };

  const handleMoveFile = async (fileItem) => {
    const fileItemId = getFileId(fileItem);
    const targetFolderId = moveTargetByFile[fileItemId];

    if (!fileItemId || !targetFolderId) return;

    setMovingId(fileItemId);
    try {
      await putJson(apiUrl, `/api/files/${fileItemId}/move`, {
        FileItemId: fileItemId,
        TargetFolderId: targetFolderId
      });
      setMoveTargetByFile((current) => ({ ...current, [fileItemId]: "" }));
      await fetchFolderContents(currentFolder);
    } catch (error) {
      alert(`Failed to move file: ${error.message}`);
    } finally {
      setMovingId(null);
    }
  };

  const handleLogout = async () => {
    try {
      const refreshToken = localStorage.getItem("shc.refreshToken");
      if (refreshToken) {
        await postJson(apiUrl, "/api/auth/logout", { RefreshToken: refreshToken }).catch(() => {});
      }
    } catch (error) {
      console.error("Logout error:", error);
    } finally {
      clearAuth();
      onLogout("login");
    }
  };

  const openFolder = (folderId) => {
    fetchFolderContents(folderId);
  };

  const goToParentFolder = () => {
    if (currentFolder) {
      fetchFolderContents(null);
    }
  };

  const storageUsed = entitlements?.StorageLimitBytes ? (entitlements.StorageLimitBytes * 0.3) : 0;
  const storageLimit = entitlements?.StorageLimitBytes || 0;
  const storagePercentage = storageLimit > 0 ? Math.min((storageUsed / storageLimit) * 100, 100) : 0;

  const [redirecting, setRedirecting] = useState(false);

  useEffect(() => {
    if (!isAuthenticated()) {
      setRedirecting(true);
      onLogout("login");
    }
  }, [onLogout]);

  if (redirecting) {
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
        </aside>
        <section className="page">
          <header className="topbar">
            <div>
              <p className="eyebrow">Loading</p>
              <h1>Please wait...</h1>
            </div>
          </header>
        </section>
      </main>
    );
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
          <button className={`module-item ${activeTab === "files" ? "active" : ""}`} onClick={() => { setActiveTab("files"); fetchFolderContents(currentFolder); }} type="button">
            <span>My Files</span>
            <small>Storage</small>
          </button>
          <button className={`module-item ${activeTab === "notifications" ? "active" : ""}`} onClick={() => setActiveTab("notifications")} type="button">
            <span>Notifications</span>
            <small>{unreadCount > 0 ? unreadCount : ""}</small>
          </button>
          <button className={`module-item ${activeTab === "subscription" ? "active" : ""}`} onClick={() => setActiveTab("subscription")} type="button">
            <span>Subscription</span>
            <small>Plan</small>
          </button>
        </nav>

        <div style={{ marginTop: "auto" }}>
          <button className="module-item" onClick={handleLogout} type="button">
            <span>Logout</span>
            <small>Exit</small>
          </button>
        </div>
      </aside>

      <section className="page">
        <header className="topbar">
          <div>
            <p className="eyebrow">Dashboard</p>
            <h1>Welcome, {userDisplayName}</h1>
          </div>
          <label className="api-field">
            API URL
            <input value={apiUrl} onChange={(event) => updateApiUrl(event.target.value)} />
          </label>
        </header>

        {activeTab === "files" && (
          <section className="content-grid">
            <section className="files-panel">
              <div className="panel">
                <div className="files-header">
                  <div>
                    <h2>My Files</h2>
                    <p>Browse and manage your files and folders.</p>
                  </div>
                  <div className="files-actions">
                    <form className="new-folder-form" onSubmit={handleCreateFolder}>
                      <input
                        type="text"
                        placeholder="New folder name"
                        value={newFolderName}
                        onChange={(e) => setNewFolderName(e.target.value)}
                        disabled={creatingFolder}
                      />
                      <button className="secondary-button" type="submit" disabled={creatingFolder || !newFolderName.trim()}>
                        {creatingFolder ? "Creating..." : "New Folder"}
                      </button>
                    </form>
                    <button className="primary-button" type="button" onClick={handleUploadClick} disabled={uploading}>
                      {uploading ? "Uploading..." : "Upload File"}
                    </button>
                    <input
                      ref={fileInputRef}
                      type="file"
                      style={{ display: "none" }}
                      onChange={handleFileChange}
                    />
                  </div>
                </div>

                {currentFolder && (
                  <button className="breadcrumb" onClick={goToParentFolder} type="button">
                    ← Back to root
                  </button>
                )}

                {selectedFile && (
                  <div className="upload-status">
                    Uploading {selectedFile.name} ({formatBytes(selectedFile.size)})...
                  </div>
                )}

                {uploadError && (
                  <div className="upload-status error">{uploadError}</div>
                )}

                {loadingFiles ? (
                  <p className="loading-text">Loading files...</p>
                ) : folderContents.folders.length === 0 && folderContents.files.length === 0 ? (
                  <p className="empty-text">No files or folders yet. Create a folder to get started.</p>
                ) : (
                  <div className="file-list">
                    {folderContents.folders.map((folder) => {
                      const folderId = getFolderId(folder);
                      const folderName = getFolderName(folder);
                      const isRenaming = renamingId === folderId;
                      const isDeleting = deletingId === folderId;

                      return (
                        <div key={folderId} className="file-item folder">
                          <div className="file-icon folder-icon">📁</div>
                          <div className="file-info">
                            {isRenaming ? (
                              <input
                                className="rename-input"
                                type="text"
                                value={renameValue}
                                onChange={(e) => setRenameValue(e.target.value)}
                                onBlur={() => submitRename(folder)}
                                onKeyDown={(e) => {
                                  if (e.key === "Enter") submitRename(folder);
                                  if (e.key === "Escape") cancelRename();
                                }}
                                autoFocus
                              />
                            ) : (
                              <strong onClick={() => openFolder(folderId)}>{folderName}</strong>
                            )}
                            <small>Folder • Created {formatDate(getValue(folder, "createdAt", "CreatedAt"))}</small>
                          </div>
                          <div className="file-actions">
                            <button
                              className="icon-button"
                              onClick={() => openFolder(folderId)}
                              title="Open"
                              type="button"
                              disabled={isRenaming}
                            >
                              Open
                            </button>
                            <button
                              className="icon-button"
                              onClick={() => startRename(folder)}
                              title="Rename"
                              type="button"
                              disabled={isRenaming || isDeleting}
                            >
                              Rename
                            </button>
                            <button
                              className="icon-button danger"
                              onClick={() => handleDeleteFolder(folder)}
                              title="Delete"
                              type="button"
                              disabled={isRenaming || isDeleting}
                            >
                              {isDeleting ? "Deleting..." : "Delete"}
                            </button>
                          </div>
                        </div>
                      );
                    })}
                    {folderContents.files.map((file) => {
                      const fileItemId = getFileId(file);
                      const fileName = getFileName(file);
                      const fileType = getValue(file, "fileType", "FileType") || "Unknown";
                      const fileSize = Number(getValue(file, "fileSize", "FileSize")) || 0;
                      const isRenaming = renamingId === fileItemId;
                      const isDeleting = deletingId === fileItemId;
                      const isDownloading = downloadProgress === fileItemId;
                      const isMoving = movingId === fileItemId;
                      const moveTarget = moveTargetByFile[fileItemId] ?? "";
                      const targetFolders = folderContents.folders.filter((folder) => getFolderId(folder));

                      return (
                        <div key={fileItemId} className="file-item">
                          <div className="file-icon">📄</div>
                          <div className="file-info">
                            {isRenaming ? (
                              <input
                                className="rename-input"
                                type="text"
                                value={renameValue}
                                onChange={(e) => setRenameValue(e.target.value)}
                                onBlur={() => submitRename(file)}
                                onKeyDown={(e) => {
                                  if (e.key === "Enter") submitRename(file);
                                  if (e.key === "Escape") cancelRename();
                                }}
                                autoFocus
                              />
                            ) : (
                              <strong>{fileName}</strong>
                            )}
                            <small>{fileType} • {formatBytes(fileSize)}</small>
                          </div>
                          <div className="file-actions">
                            <button
                              className="icon-button"
                              onClick={() => handleDownload(file)}
                              title="Download"
                              type="button"
                              disabled={isDownloading || isRenaming || isDeleting}
                            >
                              {isDownloading ? "Saving..." : "Download"}
                            </button>
                            <button
                              className="icon-button"
                              onClick={() => startRename(file)}
                              title="Rename"
                              type="button"
                              disabled={isRenaming || isDeleting || isDownloading}
                            >
                              Rename
                            </button>
                            {targetFolders.length > 0 && (
                              <div className="move-control">
                                <select
                                  value={moveTarget}
                                  onChange={(event) =>
                                    setMoveTargetByFile((current) => ({
                                      ...current,
                                      [fileItemId]: event.target.value
                                    }))
                                  }
                                  disabled={isRenaming || isDeleting || isDownloading || isMoving}
                                >
                                  <option value="">Move to...</option>
                                  {targetFolders.map((folder) => {
                                    const targetFolderId = getFolderId(folder);

                                    return (
                                      <option key={targetFolderId} value={targetFolderId}>
                                        {getFolderName(folder)}
                                      </option>
                                    );
                                  })}
                                </select>
                                <button
                                  className="icon-button"
                                  onClick={() => handleMoveFile(file)}
                                  type="button"
                                  disabled={!moveTarget || isRenaming || isDeleting || isDownloading || isMoving}
                                >
                                  {isMoving ? "Moving..." : "Move"}
                                </button>
                              </div>
                            )}
                            <button
                              className="icon-button danger"
                              onClick={() => handleDeleteFile(file)}
                              title="Delete"
                              type="button"
                              disabled={isRenaming || isDeleting || isDownloading}
                            >
                              {isDeleting ? "Deleting..." : "Delete"}
                            </button>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                )}
              </div>
            </section>

            <aside className="side-panels">
              <div className="panel profile-card">
                <h2>Profile</h2>
                {loadingProfile ? (
                  <p>Loading...</p>
                ) : profile ? (
                  <div className="profile-info">
                    <div className="profile-avatar">
                      {userDisplayName[0]?.toUpperCase() || "?"}
                    </div>
                    <div>
                      <strong>{userDisplayName}</strong>
                      <p>{userEmail}</p>
                    </div>
                  </div>
                ) : (
                  <p>Failed to load profile.</p>
                )}
              </div>

              <div className="panel storage-card">
                <h2>Storage</h2>
                {loadingSubscription ? (
                  <p>Loading...</p>
                ) : (
                  <div>
                    <div className="storage-bar">
                      <div className="storage-fill" style={{ width: `${storagePercentage}%` }}></div>
                    </div>
                    <p className="storage-text">
                      {formatBytes(storageUsed)} of {formatBytes(storageLimit)} used
                    </p>
                    {entitlements?.PlanName && (
                      <p className="plan-badge">{entitlements.PlanName}</p>
                    )}
                  </div>
                )}
              </div>

              <div className="panel access-card">
                <h2>Your Access</h2>
                <div className="pill-list compact">
                  {roles.length > 0 ? roles.map((role) => <span key={role}>{role}</span>) : <span>No role in token</span>}
                </div>
                <div className="permission-list compact">
                  {authorizedModules.length > 0
                    ? authorizedModules.map((module) => <span key={module.title}>{module.title}</span>)
                    : <span>No dashboard modules found</span>}
                </div>
              </div>
            </aside>
          </section>
        )}

        {activeTab === "notifications" && (
          <section className="content-grid">
            <section className="notifications-panel">
              <div className="panel">
                <div className="notifications-header">
                  <h2>Notifications</h2>
                  {unreadCount > 0 && (
                    <span className="unread-badge">{unreadCount} unread</span>
                  )}
                </div>
                {loadingNotifications ? (
                  <p className="loading-text">Loading notifications...</p>
                ) : notifications.length === 0 ? (
                  <p className="empty-text">No notifications.</p>
                ) : (
                  <div className="notification-list">
                    {notifications.map((notification) => (
                      <div key={notification.NotificationId} className="notification-item">
                        <div className="notification-icon">
                          {notification.Type === "System" ? "🔔" : notification.Type === "File" ? "📁" : "ℹ️"}
                        </div>
                        <div className="notification-content">
                          <strong>{notification.Title}</strong>
                          <p>{notification.Message}</p>
                          <small>{formatDate(notification.CreatedAt)}</small>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </section>
          </section>
        )}

        {activeTab === "subscription" && (
          <section className="content-grid">
            <section className="subscription-panel">
              <div className="panel">
                <h2>Subscription Details</h2>
                {loadingSubscription ? (
                  <p className="loading-text">Loading subscription...</p>
                ) : subscription ? (
                  <div className="subscription-info">
                    <div className="subscription-row">
                      <span>Plan</span>
                      <strong>{subscription.PlanName}</strong>
                    </div>
                    <div className="subscription-row">
                      <span>Status</span>
                      <strong className={`status-${subscription.Status?.toLowerCase() || "unknown"}`}>
                        {subscription.Status}
                      </strong>
                    </div>
                    <div className="subscription-row">
                      <span>Current Period</span>
                      <span>{formatDate(subscription.CurrentPeriodStart)} - {formatDate(subscription.CurrentPeriodEnd)}</span>
                    </div>
                    {subscription.AutoRenew && (
                      <div className="subscription-row">
                        <span>Auto Renew</span>
                        <span>Yes</span>
                      </div>
                    )}
                  </div>
                ) : (
                  <p className="empty-text">No active subscription.</p>
                )}
              </div>
            </section>
          </section>
        )}
      </section>
    </main>
  );
}

export default UserHomePage;
