import { useState, useEffect, useCallback } from "react";
import {
  getJson,
  postJson,
  putJson,
  defaultApiUrl,
  getUserIdFromToken,
  clearAuth,
  isAuthenticated
} from "./apiClient.js";

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

  const userId = getUserIdFromToken();

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
      setFolderContents(data);
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
            <h1>{profile ? `Welcome, ${profile.FirstName || profile.Username}` : "My Dashboard"}</h1>
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
                  <form className="new-folder-form" onSubmit={handleCreateFolder}>
                    <input
                      type="text"
                      placeholder="New folder name"
                      value={newFolderName}
                      onChange={(e) => setNewFolderName(e.target.value)}
                      disabled={creatingFolder}
                    />
                    <button className="primary-button" type="submit" disabled={creatingFolder || !newFolderName.trim()}>
                      {creatingFolder ? "Creating..." : "New Folder"}
                    </button>
                  </form>
                </div>

                {currentFolder && (
                  <button className="breadcrumb" onClick={goToParentFolder} type="button">
                    ← Back to root
                  </button>
                )}

                {loadingFiles ? (
                  <p className="loading-text">Loading files...</p>
                ) : folderContents.folders.length === 0 && folderContents.files.length === 0 ? (
                  <p className="empty-text">No files or folders yet. Create a folder to get started.</p>
                ) : (
                  <div className="file-list">
                    {folderContents.folders.map((folder) => (
                      <div key={folder.FolderId} className="file-item folder" onClick={() => openFolder(folder.FolderId)}>
                        <div className="file-icon folder-icon">📁</div>
                        <div className="file-info">
                          <strong>{folder.Name}</strong>
                          <small>Folder • Created {formatDate(folder.CreatedAt)}</small>
                        </div>
                      </div>
                    ))}
                    {folderContents.files.map((file) => (
                      <div key={file.FileItemId} className="file-item">
                        <div className="file-icon">📄</div>
                        <div className="file-info">
                          <strong>{file.FileName}</strong>
                          <small>{file.FileType || "Unknown"} • {formatBytes(file.FileSize)}</small>
                        </div>
                      </div>
                    ))}
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
                      {(profile.FirstName?.[0] || profile.Username?.[0] || "U").toUpperCase()}
                    </div>
                    <div>
                      <strong>{profile.FirstName && profile.LastName ? `${profile.FirstName} ${profile.LastName}` : profile.Username}</strong>
                      <p>{profile.Email}</p>
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
